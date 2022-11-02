using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterSingleBus : DeclarationDataAdapterCompletedBusSpecific
	{
		#region Implementation of IDeclarationDataAdapter

		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(
				CompletedVehicle.Length,
				CompletedVehicle.Width);
			var passengerCountRef = busFloorArea * (loading.Key == LoadingType.LowLoading
				? mission.BusParameter.PassengerDensityLow
				: mission.BusParameter.PassengerDensityRef);
			var passengerCountDecl = CompletedVehicle.NumberPassengerSeatsUpperDeck +
									CompletedVehicle.NumberPassengerSeatsLowerDeck
									+ (mission.MissionType == MissionType.Coach
										? 0
										: CompletedVehicle.NumberPassengersStandingLowerDeck +
										CompletedVehicle.NumberPassengersStandingUpperDeck);

			//var refLoad = passengerCount * mission.MissionType.GetAveragePassengerMass();
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading) {
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}

			var passengerCountCalc = loading.Key == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, (int)passengerCountDecl)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
			var payload = passengerCountCalc * mission.MissionType.GetAveragePassengerMass();

			var retVal = CreateNonExemptedVehicleData(vehicle, segment, mission, payload, passengerCountCalc, allowVocational);
			retVal.CurbMass = CompletedVehicle.CurbMassChassis;
			return retVal;
		}

		#endregion


		public override CombustionEngineData CreateEngineData(
			IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
		{
			var engine = vehicle.Components.EngineInputData;
			var gearbox = vehicle.Components.GearboxInputData;

			if (!engine.SavedInDeclarationMode) {
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine, SingleBusInputData.CompletedVehicle.TankSystem);
			retVal.IdleSpeed = VectoMath.Max(mode.IdleSpeed, vehicle.EngineIdleSpeed);

			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in mode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						WHTCUrban = fuel.WHTCUrban,
						WHTCRural = fuel.WHTCRural,
						WHTCMotorway = fuel.WHTCMotorway,
						ColdHotCorrectionFactor = fuel.ColdHotBalancingFactor,
						CorrectionFactorRegPer = fuel.CorrectionFactorRegPer,
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, SingleBusInputData.CompletedVehicle.TankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
															mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban,
															fuel.WHTCMotorway) * fuel.ColdHotBalancingFactor * fuel.CorrectionFactorRegPer,
					});
			}

			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement, gearbox.Type);
			retVal.EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(mode.FullLoadCurve, true);
			fullLoadCurves[0].EngineData = retVal;
			foreach (var gear in gearbox.Gears) {
				var maxTorque = VectoMath.Min(
					GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			retVal.FullLoadCurves = fullLoadCurves;

			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0) {
				retVal.ElectricalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataElectrical, mission.MissionType, WHRType.ElectricalOutput);
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
				retVal.MechanicalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataMechanical, mission.MissionType, WHRType.MechanicalOutputDrivetrain);
			}

			return retVal;
		}

		protected override TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, bool useEfficiencyFallback, VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			return TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, $"Gear {i + 1}", true);
		}


		protected override TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio, CombustionEngineData engineData)
		{
			return TorqueConverterDataReader.Create(
				torqueConverter.TCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}


		public override AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, false);
			return retVal;
		}

		#region Overrides of DeclarationDataAdapterCompletedBusGeneric

		public override RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		#endregion

		public ISingleBusInputDataProvider SingleBusInputData { get; set; }

		protected IVehicleDeclarationInputData CompletedVehicle => SingleBusInputData?.CompletedVehicle;
	}
}
