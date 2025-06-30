using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class PTOLossMapTests
{
	[TestCase]
	public void PTOIdleLosses_FixPoints()
	{
		var entryList = new List<PTOLossMap.Entry>();
		for (var i = 0; i < 2000; i += 200) {
			entryList.Add(new PTOLossMap.Entry {
				EngineSpeed = i.RPMtoRad(),
				PTOTorque = (Math.Sqrt(i) / 10).SI<NewtonMeter>()
			});
		}
		var pto = new PTOLossMap(entryList.ToArray());

		foreach (var entry in entryList) {
			Assert.AreEqual(entry.PTOTorque, pto.GetTorqueLoss(entry.EngineSpeed));
		}
	}

	[TestCase]
	public void PTOIdleLosses_Interpolate()
	{
		var entryList = new List<PTOLossMap.Entry>();
		for (var i = 0; i < 2000; i += 200) {
			entryList.Add(new PTOLossMap.Entry {
				EngineSpeed = i.RPMtoRad(),
				PTOTorque = (Math.Sqrt(i) / 10).SI<NewtonMeter>()
			});
		}
		var pto = new PTOLossMap(entryList.ToArray());

		for (var i = 1; i < entryList.Count; i++) {
			var v1 = entryList[i - 1];
			var v2 = entryList[i];

			for (var f = v1.EngineSpeed; f < v2.EngineSpeed; f += 10.RPMtoRad()) {
				AssertHelper.AreRelativeEqual(
					VectoMath.Interpolate(v1.EngineSpeed, v2.EngineSpeed, v1.PTOTorque, v2.PTOTorque, f),
					pto.GetTorqueLoss(f));
			}
		}
	}
}