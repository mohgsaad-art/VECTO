using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.PTOLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class PTOLossMapTests
{
    const double Epsilon = 1e-7;
	static Entry _entry1 = new Entry() { EngineSpeed = 0.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };
	static Entry _entry1Equal = new Entry() { EngineSpeed = 0.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };
	static Entry _entry2 = new Entry() { EngineSpeed = 1.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };
	static Entry _entry3 = new Entry() { EngineSpeed = 0.SI<PerSecond>(), PTOTorque = 1.SI<NewtonMeter>() };
	static Entry _entry1Close = new Entry() { EngineSpeed = 0.001.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };
	static Entry _entry1Epsilon = new Entry() { EngineSpeed = Epsilon.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };

    public static IEnumerable<object[]> PTOLossMapEntryComparerEqualsTestCases()
	{
		yield return new object[] { null, null, true };
        yield return new object[] { _entry1, null, false };
        yield return new object[] { null, _entry1, false };
        yield return new object[] { _entry1, _entry1, true };
        yield return new object[] { _entry1, _entry1Equal, true };
        yield return new object[] { _entry1, _entry2, false };
        yield return new object[] { _entry1, _entry3, false };
        yield return new object[] { _entry1, _entry1Close, false };
        yield return new object[] { _entry1, _entry1Epsilon, true };
    }

    [Test, TestCaseSource(nameof(PTOLossMapEntryComparerEqualsTestCases))]
    public void Test_PTOLossMapEntryComparer_Equals(Entry a, Entry b, bool expected)
    {
        var result = (new PTOLossMapEntryComparer()).Equals(a, b);

        Assert.That(result, Is.EqualTo(expected));
    }

    public static IEnumerable<object[]> EqualityTestCases()
	{
		var map1 = new PTOLossMap(new Entry[1] { _entry1 });
        var map1Equal = new PTOLossMap(new Entry[1] { _entry1 });
        var map2 = new PTOLossMap(new Entry[1] { _entry2 });
        var map3 = new PTOLossMap(new Entry[1] { _entry3 });
        var map4 = new PTOLossMap(new Entry[2] { _entry1, _entry1 });

        yield return new object[] { null, null, true };
        yield return new object[] { map1, null, false };
        yield return new object[] { null, map1, false };
        yield return new object[] { map1, map1, true };
        yield return new object[] { map1, map1Equal, true };
        yield return new object[] { map2, map1, false };
        yield return new object[] { map3, map1, false };
        yield return new object[] { map4, map1, false };
    }

    [Test, TestCaseSource(nameof(EqualityTestCases))]
    public void Test_PTOLossMap_OperatorEqual(PTOLossMap a, PTOLossMap b, bool expected)
    {
        var result = (a == b);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test, TestCaseSource(nameof(EqualityTestCases))]
    public void Test_PTOLossMap_OperatorNotEqual(PTOLossMap a, PTOLossMap b, bool notExpected)
    {
        var result = (a != b);

        Assert.That(result, Is.EqualTo(!notExpected));
    }

    [Test, TestCaseSource(nameof(EqualityTestCases))]
    public void Test_PTOLossMap_Equals(PTOLossMap a, PTOLossMap b, bool expected)
    {
        if ((a is null) || (b is null))
        {
            return;
        }

        var result = a.Equals(b);

        Assert.That(result, Is.EqualTo(expected));
    }

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