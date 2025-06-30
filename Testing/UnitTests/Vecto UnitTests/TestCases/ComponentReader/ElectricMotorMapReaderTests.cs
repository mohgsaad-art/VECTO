using System.Data;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class ElectricMotorMapReaderTests
{
	[TestCase(25, -1050, -2804.993)]
	[TestCase(255, 1050, 27477.94)]
	public void TestEM_EfficiencyCorrection(double rpm, double tq, double expectedPel)
	{
		var input = new Mock<IElectricMotorPowerMap>();
		input.Setup(i => i.PowerMap).Returns(InputDataHelper.InputDataAsTableData(EmMapHdr, EmMapData));
		var map = ElectricMotorMapReader.Create(input.Object.PowerMap, 1, ExecutionMode.Declaration);

		var entry = map.Entries.First(x => x.Torque.IsEqual(tq, 0.1) && x.MotorSpeed.AsRPM.IsEqual(rpm, 0.1));

        var inputRow = input.Object.PowerMap.AsEnumerable().First(r => r.ParseDouble(ElectricMotorMapReader.Fields.MotorSpeed).IsEqual(rpm, 0.1) &&
																r.ParseDouble(ElectricMotorMapReader.Fields.Torque).IsEqual(-tq, 0.1));
		var inputPwrEl = inputRow.ParseDouble(ElectricMotorMapReader.Fields.PowerElectrical);

		// check that in the input the efficiency is greater than 1
		Assert.IsTrue(-inputPwrEl > expectedPel);
		if (tq < 0) {
			// propulsion
			Assert.IsTrue(rpm.RPMtoRad() * -tq.SI<NewtonMeter>() / inputPwrEl > 1);
		} else {
			// recuperation
			Assert.IsTrue(inputPwrEl / (rpm.RPMtoRad() * -tq.SI<NewtonMeter>()) > 1);
		}

		// calculate electric power as the raw map contains a virtual 'torque loss' of the EM.
		var elPower = entry.MotorSpeed * entry.Torque + entry.PowerElectrical.Value().SI<NewtonMeter>() * entry.MotorSpeed;

		// < 0 means propulsion, hence the electric power needs to be 'more negative'
		Assert.IsTrue(entry.MotorSpeed * entry.Torque > elPower);

		Assert.AreEqual(expectedPel, elPower.Value(), 0.1);
    }

	private const string EmMapHdr = "";

	private static readonly string[] EmMapData = new[] {
		" 0.00, -1050.00, 0.00",
		" 0.00, -840.00, 0.00",
		" 0.00, -630.00, -0.14",
		" 0.00, -420.00, -0.43",
		" 0.00, -210.00, -0.36",
		" 0.00, -11.00, -0.01",
		" 0.00, 11.00, 0.08",
		" 0.00, 210.00, 1.32",
		" 0.00, 420.00, 2.92",
		" 0.00, 630.00, 4.81",
		" 0.00, 840.00, 6.98",
		" 0.00, 1050.00, 9.44",
		" 25.00, -1050.00, 0.00",
		" 25.00, -840.00, 0.00",
		" 25.00, -630.00, -1632.84",
		" 25.00, -420.00, -1088.56",
		" 25.00, -210.00, -544.28",
		" 25.00, -11.00, -28.51",
		" 25.00, 11.00, 29.09",
		" 25.00, 210.00, 555.33",
		" 25.00, 420.00, 1110.66",
		" 25.00, 630.00, 1666.00",
		" 25.00, 840.00, 2221.33",
		" 25.00, 1050.00, 2676.66", // single entry with efficiency > 1. this is automatically 'corrected' by VECTO
		" 255.00, -1050.00, -30758.33",
		" 255.00, -840.00, -22206.66",
		" 255.00, -630.00, -16655.00",
		" 255.00, -420.00, -11103.33",
		" 255.00, -210.00, -5551.67",
		" 255.00, -11.00, -290.80",
		" 255.00, 11.00, 296.71",
		" 255.00, 210.00, 5664.39",
		" 255.00, 420.00, 11328.77",
		" 255.00, 630.00, 16993.16",
		" 255.00, 840.00, 22657.55",
		" 255.00, 1050.00, 28321.93",
		" 2037.00, -1050.00, -221740.05",
		" 2037.00, -840.00, -177392.04",
		" 2037.00, -630.00, -133044.03",
		" 2037.00, -420.00, -88696.02",
		" 2037.00, -210.00, -44348.01",
		" 2037.00, -11.00, 0.00",
		" 2037.00, 11.00, 2370.16",
		" 2037.00, 210.00, 45248.45",
		" 2037.00, 420.00, 90496.91",
		" 2037.00, 630.00, 135745.36",
		" 2037.00, 840.00, 180993.82",
		" 2037.00, 1050.00, 226242.27",
	};
}