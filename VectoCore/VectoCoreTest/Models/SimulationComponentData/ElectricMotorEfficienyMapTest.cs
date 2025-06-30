using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData {
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ElectricMotorEfficienyMapTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		
		[TestCase(-10000, 700, -118.916545), // EM drive has negative torque and thus negative electric power
		TestCase(-20000, 1200, -141.451404),
		TestCase(10000, 700, 155.046977),
		TestCase(20000, 1200, 177.530544)]
		public void TestLookupTorqueForBatPower(double batPwr, double emSpeed, double expectedTq)
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData/Hybrids/ElectricMotor/GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld.First().LoadCurve, 1);

			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap; //ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1, ExecutionMode.Engineering);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.NotNull(tq);
			var lookup = pwrMap.LookupElectricPower(emSpeed.RPMtoRad(), tq, true);

			Assert.AreEqual(lookup.ElectricalPower.Value(), batPwr, 1e-6);
			Assert.AreEqual(expectedTq, tq.Value(), 1e-3);
		}


		//[Test]
		//public void TestMahleEMLookup()
		//{
		//	var inputProvider =
		//		JSONInputDataFactory.ReadElectricMotorData(@"E:/QUAM/Downloads/2022-07-07 Hybrid 40 kW System_sent.7z/2022-07-07 Hybrid 40 kW System_sent/TDS 40kW MTM 48-210x65 V2.04.vem", false);

		//	var pwr = inputProvider.VoltageLevels.Last().PowerMap.First().PowerMap;
		//	var fld = inputProvider.VoltageLevels.Last().FullLoadCurve;
		//	var pwrMap = ElectricMotorMapReader.Create(pwr, 1);
		//	var fldMap = ElectricFullLoadCurveReader.Create(fld, 1);

		//	//var tq = pwrMap.LookupTorque(-45058.6788.SI<Watt>(), 6542.3.RPMtoRad(), -67.0253.SI<NewtonMeter>());

		//	//Assert.AreEqual(-56.72495079, tq.Value(), 1e-6);

		//	var nEm = 4797.89676.RPMtoRad();
		//	var PEl = 47959.8400.SI<Watt>();

		//	var maxRecupTq = fldMap.FullGenerationTorque(nEm);
		//	var elPwrRecup = pwrMap.LookupElectricPower(nEm, maxRecupTq);

		//	var recupTq = pwrMap.LookupTorque(PEl, nEm, maxRecupTq);

		//	for (var tq = maxRecupTq - 50.SI<NewtonMeter>();
		//		tq < maxRecupTq + 50.SI<NewtonMeter>();
		//		tq += 1.SI<NewtonMeter>()) {
		//		var pEl = pwrMap.LookupElectricPower(nEm, tq, false);
		//		if (pEl.ElectricalPower == null) {
		//			continue;
		//		}
		//		Console.WriteLine($"{tq.Value()}, {pEl.ElectricalPower.Value()}, {(pEl.ElectricalPower - PEl).Value()}");
		//	}

		//	Assert.IsTrue(maxRecupTq > recupTq);
		//	var elPwr = pwrMap.LookupElectricPower(nEm, recupTq);
		//	Assert.AreEqual(PEl.Value(), elPwr.ElectricalPower.Value(), Constants.SimulationSettings.InterpolateSearchTolerance);
		//}

		[TestCase(-1000000, 700), // EM drive has negative torque and thus negative electric power
		TestCase(1000000, 700)]
		public void TestLookupTorqueForBatPower2(double batPwr, double emSpeed)
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData/Hybrids/ElectricMotor/GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld.First().LoadCurve, 1); 
			
			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap;//ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1, ExecutionMode.Engineering);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.IsNull(tq);

		}

		[TestCase(@"TestData/Components/ElectricMotor/vem_P_inverter_DC_90_fine.vemo", 0.8999, 0.9001),
		TestCase(@"TestData/Components/ElectricMotor/vem_P_inverter_DC_90_coarse.vemo", 0.8999, 0.9001),
		TestCase(@"TestData/Components/ElectricMotor/vem_P_inverter_DC_std.vemo", 0, 1)]
		public void TestInterpolationMethod_PowerMap(string filename, double etaMin, double etaMax)
		{
			var data = VectoCSVFile.Read(filename).ApplyFactor(ElectricMotorMapReader.Fields.PowerElectrical, 1000.0);
			var emMap = ElectricMotorMapReader.Create(data, 1, ExecutionMode.Engineering);
			
			var efficiencies = new List<double>();
			for (var n = 10.RPMtoRad(); n < 4000.RPMtoRad(); n += 10.RPMtoRad()) {
				for (var tq = -2800.SI<NewtonMeter>(); tq <= 2800.SI<NewtonMeter>(); tq += 100.SI<NewtonMeter>()) {
					if (tq.IsEqual(0)) {
						continue;
					}
					try {
						var pwr = emMap.LookupElectricPower(n, tq);
						if (pwr.ElectricalPower == null) {
							continue;
						}
						Console.WriteLine($"{pwr.Speed.AsRPM}, {pwr.Torque.Value()}, {pwr.ElectricalPower.Value()}, {pwr.Extrapolated}, {(n * tq).Value()}");
						var efficiency = tq < 0
							? pwr.Speed * pwr.Torque / pwr.ElectricalPower
							: pwr.ElectricalPower / (pwr.Speed * pwr.Torque);
						// set efficiencies close to 0 to exactly 0 because later assertion uses sharp bounds without tolerance
						efficiencies.Add(efficiency.IsEqual(0) ? 0.0 : efficiency);
					} catch (Exception e) {
						Console.WriteLine(e.Message);
					}
				}
			}

			Assert.IsTrue(efficiencies.All(x => x.IsBetween(etaMin, etaMax)), $"{efficiencies.Min()} - {efficiencies.Max()}");
		}

		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ComponentData/ElectricMachineSystem_Std_Overload.xml", 600063.423)]
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ComponentData/ElectricMachineSystem_Std_Overload2.xml", 600063.423)]
		[Category(Definitions.TESTCASE_MIGRATED)]
		public void TestElectricMotorOverloadBufferTest(string testFile, double expectedOvlBfr)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());

			var componentFactory = kernel.Get<IXMLComponentInputReader>();
			var componentData = componentFactory.CreateFromFile<IXMLElectricMotorDeclarationInputData>(testFile);
			var em = new XMLElectricMachinesDeclarationData(
				new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>> {
					new ElectricMachineEntry<IElectricMotorDeclarationInputData>() {
						Count = 1,
						ElectricMachine = componentData
					}
				});
			var da = new ElectricMachinesDataAdapter();

            var modelData = da.CreateElectricMachines(em, null, 500.SI<Volt>());
			var emData = modelData.First().Item2;

			Assert.AreEqual(expectedOvlBfr, emData.Overload.OverloadBuffer.Value(), 0.1);
		}
		
	}
}