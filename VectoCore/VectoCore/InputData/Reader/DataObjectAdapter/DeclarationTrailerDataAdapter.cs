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
*   Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using JetBrains.Annotations;
using Ninject;
using NLog.Fluent;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{

	public class DeclarationTrailerDataAdapter : AbstractSimulationDataAdapter, IDeclarationDataAdapterTrailer
	{
		public static readonly GearboxType[] SupportedGearboxTypes =
			{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

		private static Dictionary<string, double> _applyDeltaCdxA;

		static DeclarationTrailerDataAdapter()
		{
			_applyDeltaCdxA = new Dictionary<string, double>();
			_applyDeltaCdxA.Add("4-LH", 1.5);
			_applyDeltaCdxA.Add("9-LH", 1.5);
			_applyDeltaCdxA.Add("2-RD", 1.3);
		}

		public virtual DriverData CreateDriverData()
		{
			var lookAheadData = new DriverData.LACData {
				Enabled = DeclarationData.Driver.LookAhead.Enabled,

				//Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
				MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
				LookAheadDecisionFactor = new LACDecisionFactor(),
				LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
			};
			var overspeedData = new DriverData.OverSpeedData {
				Enabled = true,
				MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
				OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
			};

			var retVal = new DriverData {
				LookAheadCoasting = lookAheadData,
				OverSpeed = overspeedData,
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactor = DeclarationData.Driver.EngineStopStart.UtilityFactor,
				},
				EcoRoll = new DriverData.EcoRollData() {
					UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
					MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
					ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
					AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
					AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
				},
				PCC = new DriverData.PCCData() {
					PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
					MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
					PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
					PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
					UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
					OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
				}
			};
			return retVal;
		}

		public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, TrailerSegment trailerSegment)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("VehicleData");
			}

			return CreateNonExemptedVehicleData(data, mission, loading.Value.Item1, loading.Value.Item2, trailerSegment);
		}

		protected virtual VehicleData CreateNonExemptedVehicleData(IVehicleDeclarationInputData data,
			Mission mission, Kilogram loading, double? passengerCount, TrailerSegment trailerSegment)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
			retVal.TrailerGrossVehicleMass = mission.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0); 
			retVal.SleeperCab = data.SleeperCab;
			retVal.BodyAndTrailerMass = mission.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);
			retVal.Loading = loading;
			retVal.TrailerSegment = trailerSegment;
			

			retVal.PassengerCount = null;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();


			retVal.CargoVolume = mission.TotalCargoVolume + trailerSegment.ReferenceTrailerSegment.CargoVolume;

			var axles = data.Components.AxleWheels.AxlesDeclaration;

			var axleData = new List<Axle>();
			for (var i = 0; i < axles.Count; i++) {
				var axleInput = axles[i];
				var axle = new Axle {
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
					CertificationNumber = axleInput.Tyre.CertificationNumber,
					DigestValueInput = axleInput.Tyre.DigestValue == null ? "" : axleInput.Tyre.DigestValue.DigestValue,
				};
				axleData.Add(axle);
			}


			foreach (var trailer in mission.Trailer) {
				
				axleData.AddRange(
					trailer.TrailerWheels.Select(
						trailerWheel => new Axle {
							AxleType = AxleType.Trailer,
							AxleWeightShare = trailer.TrailerAxleWeightShare,
							TwinTyres = trailer.TwinTyres,
							RollResistanceCoefficient = trailer.TyreComponent.RollResistanceCoefficient,
							TyreTestLoad = trailer.TyreComponent.TyreTestLoad,
							FuelEfficiencyClass = trailer.TyreComponent.FuelEfficiencyClass,
							Inertia = trailerWheel.Inertia,
							WheelsDimension = trailerWheel.WheelType
						}));
			}

			retVal.AxleData = axleData;
			return retVal;
		}

		public virtual VehicleData CreateTrailerData(IVehicleDeclarationInputData data, ITrailerDeclarationInputData trailer, TrailerSegment trailerSegment,
			Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, TrailerData specificTrailerData)
		{
			if (!data.SavedInDeclarationMode)
			{
				WarnDeclarationMode("VehicleData");
			}

			return GetTrailerData(data, trailer, trailerSegment, mission, loading.Value.Item1, loading.Value.Item2, specificTrailerData);
		}
		protected virtual VehicleData GetTrailerData(IVehicleDeclarationInputData data, ITrailerDeclarationInputData trailerData, TrailerSegment trailerSegment,
			Mission mission, Kilogram loading, double? passengerCount, TrailerData specificTrailerData)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
			retVal.TrailerGrossVehicleMass = mission.SpecificTrailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0);
			retVal.SleeperCab = data.SleeperCab;
			retVal.TrailerSegment = trailerSegment;
			retVal.InputDataTrailer = trailerData;

			var maxGcm = specificTrailerData.MaxGcm;
			var curbmass = trailerData.MassInRunningOrder.Value();

			var massBody = trailerSegment.MassBody;

			var trailerCargoVolume = mission.TotalCargoVolume;
			var BodyCargoVolume = trailerData.CargoVolume;


			retVal.BodyAndTrailerMass = Kilogram.Create(curbmass) + massBody;

			retVal.Loading = Kilogram.Create(Math.Min(loading.Value(), (maxGcm - data.CurbMassChassis.Value() - curbmass)));

			retVal.PassengerCount = 1;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();
			
			retVal.CargoVolume = trailerCargoVolume + BodyCargoVolume;
			

			var axles = data.Components.AxleWheels.AxlesDeclaration;

			var axleData = new List<Axle>();
			for (var i = 0; i < axles.Count; i++)
			{
				var axleInput = axles[i];
				var axle = new Axle
				{
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.SpecificTrailerAxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
					CertificationNumber = axleInput.Tyre.CertificationNumber,
					DigestValueInput = axleInput.Tyre.DigestValue == null ? "" : axleInput.Tyre.DigestValue.DigestValue,
				};
				axleData.Add(axle);
			}

			var trailerAxles = trailerData.Axles;
			
			for (var i = 0; i < trailerAxles.Count; i++)
			{
				var axleInput = trailerAxles[i];
				var axle = new Axle
				{
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = AxleType.Trailer,
					AxleWeightShare = mission.SpecificTrailer.FirstOrDefault().TrailerAxleWeightShare,
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
				};
				axleData.Add(axle);
			}

			retVal.AxleData = axleData;
			return retVal;
		}

		public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			if (!inputData.Components.GearboxInputData.SavedInDeclarationMode)
			{
				WarnDeclarationMode("GearboxData");
			}
			return DoCreateGearboxData(inputData, runData, shiftPolygonCalc);
		}

		public virtual GearboxData DoCreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			var gearbox = inputData.Components.GearboxInputData;

			var adas = inputData.ADAS;
			var torqueConverter = inputData.Components.TorqueConverterInputData;

			var engine = runData.EngineData;
			var axlegearRatio = runData.AxleGearData.AxleGear.Ratio;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;

			var retVal = SetCommonGearboxData(gearbox);

			if (adas != null && retVal.Type.AutomaticTransmission() && adas.EcoRoll != EcoRollType.None &&
				!adas.ATEcoRollReleaseLockupClutch.HasValue)
			{
				throw new VectoException("Input parameter ATEcoRollReleaseLockupClutch required for AT transmission");
			}

			retVal.ATEcoRollReleaseLockupClutch =
				adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission()
					? adas.ATEcoRollReleaseLockupClutch.Value
					: false;

			if (!SupportedGearboxTypes.Contains(gearbox.Type))
			{
				throw new VectoSimulationException("Unsupported gearbox type: {0}!", retVal.Type);
			}

			var gearsInput = gearbox.Gears;
			if (gearsInput.Count < 1)
			{
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			SetDeclarationData(retVal);

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			var tcShiftPolygon = DeclarationData.TorqueConverter.ComputeShiftPolygon(engine.FullLoadCurves[0]);
			var vehicleCategory = runData.VehicleData.VehicleCategory == VehicleCategory.GenericBusVehicle
				? VehicleCategory.GenericBusVehicle
				: inputData.VehicleCategory;
			for (uint i = 0; i < gearsInput.Count; i++)
			{
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, false, vehicleCategory, gearbox.Type);

				var shiftPolygon = shiftPolygonCalc != null
					? shiftPolygonCalc.ComputeDeclarationShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1], gearbox.Gears, engine, axlegearRatio, dynamicTyreRadius)
					: DeclarationData.Gearbox.ComputeShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1],
						gearsInput, engine,
						axlegearRatio, dynamicTyreRadius);

				var gearData = new GearData
				{
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(gearbox, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, runData.VehicleData.VehicleCategory);
				gears.Add(i + 1, gearData);
			}

			// remove disabled gears (only the last or last two gears may be removed)
			if (inputData.TorqueLimits != null)
			{
				var toRemove = (from tqLimit in inputData.TorqueLimits where tqLimit.Gear >= gears.Keys.Max() - 1 && tqLimit.MaxTorque.IsEqual(0) select (uint)tqLimit.Gear).ToList();
				if (toRemove.Count > 0 && toRemove.Min() <= gears.Count - toRemove.Count)
				{
					throw new VectoException("Only the last 1 or 2 gears can be disabled. Disabling gear {0} for a {1}-speed gearbox is not allowed.", toRemove.Min(), gears.Count);
				}

				foreach (var entry in toRemove)
				{
					gears.Remove(entry);
				}
			}

			retVal.Gears = gears;
			if (retVal.Type.AutomaticTransmission())
			{
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime;

				retVal.TorqueConverterData = CreateTorqueConverterData(gearbox.Type, torqueConverter, ratio, engine);

				if (torqueConverter != null)
				{
					retVal.TorqueConverterData.ModelName = torqueConverter.Model;
					retVal.TorqueConverterData.DigestValueInput = torqueConverter.DigestValue?.DigestValue;
					retVal.TorqueConverterData.CertificationMethod = torqueConverter.CertificationMethod;
					retVal.TorqueConverterData.CertificationNumber = torqueConverter.CertificationNumber;
				}
			}

			return retVal;
		}

		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
		{
			var engine = vehicle.Components.EngineInputData;
			var gearbox = vehicle.Components.GearboxInputData;

			if (!engine.SavedInDeclarationMode)
			{
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine, vehicle.TankSystem);
			retVal.IdleSpeed = VectoMath.Max(mode.IdleSpeed, vehicle.EngineIdleSpeed);

			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in mode.Fuels)
			{
				retVal.Fuels.Add(
					new CombustionEngineFuelData()
					{
						WHTCUrban = fuel.WHTCUrban,
						WHTCRural = fuel.WHTCRural,
						WHTCMotorway = fuel.WHTCMotorway,
						ColdHotCorrectionFactor = fuel.ColdHotBalancingFactor,
						CorrectionFactorRegPer = fuel.CorrectionFactorRegPer,
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicle.TankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
															mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban, 
															fuel.WHTCMotorway) * fuel.ColdHotBalancingFactor * fuel.CorrectionFactorRegPer
					});
			}

			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement, gearbox.Type);
			retVal.EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1) {
				[0] = FullLoadCurveReader.Create(mode.FullLoadCurve, true), [0] = { EngineData = retVal }
			};
			foreach (var gear in gearbox.Gears)
			{
				var maxTorque = VectoMath.Min(
					GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			retVal.FullLoadCurves = fullLoadCurves;

			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0)
			{
				retVal.ElectricalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataElectrical, mission.MissionType, WHRType.ElectricalOutput); 
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0)
			{
				retVal.MechanicalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataMechanical, mission.MissionType, WHRType.MechanicalOutputDrivetrain); 
			}

			return retVal;
		}

		public CombustionEngineData CreateTrailerEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission, ITrailerDeclarationInputData trailer, LoadingType loadingType)
		{
			var engine = vehicle.Components.EngineInputData;
			var gearbox = vehicle.Components.GearboxInputData;

			if (!engine.SavedInDeclarationMode)
			{
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine, vehicle.TankSystem);
			retVal.IdleSpeed = VectoMath.Max(mode.IdleSpeed, vehicle.EngineIdleSpeed);

			GetSteeredAndLiftFactor(mission, trailer, loadingType, out var liftFactor, out var steerFactor);

			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in mode.Fuels)
			{
				retVal.Fuels.Add(
					new CombustionEngineFuelData()
					{
						WHTCUrban = fuel.WHTCUrban,
						WHTCRural = fuel.WHTCRural,
						WHTCMotorway = fuel.WHTCMotorway,
						ColdHotCorrectionFactor = fuel.ColdHotBalancingFactor,
						CorrectionFactorRegPer = fuel.CorrectionFactorRegPer,
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicle.TankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
															mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban,
															fuel.WHTCMotorway) * fuel.ColdHotBalancingFactor * fuel.CorrectionFactorRegPer,
						FcLiftCorrectionFactor = liftFactor,
						FcSteerCorrectionFactor = steerFactor
					});
			}

			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement, gearbox.Type);
			retVal.EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1) {
				[0] = FullLoadCurveReader.Create(mode.FullLoadCurve, true), [0] = { EngineData = retVal }
			};
			foreach (var gear in gearbox.Gears)
			{
				var maxTorque = VectoMath.Min(
					GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			retVal.FullLoadCurves = fullLoadCurves;

			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0)
			{
				retVal.ElectricalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataElectrical, mission.MissionType, WHRType.ElectricalOutput);
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0)
			{
				retVal.MechanicalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataMechanical, mission.MissionType, WHRType.MechanicalOutputDrivetrain);
			}

			return retVal;
		}

		private static void GetSteeredAndLiftFactor(Mission mission, ITrailerDeclarationInputData trailer,
			LoadingType loadingType, out double liftFactor, out double steerFactor)
		{
			liftFactor = 1.0;
			steerFactor = 1.0;
			var trailerType = trailer.TrailerType;
			var axles = new List<(bool Steered, bool Liftable)>();
			foreach (var trailerAxleDeclarationInputData in trailer.Axles) {
				axles.Add((trailerAxleDeclarationInputData.Steered, trailerAxleDeclarationInputData.Liftable));
			}

			if (trailerType == TypeTrailer.DB) {
				//DB steered axle not considered
				var axle = axles[0];
				axle.Steered = false;
				axles[0] = axle;
			}


			if (axles.Count(x => x.Steered && x.Liftable) > 0) {
				//found axle with both steered and liftable
				if (mission.MissionType == MissionType.LongHaul) {
					//LH
					if (loadingType == LoadingType.LowLoading) {
						// LOW LOADING
						if (axles.Count(x => x.Liftable) > 1) //1 both + 1 liftable
							liftFactor = (1 + (mission.TrailerLiftableImpact.lowPayLoadImpact * 1.5) / 100);
						else if (axles.Count(x => x.Steered) > 1) {
							//1 both + 1 steered
							liftFactor = (1 + mission.TrailerLiftableImpact.lowPayLoadImpact / 100);
							steerFactor = (1 + mission.TrailerSteeredImpact.lowPayLoadImpact / 100);
						} else //1 both
							liftFactor = (1 + mission.TrailerLiftableImpact.lowPayLoadImpact / 100);
					} else {
						//REF LOADING
						if (axles.Count(x => x.Steered) > 1) {
							//1 both + 1 steered
							liftFactor = (1 + mission.TrailerLiftableImpact.repPayLoadImpact / 100);
							steerFactor = (1 + mission.TrailerSteeredImpact.repPayLoadImpact/ 100);
						} else //1 both or 1 both + 1 liftable
							liftFactor = (1 + mission.TrailerLiftableImpact.repPayLoadImpact / 100);
					}
				} else {
					//RD & UD
					if (loadingType == LoadingType.LowLoading) {
						//Low lading
						if (axles.Count(x => x.Liftable) > 1) {
							//1 both + 1 liftable
							liftFactor = (1 + mission.TrailerLiftableImpact.lowPayLoadImpact / 100);
							steerFactor = (1 + mission.TrailerSteeredImpact.lowPayLoadImpact / 100);
						} else if (axles.Count(x => x.Steered) > 1) //1 both + 1 steered
							steerFactor = (1 + (mission.TrailerSteeredImpact.lowPayLoadImpact * 1.2) / 100);
						else // 1 both
						{
							steerFactor = (1 + mission.TrailerSteeredImpact.lowPayLoadImpact / 100);
						}
					} else {
						//Ref loading
						if (axles.Count(x => x.Liftable) > 1) {
							//1 both + 1 liftable
							liftFactor = (1 + mission.TrailerLiftableImpact.repPayLoadImpact / 100);
							steerFactor = (1 + mission.TrailerSteeredImpact.repPayLoadImpact / 100);
						} else if (axles.Count(x => x.Steered) > 1) //1 both + 1 steered
						{
							steerFactor = (1 + (mission.TrailerSteeredImpact.repPayLoadImpact * 1.2) / 100);
						} else // 1 both
						{
							steerFactor = (1 + mission.TrailerSteeredImpact.repPayLoadImpact / 100);
						}
					}
				}
			} else {
				// no axle is both liftable and steered
				if (loadingType == LoadingType.LowLoading) {
					//LOW loading
					if (axles.Count(x => x.Liftable) > 1) //if 2 liftable
						liftFactor = (1 + (mission.TrailerLiftableImpact.lowPayLoadImpact * 1.5) / 100);

					else if (axles.Count(x => x.Liftable) > 0) //if 1 liftable
						liftFactor = (1 + mission.TrailerLiftableImpact.lowPayLoadImpact / 100);

					if (axles.Count(x => x.Steered) > 1) // 2 steered
						steerFactor = (1 + (mission.TrailerSteeredImpact.lowPayLoadImpact * 1.2) / 100);
					else if (axles.Count(x => x.Steered) > 0) // 1 steered
						steerFactor = (1 + mission.TrailerSteeredImpact.lowPayLoadImpact / 100);
				} else {
					//REF loading
					if (axles.Count(x => x.Liftable) > 0)
						liftFactor = (1 + mission.TrailerLiftableImpact.repPayLoadImpact / 100);

					if (axles.Count(x => x.Steered) > 1) //2 steered
						steerFactor = (1 + (mission.TrailerSteeredImpact.repPayLoadImpact * 1.2) / 100);
					else if (axles.Count(x => x.Steered) > 0) // 1 steered
						steerFactor = (1 + mission.TrailerSteeredImpact.repPayLoadImpact / 100);
				}
			}
		}

		private static WHRData CreateWHRData(IWHRData whrInputData, MissionType missionType, WHRType type)
		{
			if (whrInputData == null || whrInputData.GeneratedPower == null) {
				throw new VectoException("Missing WHR Data");
			}

			var whr = new WHRData() {
				CFUrban = whrInputData.UrbanCorrectionFactor,
				CFRural = whrInputData.RuralCorrectionFactor,
				CFMotorway = whrInputData.MotorwayCorrectionFactor,
				CFColdHot = whrInputData.BFColdHot,
				CFRegPer = whrInputData.CFRegPer,
				WHRMap = WHRPowerReader.Create(whrInputData.GeneratedPower, type)
			};
			whr.WHRCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
				missionType.GetNonEMSMissionType(), whr.CFRural, whr.CFUrban,
				whr.CFMotorway) * whr.CFColdHot * whr.CFRegPer;
			return whr;
		}

		protected internal static NewtonMeter VehMaxTorque(
			ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2) {
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.ContainsKey(gear.Gear) &&
					limits[gear.Gear].MaxTorque <= DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque) {
					return limits[gear.Gear].MaxTorque;
				}
			}

			return null;
		}

		protected internal static NewtonMeter GbxMaxTorque(
			ITransmissionInputData gear, int numGears, NewtonMeter maxEngineTorque
		)
		{
			if (gear.Gear - 1 < numGears / 2) {
				// gears count from 1 to n, -> n entries, lower half can always limit
				if (gear.MaxTorque != null) {
					return gear.MaxTorque;
				}
			} else {
				// upper half can only limit if max-torque <= 90% of engine max torque
				if (gear.MaxTorque != null &&
					gear.MaxTorque <= DeclarationData.Engine.TorqueLimitGearboxFactor * maxEngineTorque) {
					return gear.MaxTorque;
				}
			}

			return null;
		}


		public virtual GearboxData CreateGearboxData(IGearboxDeclarationInputData gearbox, IVehicleComponentsDeclaration components, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc)
		{
			//var adas = inputData.ADAS;
			var torqueConverter = components.TorqueConverterInputData;

			var engine = runData.EngineData;
			var axlegearRatio = runData.AxleGearData.AxleGear.Ratio;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;

			var retVal = SetCommonGearboxData(gearbox);

			/*if (adas != null && retVal.Type.AutomaticTransmission() && adas.EcoRoll != EcoRollType.None &&
				!adas.ATEcoRollReleaseLockupClutch.HasValue) {
				throw new VectoException("Input parameter ATEcoRollReleaseLockupClutch required for AT transmission");
			}
	
			retVal.ATEcoRollReleaseLockupClutch =
				adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission()
					? adas.ATEcoRollReleaseLockupClutch.Value
					: false;
	
			if (!SupportedGearboxTypes.Contains(gearbox.Type)) {
				throw new VectoSimulationException("Unsupported gearbox type: {0}!", retVal.Type);
			}*/

			var gearsInput = gearbox.Gears;
			if (gearsInput.Count < 1) {
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			SetDeclarationData(retVal);

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();

			var tcShiftPolygon = DeclarationData.TorqueConverter.ComputeShiftPolygon(engine.FullLoadCurves[0]);
			//var vehicleCategory = runData.VehicleData.VehicleCategory == VehicleCategory.GenericBusVehicle
			//	? VehicleCategory.GenericBusVehicle
			//	: inputData.VehicleCategory; 
			for (uint i = 0;
				i < gearsInput.Count;
				i++) {
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, false, VehicleCategory.Unknown, gearbox.Type);

				var shiftPolygon = shiftPolygonCalc != null
					? shiftPolygonCalc.ComputeDeclarationShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[0], gearbox.Gears, engine, axlegearRatio,
						dynamicTyreRadius)
					: DeclarationData.Gearbox.ComputeShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[0],
						gearsInput, engine,
						axlegearRatio, dynamicTyreRadius);

				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(gearbox, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears,
					runData.VehicleData.VehicleCategory);
				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;
			if (retVal.Type.AutomaticTransmission()) {
				var ratio = double.IsNaN(retVal.Gears[1].Ratio)
					? 1
					: retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime;

				retVal.TorqueConverterData = CreateTorqueConverterData(gearbox.Type, torqueConverter, ratio, engine);

				if (torqueConverter != null) {
					retVal.TorqueConverterData.ModelName = torqueConverter.Model;
					retVal.TorqueConverterData.DigestValueInput = torqueConverter.DigestValue?.DigestValue;
					retVal.TorqueConverterData.CertificationMethod = torqueConverter.CertificationMethod;
					retVal.TorqueConverterData.CertificationNumber = torqueConverter.CertificationNumber;
				}
			}

			return retVal;
		}

		protected virtual TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio,
			CombustionEngineData componentsEngineInputData)
		{
			return TorqueConverterDataReader.Create(
				torqueConverter.TCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}

		protected virtual void CreateATGearData(
			IGearboxDeclarationInputData gearbox, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory)
		{
			if (gearbox.Type == GearboxType.ATPowerSplit && i == 0) {
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}
			if (gearbox.Type == GearboxType.ATSerial) {
				if (i == 0) {
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1 && gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
					// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
					// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
					CreateTCSecondGearATSerial(gearData, tcShiftPolygon);

					// NOTE: the lower gear in 'gears' dictionary has index i !!
					gears[i].Ratio = double.NaN;
					gears[i].LossMap = null;
				}
			}
		}

		private static void SetDeclarationData(GearboxData retVal)
		{
			retVal.Inertia = DeclarationData.Gearbox.Inertia;
			retVal.TractionInterruption = retVal.Type.TractionInterruption();
			retVal.TorqueReserve = DeclarationData.GearboxTCU.TorqueReserve;
			retVal.StartTorqueReserve = DeclarationData.GearboxTCU.TorqueReserveStart;
			retVal.ShiftTime = DeclarationData.Gearbox.MinTimeBetweenGearshifts;
			retVal.StartSpeed = DeclarationData.GearboxTCU.StartSpeed;
			retVal.StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration;
			retVal.DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay;
			retVal.UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay;
			retVal.UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration;
		}

		public virtual AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return DoCreateAngledriveData(data, false);
		}

		public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass, Meter vehicleLength)
		{
			if (!auxInputData.SavedInDeclarationMode) {
				WarnDeclarationMode("AuxiliariesData");
			}
			var retVal = new List<VectoRunData.AuxData>();

			if (auxInputData.Auxiliaries.Count != 5) {
				Log.Error(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
				throw new VectoException(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
			}

			foreach (var auxType in EnumHelper.GetValues<AuxiliaryType>()) {
				var auxData = auxInputData.Auxiliaries.FirstOrDefault(a => a.Type == auxType);
				if (auxData == null) {
					throw new VectoException("Auxiliary {0} not found.", auxType);
				}

				var aux = new VectoRunData.AuxData {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology
				};

				mission = mission.GetNonEMSMissionType();
				switch (auxType) {
					case AuxiliaryType.Fan:
						aux.PowerDemand = DeclarationData.Fan.LookupPowerDemand(hvdClass, mission, auxData.Technology.FirstOrDefault());
						aux.ID = Constants.Auxiliaries.IDs.Fan;
						break;
					case AuxiliaryType.SteeringPump:
						aux.PowerDemand = DeclarationData.SteeringPump.Lookup(mission, hvdClass, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.SteeringPump;
						break;
					case AuxiliaryType.HVAC:
						aux.PowerDemand = DeclarationData.HeatingVentilationAirConditioning.Lookup(
							mission,
							auxData.Technology.FirstOrDefault(), hvdClass).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
						break;
					case AuxiliaryType.PneumaticSystem:
						aux.PowerDemand = DeclarationData.PneumaticSystem.Lookup(mission, auxData.Technology.FirstOrDefault())
														.PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
						break;
					case AuxiliaryType.ElectricSystem:
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission, auxData.Technology.FirstOrDefault()).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
						break;
					default: continue;
				}

				retVal.Add(aux);
			}

			return retVal;
		}

		private void WarnDeclarationMode(string inputData)
		{
			Log.Warn("{0} not in Declaration Mode!", inputData);
		}

		public virtual RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(SquareMeter aerodynamicDragAera, Meter vehicleHeight, Mission mission, TrailerSegment trailerSegment, CorrectedTrailerData data)
		{
			 var startSpeed = Constants.SimulationSettings.CrosswindCorrection.MinVehicleSpeed;
			 var maxSpeed = Constants.SimulationSettings.CrosswindCorrection.MaxVehicleSpeed;
			 var speedStep = Constants.SimulationSettings.CrosswindCorrection.VehicleSpeedStep;

			 var maxAlpha = Constants.SimulationSettings.CrosswindCorrection.MaxAlpha;
			 var alphaStep = Constants.SimulationSettings.CrosswindCorrection.AlphaStep;

			 var startHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MinHeight;
			 var maxHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MaxHeight;
			 var heightPercentStep = Constants.SimulationSettings.CrosswindCorrection.HeightStep;
			 var heightShare = (double)heightPercentStep / maxHeightPercent;

			var a1 = data.A1;
			var a2 = data.A2;
			var a3 = data.A3;

			// first entry (0m/s) will get CdxA of second entry.
			var points = new List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> {
				new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = 0.SI<MeterPerSecond>(),
					EffectiveCrossSectionArea = 0.SI<SquareMeter>()
				}
			};

			for (var speed = startSpeed; speed <= maxSpeed; speed += speedStep) {
				var vVeh = speed.KMPHtoMeterPerSecond();

				var cdASum = 0.SI<SquareMeter>();

				for (var heightPercent = startHeightPercent; heightPercent < maxHeightPercent; heightPercent += heightPercentStep) {
					var height = heightPercent / 100.0 * vehicleHeight;
					var vWind = Physics.BaseWindSpeed * Math.Pow(height / Physics.BaseWindHeight, Physics.HellmannExponent);

					for (var alpha = 0; alpha <= maxAlpha; alpha += alphaStep) {
						var vAirX = vVeh + vWind * Math.Cos(alpha.ToRadian());
						var vAirY = vWind * Math.Sin(alpha.ToRadian());

						var beta = Math.Atan(vAirY / vAirX).ToDegree();

						// ΔCdxA = A1β + A2β² + A3β³
						var deltaCdA = a1 * beta + a2 * beta * beta + a3 * beta * beta * beta;

						// CdxA(β) = CdxA(0) + ΔCdxA(β)
						var cdA = aerodynamicDragAera + deltaCdA;

						var share = (alpha == 0 || alpha == maxAlpha ? alphaStep / 2.0 : alphaStep) / maxAlpha;

						// v_air = sqrt(v_airX²+vAirY²)
						// cdASum = CdxA(β) * v_air²/v_veh²
						cdASum += heightShare * share * cdA * (vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh);
					}
				}

				points.Add(
					new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
						Velocity = vVeh,
						EffectiveCrossSectionArea = cdASum
					});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}

		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetTrailerDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight, TrailerSegment segment, CorrectedTrailerData data, Mission mission, IDeclarationInputDataProvider vehicle)
		{
			var startSpeed = Constants.SimulationSettings.CrosswindCorrection.MinVehicleSpeed;
			var maxSpeed = Constants.SimulationSettings.CrosswindCorrection.MaxVehicleSpeed;
			var speedStep = Constants.SimulationSettings.CrosswindCorrection.VehicleSpeedStep;

			var maxAlpha = Constants.SimulationSettings.CrosswindCorrection.MaxAlpha;
			var alphaStep = Constants.SimulationSettings.CrosswindCorrection.AlphaStep;

			var startHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MinHeight;
			var maxHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MaxHeight;
			var heightPercentStep = Constants.SimulationSettings.CrosswindCorrection.HeightStep;
			var heightShare = (double)heightPercentStep / maxHeightPercent;

			// first entry (0m/s) will get CdxA of second entry.
			var points = new List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> {
				new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = 0.SI<MeterPerSecond>(),
					EffectiveCrossSectionArea = 0.SI<SquareMeter>()
				}
			};

			//corrected A1, A2, A3
			var a1 = data.A1;
			var a2 = data.A2;
			var a3 = data.A3;

			for (var speed = startSpeed; speed <= maxSpeed; speed += speedStep)
			{
				var vVeh = speed.KMPHtoMeterPerSecond();

				var cdASum = 0.SI<SquareMeter>();

				for (var heightPercent = startHeightPercent; heightPercent < maxHeightPercent; heightPercent += heightPercentStep)
				{
					var height = heightPercent / 100.0 * vehicleHeight;
					var vWind = Physics.BaseWindSpeed * Math.Pow(height / Physics.BaseWindHeight, Physics.HellmannExponent);

					for (var alpha = 0; alpha <= maxAlpha; alpha += alphaStep)
					{
						var vAirX = vVeh + vWind * Math.Cos(alpha.ToRadian());
						var vAirY = vWind * Math.Sin(alpha.ToRadian());

						var beta = Math.Atan(vAirY / vAirX).ToDegree();




						var deltaCdA = a1 * beta + a2 * beta * beta + a3 * beta * beta * beta;

						// CdxA(β) = CdxA(0) + ΔCdxA(β)
						var cdA = aerodynamicDragAera + deltaCdA; 

						var share = (alpha == 0 || alpha == maxAlpha ? alphaStep / 2.0 : alphaStep) / maxAlpha;

						// v_air = sqrt(v_airX²+vAirY²)
						// cdASum = CdxA(β) * v_air²/v_veh²
						cdASum += heightShare * share * cdA * (vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh);
					}
				}

				points.Add(
					new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry
					{
						Velocity = vVeh,
						EffectiveCrossSectionArea = cdASum
					});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}

		public virtual AirdragData CreateAirdragData(
			IAirdragDeclarationInputData airdragInputData, Mission mission, TrailerData trailerData, TrailerSegment trailerSegment)
		{
			if (airdragInputData == null || airdragInputData.AirDragArea == null) {
				return DefaultAirdragData(mission, trailerSegment, trailerData);
			}

			
			var retVal = SetCommonAirdragData(airdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;

			retVal.DeclaredAirdragArea = trailerData.TotalRefCorrection.CdxA0;

			var aerodynamicDragArea = retVal.DeclaredAirdragArea + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(aerodynamicDragArea, mission.VehicleHeight, mission, trailerSegment, trailerData.TotalRefCorrection),
					CrossWindCorrectionMode.DeclarationModeCorrection);
			return retVal;
		}

		public virtual AirdragData CreateTrailerAirdragData(
			IDeclarationInputDataProvider vehicle, Mission mission, TrailerData specificTrailerData, TrailerSegment trailerSegment)
		{
			var airdragInputData = vehicle.JobInputData.Vehicle.Components.AirdragInputData;
			if (airdragInputData == null || airdragInputData.AirDragArea == null)
			{
				return DefaultAirdragData(mission, trailerSegment, specificTrailerData); 
			}

			var retVal = SetCommonAirdragData(airdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;

			//Corrected CdxA(0)
			//var rfRun = false;

			//retVal.DeclaredAirdragArea = rfRun ? specificTrailerData.CdxABase.SI<SquareMeter>() : specificTrailerData.TotalRefCorrection.CdxA0;

			retVal.DeclaredAirdragArea = specificTrailerData.TotalCorrection.CdxA0;

			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(
					retVal.DeclaredAirdragArea,
					GetTrailerDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters, retVal.DeclaredAirdragArea, mission.VehicleHeight, trailerSegment, specificTrailerData.TotalCorrection, mission, vehicle),
					CrossWindCorrectionMode.DeclarationModeCorrection); 
			return retVal;
		}

		protected virtual AirdragData DefaultAirdragData(Mission mission, TrailerSegment trailerSegment, TrailerData trailerData)
		{
			var aerodynamicDragArea = mission.DefaultCDxA + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			return new AirdragData() {
				CertificationMethod = CertificationMethod.StandardValues,
				CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
				DeclaredAirdragArea = mission.DefaultCDxA,
				CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(aerodynamicDragArea, mission.VehicleHeight, mission, trailerSegment, trailerData.TotalRefCorrection),
					CrossWindCorrectionMode.DeclarationModeCorrection)
			};
		}

		private static bool ValidateAeroFeatures(ITrailerDeclarationInputData trailer, ref IList<string> errors)
		{
			var prevErrors = errors.Count;


			bool valid = false;
			try {
				var aeroFeatureRow = DeclarationData.AeroFeatures.GetTrailerDataRow(TrailerDataHelper.CreateAeroFeatureList(trailer));
				valid = aeroFeatureRow[trailer.TrailerType.ToString()].ToString() == "1";
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				valid = false;
			}
			var combination = TrailerDataHelper.GetAeroFeatureCombination(trailer);
			if (combination < 0 || !valid) {
				errors.Add("invalid standard aero-device declared");
			} 
			
			if (trailer.CertifiedAeroDevice) {
				var segment = DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType, trailer.BodyworkCode, trailer.VolumeOrientation,
					trailer.TPMLMAxleAssembly);
				if (!trailer.CertifiedAeroDeviceApplicableVehicleGroups.Contains(segment.VehicleGroupAnnex.ToString()))
				{
					errors.Add($"Certified Aero Device not applicable for vehicle group: {segment.VehicleGroupAnnex}! Applicable groups are {string.Join(",", trailer.CertifiedAeroDeviceApplicableVehicleGroups.ToList())}");
				}
			}
			
			

			if (trailer.AeroFeatureTechnologies.Count == 0 &&
				(trailer.YawAngle0 < 0 ||
				trailer.YawAngle3 < 0 ||
				trailer.YawAngle6 < 0 ||
				trailer.YawAngle9 < 0)) {
				errors.Add("Certified Aero Reduction values must be positive");
			}

			return prevErrors == errors.Count;
		}
		#region Validation 
		private static bool ValidateAxles(ITrailerDeclarationInputData trailer, ref IList<string> errors)
		{
			var error = false;
			var liftableCount = 0;
			if (trailer.Axles[0].Liftable) {
				liftableCount++;
			}

			if (trailer.Axles.Count > 1 && trailer.Axles[1].Liftable) {
				liftableCount++;
			}

			if (trailer.Axles.Count > 2 && trailer.Axles[2].Liftable) {
				liftableCount++;
			}

			if (trailer.TrailerType == TypeTrailer.DB && !trailer.Axles[0].Steered) {
				errors.Add("First axle has to be steered on DB trailers");
				error = true;
			}
			
			
			switch (trailer.NumberOfAxles)
			{
				case NumberOfTrailerAxles.One when trailer.Axles[0].Liftable: //one axle cant be liftable
				case NumberOfTrailerAxles.One when trailer.TrailerType == TypeTrailer.DB:
				case NumberOfTrailerAxles.One when trailer.TrailerType == TypeTrailer.DC && trailer.Axles[0].Steered:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DB && liftableCount > 0:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DB && trailer.Axles[1].Steered:
				case NumberOfTrailerAxles.Two when liftableCount > 1: //too many liftable
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DC && trailer.Axles[0].Steered:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DA && trailer.Axles[0].Steered:
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DB && liftableCount > 1: //too many liftable
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DB && (trailer.Axles[0].Liftable || trailer.Axles[1].Steered):
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DC && (trailer.Axles[1].Liftable || trailer.Axles[0].Steered || trailer.Axles[1].Steered):
				case NumberOfTrailerAxles.Three when liftableCount > 2: //too many liftable
                case NumberOfTrailerAxles.Three when trailer.Axles.Count(x => x.Steered) > 2:
					errors.Add("Wrong axle configuration");
					error = true;
					break;
				//throw new Exception("Wrong axle configuration");
			}

			return !error;
		}


		private static bool ValidateInputValues(ITrailerDeclarationInputData trailer, ref IList<string> errors)
		{

			var prevErrors = errors.Count;
			TrailerSegment segment;
			try {
				segment = DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType,
					trailer.BodyworkCode, trailer.VolumeOrientation,
					trailer.TPMLMAxleAssembly);
			} catch (Exception ex) {
				errors.Add(ex.Message);
			}

			if (trailer.VIN.Length != 17) {
				errors.Add("VIN must be 17 characters long");
				//throw new Exception("VIN must be 17 characters long");
			}

			if (trailer.TrailerType != TypeTrailer.DB && (trailer.TPMLMAxleAssembly == null ||
														trailer.TPMLMAxleAssembly.Value() < 8000)) {
				errors.Add("TPMLM Axle Assembly must be ≥ 8000");
				//throw new Exception("Trailer is not DB but no TPMLM Axle Assembly value is given");
			}

			if (trailer.TrailerType != TypeTrailer.DC && (trailer.TrailerCouplingPoint == TrailerCouplingPoint.Low ||
														trailer.TrailerCouplingPoint == TrailerCouplingPoint.High)) {
				errors.Add("Trailer is not DC but Coupling Point is given");
				//throw new Exception("Trailer is not DC but Coupling Point is given");
			}

			if (trailer.TrailerType == TypeTrailer.DC &&
				(trailer.TrailerCouplingPoint == TrailerCouplingPoint.Unknown)) {
				errors.Add("Trailer is DC but no Coupling Point is given");

			}

			var trailerAxles = trailer.Axles;
			if ((trailerAxles ?? throw new InvalidOperationException()).Any(x =>
					x.Tyre.Dimension != trailerAxles.FirstOrDefault()?.Tyre.Dimension)) {
				errors.Add("invalid trailer configurations - different tyre dimensions specified");
				//throw new Exception("invalid trailer configurations - different tyre dimensions specified");
			}

			if (trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.LengthBetweenCentersOfAxles.Value() > 0) {
				errors.Add("One axle trailers should have Length Between Centers Of Axles = 0");
				//throw new Exception("One axle trailers should have Length Between Centers Of Axles = 0");
			}

			if (trailer.TPMLMAxleAssembly != null) {
				if ((trailer.TrailerType == TypeTrailer.DC && trailer.TPMLMAxleAssembly > trailer.TPMLMTotalTrailer)
					|| (trailer.TrailerType == TypeTrailer.DA && trailer.TPMLMAxleAssembly >= trailer.TPMLMTotalTrailer))
				{
					errors.Add("Invalid masses declared");
				}
			}

			//if (trailer.TrailerType == TypeTrailer.DC && trailer.TPMLMAxleAssembly > trailer.TPMLMTotalTrailer) {
			//	errors.Add("Invalid masses declared");
			//	//throw new Exception("Invalid masses declared");
			//}


			//if (trailer.TrailerType != TypeTrailer.DA && trailer.TPMLMAxleAssembly >= trailer.TPMLMTotalTrailer) {
			//	errors.Add("Invalid masses declared");
			//	//throw new Exception("Invalid masses declared");
			//}


			if (trailer.ExternalBodyHeight >= trailer.TotalTrailerHeight) {
				errors.Add("External body height value is larger than the total trailer height");
				//throw new Exception("External body width value is larger than the total trailer height");
			}


			if (trailer.TrailerType == TypeTrailer.DA && trailer.ExternalBodyLength > 14) {
				errors.Add("Invalid external body length declared");
				//throw new Exception("Invalid external body length declared");
			}

			if (trailer.TrailerType == TypeTrailer.DC && trailer.ExternalBodyLength > 12) {
				errors.Add("Invalid external body length declared");
				//throw new Exception("Invalid external body width declared");
			}

			if (trailer.TrailerType == TypeTrailer.DB && trailer.ExternalBodyLength > 12) {

				errors.Add("Invalid external body length declared");
				//throw new Exception("Invalid external body length declared");
			}

			if (trailer.ExternalBodyWidth < 1 || trailer.ExternalBodyWidth > 2.6) {
				errors.Add("External body width is invalid");
				//throw new Exception("External body width is invalid");
			}

			if (trailer.ExternalBodyWidth < 1 || trailer.ExternalBodyWidth > 4) {
				errors.Add("External body width is invalid"); 
				//throw new Exception("External body width is invalid");
			}

			if ((trailer.TrailerType != TypeTrailer.DC || trailer.TrailerType != TypeTrailer.DA) &&
				trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.LegislativeCategory.ToUpper() != "O3") {
				errors.Add("Invalid legislative category");
				//throw new Exception("Invalid xml values");
			}

			if (trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.LengthBetweenCentersOfAxles.Value() > 0) {
				errors.Add("Invalid value for one axle");
				//throw new Exception("Invalid value for one axle");
			}

			if (trailer.TotalTrailerHeight < 1 || trailer.TotalTrailerHeight > 4) {
				errors.Add("Invalid total height");
				//throw new Exception("Invalid total height");
			}

			if (trailer.MassInRunningOrder.Value() < 1000 || trailer.TPMLMTotalTrailer.Value() < 1000) {
				errors.Add("Invalid mass in running order");
				//throw new Exception("Invalid values");
			}

			if (trailer.TPMLMTotalTrailer.Value() < 1000) {
				errors.Add("Invalid TPMLM total trailer");
			}

			return prevErrors == errors.Count; // no new errors added 
		}
		
		public void ValidateTrailerInput(ITrailerDeclarationInputData trailer)
		{
			IList<string> errors = new List<string>();
			if (!ValidateTrailerInput(trailer, ref errors)) {
				throw new VectoException(string.Join("\n", errors));
			}
		}

		public static bool ValidateTrailerInput(ITrailerDeclarationInputData trailer, ref IList<string> errors)
		{
			return ValidateInputValues(trailer, ref errors) &
						ValidateAxles(trailer, ref errors) & ValidateAeroFeatures(trailer, ref errors);
		}
