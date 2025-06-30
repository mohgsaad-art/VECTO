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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration
{
	// ReSharper disable once InconsistentNaming
	public class Truck40tPowerTrain
	{
		public const string ShiftPolygonFile = @"TestData/Components/ShiftPolygons.vgbs";
		public const string AccelerationFile = @"TestData/Components/Truck.vacc";
		public const string EngineFile = @"TestData/Components/40t_Long_Haul_Truck.veng";
		public const string AxleGearLossMap = @"TestData/Components/Axle 40t Truck.vtlm";
		public const string GearboxIndirectLoss = @"TestData/Components/Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData/Components/Direct Gear.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData/Components/ShiftPolygons.vgbs";
		//public const string GearboxFullLoadCurveFile = @"TestData/Components/Gearbox.vfld";

		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName,
			bool overspeed = false, GearboxType gbxType = GearboxType.AMT)
		{
			var container = CreatePowerTrain(cycleData, modFileName.Replace(".vmod", ""), 7500.SI<Kilogram>(),
				19300.SI<Kilogram>(), overspeed, gbxType);

			return new DistanceRun(container);
		}

		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, Kilogram massExtra,
			Kilogram loading, bool overspeed = false)
		{
			var container = CreatePowerTrain(cycleData, modFileName.Replace(".vmod", ""), massExtra, loading, overspeed);

			return new DistanceRun(container);
		}

		public static IVehicleContainer CreatePowerTrain(DrivingCycleData cycleData, string modFileName,
			Kilogram massExtra, Kilogram loading, bool overspeed = false, GearboxType gbxType = GearboxType.AMT)
		{
			var gearboxData = CreateGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, gearboxData.Gears.Count);
			var axleGearData = CreateAxleGearData();
			var vehicleData = CreateVehicleData(massExtra, loading);
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, overspeed);

			var runData = new VectoRunData() {
				JobRunId = 0,
				JobName = Path.GetFileName(modFileName),
				EngineData = engineData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				VehicleData = vehicleData,
				AirdragData = airdragData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				Retarder = new RetarderData() { Type = RetarderType.None},
				Aux = new List<VectoRunData.AuxData>(),
				GearshiftParameters = CreateGearshiftData(),
				SimulationType = SimulationType.DistanceCycle,
				Cycle = cycleData,
				DriverData = driverData,
			};

			var fileWriter = new FileOutputWriter(modFileName);
			var modData = new ModalDataContainer(runData, fileWriter, null)
			{
				WriteModalResults = true
			};
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, null);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var engine = new CombustionEngine(container, engineData);
			var clutch = new Clutch(container, engineData);

            IShiftStrategy gbxStrategy;
			switch (gbxType) {
				case GearboxType.MT:
					gbxStrategy = new MTShiftStrategy(container);
					break;
				case GearboxType.AMT:
					gbxStrategy = new AMTShiftStrategy(container);
					break;
				default:
					throw new ArgumentOutOfRangeException("gbxType", gbxType, null);
			}

			dynamic tmp = cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airdragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new AMTGearbox(container, gbxStrategy))
				.AddComponent(clutch)
				.AddComponent(engine);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("ZERO", 0.SI<Watt>());
			engine.Connect(aux.Port());
			container.AddAuxiliary("ZERO");

			return container;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) => Tuple.Create((uint)i, new GearData {
					//MaxTorque = 2300.SI<NewtonMeter>(),
					LossMap =
						TransmissionLossMapReader.ReadFromFile(ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
							$"Gear {i}"),
					Ratio = ratio,
					ShiftPolygon = ShiftPolygonReader.ReadFromFile(ShiftPolygonFile)
				})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
			};
		}

		private static ShiftStrategyParameters CreateGearshiftData()
		{
			return new ShiftStrategyParameters() {
				TimeBetweenGearshifts = 2.SI<Second>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration

			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			const double ratio = 2.59;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.ReadFromFile(AxleGearLossMap, ratio, "AxleGear")
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram massExtra, Kilogram loading)
		{
			var wheelsType = "385/65 R 22.5";
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.2,
					Inertia = 14.9.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 31300.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.25,
					Inertia = 14.9.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = true,
					TyreTestLoad = 31300.SI<Newton>()
				},

				// trailer - declaration wheel data
				new Axle {
					AxleWeightShare = 0.55 / 3,
					TwinTyres = DeclarationData.Trailer.TwinTyres,
					RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
					TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
					Inertia = DeclarationData.Wheels.Lookup(wheelsType).Inertia
				},
				new Axle {
					AxleWeightShare = 0.55 / 3,
					TwinTyres = DeclarationData.Trailer.TwinTyres,
					RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
					TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
					Inertia = DeclarationData.Wheels.Lookup(wheelsType).Inertia
				},
				new Axle {
					AxleWeightShare = 0.55 / 3,
					TwinTyres = DeclarationData.Trailer.TwinTyres,
					RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
					TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
					Inertia = DeclarationData.Wheels.Lookup(wheelsType).Inertia
				}
			};
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 7100.SI<Kilogram>() + massExtra,
				Loading = loading,
				DynamicTyreRadius = 0.4882675.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false,
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(6.2985.SI<SquareMeter>(), 0.SI<SquareMeter>(), 
                        CrossWindCorrectionCurveReader.GetNoCorrectionCurve(6.2985.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData {
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),
					//Deceleration = -0.5.SI<MeterPerSquareSecond>(),
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan,
					UtilityFactorStandstill = DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor,
				},
				OverSpeed = new DriverData.OverSpeedData() {
						Enabled = overspeed,
						MinSpeed = 50.KMPHtoMeterPerSecond(),
						OverSpeed = 5.KMPHtoMeterPerSecond(),
					}
			};
		}
	}
}