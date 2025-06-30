using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class JsonReadHybridTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase()]
		public void TestReadBatteryPack()
		{
			var inputProvider = JSONInputDataFactory.ReadREESSData(@"TestData/Hybrids/Battery/GenericBattery.vbat", false) as IBatteryPackEngineeringInputData;

			Assert.AreEqual(7.5.SI(Unit.SI.Ampere.Hour), inputProvider.Capacity);
			
			var soc = inputProvider.VoltageCurve;
			Assert.AreEqual("0", soc.Rows[0][BatterySOCReader.Fields.StateOfCharge]);
			Assert.AreEqual("590", soc.Rows[0][BatterySOCReader.Fields.BatteryVoltage]);

			Assert.AreEqual(0.20, inputProvider.MinSOC);
			Assert.AreEqual(0.80, inputProvider.MaxSOC);
			Assert.AreEqual("50", inputProvider.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.StateOfCharge]);
			Assert.AreEqual("375", inputProvider.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.MaxDischargeCurrent]);
			Assert.AreEqual("375", inputProvider.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.MaxDischargeCurrent]);

			var socMap = BatterySOCReader.Create(soc);

			Assert.AreEqual(590, socMap.Lookup(0).Value());
			Assert.AreEqual(658, socMap.Lookup(1).Value());
			Assert.AreEqual(640, socMap.Lookup(0.5).Value());

			Assert.AreEqual(639, socMap.Lookup(0.45).Value());
		}

		[TestCase()]
		public void TestReadElectricMotor()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData/Hybrids/ElectricMotor/GenericEMotor.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			Assert.AreEqual("0", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMap = ElectricFullLoadCurveReader.Create(fld.First().LoadCurve, 1);
			Assert.AreEqual(-401.07, fldMap.FullLoadDriveTorque(0.RPMtoRad()).Value());
			Assert.AreEqual(401.07, fldMap.FullGenerationTorque(0.RPMtoRad()).Value());

			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap;
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			Assert.AreEqual("0", pwr.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwr.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9844.9", pwr.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwr, 1, ExecutionMode.Engineering);
			Assert.AreEqual(0, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);
			Assert.AreEqual(-298.4752, pwrMap.LookupElectricPower(1.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

			Assert.AreEqual(-20853.4819, pwrMap.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

        }

		[TestCase()]
		public void TestReadElectricMotorV3()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData/Hybrids/ElectricMotor/GenericEMotorV3.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			Assert.AreEqual(400, inputProvider.VoltageLevels[0].VoltageLevel.Value());

			var fldLow = inputProvider.VoltageLevels.First().FullLoadCurve;
			Assert.AreEqual("0", fldLow.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fldLow.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fldLow.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMapLow = ElectricFullLoadCurveReader.Create(fldLow.First().LoadCurve, 1);
			Assert.AreEqual(-401.07, fldMapLow.FullLoadDriveTorque(0.RPMtoRad()).Value());
			Assert.AreEqual(401.07, fldMapLow.FullGenerationTorque(0.RPMtoRad()).Value());

			var pwrLow = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap;
			// var pwrLow = inputProvider.VoltageLevels.First().EfficiencyMap;
			Assert.AreEqual("0", pwrLow.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwrLow.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9844.9", pwrLow.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMapLow = ElectricMotorMapReader.Create(pwrLow, 1, ExecutionMode.Engineering);
			Assert.AreEqual(-0, pwrMapLow.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);
			Assert.AreEqual(-298.4752, pwrMapLow.LookupElectricPower(1.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

			Assert.AreEqual(-20853.4819, pwrMapLow.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);



			Assert.AreEqual(600, inputProvider.VoltageLevels[1].VoltageLevel.Value());

			var fldHi = inputProvider.VoltageLevels.Last().FullLoadCurve;
			Assert.AreEqual("0", fldHi.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("476.284", fldHi.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-486.284", fldHi.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMapHi = ElectricFullLoadCurveReader.Create(fldHi.First().LoadCurve, 1);
			Assert.AreEqual(-476.284, fldMapHi.FullLoadDriveTorque(0.RPMtoRad()).Value());
			Assert.AreEqual(486.284, fldMapHi.FullGenerationTorque(0.RPMtoRad()).Value());

			var pwrHi = inputProvider.VoltageLevels.Last().PowerMap.First().PowerMap;
			// var pwrHi = inputProvider.VoltageLevels.Last().EfficiencyMap;
			Assert.AreEqual("0", pwrHi.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwrHi.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("8860.41", pwrHi.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwrHi, 1, ExecutionMode.Engineering);
			Assert.AreEqual(0, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);
			Assert.AreEqual(-268.6277, pwrMap.LookupElectricPower(1.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

			Assert.AreEqual(-18768.1337, pwrMap.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestElectricMotorV3_Lookup()
		{
			var inputProvider =
				new JSONComponentInputData(@"TestData/Hybrids/ElectricMotor/GenericEMotorV3.vem", null,
					false);
			var daa = new EngineeringDataAdapter();
			var emData = daa.CreateElectricMachines(inputProvider.ElectricMachines, null, null).First().Item2;


			Assert.AreEqual(26, emData.DragCurveLookup(2000.RPMtoRad(), 0u).Value());
			
			var em = emData.EfficiencyData;
			var gear = new GearshiftPosition(0);

			Assert.AreEqual(-334.2300, em.FullLoadDriveTorque(400.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(-396.076, em.FullLoadDriveTorque(600.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(-365.153, em.FullLoadDriveTorque(500.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(-380.6145, em.FullLoadDriveTorque(550.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());

			Assert.AreEqual(334.2300, em.FullGenerationTorque(400.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(406.076, em.FullGenerationTorque(600.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(370.153, em.FullGenerationTorque(500.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());
			Assert.AreEqual(388.1145, em.FullGenerationTorque(550.SI<Volt>(), 2000.RPMtoRad(), gear.Gear).Value());

            Assert.AreEqual(-101.70072, em.EfficiencyMapLookupTorque(400.SI<Volt>(), -25000.SI<Watt>(), 2000.RPMtoRad(), -300.SI<NewtonMeter>(), gear).Value(), 1e-3);
			Assert.AreEqual(-25000, em.LookupElectricPower(400.SI<Volt>(), 2000.RPMtoRad(), -101.70072.SI<NewtonMeter>(), gear).ElectricalPower.Value(), 1e-1);

			Assert.AreEqual(-114.51788, em.EfficiencyMapLookupTorque(600.SI<Volt>(), -25000.SI<Watt>(), 2000.RPMtoRad(), -300.SI<NewtonMeter>(), gear).Value(), 1e-3);
			Assert.AreEqual(-25000, em.LookupElectricPower(600.SI<Volt>(), 2000.RPMtoRad(), -114.51788.SI<NewtonMeter>(), gear).ElectricalPower.Value(), 1e-1);

			Assert.AreEqual(-107.772009, em.EfficiencyMapLookupTorque(500.SI<Volt>(), -25000.SI<Watt>(), 2000.RPMtoRad(), -300.SI<NewtonMeter>(), gear).Value(), 1e-3);
			Assert.AreEqual(-25000, em.LookupElectricPower(500.SI<Volt>(), 2000.RPMtoRad(), -107.772009.SI<NewtonMeter>(), gear).ElectricalPower.Value(), 1e-1);

			Assert.AreEqual(-111.053784, em.EfficiencyMapLookupTorque(550.SI<Volt>(), -25000.SI<Watt>(), 2000.RPMtoRad(), -300.SI<NewtonMeter>(), gear).Value(), 1e-3);
			Assert.AreEqual(-25000, em.LookupElectricPower(550.SI<Volt>(), 2000.RPMtoRad(), -111.053784.SI<NewtonMeter>(), gear).ElectricalPower.Value(), 1e-1);

		}


		[TestCase()]
		public void TestReadElectricMotorAggregation()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData/Hybrids/ElectricMotor/GenericEMotor.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			Assert.AreEqual("0", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fld.First().LoadCurve.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMap = ElectricFullLoadCurveReader.Create(fld.First().LoadCurve, 2);
			Assert.AreEqual(-802.14 , fldMap.FullLoadDriveTorque(0.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(802.14, fldMap.FullGenerationTorque(0.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(-802.14, fldMap.FullLoadDriveTorque(50.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(802.14, fldMap.FullGenerationTorque(50.RPMtoRad()).Value(), 1e-3);

			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap; //ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			Assert.AreEqual("0", pwr.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwr.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9844.9", pwr.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwr, 2, ExecutionMode.Engineering);
			Assert.AreEqual(0, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);
			Assert.AreEqual(-208.04255, pwrMap.LookupElectricPower(1.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);


			Assert.AreEqual(-16412.2624, pwrMap.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

        }

        [TestCase()]
		public void TestReadHybridVehicle()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData/Hybrids/GenericVehicle_Group2_P2/Class2_RigidTruck_ParHyb_ENG.vecto");

			var engineering = inputProvider as IEngineeringInputDataProvider;

			Assert.NotNull(engineering);
			Assert.AreEqual(0.8, engineering.JobInputData.Vehicle.InitialSOC);

			var bat = engineering.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements.First().REESSPack as IBatteryPackEngineeringInputData;
			var ri = BatteryInternalResistanceReader.Create(bat.InternalResistanceCurve, false);
			var imax = BatteryMaxCurrentReader.Create(bat.MaxCurrentMap);
			
			Assert.NotNull(bat);
			Assert.AreEqual(2, engineering.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements.First().Count);
			Assert.AreEqual("50", bat.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.StateOfCharge]);
			Assert.AreEqual("375", bat.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.MaxDischargeCurrent]);
			Assert.AreEqual("375", bat.MaxCurrentMap.Rows[1][BatteryMaxCurrentReader.Fields.MaxDischargeCurrent]);
			Assert.AreEqual(375, imax.LookupMaxChargeCurrent(0.5).Value());
			Assert.AreEqual(-375, imax.LookupMaxDischargeCurrent(0.5).Value());
			Assert.AreEqual(0.04, ri.Lookup(0.5, 0.SI<Second>()).Value());

			var em = engineering.JobInputData.Vehicle.Components.ElectricMachines;

			Assert.NotNull(em);
			Assert.AreEqual(1, em.Entries.Count);

			Assert.AreEqual(PowertrainPosition.HybridP2, em.Entries[0].Position);

			Assert.AreEqual(0.2, em.Entries[0].ElectricMachine.Inertia.Value());

		}



		[TestCase]
		public void TestReadFuelCellHybridVehicle()
		{
			var filePath = @"TestData\H2_FCV\GenericVehicleE2 - FCHV\FCHV.vecto";

			Assert.IsTrue(File.Exists(filePath));

			var inputProvider = JSONInputDataFactory.ReadComponentData(filePath) as IEngineeringInputDataProvider;
			Assert.IsNotNull(inputProvider);
			Assert.AreEqual(VectoSimulationJobType.FCHV, inputProvider.JobInputData.JobType);
			
			Assert.AreEqual(3, inputProvider.JobInputData.Cycles.Count);

			// Vehicle
			var vehicle = inputProvider.JobInputData.Vehicle;
			Assert.IsNotNull(vehicle);
			Assert.NotNull(vehicle.Manufacturer);
			


			// Components

			var components = vehicle.Components;
			Assert.NotNull(components);
            // Gbx

            Assert.NotNull(inputProvider.JobInputData.Vehicle.Components.GearboxInputData);



			// FuelCellSystem
			var fuelCellSystem = vehicle.Components.FuelCellSystemInputData;
			Assert.NotNull(fuelCellSystem);
			
			Assert.NotNull(fuelCellSystem.FuelCellStrings);

			Assert.AreEqual(2, fuelCellSystem.FuelCellStrings.Count);
			Assert.AreEqual(2, fuelCellSystem.FuelCellStrings[0].Count);
			AssertFuelCellComponent(fuelCellSystem.FuelCellStrings[0].FuelCellComponent);

			Assert.AreEqual(1, fuelCellSystem.FuelCellStrings[1].Count);
			AssertFuelCellComponent(fuelCellSystem.FuelCellStrings[1].FuelCellComponent);
        }

		[TestCase]
		public void ReadFuelCellComponent()
		{
			var filePath = @"TestData\H2_FCV\GenericVehicleE2 - FCHV\GenericFuelCellComponent.vfcc";
			var inputDataProvider = JSONInputDataFactory.ReadFuelCellComponentEngineeringInputData(filePath, false) as IFuelCellComponentEngineeringInputData;

			AssertFuelCellComponent(inputDataProvider);
		}

		private static void AssertFuelCellComponent(IFuelCellComponentEngineeringInputData inputDataProvider)
		{
			Assert.IsNotNull(inputDataProvider);
			Assert.IsNotNull(inputDataProvider.MassFlowMap);
			Assert.AreEqual("Fuel Cell Manufacturer", inputDataProvider.Manufacturer);
			Assert.AreEqual("Generic Fuel Cell", inputDataProvider.Model);
			Assert.AreEqual(100.SI(Unit.SI.Kilo.Watt), inputDataProvider.MaxElectricPower);
			Assert.AreEqual(10.SI(Unit.SI.Kilo.Watt), inputDataProvider.MinElectricPower);
		}


		[TestCase()]
		public void TestCreateHybridPowertrain()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData/Hybrids/GenericVehicle_Group2_P2/Class2_RigidTruck_ParHyb_ENG.vecto");

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, null);

			var sumContainer = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			//var run = factory.SimulationRuns().ToArray()[0];

			//Assert.NotNull(run);

			//var pt = run.GetContainer();

			//Assert.NotNull(pt);

			//var port = run.GetContainer().GetCycleOutPort();
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}

		[TestCase()]
		public void TestCreateBatteryElectricPowertrain()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData/BatteryElectric/GenericVehicleB4/BEV_ENG.vecto");

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, null);

			var sumContainer = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			//var run = factory.SimulationRuns().ToArray()[0];

			//Assert.NotNull(run);

			//var pt = run.GetContainer();

			//Assert.NotNull(pt);

			//var port = run.GetContainer().GetCycleOutPort();
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}
    }
}