#endregion validation
		private static string GetAxleWheelDimension(IVehicleDeclarationInputData genericVehicle)
		{
			return genericVehicle.Components.AxleWheels.XMLSource.ChildNodes[0].ChildNodes[0].FirstChild.ChildNodes[3].FirstChild.ChildNodes[5].InnerText;
		}

		private static SquareMeter CalcLengthCorrection(TrailerSegment trailerSegment, ITrailerDeclarationInputData trailer, int yaw, CorrectedTrailerData correctedCdxABase)
		{
			//acr_base * (cdA_base_yawX / acr_base + (Lf-Lbf) * dCdf + (La-Lba) * dCda + (Lr-Lbr) * dCdr)
			var s = trailerSegment.GenericCadVehicle;
			
			double cdaBase,dcdf,dcda,dcdr;

			if (yaw == 0)
			{
				cdaBase = correctedCdxABase.CdxA0.Value();
				dcdf = s.DCdf0;
				dcda = s.DCda0;
				dcdr = s.DCdr0;
			}
			else if (yaw == 3) {
                cdaBase = correctedCdxABase.CdxA3.Value();
				dcdf = s.DCdf3;
				dcda = s.DCda3;
				dcdr = s.DCdr3;
			}
			else if(yaw == 6)
			{
				cdaBase = correctedCdxABase.CdxA6.Value();
				dcdf = s.DCdf6;
				dcda = s.DCda6;
				dcdr = s.DCdr6;
			}
			else
			{
				cdaBase = correctedCdxABase.CdxA9.Value();
				dcdf = s.DCdf9;
				dcda = s.DCda9;
				dcdr = s.DCdr9;
			}

			var wheelsDiameter = trailerSegment.Missions.FirstOrDefault()?.Trailer.FirstOrDefault()?.TrailerWheels.FirstOrDefault().WheelsDiameter;
			
			var lf = trailer.TrailerType == TypeTrailer.DB ? 
				trailer.LengthFromFrontToFirstAxle + (wheelsDiameter / 2) 
				: trailer.LengthFromFrontToFirstAxle - (wheelsDiameter / 2);

			var la = trailer.TrailerType == TypeTrailer.DB ? 
				trailer.LengthBetweenCentersOfAxles - wheelsDiameter 
				: trailer.LengthBetweenCentersOfAxles + wheelsDiameter;

			var lr = trailer.TrailerType == TypeTrailer.DB
				? trailer.ExternalBodyLength - (lf + la)
				: trailer.TrailerType == TypeTrailer.DC && trailer.NumberOfAxles == NumberOfTrailerAxles.One
					? trailer.ExternalBodyLength - lf - la
					: trailer.ExternalBodyLength - (trailer.LengthFromFrontToFirstAxle +
													trailer.LengthBetweenCentersOfAxles + (wheelsDiameter / 2));

			var correction = s.AcrBase.Value() * (cdaBase / s.AcrBase.Value() + (lf - s.Lbf).Value() * dcdf + (la-s.Lba).Value() * dcda + (lr-s.Lbr).Value() * dcdr);

			return SquareMeter.Create(correction);
		}


		private static SquareMeter CalcRefLengthCorrection(TrailerSegment trailerSegment, 
			Meter trailerLengthTrailerFrontToFirstAxle, 
			Meter trailerLengthBetweenCentersOfAxles, 
			Meter trailerExternalBodyLength, 
			TypeTrailer specificTrailerType, 
			NumberOfTrailerAxles specificTrailerNumberOfAxles, int yaw, CorrectedTrailerData correctedTrailerData)
		{
			//acr_base * (cdA_base_yawX / acr_base + (Lf-Lbf) * dCdf + (La-Lba) * dCda + (Lr-Lbr) * dCdr)
			var s = trailerSegment.GenericCadVehicle;

			double cdaBase, dcdf, dcda, dcdr;

			switch (yaw) {
				case 0:
					cdaBase = correctedTrailerData.CdxA0.Value();
					dcdf = s.DCdf0;
					dcda = s.DCda0;
					dcdr = s.DCdr0;
					break;
				case 3:
					cdaBase = correctedTrailerData.CdxA3.Value();
					dcdf = s.DCdf3;
					dcda = s.DCda3;
					dcdr = s.DCdr3;
					break;
				case 6:
					cdaBase = correctedTrailerData.CdxA6.Value();
					dcdf = s.DCdf6;
					dcda = s.DCda6;
					dcdr = s.DCdr6;
					break;
				case 9:
					cdaBase = correctedTrailerData.CdxA9.Value();
					dcdf = s.DCdf9;
					dcda = s.DCda9;
					dcdr = s.DCdr9;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var wheelsDiameter = trailerSegment.Missions.FirstOrDefault()?.Trailer.FirstOrDefault()?.TrailerWheels.FirstOrDefault().WheelsDiameter;
			
			var lf = specificTrailerType == TypeTrailer.DB ?
				trailerLengthTrailerFrontToFirstAxle + (wheelsDiameter / 2)
				: trailerLengthTrailerFrontToFirstAxle - (wheelsDiameter / 2);

			var la = specificTrailerType == TypeTrailer.DB ?
				trailerLengthBetweenCentersOfAxles - wheelsDiameter
				: trailerLengthBetweenCentersOfAxles + wheelsDiameter;

			var lr = specificTrailerType == TypeTrailer.DB
				? trailerExternalBodyLength - (lf + la)
				: specificTrailerType == TypeTrailer.DC && specificTrailerNumberOfAxles == NumberOfTrailerAxles.One
					? trailerExternalBodyLength - lf - la
					: trailerExternalBodyLength - (trailerLengthTrailerFrontToFirstAxle +
													trailerLengthBetweenCentersOfAxles + (wheelsDiameter / 2));

			var correction = s.AcrBase.Value() * (cdaBase / s.AcrBase.Value() + (lf - s.Lbf).Value() * dcdf + (la - s.Lba).Value() * dcda + (lr - s.Lbr).Value() * dcdr);

			return SquareMeter.Create(correction);
		}


		private static SquareMeter CalcHeightCorrection(TrailerSegment trailerSegment,
			Meter trailerTotalTrailerHeight, double previousCorrection)
		{
			//CdxA(0)_width_correction*(Total_height_of_the_trailer/Total_trailer_height_generic_cad)
			var s = trailerSegment.GenericCadVehicle;

			var correction = previousCorrection * (trailerTotalTrailerHeight / s.TotalHeight);
			return SquareMeter.Create(correction);
		}


		private static SquareMeter CalcVolumeCorrection(TrailerSegment trailerSegment,
			Meter trailerExternalBodyHeight, double previousCorrection)
		{
			//CdxA(3)_height_correction*(External_height_of_the_body/External_body_height_generic_cad)
			var s = trailerSegment.GenericCadVehicle;

			var correction = previousCorrection * (trailerExternalBodyHeight / s.ExternalBodyHeight);
			return SquareMeter.Create(correction);
		}

		private static SquareMeter CalcAeroCorrection(TrailerSegment trailerSegment, ITrailerDeclarationInputData trailer, double previousCorrection, int yaw)
		{
			//CdxA(0)_volume-oriented_correction*(1-dCdxA_combination(0))
			var s = trailerSegment.GenericCadVehicle;
			double[] aCorrection = { };

			if (trailer.YawAngle0 != 0 || trailer.YawAngle3 != 0 || trailer.YawAngle6 != 0 || trailer.YawAngle9 != 0)
				aCorrection = new [] { trailer.YawAngle0, trailer.YawAngle3, trailer.YawAngle6, trailer.YawAngle9 };

			else {
				var combination = TrailerDataHelper.GetAeroFeatureCombination(trailer);

				switch (combination) {
					case 0:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero0;
						break;
					case 1:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero1;
						break;
					case 2:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero2;
						break;
					case 3:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero3;
						break;
					case 4:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero4;
						break;
					case 5:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero5;
						break;
					case 6:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero6;
						break;
					case 7:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero7;
						break;
					case 8:
						aCorrection = trailerSegment.SpecificTrailerSegment.Aero8;
						break;
				}
			}

			//If value is positive, fix it by converting it to negative
			if (aCorrection[yaw] > 0)
				aCorrection[yaw] = aCorrection[yaw] * -1;
			var correction = previousCorrection * (1 + (aCorrection[yaw]/100));

			return SquareMeter.Create(correction);
		}

		private static CorrectedTrailerData GetSpecificTrailerCorrection(TrailerSegment trailerSegment, ITrailerDeclarationInputData trailer)
		{


			var data = GetCorrectedTrailerData(trailerSegment);

			#region LengthAxleCorrection

			if (trailerSegment.SpecificTrailerSegment.LengthAxleCorrection) {
				var s = trailerSegment.GenericCadVehicle;
				data.CdxA0 = CalcRefLengthCorrection(trailerSegment,
					trailer.LengthFromFrontToFirstAxle,
					trailer.LengthBetweenCentersOfAxles,
					trailer.ExternalBodyLength,
					trailer.TrailerType,
					trailer.NumberOfAxles, 0, data);

				if (trailer.TrailerType != TypeTrailer.DA) {
					data.CdxA3 = CalcRefLengthCorrection(trailerSegment,
						trailer.LengthFromFrontToFirstAxle,
						trailer.LengthBetweenCentersOfAxles,
						trailer.ExternalBodyLength,
						trailer.TrailerType,
						trailer.NumberOfAxles, 3, data);
					data.CdxA6 = CalcRefLengthCorrection(trailerSegment,
						trailer.LengthFromFrontToFirstAxle,
						trailer.LengthBetweenCentersOfAxles,
						trailer.ExternalBodyLength,
						trailer.TrailerType,
						trailer.NumberOfAxles, 6, data);
					data.CdxA9 = CalcRefLengthCorrection(trailerSegment,
						trailer.LengthFromFrontToFirstAxle,
						trailer.LengthBetweenCentersOfAxles,
						trailer.ExternalBodyLength,
						trailer.TrailerType,
						trailer.NumberOfAxles, 9, data);
				} else {
					data.UpdateCdAxX();
				}
			}

			#endregion

			//WidthCorrection not implemented yet
			var widthCdxA0 = data.CdxA0;

			#region HeightCorrection

			if (trailerSegment.SpecificTrailerSegment.HeightCorrection) {
				data.CdxA0 = CalcHeightCorrection(trailerSegment, trailer.TotalTrailerHeight, data.CdxA0.Value());
				data.CdxA3 = CalcHeightCorrection(trailerSegment, trailer.TotalTrailerHeight, data.CdxA3.Value());
				data.CdxA6 = CalcHeightCorrection(trailerSegment, trailer.TotalTrailerHeight, data.CdxA6.Value());
				data.CdxA9 = CalcHeightCorrection(trailerSegment, trailer.TotalTrailerHeight, data.CdxA9.Value());
			}

			#endregion

			#region VolumeCorrection
		   
			if (trailerSegment.SpecificTrailerSegment.VolumeCorrection){
				data.CdxA3 = CalcVolumeCorrection(trailerSegment, trailer.ExternalBodyHeight, data.CdxA3.Value());
				data.CdxA6 = CalcVolumeCorrection(trailerSegment, trailer.ExternalBodyHeight, data.CdxA6.Value());
				data.CdxA9 = CalcVolumeCorrection(trailerSegment, trailer.ExternalBodyHeight, data.CdxA9.Value());
			}
			#endregion

			#region Coupling point

			//if (trailer.TrailerType == TypeTrailer.DC && trailer.TrailerCouplingPoint == TrailerCouplingPoint.High) {
			if(trailerSegment.SpecificTrailerSegment.CouplingPointCorrection && trailer.TrailerCouplingPoint == TrailerCouplingPoint.High){
				data.CdxA0 *= 1.1037;
				data.CdxA3 *= 1.1348;
				data.CdxA6 *= 1.1804;
				data.CdxA9 *= 1.2235;
			}
			#endregion 

			#region Aero feature Correction

			if (trailerSegment.SpecificTrailerSegment.AeroCorrection) {

				data.CdxA0 = CalcAeroCorrection(trailerSegment, trailer, data.CdxA0.Value(), 0);
				data.CdxA3 = CalcAeroCorrection(trailerSegment, trailer, data.CdxA3.Value(), 1);
				data.CdxA6 = CalcAeroCorrection(trailerSegment, trailer, data.CdxA6.Value(), 2);
				data.CdxA9 = CalcAeroCorrection(trailerSegment, trailer, data.CdxA9.Value(), 3);
			}

			#endregion

			data.CdxA00 = data.CdxA0 - widthCdxA0;
			data.CdxA03 = data.CdxA3 - widthCdxA0;
			data.CdxA06 = data.CdxA6 - widthCdxA0;
			data.CdxA09 = data.CdxA9 - widthCdxA0;

			//(ΔcdA(9)_spe - (3 * ΔcdA(6)_spe) +(3 * ΔcdA(3)_spe) -ΔcdA(0)_spe ) *(1 / 162)
			var a3 = (data.CdxA09.Value() - (3 * data.CdxA06).Value() + (3 * data.CdxA03).Value() - data.CdxA00.Value()) * 0.0061728395;
			//(ΔcdA(6)_spe - (2 * ΔcdA(3)_spe) + ΔcdA(0)_spe - (162 * a3_spe) ) *(1 / 18)
			var a2 = (data.CdxA06.Value() - (2 * data.CdxA03).Value() + data.CdxA00.Value() - (162 * a3)) * 0.05555555555;
			//(ΔcdA(3)_spe - ΔcdA(0)_spe - (9 * a2_spe) - (27 * a3_spe)) * (1 / 3)
			var a1 = (data.CdxA03.Value() - data.CdxA00.Value() - (9 * a2) - (27 * a3)) * 0.33333333333;

			data.A1 = SquareMeter.Create(a1);
			data.A2 = SquareMeter.Create(a2);
			data.A3 = SquareMeter.Create(a3);

			return data;
		}

		private static CorrectedTrailerData GetCorrectedTrailerData(TrailerSegment trailerSegment)
		{
			var data = new CorrectedTrailerData();
			data.CdxA0 = SquareMeter.Create(trailerSegment.GenericCadVehicle.CdxA0);

			if (_applyDeltaCdxA.TryGetValue(trailerSegment.TowingVehicle, out var deltaCdxA)) {
				data.CdxA0 += SquareMeter.Create(deltaCdxA);
			}

			data.A1 = trailerSegment.GenericCadVehicle.A1;
			data.A2 = trailerSegment.GenericCadVehicle.A2;
			data.A3 = trailerSegment.GenericCadVehicle.A3;


			data.UpdateCdAxX();
			return data;
		}

		public CorrectedTrailerData GetReferenceTrailerCorrection(TrailerSegment trailerSegment,
			ITrailerDeclarationInputData trailer)
		{
			var data = GetCorrectedTrailerData(trailerSegment);

			#region LengthAxleCorrection

			if (trailerSegment.ReferenceTrailerSegment.LengthAxleCorrection) {
				var s = trailerSegment.GenericCadVehicle;
				ReferenceTrailerSegment referenceTrailerSegment = trailerSegment.ReferenceTrailerSegment;
				data.CdxA0 = CalcRefLengthCorrection(trailerSegment, 
					referenceTrailerSegment.LengthTrailerFrontToFirstAxle, 
					referenceTrailerSegment.LengthBetweenCentersOfAxles, 
					referenceTrailerSegment.ExternalBodyLength, 
					trailer.TrailerType, trailer.NumberOfAxles, 0, data);

				if (trailer.TrailerType != TypeTrailer.DA) {
					data.CdxA3 = CalcRefLengthCorrection(trailerSegment, 
						referenceTrailerSegment.LengthTrailerFrontToFirstAxle, 
						referenceTrailerSegment.LengthBetweenCentersOfAxles, 
						referenceTrailerSegment.ExternalBodyLength, 
						trailer.TrailerType, trailer.NumberOfAxles, 3, data);
					data.CdxA6 = CalcRefLengthCorrection(trailerSegment, 
						referenceTrailerSegment.LengthTrailerFrontToFirstAxle, 
						referenceTrailerSegment.LengthBetweenCentersOfAxles, 
						referenceTrailerSegment.ExternalBodyLength,
						trailer.TrailerType, trailer.NumberOfAxles, 6, data);
					data.CdxA9 = CalcRefLengthCorrection(trailerSegment,
						referenceTrailerSegment.LengthTrailerFrontToFirstAxle,
						referenceTrailerSegment.LengthBetweenCentersOfAxles, 
						referenceTrailerSegment.ExternalBodyLength,
						trailer.TrailerType, trailer.NumberOfAxles, 9, data);
				} else {
					data.UpdateCdAxX();
				}
			}

			#endregion

			//WidthCorrection not implemented yet
			var widthCdxA0 = data.CdxA0;

			#region HeightCorrection

			if (trailerSegment.ReferenceTrailerSegment.HeightCorrection) {
				ReferenceTrailerSegment refTrailer = trailerSegment.ReferenceTrailerSegment;
				data.CdxA0 = CalcHeightCorrection(trailerSegment, refTrailer.TotalTrailerHeight, data.CdxA0.Value());
				data.CdxA3 = CalcHeightCorrection(trailerSegment, refTrailer.TotalTrailerHeight, data.CdxA3.Value());
				data.CdxA6 = CalcHeightCorrection(trailerSegment, refTrailer.TotalTrailerHeight, data.CdxA6.Value());
				data.CdxA9 = CalcHeightCorrection(trailerSegment, refTrailer.TotalTrailerHeight, data.CdxA9.Value());
			}

			#endregion

			#region VolumeCorrection

			if (trailerSegment.ReferenceTrailerSegment.VolumeCorrection) {
				ReferenceTrailerSegment refTrailer = trailerSegment.ReferenceTrailerSegment;
				data.CdxA3 = CalcVolumeCorrection(trailerSegment, refTrailer.ExternalBodyHeight, data.CdxA3.Value());
				data.CdxA6 = CalcVolumeCorrection(trailerSegment, refTrailer.ExternalBodyHeight, data.CdxA6.Value());
				data.CdxA9 = CalcVolumeCorrection(trailerSegment, refTrailer.ExternalBodyHeight, data.CdxA9.Value());
			}

			#endregion

			#region CoulpingPointCorrection

			if (trailerSegment.ReferenceTrailerSegment.CouplingPointCorrection) {
				throw new NotImplementedException(
					"Trailer coupling point correction not implemented for reference trailers");
			}

			#endregion	

			data.CdxA00 = data.CdxA0 - widthCdxA0;
			data.CdxA03 = data.CdxA3 - widthCdxA0;
			data.CdxA06 = data.CdxA6 - widthCdxA0;
			data.CdxA09 = data.CdxA9 - widthCdxA0;

			//(ΔcdA(9)_spe - (3 * ΔcdA(6)_spe) +(3 * ΔcdA(3)_spe) -ΔcdA(0)_spe ) *(1 / 162)
			var a3 =
				(data.CdxA09.Value() - (3 * data.CdxA06).Value() + (3 * data.CdxA03).Value() - data.CdxA00.Value()) *
				0.0061728395;
			//(ΔcdA(6)_spe - (2 * ΔcdA(3)_spe) + ΔcdA(0)_spe - (162 * a3_spe) ) *(1 / 18)
			var a2 = (data.CdxA06.Value() - (2 * data.CdxA03).Value() + data.CdxA00.Value() - (162 * a3)) *
					0.05555555555;
			//(ΔcdA(3)_spe - ΔcdA(0)_spe - (9 * a2_spe) - (27 * a3_spe)) * (1 / 3)
			var a1 = (data.CdxA03.Value() - data.CdxA00.Value() - (9 * a2) - (27 * a3)) * 0.33333333333;

			data.A1 = SquareMeter.Create(a1);
			data.A2 = SquareMeter.Create(a2);
			data.A3 = SquareMeter.Create(a3);

			return data;
		}


		public TrailerData GetTrailerData(TrailerSegment trailerSegment, ITrailerDeclarationInputData trailer,
			IVehicleDeclarationInputData genericVehicle)
		{
			var wheels = new Wheels();

			//Read the + tpmlm axle assembly and add accordingly
			if (!double.TryParse(trailerSegment.MaxGcm, out double maxGcm)
				&& trailerSegment.MaxGcm.Contains("+")) {
				double.TryParse(trailerSegment.MaxGcm.Substring(0, trailerSegment.MaxGcm.IndexOf("+")), out maxGcm);
				maxGcm += trailer.TPMLMAxleAssembly.Value();
			}

			var specificTrailerData = new TrailerData {
				VolumeOrientation = trailer.VolumeOrientation,
				VehicleGroup = trailerSegment.VehicleGroup,
				VehicleGroupActive = trailerSegment.VehicleGroupActive,
				GenericVehicleCode = trailerSegment.GenericCode,
				CurbMassTrailer = trailer.MassInRunningOrder,
				Inertia = wheels.Lookup(GetAxleWheelDimension(genericVehicle)).Inertia,
				TotalCorrection = GetSpecificTrailerCorrection(trailerSegment, trailer),
				TotalRefCorrection = GetReferenceTrailerCorrection(trailerSegment, trailer),
				MaxGcm = maxGcm,
				CdxABase = trailerSegment.GenericCadVehicle.CdxA0
			};

			return specificTrailerData;
		}


	}
}
