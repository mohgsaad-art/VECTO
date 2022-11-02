using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
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
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld, 1);

			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap; //ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.NotNull(tq);
			var lookup = pwrMap.LookupElectricPower(emSpeed.RPMtoRad(), tq, true);

			Assert.AreEqual(lookup.ElectricalPower.Value(), batPwr, 1e-6);
			Assert.AreEqual(expectedTq, tq.Value(), 1e-3);
		}

		[TestCase(-1000000, 700), // EM drive has negative torque and thus negative electric power
		TestCase(1000000, 700)]
		public void TestLookupTorqueForBatPower2(double batPwr, double emSpeed)
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld, 1); 
			
			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap;//ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.IsNull(tq);

		}

		[TestCase(@"TestData\Components\ElectricMotor\vem_P_inverter_DC_90_fine.vemo", 0.8999, 0.9001),
		TestCase(@"TestData\Components\ElectricMotor\vem_P_inverter_DC_90_coarse.vemo", 0.8999, 0.9001),
		TestCase(@"TestData\Components\ElectricMotor\vem_P_inverter_DC_std.vemo", 0, 1)]
		public void TestInterpolationMethod_PowerMap(string filename, double etaMin, double etaMax)
		{
			EfficiencyMap emMap;
			using (var fs = File.OpenRead(filename)) {
				emMap = ElectricMotorMapReader.Create(fs, 1);
			}

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

		
	}
}