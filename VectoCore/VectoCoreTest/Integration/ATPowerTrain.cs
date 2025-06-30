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
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration
{
	public class ATPowerTrain
	{
		protected static readonly PerSecond MaxTcSpeed = 1500.RPMtoRad();

		public const string AccelerationFile = @"TestData/Components/Truck.vacc";
		public const string EngineFile = @"TestData/Components/AT_GBX/Engine.veng";
		//public const string AxleGearLossMap = @"TestData/Components/AT_GBX/Axle.vtlm";
		//public const string GearboxIndirectLoss = @"TestData/Components/AT_GBX/Indirect Gear.vtlm";
		//public const string GearboxDirectLoss = @"TestData/Components/AT_GBX/Direct Gear.vtlm";
		public const string TorqueConverterGenericFile = @"TestData/Components/AT_GBX/TorqueConverter.vtcc";
		public const string TorqueConverterPowerSplitFile = @"TestData/Components/AT_GBX/TorqueConverterPowerSplit.vtcc";
		public const string GearboxShiftPolygonFile = @"TestData/Components/AT_GBX/AT-Shift.vgbs";


		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, GearboxType gbxType,
			SummaryDataContainer summaryDataContainer, string modFileName,
			bool overspeed = false, KilogramSquareMeter gearBoxInertia = null)
		{
			var container = CreatePowerTrain(cycleData, gbxType, summaryDataContainer, Path.GetFileNameWithoutExtension(modFileName), overspeed,
				gearBoxInertia);
			return new DistanceRun(container);
		}

		public static IVehicleContainer CreatePowerTrain(DrivingCycleData cycleData, GearboxType gbxType,
			SummaryDataContainer summaryDataContainer, string modFileName,
			bool overspeed = false, KilogramSquareMeter gearBoxInertia = null)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			var powertrainBuilder = kernel.Get<ISimplePowertrainBuilder>();


			var gearboxData = CreateGearboxData(gbxType);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, gearboxData.Gears.Count);
			var axleGearData = CreateAxleGearData(gbxType);

			if (gearBoxInertia != null) {
				gearboxData.Inertia = gearBoxInertia;
			}

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, overspeed);

			var runData = new VectoRunData() {
				JobRunId = 0,
				AxleGearData = axleGearData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				GearboxData = gearboxData,
				GearshiftParameters = CreateGearshiftData(),
				EngineData = engineData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				JobName = modFileName,
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				SimulationType = SimulationType.DistanceCycle,
				DriverData = driverData,
				ExecutionMode = ExecutionMode.Engineering,
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modData = new ModalDataContainer(runData, fileWriter, null) {
				WriteModalResults = true,
			};
			var container =
				VehicleContainer.CreateVehicleContainer(runData, modData,
					summaryDataContainer);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var engine = new CombustionEngine(container, engineData);
			var tmp = cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airdragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new APTGearbox(container, new ATShiftStrategyOptimized(container)))
				.AddComponent(engine);
			new ATClutchInfo(container);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("ZERO", 0.SI<Watt>());
			container.AddAuxiliary("ZERO");

			engine.Connect(aux.Port());

			return container;
		}

		public static GearboxData CreateGearboxData(GearboxType gbxType)
		{
			var ratios = gbxType == GearboxType.ATSerial
				? new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 }
				: new[] { 1.35, 1.0, 0.73 };
			var torqueConverterFile = gbxType == GearboxType.ATSerial
				? TorqueConverterGenericFile
				: TorqueConverterPowerSplitFile;
			return new GearboxData {
				Type = gbxType == GearboxType.ATSerial ? GearboxType.ATSerial : GearboxType.ATPowerSplit,
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = ratio.IsEqual(1)
								? TransmissionLossMapReader.Create(0.96, ratio, $"Gear {i}")
								: TransmissionLossMapReader.Create(0.98, ratio, $"Gear {i}"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? (gbxType == GearboxType.ATPowerSplit ? 1.0 : ratio) : double.NaN,
							TorqueConverterGearLossMap = i == 0
								? TransmissionLossMapReader.Create(gbxType == GearboxType.ATPowerSplit ? 1.0 : 0.98, ratio,
									$"Gear {i}")
								: null,
							TorqueConverterShiftPolygon = i == 0 ? ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile) : null
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
				
				PowershiftShiftTime = 0.8.SI<Second>(),
				TorqueConverterData =
					TorqueConverterDataReader.ReadFromFile(torqueConverterFile, 1000.RPMtoRad(),
						MaxTcSpeed, ExecutionMode.Engineering, gbxType == GearboxType.ATSerial ? 1 : 1 / ratios[0],
						DeclarationData.Gearbox.UpshiftMinAcceleration, DeclarationData.Gearbox.UpshiftMinAcceleration)
			};
		}

		public static ShiftStrategyParameters CreateGearshiftData()
		{
			return new ShiftStrategyParameters() {
				TimeBetweenGearshifts = 1.SI<Second>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
				TorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
			};
		}

		private static AxleGearData CreateAxleGearData(GearboxType gbxType)
		{
			var ratio = gbxType == GearboxType.ATSerial ? 6.2 : 5.8;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.38,
					Inertia = 20.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = false,
					TyreTestLoad = 30436.0.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.62,
					Inertia = 18.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = true,
					TyreTestLoad = 30436.SI<Newton>()
				},
			};
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.465.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(3.2634.SI<SquareMeter>(), 0.SI<SquareMeter>(),
                        CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
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
					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan,
					UtilityFactorStandstill = DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor,
				},
				OverSpeed = new DriverData.OverSpeedData {
						Enabled = overspeed,
						MinSpeed = 50.KMPHtoMeterPerSecond(),
						OverSpeed = 5.KMPHtoMeterPerSecond()
					}
			};
		}
	}
}