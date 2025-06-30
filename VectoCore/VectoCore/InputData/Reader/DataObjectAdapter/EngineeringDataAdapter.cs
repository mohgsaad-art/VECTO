#define TRACE_FC


/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
    public interface IEngineeringDataAdapter
    {
        VehicleData CreateVehicleData(IVehicleEngineeringInputData data);
        VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemsEngineering adas);
        AirdragData CreateAirdragData(IAirdragEngineeringInputData airdragData, IVehicleEngineeringInputData data);

        CombustionEngineData CreateEngineData(
            IVehicleEngineeringInputData vehicle, IEngineModeEngineeringInputData engineMode);

        CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine, IEngineModeEngineeringInputData engineMode);
        WHRData CreateWHRData(IWHRData whrInputData, WHRType whrType);
		WheelEndData CreateWheelEndData(VehicleClass vehicleClass, IVehicleDeclarationInputData vehicle);
        GearboxData CreateGearboxData(IEngineeringInputDataProvider inputData, VectoRunData runData);
        AxleGearData CreateAxleGearData(IAxleGearInputData data);
        AngledriveData CreateAngledriveData(IAngledriveInputData data);
        IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData);
        DriverData CreateDriverData(IDriverEngineeringInputData driver);
        RetarderData CreateRetarderData(IRetarderInputData retarder, PowertrainPosition powertrainPosition);
        PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto);

        IAuxiliaryConfig CreateBusAuxiliariesData(IAuxiliariesEngineeringInputData auxInputData,
            VehicleData vehicleData, VectoSimulationJobType jobType);

        ShiftStrategyParameters CreateGearshiftData(GearboxType gbxType, IGearshiftEngineeringInputData gsInputData, double axleRatio, PerSecond engineIdlingSpeed);
        BatterySystemData CreateBatteryData(IElectricStorageSystemEngineeringInputData batteryInputData, double initialSOC);
        SuperCapData CreateSuperCapData(IElectricStorageSystemEngineeringInputData reessInputData, double initialSOC);

        IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
            IElectricMachinesEngineeringInputData electricMachines,
            IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
            GearList gearlist = null);

        ElectricMotorData CreateElectricMachine(PowertrainPosition powertrainPosition,
            IElectricMotorEngineeringInputData motorData, int count,
            double ratio, double[] ratioPerGear, double efficiency, TableData adcLossMap,
            IList<Tuple<Volt, TableData>> torqueLimits, Volt averageVoltage, GearList gearList);

        HybridStrategyParameters CreateHybridStrategyParameters(
            IEngineeringJobInputData jobInputData,
            CombustionEngineData combustionEngineData, GearboxData gearboxData);

        List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCEngineeringInputData iepc, Volt averageVoltage);
        GearboxData CreateIEPCGearboxData(IEngineeringInputDataProvider inputData, VectoRunData runData);
        PTOData CreateBatteryElectricPTOTransmissionData(IPTOTransmissionInputData pto);
    }


    public class EngineeringDataAdapter : AbstractSimulationDataAdapter, IEngineeringDataAdapter
	{
		private string _jobFilePath;
		private AirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();

		public IOutputDataWriter DebugOutputDataWriter
		{
			get;
			set;
		}

		public string JobFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(_jobFilePath))
				{
					return DebugOutputDataWriter?.JobFile;
				}
				return _jobFilePath;
			}
			set
			{
				_jobFilePath = value;
			}
		}

		public VehicleData CreateVehicleData(IVehicleEngineeringInputData data)
		{
			if (data.SavedInDeclarationMode) {
				WarnEngineeringMode("VehicleData");
			}
			var retVal = SetCommonVehicleData(data);
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.BodyAndTrailerMass = data.CurbMassExtra;

			//retVal.CurbWeight += data.CurbMassExtra;
			retVal.TrailerGrossVehicleMass = 0.SI<Kilogram>();
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;
			retVal.ADAS = CreateADAS(data.ADAS);

			var axles = data.Components.AxleWheels.AxlesEngineering;

			retVal.AxleData = axles.Select(
				axle => new Axle {
					WheelsDimension = axle.Tyre.Dimension,
					Inertia = axle.Tyre.Inertia,
					TwinTyres = axle.TwinTyres,
					RollResistanceCoefficient = axle.Tyre.RollResistanceCoefficient,
					AxleWeightShare = axle.AxleWeightShare,
					TyreTestLoad = axle.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axle.Tyre.FuelEfficiencyClass,
					AxleType = axle.AxleType,
					//Wheels = axle.WheelsStr
				}).ToList();

			retVal.VehicleClass = DetectVehicleClass(data);

			return retVal;
		}

		private static VehicleClass DetectVehicleClass(IVehicleEngineeringInputData data)
		{
			var segmentTruck = DeclarationData.GetTruckSegment(data, throwException: false);
			if (segmentTruck.Segment.Found) {
				return segmentTruck.Segment.VehicleClass;
			}

			var segmentPrimaryBus = DeclarationData.PrimaryBusSegments.Lookup(
				data.VehicleCategory,
				data.AxleConfiguration,
				data.Articulated);
			if (segmentPrimaryBus.Found) {
				return segmentPrimaryBus.VehicleClass;
			}

			return VehicleClass.Unknown;
		}

		public VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemsEngineering adas)
		{
			return adas == null ?
				new VehicleData.ADASData() {
					EngineStopStart = false,
					EcoRoll = EcoRollType.None,
					PredictiveCruiseControl = PredictiveCruiseControlType.None
				} :
				new VehicleData.ADASData {
					EngineStopStart = adas.EngineStopStart,
					PredictiveCruiseControl = adas.PredictiveCruiseControl,
					EcoRoll = ((adas.PredictiveCruiseControl == PredictiveCruiseControlType.None)
							&& adas.EcoRoll.WithoutEngineStop())
					? EcoRollType.None
					: adas.EcoRoll,
				};
		}

		public IList<AxlePowertrainData> CreateAxlePowertrainsData(IEngineeringInputDataProvider input, Volt averageVoltage, IPowertrainBuilder powertrainBuilder)
		{
			IList<AxlePowertrainData> axlePts = new List<AxlePowertrainData>();

			var axlePowertrainData = input.JobInputData.Vehicle.Components.AxlePowertrainEngineeringInputData;

			if (axlePowertrainData == null)
			{	
				return axlePts;
			}

			foreach (var axlePtData in axlePowertrainData)
			{
				var axlegearData = (axlePtData.AxleGearInputData != null)
					? CreateAxleGearData(axlePtData.AxleGearInputData)
					: null;

				var emData = CreateElectricMachine(
					axlePtData.ElectricMotor, 
					input.JobInputData.Vehicle.ElectricMotorTorqueLimits, 
					averageVoltage);

				var angledriveData = CreateAngledriveData(axlePtData.AngledriveInputData);

				var (gearboxData, gearshiftParams) = CreateGearboxDataForAxlePowertrain(
					emData, axlePtData, axlegearData, angledriveData, input, powertrainBuilder);

				var pto = axlePtData.Type.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)
					? CreateBatteryElectricPTOTransmissionData(axlePtData.PTOTransmissionInputData)
					: CreatePTOTransmissionData(axlePtData.PTOTransmissionInputData);

				axlePts.Add(new AxlePowertrainData()
				{
					AxleNumber = axlePtData.AxleNumber,
					Type = axlePtData.Type,
					AxleGearData = axlegearData,
					ElectricMachineData = emData,
					AngledriveData = angledriveData,
					GearboxData = gearboxData,
					Retarder = CreateRetarderData(axlePtData.RetarderInputData, emData.Item1),
					PTO = pto,
					GearshiftParameters = gearshiftParams,
				});
			}

			return axlePts;
		}

		private Tuple<GearboxData, ShiftStrategyParameters> CreateGearboxDataForAxlePowertrain(
			Tuple<PowertrainPosition, ElectricMotorData> emData,
			IAxlePowertrainEngineeringInputData axlePtData,
			AxleGearData axlegearData,
			AngledriveData angledriveData,
			IEngineeringInputDataProvider input,
			IPowertrainBuilder powertrainBuilder
			)
		{
			GearboxData gearboxData = null;

			ShiftStrategyParameters gearshiftParams = new ShiftStrategyParameters()
			{
				StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
			};

			if (emData.Item1 == PowertrainPosition.BatteryElectricE2)
			{
				gearshiftParams = CreateGearshiftData(
					axlePtData.GearboxInputData.Type,
					axlePtData.GearshiftInputData,
					axlegearData?.AxleGear.Ratio ?? 1.0 * (angledriveData?.Angledrive.Ratio ?? 1.0), null);

     //           gearboxData = CreateGearboxData(
					//input,
					//axlePtData.GearboxInputData,
					//axlePtData.TorqueConverterInputData,
					
			}

			return new Tuple<GearboxData, ShiftStrategyParameters>(gearboxData, gearshiftParams);
		}

		private Tuple<PowertrainPosition, ElectricMotorData> CreateElectricMachine(
			ElectricMachineEntry<IElectricMotorEngineeringInputData> em,
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
			GearList gearlist = null
			)
		{
			return (em == null)
				? null
				: Tuple.Create(
					em.Position,
					CreateElectricMachine(
						em.Position, 
						em.ElectricMachine, 
						em.Count, 
						em.RatioADC, 
						em.RatioPerGear, 
						em.MechanicalTransmissionEfficiency,
						em.MechanicalTransmissionLossMap, 
						torqueLimits?.First(t => t.Key.Position == em.Position).Value, 
						averageVoltage, 
						gearlist));
		}

		public AirdragData CreateAirdragData(IAirdragEngineeringInputData airdragData, IVehicleEngineeringInputData data)
		{
			var retVal = SetCommonAirdragData(airdragData);
			retVal.CrossWindCorrectionMode = airdragData.CrossWindCorrectionMode;

			var deltaCdxAIMC = 0.SI<SquareMeter>();
			if (data.InMotionCharging.Enabled) {
				if (data.InMotionCharging.ShareIMCAvailabilityTotalMission < 0 || data.InMotionCharging.ShareIMCAvailabilityTotalMission > 1) {
					throw new VectoException(
						"Share of In-motion charging infrastructure availability has to be between 0% and 100%");
				}
                deltaCdxAIMC = data.InMotionCharging.DeltaCdxA *
								data.InMotionCharging.ShareIMCAvailabilityTotalMission;
				
			}

			switch (airdragData.CrossWindCorrectionMode) {
				case CrossWindCorrectionMode.NoCorrection:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airdragData.AirDragArea, deltaCdxAIMC,
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(airdragData.AirDragArea),
						CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airdragData.AirDragArea, deltaCdxAIMC,
                        CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(
							airdragData.CrosswindCorrectionMap,
							airdragData.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(
						airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(airdragData.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					var airDragArea = airdragData.AirDragArea ??
									DeclarationData.TruckSegments.LookupCdA(
										data.VehicleCategory, data.AxleConfiguration, data.GrossVehicleMassRating, false);
					var height = data.Height ?? (data.VehicleCategory.IsLorry()
									? DeclarationData.TruckSegments.LookupHeight(
										data.VehicleCategory, data.AxleConfiguration,
										data.GrossVehicleMassRating, false)
									: 4.SI<Meter>());
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airDragArea, deltaCdxAIMC,
                        _airdragDataAdapter.GetDeclarationAirResistanceCurve(
							GetAirdragParameterSet(
								data.VehicleCategory, data.AxleConfiguration, data.Components.AxleWheels.AxlesEngineering.Count, data.GrossVehicleMassRating), airDragArea,
							height),
						CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", airdragData.CrossWindCorrectionMode.ToString());
			}

			return retVal;
		}

		private string GetAirdragParameterSet(VehicleCategory vehicleCategory, AxleConfiguration axles, int numAxles,
			Kilogram tpmlm)
		{
			//TODO: check if is i
			switch (vehicleCategory) {
				case VehicleCategory.Van:
					return "MediumLorriesVan";
				case VehicleCategory.RigidTruck:
					if (tpmlm.IsSmallerOrEqual(7400)) {
						return "MediumLorriesRigid";
					}
					return numAxles > axles.NumAxles() ? "RigidTrailer" : "RigidSolo";
				case VehicleCategory.Tractor: return "TractorSemitrailer";
				case VehicleCategory.CityBus:
				//case VehicleCategory.InterurbanBus:
				case VehicleCategory.Coach: return "CoachBus";
				default: throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Error("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		public CombustionEngineData CreateEngineData(
			IVehicleEngineeringInputData vehicle, IEngineModeEngineeringInputData engineMode)
		{
			var engine = vehicle.Components.EngineInputData;
			var gbx = vehicle.Components.GearboxInputData;
			var torqueConverter = vehicle.Components.TorqueConverterInputData;
			var tankSystem = vehicle.TankSystem;
			
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}
			
			var retVal = SetCommonCombustionEngineData(engine, tankSystem);
			retVal.IdleSpeed = VectoMath.Max(engineMode.IdleSpeed, vehicle.EngineIdleSpeed);
			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in engineMode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, tankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = fuel.WHTCEngineering,
					});
			}

			retVal.Inertia = engine.Inertia +
							(gbx != null && gbx.Type.AutomaticTransmission()
								? (gbx.Type == GearboxType.APTN || gbx.Type == GearboxType.IHPC ? 0.SI<KilogramSquareMeter>() : torqueConverter.Inertia)
								: 0.SI<KilogramSquareMeter>());
			retVal.EngineStartTime = engine.EngineStartTime ?? DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits.ToDictionary(e => e.Gear);

			var gears = GearboxDataAdapterBase.FilterDisabledGears(vehicle.TorqueLimits, gbx);
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(gears.Count + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve);
			fullLoadCurves[0].EngineData = retVal;
			foreach (var gear in gears) {
				var maxTorque = VectoMath.Min(gear.MaxTorque, limits.GetVECTOValueOrDefault(gear.Gear)?.MaxTorque);
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			retVal.FullLoadCurves = fullLoadCurves;

				
			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0) {
				retVal.ElectricalWHR = CreateWHRData(
					engineMode.WasteHeatRecoveryDataElectrical, WHRType.ElectricalOutput);
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
				retVal.MechanicalWHR = CreateWHRData(
					engineMode.WasteHeatRecoveryDataMechanical, WHRType.MechanicalOutputDrivetrain);
			}

			return retVal;
		}

		public WHRData CreateWHRData(IWHRData whrInputData, WHRType whrType)
		{
			if (whrInputData == null || whrInputData.GeneratedPower == null) {
				return null;
			}

			return new WHRData() {
				CFUrban = 1,
				CFRural = 1,
				CFMotorway = 1,
				CFColdHot = 1,
				CFRegPer = 1,
				WHRMap = WHRPowerReader.Create(whrInputData.GeneratedPower, whrType),
				WHRCorrectionFactor = whrInputData.EngineeringCorrectionFactor,
			};
		}


		public CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine, IEngineModeEngineeringInputData engineMode)
		{
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}
			var retVal = SetCommonCombustionEngineData(engine, null);
			retVal.IdleSpeed = engineMode.IdleSpeed;
			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in engineMode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, null),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = fuel.WHTCEngineering,
					});
			}

			retVal.Inertia = engine.Inertia;
			retVal.EngineStartTime = engine.EngineStartTime ?? DeclarationData.Engine.DefaultEngineStartTime;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve);
			retVal.FullLoadCurves = fullLoadCurves;
			return retVal;
		}

		public GearboxData CreateGearboxData(
			IEngineeringInputDataProvider inputData,
			VectoRunData runData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var gearbox = vehicle.Components.GearboxInputData;
			var torqueConverter = vehicle.Components.TorqueConverterInputData;


            var adas = vehicle.ADAS;
			var gearshiftData = inputData.DriverInputData.GearshiftInputData;

			var engineData = runData.EngineData;
			var axlegearRatio = runData.AxleGearData.AxleGear.Ratio;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;
			var vehicleCategory = runData.VehicleData.VehicleCategory;

			if (gearbox.SavedInDeclarationMode)
			{
				WarnEngineeringMode("GearboxData");
			}

			var retVal = SetCommonGearboxData(gearbox);

			if (adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission() && !adas.ATEcoRollReleaseLockupClutch.HasValue)
			{
				throw new VectoException("Parameter ATEcoRollReleaseLockupClutch required for AT gearbox");
			}

			if ((vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.VehicleType == VectoSimulationJobType.SerialHybridVehicle) &&
				gearbox.Type.AutomaticTransmission())
			{
				// PEV with APT-S or APT-P transmission are simulated as APT-N
				retVal.Type = GearboxType.APTN;
			}
			retVal.ATEcoRollReleaseLockupClutch = adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission() ? adas.ATEcoRollReleaseLockupClutch.Value : false;

			if (retVal.Type == GearboxType.IEPC)
			{
				if (gearbox.Gears.Count > 0)
				{
					throw new VectoSimulationException("No gears are allowed for IEPC gearbox.");
				}

				retVal.Inertia = 0.SI<KilogramSquareMeter>();
				retVal.TractionInterruption = 0.SI<Second>();
				return retVal;
			}

			if (gearbox.Gears.Count < 2)
			{
				throw new VectoSimulationException("At least two Gear-Entries must be defined in Gearbox!");
			}

			var gearsInput = GearboxDataAdapterBase.FilterDisabledGears(inputData.JobInputData.Vehicle.TorqueLimits, gearbox);

			SetEngineeringData(gearbox, retVal);

			var hasTorqueConverter = retVal.Type.AutomaticTransmission() && retVal.Type != GearboxType.APTN && retVal.Type != GearboxType.IHPC;

			var gearDifferenceRatio = hasTorqueConverter && gearsInput.Count > 2
				? gearsInput[0].Ratio / gearsInput[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			ShiftPolygon tcShiftPolygon = null;
			if (hasTorqueConverter)
			{
				tcShiftPolygon = torqueConverter.ShiftPolygon != null
					? ShiftPolygonReader.Create(torqueConverter.ShiftPolygon)
					: DeclarationData.TorqueConverter.ComputeShiftPolygon(engineData.FullLoadCurves[0]);
			}
			var isBatteryElectric = vehicle.VehicleType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
				VectoSimulationJobType.SerialHybridVehicle);

            retVal.ShiftStrategy = ShiftStrategyFactory?.GetShiftStrategyName(gearbox.Type, vehicle.VehicleType);
			var shiftPolygonCalc = ShiftStrategyFactory?.CreateShiftPolygonCalculator(retVal.ShiftStrategy, runData.GearshiftParameters);

            for (uint i = 0; i < gearsInput.Count; i++)
			{
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, true, VehicleCategory.Unknown, gearbox.Type);

				ShiftPolygon shiftPolygon;
				if (gear.ShiftPolygon != null && gear.ShiftPolygon.SourceType != DataSourceType.Missing)
				{
					shiftPolygon = ShiftPolygonReader.Create(gear.ShiftPolygon);
				}
				else if (shiftPolygonCalc != null)
				{
					shiftPolygon = shiftPolygonCalc.ComputeDeclarationShiftPolygon(gearbox.Type, (int)i,
						engineData?.FullLoadCurves[i + 1], gearsInput, engineData, axlegearRatio,
						dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
				}
				else
				{
					shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(gearbox.Type, (int)i,
						engineData?.FullLoadCurves[i + 1], gearsInput, engineData, axlegearRatio,
						dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
				}

				ShiftPolygon extendedShiftPolygon = null;
				if (gearbox.Type == GearboxType.MT)
				{
					extendedShiftPolygon = shiftPolygonCalc != null
						? shiftPolygonCalc.ComputeDeclarationExtendedShiftPolygon(
							gearbox.Type, (int)i, engineData?.FullLoadCurves[i + 1], gearbox.Gears, engineData, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2)
						: DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygonExtended(
							(int)i, engineData?.FullLoadCurves[i + 1],
							gearsInput,
							engineData,
							axlegearRatio,
							dynamicTyreRadius);
				}


				var deratedEmShiftPolygon = isBatteryElectric
					? CalculateDeratedEmShiftPolygon(runData, shiftPolygonCalc, gearbox, i, axlegearRatio,
						dynamicTyreRadius)
					: null;

                var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					DeRatedEmShiftPolygon = deratedEmShiftPolygon,
					ExtendedShiftPolygon = extendedShiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(retVal.Type, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, vehicleCategory, runData.Cycle);
				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;

			if (hasTorqueConverter)
			{
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = gearbox.PowershiftShiftTime;
				retVal.TorqueConverterData = TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					torqueConverter.ReferenceRPM, torqueConverter.MaxInputSpeed, ExecutionMode.Engineering, ratio,
					gearshiftData.CLUpshiftMinAcceleration, gearshiftData.CCUpshiftMinAcceleration);
			}

			// update disengageWhenHaltingSpeed
			if (retVal.Type.AutomaticTransmission())
			{
				var firstGear = retVal.GearList.First(x => x.IsLockedGear());
				if (retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any())
				{
					var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
						.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
					var vehicleSpeedDisengage = downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
												dynamicTyreRadius;
					retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
				}
			}

			return retVal;
		}

		protected virtual void CreateATGearData(
			GearboxType gearboxType, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory, IDrivingCycleData cycle)
		{
			if (gearboxType == GearboxType.ATPowerSplit && i == 0) {
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}
			if (gearboxType == GearboxType.ATSerial) {
				if (i == 0) {
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1) {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}	
					else if (gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
						// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
						// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);

						// NOTE: the lower gear in 'gears' dictionary has index i !!
						gears[i].Ratio = double.NaN;
						gears[i].LossMap = null;
					}
				}
				else {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}
				}
			}
		}

		protected virtual ShiftPolygon CalculateDeratedEmShiftPolygon(VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, IGearboxDeclarationInputData gearbox, uint i, double axlegearRatio,
			Meter dynamicTyreRadius)
		{
			var em = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2;
			var shiftStrategyParameters = runData.GearshiftParameters;
			var emFld = em.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = em.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(em.EfficiencyData.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = em.RatioADC,
			};
			var deratedEmShiftPolygon = shiftPolygonCalculator.ComputeElectricMotorDeclarationShiftPolygon(
				gearbox.Type, (int)i,
				gearbox.Gears, axlegearRatio,
				dynamicTyreRadius,
				em,
				limitedEm);
			return deratedEmShiftPolygon;
			//retVal[i + 1] = shiftPolygon;
		}

        private static void SetEngineeringData(
			IGearboxEngineeringInputData gearbox, GearboxData retVal) {
			retVal.Inertia = gearbox.Type.ManualTransmission() ? gearbox.Inertia : 0.SI<KilogramSquareMeter>();
			retVal.TractionInterruption = gearbox.Type == GearboxType.APTN || gearbox.Type == GearboxType.IHPC ? 0.SI<Second>() : gearbox.TractionInterruption;
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData data) {
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, true);
			return retVal;
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData data) {
			return DoCreateAngledriveData(data, true);
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData) {
			if (auxInputData.Auxiliaries is null)
				throw new VectoException("Missing Power Demands for ICE Off Driving, ICE Off Standstill, and Base Demand");

			var pwrICEOn = auxInputData.Auxiliaries.ConstantPowerDemand;
			var pwrICEOffDriving = auxInputData.Auxiliaries.PowerDemandICEOffDriving;
			var pwrICEOffStandstill = auxInputData.Auxiliaries.PowerDemandICEOffStandstill;

			var baseDemand = pwrICEOffStandstill;
			var stpDemand = pwrICEOffDriving - pwrICEOffStandstill;
			var fanDemand = pwrICEOn - pwrICEOffDriving;

			var auxList = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_BASE, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = baseDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_STP, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = stpDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_FAN, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = fanDemand},
				// Temporarily disabling fix for codeu issue 15 because it causes distnace-based testcases to fail 
				//new VectoRunData.AuxData { ID = DrivingCycleDataReader.Fields.AdditionalAuxPowerDemand, DemandType = AuxiliaryDemandType.Direct }
			};

			return auxList;
		}

		public DriverData CreateDriverData(IDriverEngineeringInputData driver) {
			if (driver.SavedInDeclarationMode) {
				WarnEngineeringMode("DriverData");
			}

			AccelerationCurveData accelerationData = null;
			if (driver.AccelerationCurve != null) {
				accelerationData = AccelerationCurveReader.Create(driver.AccelerationCurve.AccelerationCurve);
			}

			if (driver.Lookahead == null) {
				throw new VectoSimulationException("Error: Lookahead Data is missing.");
			}

			var lookAheadData = new DriverData.LACData {
				Enabled = driver.Lookahead.Enabled,

				//Deceleration = driver.Lookahead.Deceleration,
				MinSpeed = driver.Lookahead.MinSpeed,
				LookAheadDecisionFactor =
					new LACDecisionFactor(
						driver.Lookahead.CoastingDecisionFactorOffset, driver.Lookahead.CoastingDecisionFactorScaling,
						driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup,
						driver.Lookahead.CoastingDecisionFactorVelocityDropLookup),
				LookAheadDistanceFactor = driver.Lookahead.LookaheadDistanceFactor
			};
			var overspeedData = new DriverData.OverSpeedData {
				Enabled = driver.OverSpeedData.Enabled,
				MinSpeed = driver.OverSpeedData.MinSpeed,
				OverSpeed = driver.OverSpeedData.OverSpeed,
			};
			var retVal = new DriverData {
				AccelerationCurve = accelerationData,
				LookAheadCoasting = lookAheadData,
				OverSpeed = overspeedData,
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay =
						driver.EngineStopStartData?.ActivationDelay ?? DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay,
					MaxEngineOffTimespan = driver.EngineStopStartData?.MaxEngineOffTimespan ?? DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan,
					UtilityFactorStandstill = driver.EngineStopStartData?.UtilityFactorStandstill ?? DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor,
					UtilityFactorDriving = driver.EngineStopStartData?.UtilityFactorDriving ?? DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor,
				},
				EcoRoll = new DriverData.EcoRollData() {
					UnderspeedThreshold = driver.EcoRollData?.UnderspeedThreshold ?? DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
					MinSpeed = driver.EcoRollData?.MinSpeed ?? DeclarationData.Driver.EcoRoll.MinSpeed,
					ActivationPhaseDuration = driver.EcoRollData?.ActivationDelay ?? DeclarationData.Driver.EcoRoll.ActivationDelay,
					AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
					AccelerationUpperLimit = driver.EcoRollData?.AccelerationUpperLimit ?? DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
				},
				PCC = new DriverData.PCCData() {
					PCCEnableSpeed = driver.PCCData?.PCCEnabledSpeed ?? DeclarationData.Driver.PCC.PCCEnableSpeed,
					MinSpeed = driver.PCCData?.MinSpeed ?? DeclarationData.Driver.PCC.MinSpeed,
					PreviewDistanceUseCase1 = driver.PCCData?.PreviewDistanceUseCase1 ?? DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
					PreviewDistanceUseCase2 = driver.PCCData?.PreviewDistanceUseCase2 ?? DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
					UnderSpeed = driver.PCCData?.Underspeed ?? DeclarationData.Driver.PCC.Underspeed,
					OverspeedUseCase3 = driver.PCCData?.OverspeedUseCase3 ?? DeclarationData.Driver.PCC.OverspeedUseCase3
				}
			};
			return retVal;
		}

		//=================================
		public RetarderData CreateRetarderData(IRetarderInputData retarder, PowertrainPosition powertrainPosition)
		{
			return SetCommonRetarderData(retarder, powertrainPosition);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				var ptoData = new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = pto.PTOLossMap == null ? PTOIdleLossMapReader.GetZeroLossMap() : PTOIdleLossMapReader.Create(pto.PTOLossMap),
				};
				if (pto.PTOCycleDuringStop != null) {
					ptoData.PTOCycle = DrivingCycleDataReader.ReadFromDataTable(pto.PTOCycleDuringStop, "PTO", false);

				}
				return ptoData;
			}

			return null;
		}

		public IAuxiliaryConfig CreateBusAuxiliariesData(IAuxiliariesEngineeringInputData auxInputData,
			VehicleData vehicleData, VectoSimulationJobType jobType)
		{
			if (auxInputData == null || auxInputData.BusAuxiliariesData == null) {
				return null;
			}

			var busAux = auxInputData.BusAuxiliariesData;
			return jobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.Multiple_PEV, VectoSimulationJobType.Multiple_FCHV)
				? GetBatteryElectricBusAuxiliariesData(vehicleData, busAux)
				: GetBusAuxiliariesData(vehicleData, busAux);
		}

		private IAuxiliaryConfig GetBusAuxiliariesData(VehicleData vehicleData, IBusAuxiliariesEngineeringData busAux)
		{
			var retVal = new AuxiliaryConfig() {
				//InputData = auxInputData.BusAuxiliariesData,
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					//StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
					ResultCardIdle = new DummyResultCard(),
					ResultCardOverrun = new DummyResultCard(),
					ResultCardTraction = new DummyResultCard(),
					AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
					DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
					AlternatorMap = new SimpleAlternator(busAux.ElectricSystem.AlternatorEfficiency),
					AlternatorType =
						busAux.ElectricSystem.ESSupplyFromHEVREESS &&
						busAux.ElectricSystem.AlternatorType != AlternatorType.Smart
							? AlternatorType.None
							: busAux.ElectricSystem.AlternatorType,
					ConnectESToREESS = busAux.ElectricSystem.ESSupplyFromHEVREESS,
					DCDCEfficiency = busAux.ElectricSystem.DCDCConverterEfficiency.LimitTo(0, 1),
					MaxAlternatorPower = busAux.ElectricSystem.MaxAlternatorPower,
					ElectricStorageCapacity = busAux.ElectricSystem.ElectricStorageCapacity ?? 0.SI<WattSecond>(),
					StoredEnergyEfficiency = busAux.ElectricSystem.ElectricStorageEfficiency,
					ElectricalConsumers = GetElectricConsumers(busAux.ElectricSystem)
				},
				PneumaticAuxiliariesConfig = new PneumaticsConsumersDemand() {
					AdBlueInjection = 0.SI<NormLiterPerSecond>(),
					AirControlledSuspension = busAux.PneumaticSystem.AverageAirConsumed,
					Braking = 0.SI<NormLiterPerKilogram>(),
					BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
					DeadVolBlowOuts = 0.SI<PerSecond>(),
					DeadVolume = 0.SI<NormLiter>(),
					NonSmartRegenFractionTotalAirDemand = 0,
					SmartRegenFractionTotalAirDemand = 0,
					OverrunUtilisationForCompressionFraction =
						Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
					DoorOpening = 0.SI<NormLiter>(),
					StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
				},
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
					CompressorMap =
						new CompressorMap(CompressorMapReader.Create(busAux.PneumaticSystem.CompressorMap, 1.0),
							"engineering mode", busAux.PneumaticSystem.CompressorMap.Source),
					CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
					CompressorGearRatio = busAux.PneumaticSystem.GearRatio,
					SmartAirCompression = busAux.PneumaticSystem.SmartAirCompression,
					SmartRegeneration = false,
					KneelingHeight = 0.SI<Meter>(),
					AirSuspensionControl = ConsumerTechnology.Pneumatically,
					AdBlueDosing = ConsumerTechnology.Electrically,
					Doors = ConsumerTechnology.Electrically
				},
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				SSMInputsCooling = new SSMEngineeringInputs() {
					MechanicalPower = busAux.HVACData.MechanicalPowerDemand,
					ElectricPower = busAux.HVACData.ElectricalPowerDemand,
					AuxHeaterPower = busAux.HVACData.AuxHeaterPower,
					HeatingDemand = busAux.HVACData.AverageHeatingDemand,
					AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
					FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
					CoolantHeatTransferredToAirCabinHeater =
						Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				},
				VehicleData = vehicleData,
			};
			retVal.SSMInputsHeating = retVal.SSMInputsCooling;
			return retVal;
		}

		private IAuxiliaryConfig GetBatteryElectricBusAuxiliariesData(VehicleData vehicleData, IBusAuxiliariesEngineeringData busAux)
		{
			var retVal = new AuxiliaryConfig() {
				//InputData = auxInputData.BusAuxiliariesData,
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					//StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
					ResultCardIdle = new DummyResultCard(),
					ResultCardOverrun = new DummyResultCard(),
					ResultCardTraction = new DummyResultCard(),
					AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
					DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
					AlternatorMap = new SimpleAlternator(1),
					AlternatorType = AlternatorType.None,
					ConnectESToREESS = true,
					DCDCEfficiency = busAux.ElectricSystem.DCDCConverterEfficiency.LimitTo(0, 1),
					MaxAlternatorPower = 0.SI<Watt>(),
					ElectricStorageCapacity =  0.SI<WattSecond>(),
					StoredEnergyEfficiency = 1,
					ElectricalConsumers = GetElectricConsumers(busAux.ElectricSystem)
				},
				PneumaticAuxiliariesConfig = new PneumaticsConsumersDemand() {
					AdBlueInjection = 0.SI<NormLiterPerSecond>(),
					AirControlledSuspension = 0.SI<NormLiterPerSecond>(),
					Braking = 0.SI<NormLiterPerKilogram>(),
					BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
					DeadVolBlowOuts = 0.SI<PerSecond>(),
					DeadVolume = 0.SI<NormLiter>(),
					NonSmartRegenFractionTotalAirDemand = 0,
					SmartRegenFractionTotalAirDemand = 0,
					OverrunUtilisationForCompressionFraction =
						Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
					DoorOpening = 0.SI<NormLiter>(),
					StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
				},
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
					CompressorMap = new CompressorMap(new List<CompressorMapValues>() {
							new CompressorMapValues(0.RPMtoRad(), 1.SI<NormLiterPerSecond>() , 1.SI<Watt>(), 0.SI<Watt>()),
							new CompressorMapValues(1e12.RPMtoRad(), 1.SI<NormLiterPerSecond>() , 1.SI<Watt>(), 0.SI<Watt>())
						}, 
						"engineering mode", "none"),
					CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
					CompressorGearRatio = 0,
					SmartAirCompression = false,
					SmartRegeneration = false,
					KneelingHeight = 0.SI<Meter>(),
					AirSuspensionControl = ConsumerTechnology.Pneumatically,
					AdBlueDosing = ConsumerTechnology.Electrically,
					Doors = ConsumerTechnology.Electrically
				},
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				SSMInputsCooling = new SSMEngineeringInputs() {
					MechanicalPower = 0.SI<Watt>(),
					ElectricPower = 0.SI<Watt>(),
					AuxHeaterPower = 0.SI<Watt>(),
					HeatingDemand = 0.SI<WattSecond>(),
					AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
					FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
					CoolantHeatTransferredToAirCabinHeater =
						Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				},
				VehicleData = vehicleData,
			};
			retVal.SSMInputsHeating = retVal.SSMInputsCooling;
			return retVal;
		}

		

		private Dictionary<string, ElectricConsumerEntry> GetElectricConsumers(IBusAuxElectricSystemEngineeringData busAuxElectricSystem)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();

			var iBase = busAuxElectricSystem.CurrentDemandEngineOffStandstill;
			var iSP = busAuxElectricSystem.CurrentDemandEngineOffDriving -
					busAuxElectricSystem.CurrentDemandEngineOffStandstill;
			var iFan = busAuxElectricSystem.CurrentDemand - busAuxElectricSystem.CurrentDemandEngineOffDriving;

			retVal["BaseLoad"] = new ElectricConsumerEntry() {
				Current = iBase,
				BaseVehicle = true
			};
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry() {
				Current = iSP,
				ActiveDuringEngineStopStandstill = false,
			};
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry() {
				Current = iFan,
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
			};
			return retVal;
		}


		public ShiftStrategyParameters CreateGearshiftData(GearboxType gbxType, IGearshiftEngineeringInputData gsInputData, double axleRatio, PerSecond engineIdlingSpeed)
		{
			if (gsInputData == null) {
				return null;
			}

			var retVal = new ShiftStrategyParameters {
				TorqueReserve = gsInputData.TorqueReserve,
				StartTorqueReserve = gsInputData.StartTorqueReserve,
				TimeBetweenGearshifts = gsInputData.MinTimeBetweenGearshift,
				StartSpeed = gsInputData.StartSpeed,
				DownshiftAfterUpshiftDelay = gsInputData.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = gsInputData.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = gsInputData.UpshiftMinAcceleration,

				StartAcceleration = gsInputData.StartAcceleration ?? DeclarationData.GearboxTCU.StartAcceleration,
				RatingFactorCurrentGear = gsInputData.RatingFactorCurrentGear ?? (gbxType.AutomaticTransmission()
											? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
											: DeclarationData.GearboxTCU.RatingFactorCurrentGear),

				RatioEarlyUpshiftFC = (gsInputData.RatioEarlyUpshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyUpshiftFC) / axleRatio,
				RatioEarlyDownshiftFC = (gsInputData.RatioEarlyDownshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyDownshiftFC) / axleRatio,
				AllowedGearRangeFC = gsInputData.AllowedGearRangeFC ?? (gbxType.AutomaticTransmission() ? DeclarationData.GearboxTCU.AllowedGearRangeFCAT : DeclarationData.GearboxTCU.AllowedGearRangeFCAMT),
				VelocityDropFactor = gsInputData.VeloictyDropFactor ?? DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = gsInputData.AccelerationFactor ?? DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = gsInputData.MinEngineSpeedPostUpshift ?? DeclarationData.GearboxTCU.MinEngineSpeedPostUpshift,
				ATLookAheadTime = gsInputData.ATLookAheadTime ?? DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsDown = gsInputData.LoadStageThresholdsDown?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				LoadStageThresoldsUp = gsInputData.LoadStageThresholdsUp?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				ShiftSpeedsTCToLocked = engineIdlingSpeed == null ? null : (gsInputData.ShiftSpeedsTCToLocked ?? DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked).Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),

				// voith gs parameters

				GearshiftLines = gsInputData.LoadStageShiftLines,

				LoadstageThresholds = gsInputData.LoadStageThresholdsUp != null && gsInputData.LoadStageThresholdsDown != null ? gsInputData.LoadStageThresholdsUp.Zip(gsInputData.LoadStageThresholdsDown, Tuple.Create) : null
			};

			if (gsInputData.PEV_DeRatingDownshiftSpeedFactor != null) {
				retVal.PEV_DeRatedDownshiftSpeedFactor = gsInputData.PEV_DeRatingDownshiftSpeedFactor.Value;
			}

			if (gsInputData.PEV_DownshiftSpeedFactor != null) {
				retVal.PEV_DownshiftSpeedFactor = gsInputData.PEV_DownshiftSpeedFactor.Value;
			}

			if (gsInputData.PEV_TargetSpeedBrakeNorm != null) {
				retVal.PEV_TargetSpeedBrakeNorm = gsInputData.PEV_TargetSpeedBrakeNorm.Value;
			}

			if (gsInputData.PEV_DownshiftMinSpeedFactor != null) {
				retVal.PEV_DownshiftMinSpeedFactor = gsInputData.PEV_DownshiftMinSpeedFactor.Value;
			}

			return retVal;
		}

		public BatterySystemData CreateBatteryData(IElectricStorageSystemEngineeringInputData batteryInputData, double initialSOC)
		{
			if (batteryInputData == null) {
				return null;
			}

			var bat = batteryInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.Battery).ToArray();

			if (bat.Length == 0) {
				return null;
			}

			var addJunctionBoxResistance = false;
			var addConnectorSystemResistance = false;
            var retVal = new BatterySystemData();
			var batteryCount = 0;
			foreach (var entry in bat) {
				var b = entry.REESSPack as IBatteryPackDeclarationInputData;
				if (b == null) {
					continue;
				}

				if (b.JunctionboxIncluded != null && !b.JunctionboxIncluded.Value) {
					addJunctionBoxResistance = true;
				}

				if (b.ConnectorsSubsystemsIncluded != null && !b.ConnectorsSubsystemsIncluded.Value) {
					addConnectorSystemResistance = true;
				}
                for (var i = 0; i < entry.Count; i++) {
					retVal.Batteries.Add(Tuple.Create(entry.StringId, new BatteryData() {
						MinSOC = b.MinSOC.Value,
						MaxSOC = b.MaxSOC.Value,
						MaxCurrent = BatteryMaxCurrentReader.Create(b.MaxCurrentMap),
						Capacity = b.Capacity,
						InternalResistance =
							BatteryInternalResistanceReader.Create(b.InternalResistanceCurve, false),
						SOCMap = BatterySOCReader.Create(b.VoltageCurve),
						BatteryId = batteryCount++,
					}));
				}
			}
			if (addJunctionBoxResistance) {
				retVal.ConnectionSystemResistance += DeclarationData.Battery.JunctionBoxResistance;
			}

			if (addConnectorSystemResistance) {
				retVal.ConnectionSystemResistance += DeclarationData.Battery.CablesAndConnectorsResistance;
			}

            retVal.InitialSoC = initialSOC;
			return retVal;
		}

		public FuelCellPowerMap CreateFuelCellPowerMap(IModalDataContainer modData,
			FuelCellSystemData fcData,
			BatterySystemData batSystemData)
		{
			return CreateDynamicFuelCellPowerMap(modData, fcData, batSystemData);

			//return CreateStaticFuelCellPowerMap(modData);
		}

		public FuelCellSystemShareMap CreateFuelCellShareMap(FuelCellSystemData fuelCellSystemData)
		{
			var fcSystemMassFlowMap = new FuelCellSystemMassFlowMap(fuelCellSystemData.FuelCellStrings.ElementAt(0).MassFlowMap,
				fuelCellSystemData.FuelCellStrings.ElementAtOrDefault(1)?.MassFlowMap);
			return new FuelCellSystemShareMap(fcSystemMassFlowMap);
		}

        private FuelCellPowerMap CreateDynamicFuelCellPowerMap(IModalDataContainer modData, FuelCellSystemData fcData, BatterySystemData batData)
		{
			var fcPostProcessor = new FuelCellPreRunPostprocessor(modData) {
				WriterBasePath = JobFilePath
			};
			fcData.PreRunPostProcessing = fcPostProcessor;


			var result = fcPostProcessor.CalculateFuelCellPowerDemand(fcData, batData.Clone(), modData.WriteModalResults);
			//Debug($"Window distance = {result.Distance}, SoC = {result.InitSoc}");
			batData.InitialSoC = result.InitSoc;
			return new FuelCellPowerMap(result.Entries);
		}

		[Obsolete]
		private static FuelCellPowerMap CreateStaticFuelCellPowerMap(IModalDataContainer modData)
		{
			//Constant power for the whole cycle
			var p_reess_terminal_dt = modData.GetValues(x => new {
				p_reess_terminal = x.Field<Watt>(ModalResultField.P_reess_terminal.GetName()),
				dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
			});


			var constantPower = p_reess_terminal_dt.Sum(x => x.p_reess_terminal * x.dt) /
								p_reess_terminal_dt.Sum(x => x.dt);


			var distanceValues = modData.GetValues(x => x.Field<Meter>(ModalResultField.dist.GetName()));
			var entries = new List<FuelCellPowerMap.FuelCellPowerMapEntry>();
			foreach (var dist in distanceValues) {
				entries.Add(new FuelCellPowerMap.FuelCellPowerMapEntry() {
					Distance = dist,
					Power = -constantPower,
				});
			}

			return new FuelCellPowerMap(entries.ToArray());
		}


		public FuelCellSystemData CreateFuelCellSystemData(IFuelCellSystemEngineeringInputData fuelCellSystemInputData)
		{
			if (fuelCellSystemInputData.FuelCellStrings.Count > 2) {
				throw new VectoException("Number of fuel cell strings must be <= 2");
			}

			if (fuelCellSystemInputData.FuelCellStrings.Aggregate(0, (a, fcs) => a + fcs.Count) < 1) {
				throw new VectoException("At least one fuel cell has to be provided");
			}

			if (fuelCellSystemInputData.FuelCellStrings.Any(fcs => fcs.Count > 3)) {
				throw new VectoException("Number of fuel cells per string must be <= 3");
			}
			
			var fuelCellSystemData = new FuelCellSystemData();
			fuelCellSystemData.MaxWindowSize = fuelCellSystemInputData.MaxWindowSize;
			fuelCellSystemData.FuelCellStrings = new List<FuelCellStringData>();
			var id = 0;
			foreach (var fcC in fuelCellSystemInputData.FuelCellStrings) {
				fuelCellSystemData.FuelCellStrings.Add(CreateFuelCellStringData(fcC.FuelCellComponent, fcC.Count));
			}

			return fuelCellSystemData;
		}

		public FuelCellStringData CreateFuelCellStringData(IFuelCellComponentEngineeringInputData fuelCellComponent, int count)
		{
			var fcData = CreateFuelCellData(fuelCellInputData: fuelCellComponent);
			var fuelCellStringData = new FuelCellStringData(fcData, count);
			fuelCellStringData.MassFlowMap = new FuelCellStringMassFlowMap(fcData.MassFlowMap, count);
			return fuelCellStringData;
		}


		public FuelCellData CreateFuelCellData(IFuelCellComponentEngineeringInputData fuelCellInputData)
		{
			var fuelCellData =  new FuelCellData() {
				MassFlowMap = FuelCellMassFlowMapReader.Create(fuelCellInputData.MassFlowMap, fuelCellInputData.MinElectricPower, fuelCellInputData.MaxElectricPower),
				MaxElectricPower = fuelCellInputData.MaxElectricPower,
				MinElectricPower = fuelCellInputData.MinElectricPower,
				//Id = new FuelCellData.FuelCellId() {
				//	Id = id, 
				//	SubId = subId,
				//}
			};

			if (fuelCellData.MinElectricPower.IsSmaller(fuelCellData.MassFlowMap.MinPowerMap) ||
				fuelCellData.MaxElectricPower.IsGreater(fuelCellData.MassFlowMap.MaxPowerMap)) {
				throw new VectoException("Fuel cell limits exceed mass flow map");
			}

			return fuelCellData;
		}

		public BatterySystemData CreateFuelCellPreProcessingBattery(
			FuelCellSystemData fuelCellSystemData,
			BatterySystemData batterySystemData,
			out Tuple<int, BatteryData> fuelCellBattery)
		{
			Watt fcP = fuelCellSystemData.FuelCellStrings.Sum(fc => fc.MaxPower * fc.FcCount);

			return CreateFuelCellPreProcessingBattery(fcP, batterySystemData, out fuelCellBattery);
		}

		public BatterySystemData CreateFuelCellPreProcessingBattery(
			IFuelCellSystemEngineeringInputData fuelCellSystemInputData, BatterySystemData batterySystemData, out Tuple<int, BatteryData> fuelCellBattery)
		{
			var fcP = fuelCellSystemInputData.FuelCellStrings.Sum(fc => fc.FuelCellComponent.MaxElectricPower * fc.Count);

			return CreateFuelCellPreProcessingBattery(fcP, batterySystemData, out fuelCellBattery);
        }

		public BatterySystemData CreateFuelCellPreProcessingBattery(
			Watt fuelCellStringSumPower, BatterySystemData batterySystemData, out Tuple<int, BatteryData> fuelCellBattery)
		{
			var pevBat = batterySystemData;
			pevBat.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
			var fcP = fuelCellStringSumPower;
			var V = pevBat.CalculateVoltageCenterSoc();
			var I = fcP / V;

			var resistance = 1E-12.SI<Ohm>();

			var batteryData = new BatteryData()
			{
				BatteryId = FuelCellSystemData.FuelCellBatID,
				ChargeDepletingBattery = true,
				MinSOC = 0,
				MaxSOC = 1,
				InputData = null,
				Capacity = 1E5.SI<AmpereSecond>(), //?
				MaxCurrent = new MaxCurrentMap(new[] {
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 0,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					},
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 0.5,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					},
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 1,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					}
				}),
				SOCMap = new SOCMap(new[] {
					new SOCMap.SOCMapEntry() {
						SOC = 0,
						BatteryVolts = V
					},
					new SOCMap.SOCMapEntry() {
						SOC = 0.5,
						BatteryVolts = V
					},
					new SOCMap.SOCMapEntry(){
						SOC = 1,
						BatteryVolts = V
					}
				}),
				InternalResistance = new InternalResistanceMap(new[] {
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 0,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					},
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 0.5,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					},
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 1,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					}
				})
			};
			fuelCellBattery = Tuple.Create(0xFCB, batteryData);
			batterySystemData.Batteries.Add(fuelCellBattery);
			return batterySystemData;
		}

		public SuperCapData CreateSuperCapData(IElectricStorageSystemEngineeringInputData reessInputData, double initialSOC)
		{
			if (reessInputData == null)
			{
				return null;
			}

			var superCaps = reessInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.SuperCap).ToArray();

			var superCap = superCaps.FirstOrDefault()?.REESSPack as ISuperCapDeclarationInputData;

			if (superCap == null) {
				return null;
			}

			return new SuperCapData()
			{
				Capacity = superCaps.First().Count * superCap.Capacity,
				InternalResistance = superCap.InternalResistance / superCaps.First().Count,
				MinVoltage = superCap.MinVoltage,
				MaxVoltage = superCap.MaxVoltage,
				MaxCurrentCharge = superCap.MaxCurrentCharge,
				MaxCurrentDischarge = -superCap.MaxCurrentDischarge,
				InitialSoC = initialSOC
			};
		}

		public IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
			IElectricMachinesEngineeringInputData electricMachines,
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
			GearList gearlist = null)
		{
			if (electricMachines == null) {
				return null;
			}

			if (electricMachines.Entries.Any(x => x.ElectricMachine.SavedInDeclarationMode)) {
				WarnEngineeringMode("Electric motor");
			}

			if (electricMachines.Entries.Select(x => x.Position).Where(x => x != PowertrainPosition.GEN).Distinct().Count() > 1) {
				throw new VectoException("multiple electric propulsion motors are not supported at the moment");
			}

			return electricMachines.Entries
				.Select(x => Tuple.Create(x.Position,
					CreateElectricMachine(x.Position, x.ElectricMachine, x.Count, x.RatioADC, x.RatioPerGear, x.MechanicalTransmissionEfficiency,
						x.MechanicalTransmissionLossMap, torqueLimits?.First(t =>t.Key.Position == x.Position).Value, averageVoltage, gearlist))).ToList();
		}

		public ElectricMotorData CreateElectricMachine(PowertrainPosition powertrainPosition,
			IElectricMotorEngineeringInputData motorData, int count,
			double ratio, double[] ratioPerGear, double efficiency, TableData adcLossMap,
			IList<Tuple<Volt, TableData>> torqueLimits, Volt averageVoltage, GearList gearList)
		{
			var voltageLevels = new List<ElectricMotorVoltageLevelData>();

			foreach (var entry in motorData.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
				var fullLoadCurve = ElectricFullLoadCurveReader.Create(entry.FullLoadCurve.First().LoadCurve, count);
				var maxTorqueCurve = torqueLimits == null
					? null
					: ElectricFullLoadCurveReader.Create(
						torqueLimits.Where(x => x.Item1 == null || x.Item1.IsEqual(entry.VoltageLevel)).FirstOrDefault()?.Item2, count);

				var fullLoadCurveCombined = IntersectEMFullLoadCurves(fullLoadCurve, maxTorqueCurve);

				var vLevelData = powertrainPosition == PowertrainPosition.IHPC
					? CreateIHPCVoltageLevelData(count, entry, fullLoadCurveCombined, gearList)
					: CreateEmVoltageLevelData(count, entry, fullLoadCurveCombined);
				voltageLevels.Add(vLevelData);
				
			}

			if (averageVoltage == null) {
				// if no average voltage is provided (e.g. for supercap) use mean value of measured voltage maps
				averageVoltage = (voltageLevels.Min(x => x.Voltage) + voltageLevels.Max(x => x.Voltage)) / 2.0;
			}

			var lossMap = powertrainPosition == PowertrainPosition.IHPC
				? TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC IHPC LossMap Eff")
				: adcLossMap != null
					? TransmissionLossMapReader.CreateEmADCLossMap(adcLossMap, ratio, "EM ADC LossMap", true)
					: TransmissionLossMapReader.CreateEmADCLossMap(efficiency, ratio, "EM ADC LossMap Eff");

			var retVal = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels},
				EMDragCurve = ElectricMotorDragCurveReader.Create(motorData.DragCurve, count),
				Inertia = motorData.Inertia * count,
				OverloadRecoveryFactor = motorData.OverloadRecoveryFactor,
				RatioADC = ratio,
				RatioPerGear = ratioPerGear,
				TransmissionLossMap = lossMap,
			};
			retVal.Overload = CalculateOverloadData(motorData, count, retVal.EfficiencyData, averageVoltage);
			return retVal;
		}

		private ElectricMotorVoltageLevelData CreateIHPCVoltageLevelData(int count, IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined, GearList gearList)
		{
			if (gearList == null) {
				throw new VectoException("no gears provided for IHPC EM");
			}

			if (gearList.Count() != entry.PowerMap.Count) {
				throw new VectoException(
					$"number of gears in transmission does not match gears in electric motor (IHPC) - {gearList.Count()}/{entry.PowerMap.Count}");
			}
			var effMap = new Dictionary<uint, EfficiencyMap>();
			foreach (var gear in gearList) {
				effMap.Add(gear.Gear, ElectricMotorMapReader.Create(entry.PowerMap[(int)gear.Gear - 1].PowerMap, count, ExecutionMode.Engineering));
			}
			return new IEPCVoltageLevelData() {
				Voltage = entry.VoltageLevel,
				FullLoadCurve = fullLoadCurveCombined,
				EfficiencyMaps = effMap,
			};
		}

		private static ElectricMotorVoltageLevelData CreateEmVoltageLevelData(int count, IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined)
		{
			return new ElectricMotorVoltageLevelData() {
				Voltage = entry.VoltageLevel,
					
				FullLoadCurve = fullLoadCurveCombined,
				// DragCurve = ElectricMotorDragCurveReader.Create(entry.DragCurve, count),
				EfficiencyMap = ElectricMotorMapReader.Create(entry.PowerMap.First().PowerMap, count, ExecutionMode.Engineering), //PowerMap
			};
		}

		private OverloadData CalculateOverloadData(IElectricMotorEngineeringInputData motorData, int count, VoltageLevelData voltageLevels, Volt averageVoltage)
		{
			
			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < motorData.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.First(), count, voltageLevels);
			}
			if (averageVoltage > motorData.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.Last(), count, voltageLevels);
			}

			var (vLow, vHigh) = motorData.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevels);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevels);

			var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

		private OverloadData CalculateOverloadData(IIEPCEngineeringInputData iepc, int count,
			VoltageLevelData voltageLevels, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
		{
			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < iepc.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.First(), count, voltageLevels, gearRatioUsedForMeasurement);
			}
			if (averageVoltage > iepc.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.Last(), count, voltageLevels, gearRatioUsedForMeasurement);
			}

			var (vLow, vHigh) = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevels, gearRatioUsedForMeasurement);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevels, gearRatioUsedForMeasurement);

			var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

		private OverloadData CalculateOverloadBuffer(IElectricMotorVoltageLevel voltageEntry,
			int count, VoltageLevelData voltageLevels, Tuple<uint, double> gearUsedForMeasurement = null)
		{
			var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
			var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
			var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
			var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
			var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
			var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;

            //ElectricMotorRatedSpeedHelper.GetRatedSpeed(voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque)
            var FullLoadCurve = voltageLevels.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve;
            // FullLoadCurve express torque and speed at the rotor level, not the output shaft

            //var FullLoadEntries = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries; 
            var continuousPower = continuousTorque * continuousTorqueSpeed;

            var continuousTorqueSpeedRef = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque));
            var continuousTorqueSpeedRefGen = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullGenerationTorque));

            var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
            var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

            var peakElPwr = voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, overloadTestSpeed, -overloadTorque, gear, true)
				.ElectricalPower;
			var peakPwrLoss = -peakElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive
			
			var contElPwr = voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								-continuousTorque, gear).ElectricalPower ??
							voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								voltageLevels.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear),
								gear, true).ElectricalPower;
			var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive
			var overloadBuffer = VectoMath.Max(peakPwrLoss - continuousPowerLoss, continuousPowerLoss * 0.0001) * voltageEntry.OverloadTime;

            return new OverloadData() {
				OverloadBuffer = overloadBuffer,
				ContinuousTorque = continuousTorque,
                ContinuousTorqueGen = continuousTorqueRefGen,
                ContinuousPower = continuousPower,
                ContinuousPowerLoss = continuousPowerLoss
			};
		}

		public HybridStrategyParameters CreateHybridStrategyParameters(
			IEngineeringJobInputData jobInputData,
			CombustionEngineData combustionEngineData, GearboxData gearboxData)
		{
			var hybridStrategyParameters = jobInputData.HybridStrategyParameters;
			
			var torqueLimit = CreateMaxPropulsionTorque(jobInputData.Vehicle, combustionEngineData, gearboxData);

			var retVal = new HybridStrategyParameters() {
				EquivalenceFactorDischarge = hybridStrategyParameters.EquivalenceFactorDischarge,
				EquivalenceFactorCharge = hybridStrategyParameters.EquivalenceFactorCharge,
				MinSoC = hybridStrategyParameters.MinSoC,
				MaxSoC = hybridStrategyParameters.MaxSoC,
				TargetSoC = hybridStrategyParameters.TargetSoC,
				MinICEOnTime = hybridStrategyParameters.MinimumICEOnTime,
				AuxReserveTime = hybridStrategyParameters.AuxBufferTime,
				AuxReserveChargeTime = hybridStrategyParameters.AuxBufferChargeTime,
				MaxPropulsionTorque = torqueLimit,
				ICEStartPenaltyFactor = hybridStrategyParameters.ICEStartPenaltyFactor,
				CostFactorSOCExponent = double.IsNaN(hybridStrategyParameters.CostFactorSOCExpponent) ? 5 : hybridStrategyParameters.CostFactorSOCExpponent,
				GensetMinOptPowerFactor = double.IsNaN(hybridStrategyParameters.GensetMinOptPowerFactor) ? 0 : hybridStrategyParameters.GensetMinOptPowerFactor,
			};
			return retVal;
		}

		protected internal static Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque> CreateMaxPropulsionTorque(IVehicleEngineeringInputData vehicleInputData, CombustionEngineData engineData, GearboxData gearboxData)
		{

			// engine data contains full-load curves already cropped with max gearbox torque and max ICE torque (vehicle level)

			var maxBoostingTorque = vehicleInputData.BoostingLimitations;
			var offset = maxBoostingTorque == null ? null : MaxBoostingTorqueReader.Create(maxBoostingTorque);
			var belowIdle = offset?.FullLoadEntries.Where(x => x.MotorSpeed < engineData.IdleSpeed).ToList();

			var retVal = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();
			var isP3OrP4Hybrid = vehicleInputData.Components.ElectricMachines.Entries.Select(x => x.Position)
				.Any(x => x == PowertrainPosition.HybridP3 || x == PowertrainPosition.HybridP4);
			var isAtGearbox = gearboxData?.Type.IsOneOf(GearboxType.ATSerial, GearboxType.ATPowerSplit) ?? false;
			foreach (var key in engineData.FullLoadCurves.Keys) {
				if (key == 0) {
					continue;
				}
				if (maxBoostingTorque == null) {
					if (gearboxData.Gears[key].MaxTorque == null) {
						continue;
					}
					// don't know what to do...
					// idea 1: apply gearbox limit for whole speed range
					// idea 2: use em max torque as boosting limitation
					var gbxLimit = new[] {
						new VehicleMaxPropulsionTorque.FullLoadEntry()
							{ MotorSpeed = 0.RPMtoRad(), FullDriveTorque = gearboxData.Gears[key].MaxTorque },
						new VehicleMaxPropulsionTorque.FullLoadEntry() {
							MotorSpeed = engineData.FullLoadCurves[0].N95hSpeed * 1.1,
							FullDriveTorque = gearboxData.Gears[key].MaxTorque
						}
					}.ToList();
					var bKey = isAtGearbox
						? new GearshiftPosition(key, true)
						: new GearshiftPosition(key);
					if (isAtGearbox && gearboxData.Gears[key].HasTorqueConverter) {
						retVal[new GearshiftPosition(key, false)] = new VehicleMaxPropulsionTorque(gbxLimit);
                    }
					retVal[bKey] = new VehicleMaxPropulsionTorque(gbxLimit);
                    continue;
				} 

				// case boosting limit is defined, gearbox limit can be defined or not (handled in Intersect method)

				// entries contains ICE full-load curve with the boosting torque added. handles ICE speeds below idle
				var entries = belowIdle.Select(fullLoadEntry => new VehicleMaxPropulsionTorque.FullLoadEntry()
						{ MotorSpeed = fullLoadEntry.MotorSpeed, FullDriveTorque = fullLoadEntry.FullDriveTorque })
					.Concat(
						engineData.FullLoadCurves[key].FullLoadEntries.Where(x => x.EngineSpeed > engineData.IdleSpeed)
							.Select(fullLoadCurveEntry =>
								new VehicleMaxPropulsionTorque.FullLoadEntry() {
									MotorSpeed = fullLoadCurveEntry.EngineSpeed,
									FullDriveTorque = fullLoadCurveEntry.TorqueFullLoad +
													VectoMath.Max(
														offset?.FullLoadDriveTorque(fullLoadCurveEntry.EngineSpeed),
														0.SI<NewtonMeter>())
								}))
					.Concat(
						new[] { engineData.IdleSpeed, engineData.IdleSpeed - 0.1.RPMtoRad() }.Select(x =>
							new VehicleMaxPropulsionTorque.FullLoadEntry() {
								MotorSpeed = x,
								FullDriveTorque = engineData.FullLoadCurves[0].FullLoadStationaryTorque(x) +
												VectoMath.Max(offset?.FullLoadDriveTorque(x),
													0.SI<NewtonMeter>())
							}))
					.OrderBy(x => x.MotorSpeed).ToList();

				// if no gearbox limit is defined, MaxTorque is null;
				// in case of P3 or P4, do not apply gearbox limit to propulsion limit as ICE is already cropped with max torque
				var gearboxTorqueLimit = isP3OrP4Hybrid ? null : gearboxData.Gears[key].MaxTorque;
				var dKey = isAtGearbox
					? new GearshiftPosition(key, true)
					: new GearshiftPosition(key);
				if (isAtGearbox && gearboxData.Gears[key].HasTorqueConverter) {
					retVal[new GearshiftPosition(key, false)] =
						new VehicleMaxPropulsionTorque(IntersectMaxPropulsionTorqueCurve(entries, gearboxTorqueLimit));
				}
                retVal[dKey] = new VehicleMaxPropulsionTorque(IntersectMaxPropulsionTorqueCurve(entries, gearboxTorqueLimit));

            }

			return retVal;
		}

		public List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCEngineeringInputData iepc, Volt averageVoltage)
		{
			if (iepc == null) {
				return null;
			}

			var pos = PowertrainPosition.IEPC;
			var count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;

			// the full-load curve is measured in the gear with the ratio closest to 1,
			// in case two gears have the same difference, the higher one is used
			var gearRatioUsedForMeasurement = iepc.Gears
				.Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
				.OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();
			

			var voltageLevels = new List<ElectricMotorVoltageLevelData>();
			foreach (var entry in iepc.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
				var effMap = new Dictionary<uint, EfficiencyMap>();
				var fldCurve =
					IEPCFullLoadCurveReader.Create(entry.FullLoadCurve.First().LoadCurve, count, gearRatioUsedForMeasurement.Ratio);

                var fldCurves = new Dictionary<uint, ElectricMotorFullLoadCurve>();
                foreach (var curve in entry.FullLoadCurve.Where(x => x.Gear > 0))
                {
                    var ratio = iepc.Gears.First(x => x.GearNumber == curve.Gear).Ratio;
                    fldCurves.Add((uint)curve.Gear, IEPCFullLoadCurveReader.Create(curve.LoadCurve, count, ratio));
                }

                for (var i = 0u; i < entry.PowerMap.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					effMap.Add(i + 1, IEPCMapReader.Create(entry.PowerMap[(int)i].PowerMap, count, ratio, fldCurve ?? fldCurves[i + 1], ExecutionMode.Engineering));
				}

                voltageLevels.Add(new IEPCVoltageLevelData() {
					Voltage = entry.VoltageLevel,
					FullLoadCurve = fldCurve,
					FullLoadCurves = fldCurves,
					EfficiencyMaps = effMap,
				});
			}

			var dragCurves = new Dictionary<uint, DragCurve>();
			if (iepc.DragCurves.Count > 1) {
				for (var i = 0u; i < iepc.DragCurves.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(iepc.DragCurves[(int)i].DragCurve, count, ratio));
				}
			} else {
				var dragCurve = iepc.DragCurves.First().DragCurve;
				for (var i = 0u; i < iepc.Gears.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(dragCurve, count, ratio));
				}
			}

			var retVal = new IEPCElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels },
				IEPCDragCurves = dragCurves,
				Inertia = iepc.Inertia * count,
				OverloadRecoveryFactor = iepc.OverloadRecoveryFactor,
				RatioADC = 1,
				RatioPerGear = null,
				TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC LossMap Eff"),
				
			};
			retVal.Overload = CalculateOverloadData(iepc, count, retVal.EfficiencyData, averageVoltage,
				Tuple.Create((uint)gearRatioUsedForMeasurement.GearNumber, gearRatioUsedForMeasurement.Ratio));
			;
			return new List<Tuple<PowertrainPosition, ElectricMotorData>>() { Tuple.Create<PowertrainPosition, ElectricMotorData>(pos, retVal) };
		}




		public GearboxData CreateIEPCGearboxData(IEngineeringInputDataProvider inputData, VectoRunData runData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var iepc = vehicle.Components.IEPCEngineeringInputData;

			var axlegearRatio = runData.AxleGearData?.AxleGear.Ratio ?? 1.0; 
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;


			var retVal = new GearboxData() {
				Type = GearboxType.APTN,
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
				InputData = new IEPCGearboxInputData(iepc),
			};

			var gearInput = iepc.Gears.Select((x, idx) => new TransmissionInputData() {
				Gear = idx + 1,
				Ratio = x.Ratio,
				MaxInputSpeed = x.MaxOutputShaftSpeed == null ? null : x.MaxOutputShaftSpeed * x.Ratio,
				MaxTorque = x.MaxOutputShaftTorque == null ? null : x.MaxOutputShaftTorque / x.Ratio,
			}).Cast<ITransmissionInputData>().ToList();
			var gears = new Dictionary<uint, GearData>();

			retVal.ShiftStrategy =
				ShiftStrategyFactory?.GetShiftStrategyName(GearboxType.APTN, vehicle.VehicleType);
			var shiftPolygonCalc = ShiftStrategyFactory?.CreateShiftPolygonCalculator(retVal.ShiftStrategy, runData.GearshiftParameters);

			for (uint i = 0; i < iepc.Gears.Count; i++) {
				var gear = iepc.Gears[(int)i];
				var lossMap = TransmissionLossMapReader.Create(1, gear.Ratio, $"Gear{i+1}");

				ShiftPolygon shiftPolygon = null;
				if (iepc.Gears.Count > 1) {
					if (shiftPolygonCalc != null) {
						shiftPolygon = shiftPolygonCalc.ComputeDeclarationShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					} else {
						shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					}
				}
				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxOutputShaftSpeed == null ? null : gear.MaxOutputShaftSpeed * gear.Ratio,
					MaxTorque = gear.MaxOutputShaftTorque == null ? null : gear.MaxOutputShaftTorque / gear.Ratio,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;

			// update disengageWhenHaltingSpeed
			var firstGear = retVal.GearList.First(x => x.IsLockedGear());
			if (iepc.Gears.Count > 1 && retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any()) {
				var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
					.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
				var vehicleSpeedDisengage = downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
											dynamicTyreRadius;
				retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
			}

			return retVal;
		}

		public PTOData CreateBatteryElectricPTOTransmissionData(IPTOTransmissionInputData pto)
		{


			var ptoData = new PTOData() {
				TransmissionType = pto.PTOTransmissionType,
				LossMap = pto.PTOLossMap == null
					? PTOIdleLossMapReader.GetZeroLossMap()
					: PTOIdleLossMapReader.Create(pto.PTOLossMap),
				PTOCycle = pto.EPTOCycleDuringStop != null
					? DrivingCycleDataReader.ReadFromDataTable(pto.EPTOCycleDuringStop, "PTO", false)
					: null,
			};

			return ptoData;
		}

		internal VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};

			return retVal;
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData retarderInputData,
			PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
		{
			try
			{
				var retarder = new RetarderData { Type = retarderInputData.Type };

				switch (retarder.Type)
				{
					case RetarderType.TransmissionInputRetarder:
					case RetarderType.TransmissionOutputRetarder:
						if (!(position.IsParallelHybrid() || position.IsOneOf(PowertrainPosition.HybridPositionNotSet, PowertrainPosition.BatteryElectricE2)))
						{
							throw new ArgumentException("Transmission retarder is only allowed in powertrains that " +
														"contain a gearbox: Conventional, HEV-P, and PEV-E2.", nameof(retarder));
						}

						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;

					case RetarderType.AxlegearInputRetarder:
						if (position != PowertrainPosition.BatteryElectricE3)
							throw new ArgumentException("AxlegearInputRetarder is only allowed for PEV-E3, HEV-S3, S-IEPC, E-IEPC. ", nameof(retarder));
						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;

					case RetarderType.None:
					case RetarderType.LossesIncludedInTransmission:
					case RetarderType.EngineRetarder:
						retarder.Ratio = 1;
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(retarder), retarder.Type, "RetarderType unknown");
				}

				if (retarder.Type.IsDedicatedComponent())
				{
					retarder.SavedInDeclarationMode = retarderInputData.SavedInDeclarationMode;
					retarder.Manufacturer = retarderInputData.Manufacturer;
					retarder.ModelName = retarderInputData.Model;
					retarder.Date = retarderInputData.Date;
					retarder.CertificationMethod = retarderInputData.CertificationMethod;
					retarder.CertificationNumber = retarderInputData.CertificationNumber;
					retarder.DigestValueInput = retarderInputData.DigestValue != null ? retarderInputData.DigestValue.DigestValue : "";
				}

				return retarder;
			}
			catch (Exception e)
			{
				throw new VectoException("Error while Reading Retarder Data: {0}", e.Message);
			}
		}

		internal static GearboxData SetCommonGearboxData(IGearboxDeclarationInputData data)
		{
			return new GearboxData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationMethod != CertificationMethod.StandardValues ?
					data.CertificationNumber : "",
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				Type = data.Type
			};
		}

		protected static void CreateTCSecondGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected static void CreateTCFirstGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected virtual void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1, 1, $"TCGear {i + 1}");
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		/// <summary>
		/// Creates an AngledriveData or returns null if there is no anglegear.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="useEfficiencyFallback">if true, the Efficiency value is used if no LossMap is found.</param>
		/// <returns></returns>
		internal AngledriveData DoCreateAngledriveData(IAngledriveInputData data, bool useEfficiencyFallback)
		{
			try
			{
				var type = data?.Type ?? AngledriveType.None;

				switch (type)
				{
					case AngledriveType.LossesIncludedInGearbox:
					case AngledriveType.None:
						return null;
					case AngledriveType.SeparateAngledrive:
						var angledriveData = new AngledriveData
						{
							InputData = data,
							SavedInDeclarationMode = data.SavedInDeclarationMode,
							Manufacturer = data.Manufacturer,
							ModelName = data.Model,
							Date = data.Date,
							CertificationMethod = data.CertificationMethod,
							CertificationNumber = data.CertificationNumber,
							DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
							Type = type,
							Angledrive = new TransmissionData { Ratio = data.Ratio }
						};
						try
						{
							angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.LossMap,
								data.Ratio, "Angledrive", true);
						}
						catch (VectoException ex)
						{
							Log.Info("Angledrive Loss Map not found.");
							if (useEfficiencyFallback)
							{
								Log.Info("Angledrive Trying with Efficiency instead of Loss Map.");
								angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.Efficiency,
									data.Ratio, "Angledrive");
							}
							else
							{
								throw new VectoException("Angledrive: LossMap not found.", ex);
							}
						}
						return angledriveData;
					default:
						throw new ArgumentOutOfRangeException(nameof(data), "Unknown Angledrive Type.");
				}
			}
			catch (Exception e)
			{
				throw new VectoException("Error while reading Angledrive data: {0}", e.Message, e);
			}
		}

		/// <summary>
		/// Intersects max torque curve.
		/// </summary>
		/// <param name="maxTorqueEntries"></param>
		/// <param name="maxTorque"></param>
		/// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
		internal static IList<VehicleMaxPropulsionTorque.FullLoadEntry> IntersectMaxPropulsionTorqueCurve(IList<VehicleMaxPropulsionTorque.FullLoadEntry> maxTorqueEntries, NewtonMeter maxTorque)
		{
			if (maxTorque == null)
			{
				return maxTorqueEntries;
			}

			var entries = new List<VehicleMaxPropulsionTorque.FullLoadEntry>();
			var firstEntry = maxTorqueEntries.First();
			if (firstEntry.FullDriveTorque < maxTorque)
			{
				entries.Add(maxTorqueEntries.First());
			}
			else
			{
				entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
				{
					MotorSpeed = firstEntry.MotorSpeed,
					FullDriveTorque = maxTorque,
				});
			}
			foreach (var entry in maxTorqueEntries.Pairwise(Tuple.Create))
			{
				if (entry.Item1.FullDriveTorque <= maxTorque && entry.Item2.FullDriveTorque <= maxTorque)
				{
					// segment is below maxTorque line -> use directly
					entries.Add(entry.Item2);
				}
				else if (entry.Item1.FullDriveTorque > maxTorque && entry.Item2.FullDriveTorque > maxTorque)
				{
					// segment is above maxTorque line -> add limited entry
					entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
					{
						MotorSpeed = entry.Item2.MotorSpeed,
						FullDriveTorque = maxTorque,
					});
				}
				else
				{
					// segment intersects maxTorque line -> add new entry at intersection
					var edgeFull = Edge.Create(
						new Point(entry.Item1.MotorSpeed.Value(), entry.Item1.FullDriveTorque.Value()),
						new Point(entry.Item2.MotorSpeed.Value(), entry.Item2.FullDriveTorque.Value()));

					var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
					if (!entries.Any(x => x.MotorSpeed.IsEqual(intersectionX)) && !intersectionX.IsEqual(entry.Item2.MotorSpeed.Value()))
					{
						entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
						{
							MotorSpeed = intersectionX.SI<PerSecond>(),
							FullDriveTorque = maxTorque,
						});
					}

					entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
					{
						MotorSpeed = entry.Item2.MotorSpeed,
						FullDriveTorque = entry.Item2.FullDriveTorque > maxTorque ? maxTorque : entry.Item2.FullDriveTorque,

					});
				}
			}


			return entries;
		}

        private class DummyVehicleContainer : VehicleContainer
        {
            public DummyVehicleContainer(VectoRunData runData) : base(runData, null,
                null, null)
            { }
        }
    }

	public class IEPCGearboxInputData : IGearboxDeclarationInputData
	{
		protected readonly IIEPCDeclarationInputData _iepc;
		private IList<ITransmissionInputData> _gears;

		public IEPCGearboxInputData(IIEPCDeclarationInputData iepc)
		{
			_iepc = iepc;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource => _iepc.DataSource;
		public bool SavedInDeclarationMode => _iepc.SavedInDeclarationMode;
		public string Manufacturer => _iepc.Manufacturer;
		public string Model => _iepc.Model;
		public DateTime Date => _iepc.Date;
		public string AppVersion => _iepc.AppVersion;
		public CertificationMethod CertificationMethod => _iepc.CertificationMethod;
		public string CertificationNumber => _iepc.CertificationNumber;
		public DigestData DigestValue => _iepc.DigestValue;

		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public GearboxType Type => GearboxType.APTN;
		public IList<ITransmissionInputData> Gears => _gears ?? (_gears = ConvertGears());

		public bool DifferentialIncluded => false;
		public double AxlegearRatio => double.NaN;

		#endregion
		protected virtual IList<ITransmissionInputData> ConvertGears()
		{
			return _iepc.Gears.Select(x => new TransmissionInputData() {
				Gear = x.GearNumber,
				Ratio = x.Ratio,
				MaxInputSpeed = x.MaxOutputShaftSpeed == null ? null : x.MaxOutputShaftSpeed * x.Ratio,
				MaxTorque = x.MaxOutputShaftTorque == null ? null : x.MaxOutputShaftTorque / x.Ratio,
			}).Cast<ITransmissionInputData>().ToList();
		}


	}
}
