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
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
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
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration
{
	public class CoachAdvancedAuxPowertrain
	{
		public const string AccelerationFile = @"TestData/Components/Truck.vacc";
		public const string EngineFile = @"TestData/Components/24t Coach.veng";
		public const string EngineFileHigh = @"TestData/Components/24t Coach_high.veng";
		public const string AxleGearLossMap = @"TestData/Components/Axle.vtlm";
		public const string GearboxIndirectLoss = @"TestData/Components/Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData/Components/Direct Gear.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData/Components/ShiftPolygons.vgbs";
		//public const string GearboxFullLoadCurveFile = @"TestData/Components/Gearbox.vfld";

		public const string AdvancedAuxFile = @"TestData/Integration/BusAuxiliaries/AdvAuxTest.aaux";

		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName,
			bool overspeed = false, bool highEnginePower = true)
		{
			var container = CreatePowerTrain(cycleData, modFileName.Replace(".vmod", ""), overspeed, highEnginePower);

			return new DistanceRun(container);
		}

		public static IVehicleContainer CreatePowerTrain(DrivingCycleData cycleData, string modFileName, bool overspeed = false,
			bool highEnginePower = true)
		{
			var gearboxData = CreateGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(highEnginePower ? EngineFileHigh : EngineFile,
				gearboxData.Gears.Count);
			var axleGearData = CreateAxleGearData();
			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, overspeed);

			var runData = new VectoRunData() {
				JobRunId = 0,
				JobName = modFileName,
				AxleGearData = axleGearData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				GearboxData = gearboxData,
				GearshiftParameters = CreateGearshiftData(),
				EngineData = engineData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				SimulationType = SimulationType.DistanceCycle,
				Cycle = cycleData, 
				DriverData = driverData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				BusAuxiliaries = BusAuxiliaryInputData.ReadBusAuxiliaries(AdvancedAuxFile, vehicleData)
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modData = new ModalDataContainer(runData, fileWriter, null)
			{
				WriteModalResults = true
			};

			var container = VehicleContainer.CreateVehicleContainer(runData, modData, null);
			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var engine = new CombustionEngine(container, engineData);

            cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airdragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new AMTGearbox(container, new AMTShiftStrategy(container)))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(engine);

			var aux = new BusAuxiliariesAdapter(container, runData.BusAuxiliaries);
			
			var auxCfg = runData.BusAuxiliaries;
			var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
				: (ISimpleBattery)new NoBattery(container);
			aux.ElectricStorage = electricStorage;
			engine.Connect(aux.Port());

			return container;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = ratio.IsEqual(1)
								? TransmissionLossMapReader.ReadFromFile(GearboxIndirectLoss, ratio, $"Gear {i}")
								: TransmissionLossMapReader.ReadFromFile(GearboxDirectLoss, ratio, $"Gear {i}"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
			};
		}

		private static ShiftStrategyParameters CreateGearshiftData()
		{
			return new ShiftStrategyParameters() {
				TimeBetweenGearshifts = 2.SI<Second>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
				TorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration
			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			const double ratio = 3.240355;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.ReadFromFile(AxleGearLossMap, ratio, "AxleGear")
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.4375,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.375,
					Inertia = 10.83333.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = true,
					TyreTestLoad = 52532.55.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.1875,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				}
			};
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				CurbMass = 15700.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.52.SI<Meter>(),
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
				OverSpeed = new DriverData.OverSpeedData {
						Enabled = overspeed,
						MinSpeed = 50.KMPHtoMeterPerSecond(),
						OverSpeed = 5.KMPHtoMeterPerSecond()
					}
			};
		}
	}
}