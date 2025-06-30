using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class TrasmissionLossmapReaderTests
{
	/// <summary>
	/// VECTO-230
	/// </summary>
	[TestCase]
	public void TestLossMapValuesWithEfficiency()
	{
		var lossMap = TransmissionLossMapReader.Create(0.95, 1.0, "Dummy");

		AssertHelper.AreRelativeEqual(0.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(50.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(1000.RPMtoRad(), 950.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(40.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(1000.RPMtoRad(), 760.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(25.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(1000.RPMtoRad(), 475.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(5.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(1000.RPMtoRad(), 95.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(50.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(500.RPMtoRad(), 950.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(25.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(500.RPMtoRad(), 475.SI<NewtonMeter>()).Value);

		AssertHelper.AreRelativeEqual(5.SI<NewtonMeter>(),
			lossMap.GetTorqueLoss(100.RPMtoRad(), 95.SI<NewtonMeter>()).Value);
	}
}