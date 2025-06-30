using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class FactorMethodGenericBatteryDataTests
{
    [TestCase(0.5)]
    public void TestGenericBatteryData(double initialSoC)
	{
		var electricStorage = GetMockBatteryVIFInputData();
        var genericBusBatteryData = new GenericBusBatteryData();
        var batterySystemData = genericBusBatteryData.CreateBatteryData(electricStorage, VectoSimulationJobType.BatteryElectricVehicle, true);

        Assert.AreEqual(initialSoC, batterySystemData.InitialSoC);
        Assert.AreEqual(2, batterySystemData.Batteries.Count);

        var battery0 = batterySystemData.Batteries[0];
        Assert.AreEqual(0.785, battery0.Item2.MaxSOC, 1e-6);
        Assert.AreEqual(0.215, battery0.Item2.MinSOC, 1e-6);
        Assert.AreEqual(72, battery0.Item2.Capacity.AsAmpHour);
        Assert.AreEqual(2, battery0.Item2.InternalResistance.Entries.Length);

        Assert.AreEqual(0, battery0.Item2.InternalResistance.Entries[0].SoC);
        Assert.AreEqual(3, battery0.Item2.InternalResistance.Entries[0].Resistance.Count);

        var resistance = battery0.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value();
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value());
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[1].Item2.Value());
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[2].Item2.Value());
        Assert.AreEqual(1, battery0.Item2.InternalResistance.Entries[1].SoC);
        Assert.AreEqual(3, battery0.Item2.InternalResistance.Entries[1].Resistance.Count);
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[0].Item2.Value());
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[1].Item2.Value());
        Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[2].Item2.Value());

        var battery1 = batterySystemData.Batteries[1];
        Assert.AreEqual(0.785, battery1.Item2.MaxSOC, 1e-6);
        Assert.AreEqual(0.215, battery1.Item2.MinSOC, 1e-6);
        Assert.AreEqual(72, battery1.Item2.Capacity.AsAmpHour);
        Assert.AreEqual(2, battery1.Item2.InternalResistance.Entries.Length);

        Assert.AreEqual(0, battery1.Item2.InternalResistance.Entries[0].SoC);
        Assert.AreEqual(3, battery1.Item2.InternalResistance.Entries[0].Resistance.Count);

        resistance = battery1.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value();
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value());
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[1].Item2.Value());
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[2].Item2.Value());
        Assert.AreEqual(1, battery1.Item2.InternalResistance.Entries[1].SoC);
        Assert.AreEqual(3, battery1.Item2.InternalResistance.Entries[1].Resistance.Count);
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[0].Item2.Value());
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[1].Item2.Value());
        Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[2].Item2.Value());
    }

	[TestCase(1)]
	public void TestGenericSuperCapData(double initialSoC)
	{
		var superCap = GetMockSuperCapInputData();

		var genericBusSuperCapData = new GenericBusSuperCapData();
		var superCapData = genericBusSuperCapData.CreateGenericSuperCapData(superCap);

		Assert.AreEqual(37.0, superCapData.Capacity.Value());
		Assert.AreEqual(0, superCapData.MinVoltage.Value());
		Assert.AreEqual(330.0, superCapData.MaxVoltage.Value());
		Assert.AreEqual(100, superCapData.MaxCurrentCharge.Value());
		Assert.AreEqual(-100, superCapData.MaxCurrentDischarge.Value());
		Assert.AreEqual(initialSoC, superCapData.InitialSoC);
		Assert.AreEqual(0.55282, superCapData.InternalResistance.Value(), 1e-4);
	}

	private ISuperCapDeclarationInputData GetMockSuperCapInputData()
	{
		var cap = new Mock<ISuperCapDeclarationInputData>();
		cap.Setup(c => c.Capacity).Returns(37.0.SI<Farad>());
		cap.Setup(c => c.MinVoltage).Returns(0.SI<Volt>());
		cap.Setup(c => c.MaxVoltage).Returns(330.SI<Volt>());
		cap.Setup(c => c.MaxCurrentCharge).Returns(100.SI<Ampere>());
		cap.Setup(c => c.MaxCurrentDischarge).Returns(100.SI<Ampere>());
		return cap.Object;
	}

	private IElectricStorageSystemDeclarationInputData GetMockBatteryVIFInputData()
	{
		var bat = new Mock<IElectricStorageSystemDeclarationInputData>();
		var b1 = new Mock<IElectricStorageDeclarationInputData>();
		var b2 = new Mock<IElectricStorageDeclarationInputData>();
		bat.Setup(b => b.ElectricStorageElements).Returns(new List<IElectricStorageDeclarationInputData>()
			{ b1.Object, b2.Object });
		var reess = new Mock<IBatteryPackDeclarationInputData>();
		b1.Setup(b => b.REESSPack).Returns(reess.Object);
		b1.Setup(b => b.StringId).Returns(0);

		b2.Setup(b => b.REESSPack).Returns(reess.Object);
		b1.Setup(b => b.StringId).Returns(1);

        reess.Setup(r => r.Capacity).Returns(72.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());
		reess.Setup(r => r.BatteryType).Returns(BatteryType.HPBS);
		var current = InputDataHelper.InputDataAsTableData(CurrentHdr, CurrentData);
		var ovc = InputDataHelper.InputDataAsTableData(OvcHdr, OvcData);
		reess.Setup(r => r.MaxCurrentMap).Returns(current);
		reess.Setup(r => r.VoltageCurve).Returns(ovc);
		reess.Setup(r => r.MinSOC).Returns(0.2);
		reess.Setup(r => r.MaxSOC).Returns(0.8);
        return bat.Object;
	}

	private const string CurrentHdr = "Soc, I_Charge, I_discharge";
	private static readonly string[] CurrentData = new[] {
        "0, 50, 0",
        "100, 0, 50"
	};

	private const string OvcHdr = "SoC, V";
	private static readonly string[] OvcData = new[] {
        "0, 620.0",
        "100, 640.0"
	};
}