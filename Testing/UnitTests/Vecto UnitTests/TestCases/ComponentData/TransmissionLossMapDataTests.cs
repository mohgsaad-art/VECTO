using System.Data;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class TransmissionLossMapDataTests
{
    [TestCase]
    public void TestLossMap_IN_10_CONST_Interpolation_Extrapolation()
    {
        var data = new DataTable();
        data.Columns.Add("");
        data.Columns.Add("");
        data.Columns.Add("");
        data.Rows.Add("0", "0", "10"); //         (0,100):10 --  (100,100):10
        data.Rows.Add("0", "100", "10"); //        |      \          |
        data.Rows.Add("100", "0", "10"); //        |       \         |
        data.Rows.Add("100", "100", "10"); //    (0,0):10  ----- (100,10):10

        var map = TransmissionLossMapReader.Create(data, 1.0, "1");

        // test inside the triangles
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()).Value);

        // test interpolation on edges
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()).Value);

        // test interpolation on corner points
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()).Value);

        // test outside the corners
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()).Value);

        // test outside the edges
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
    }

    [TestCase]
    public void TestLossMap_IN_Interpolation_Extrapolation()
    {
        var data = new DataTable();
        data.Columns.Add("");
        data.Columns.Add("");
        data.Columns.Add("");
        data.Rows.Add("0", "0", "0"); //         (0,110):10 --  (100,140):40
        data.Rows.Add("0", "110", "10"); //        |      \         |
        data.Rows.Add("100", "10", "10"); //        |       \       |
        data.Rows.Add("100", "140", "40"); //    (0,0):0 ----- (100,10):10

        var map = TransmissionLossMapReader.Create(data, 1.0, "1");

        // test inside the triangles
        AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()).Value);

        // test interpolation on edges
        AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(4.5, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()).Value);

        // test interpolation on corner points
        AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(28, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()).Value);

        // test outside the corners
        AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(40, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()).Value);

        // test outside the edges
        AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
        AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
    }


    [TestCase()]
	public void TestLossMapExtension()
	{
		var gbxData = InputDataHelper.InputDataAsTableData(GbxMapHdr, GbxMapData);

		var lossMapOrig = TransmissionLossMapReader.Create(gbxData, 1.0, "origLossMap");
		var extendedMap = TransmissionLossMapReader.Create(gbxData, 1.0, "origLossMap", true);

		var rpm = 100.RPMtoRad();
		var tq = -3000.SI<NewtonMeter>();
		var lookupOrig = lossMapOrig.GetTorqueLoss(rpm, tq);
		var lookupExt = extendedMap.GetTorqueLoss(rpm, tq);

		//foreach (var entry in gearboxDataExt.Gears[7].LossMap._entries) {
		//	Console.WriteLine(string.Format("{0},{1},{2}", entry.InputSpeed.AsRPM, entry.InputTorque.Value(), entry.TorqueLoss.Value()));
		//}

		Assert.IsTrue(lookupOrig.Extrapolated);
		Assert.IsFalse(lookupExt.Extrapolated);

		rpm = 1200.RPMtoRad();
		lookupOrig = lossMapOrig.GetTorqueLoss(rpm, tq);
		lookupExt = extendedMap.GetTorqueLoss(rpm, tq);

		Assert.IsTrue(lookupOrig.Extrapolated);
		Assert.IsFalse(lookupExt.Extrapolated);
	}

    [TestCase()]
	public void TestAxlegearLossMapExtension()
	{
		var lossMap = InputDataHelper.InputDataAsTableData(AxlMapHdr, AxlMapData);
		var origLossMap = TransmissionLossMapReader.Create(lossMap, 3.240355, "AxleOrig");
		var extendedLossMap = TransmissionLossMapReader.Create(lossMap, 3.240355, "AxleExtended", true);

		//foreach (var entry in gearboxDataExt.AxleGear.LossMap._entries) {
		//	Console.WriteLine(string.Format("{0},{1},{2}", entry.InputSpeed.AsRPM, entry.InputTorque.Value(), entry.TorqueLoss.Value()));
		//}

		var rpm = 100.RPMtoRad();
		var tq = 80000.SI<NewtonMeter>();
		var lookupOrig = origLossMap.GetTorqueLoss(rpm, tq);
		var lookupExt = extendedLossMap.GetTorqueLoss(rpm, tq);

		Assert.IsTrue(lookupOrig.Extrapolated);
		Assert.IsFalse(lookupExt.Extrapolated);

		rpm = 1000.RPMtoRad();
		lookupOrig = origLossMap.GetTorqueLoss(rpm, tq);
		lookupExt = extendedLossMap.GetTorqueLoss(rpm, tq);

		Assert.IsTrue(lookupOrig.Extrapolated);
		Assert.IsFalse(lookupExt.Extrapolated);
	}

	[TestCase()]
	public void TestLossMapExtensionNegativeRegressionSlope()
	{
		var entries = new[] {
			"Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm] # this is a comment",
			"200,-2500,60",
			"200,0,30",
			"200,100,35",
			"200,200,40",
			"200,500,32",
			"200,1000,30",
			"200,2000,25",
			"200,2500,23",
			"500,-2500,60",
			"500,0,35",
			"500,2500,23"
		};
		var mstream = new MemoryStream();
		var writer = new StreamWriter(mstream);

		foreach (var entry in entries) {
			writer.WriteLine(entry);
		}
		writer.Flush();
		mstream.Flush();
		mstream.Seek(0, SeekOrigin.Begin);

		var lossMap = TransmissionLossMapReader.Create(VectoCSVFile.ReadStream(mstream), 1.0, "TestGear", true);

		var validation = lossMap.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.AMT, false);

		NUnit.Framework.Assert.AreEqual(0, validation.Count);

		var lookup1 = lossMap.GetTorqueLoss(200.RPMtoRad(), 500.SI<NewtonMeter>());
		NUnit.Framework.Assert.IsFalse(lookup1.Extrapolated);
		NUnit.Framework.Assert.AreEqual(31.8725, lookup1.Value.Value(), 1e-3);

		var lookup2 = lossMap.GetTorqueLoss(200.RPMtoRad(), 7000.SI<NewtonMeter>());
		NUnit.Framework.Assert.IsFalse(lookup2.Extrapolated);
		NUnit.Framework.Assert.AreEqual(23, lookup2.Value.Value());
	}


    const string AxlMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm] # this is a comment";
    private readonly static string[] AxlMapData = new string[] {
        "# rpm, Nm, Nm",
        "# this is a comment",
        "0,-2500,77.5  # this is a comment",
        "0,-1500,62.5",
        "0,-500,47.5",
        "0,500,47.5",
        "0,1500,62.5",
        "0,2500,77.5",
        "0,3500,92.5",
        "0,4500,107.5",
        "# this is a comment",
        "0,5500,122.5",
        "0,6500,137.5",
        "0,7500,152.5",
        "0,8500,167.5",
        "0,9500,182.5",
        "0,10500,197.5",
        "0,11500,212.5",
        "0,12500,227.5",
        "0,13500,242.5",
        "0,14500,257.5",
        "0,15500,272.5",
        "200,-2500,77.5",
        "200,-1500,62.5",
        "200,-500,47.5",
        "200,500,47.5",
        "200,1500,62.5",
        "200,2500,77.5",
        "200,3500,92.5",
        "200,4500,107.5",
        "200,5500,122.5",
        "200,6500,137.5",
        "200,7500,152.5",
        "200,8500,167.5",
        "200,9500,182.5",
        "200,10500,197.5",
        "200,11500,212.5",
        "200,12500,227.5",
        "200,13500,242.5",
        "200,14500,257.5",
        "200,15500,272.5",
        "400,-2500,77.5",
        "400,-1500,62.5",
        "400,-500,47.5",
        "400,500,47.5",
        "400,1500,62.5",
        "400,2500,77.5",
        "400,3500,92.5",
        "400,4500,107.5",
        "400,5500,122.5",
        "400,6500,137.5",
        "400,7500,152.5",
        "400,8500,167.5",
        "400,9500,182.5",
        "400,10500,197.5",
        "400,11500,212.5",
        "400,12500,227.5",
        "400,13500,242.5",
        "400,14500,257.5",
        "400,15500,272.5",
        "600,-2500,77.5",
        "600,-1500,62.5",
        "600,-500,47.5",
        "600,500,47.5",
        "600,1500,62.5",
        "600,2500,77.5",
        "600,3500,92.5",
        "600,4500,107.5",
        "600,5500,122.5",
        "600,6500,137.5",
        "600,7500,152.5",
        "600,8500,167.5",
        "600,9500,182.5",
        "600,10500,197.5",
        "600,11500,212.5",
        "600,12500,227.5",
        "600,13500,242.5",
        "600,14500,257.5",
        "600,15500,272.5",
        "800,-2500,77.5",
        "800,-1500,62.5",
        "800,-500,47.5",
        "800,500,47.5",
        "800,1500,62.5",
        "800,2500,77.5",
        "800,3500,92.5",
        "800,4500,107.5",
        "800,5500,122.5",
        "800,6500,137.5",
        "800,7500,152.5",
        "800,8500,167.5",
        "800,9500,182.5",
        "800,10500,197.5",
        "800,11500,212.5",
        "800,12500,227.5",
        "800,13500,242.5",
        "800,14500,257.5",
        "800,15500,272.5",
        "1000,-2500,77.5",
        "1000,-1500,62.5",
        "1000,-500,47.5",
        "1000,500,47.5",
        "1000,1500,62.5",
        "1000,2500,77.5",
        "1000,3500,92.5",
        "1000,4500,107.5",
        "1000,5500,122.5",
        "1000,6500,137.5",
        "1000,7500,152.5",
        "1000,8500,167.5",
        "1000,9500,182.5",
        "1000,10500,197.5",
        "1000,11500,212.5",
        "1000,12500,227.5",
        "1000,13500,242.5",
        "1000,14500,257.5",
        "1000,15500,272.5",
        "1200,-2500,77.5",
        "1200,-1500,62.5",
        "1200,-500,47.5",
        "1200,500,47.5",
        "1200,1500,62.5",
        "1200,2500,77.5",
        "1200,3500,92.5",
        "1200,4500,107.5",
        "1200,5500,122.5",
        "1200,6500,137.5",
        "1200,7500,152.5",
        "1200,8500,167.5",
        "1200,9500,182.5",
        "1200,10500,197.5",
        "1200,11500,212.5",
        "1200,12500,227.5",
        "1200,13500,242.5",
        "1200,14500,257.5",
        "1200,15500,272.5",
        "1400,-2500,77.5",
        "1400,-1500,62.5",
        "1400,-500,47.5",
        "1400,500,47.5",
        "1400,1500,62.5",
        "1400,2500,77.5",
        "1400,3500,92.5",
        "1400,4500,107.5",
        "1400,5500,122.5",
        "1400,6500,137.5",
        "1400,7500,152.5",
        "1400,8500,167.5",
        "1400,9500,182.5",
        "1400,10500,197.5",
        "1400,11500,212.5",
        "1400,12500,227.5",
        "1400,13500,242.5",
        "1400,14500,257.5",
        "1400,15500,272.5",
        "1600,-2500,77.5",
        "1600,-1500,62.5",
        "1600,-500,47.5",
        "1600,500,47.5",
        "1600,1500,62.5",
        "1600,2500,77.5",
        "1600,3500,92.5",
        "1600,4500,107.5",
        "1600,5500,122.5",
        "1600,6500,137.5",
        "1600,7500,152.5",
        "1600,8500,167.5",
        "1600,9500,182.5",
        "1600,10500,197.5",
        "1600,11500,212.5",
        "1600,12500,227.5",
        "1600,13500,242.5",
        "1600,14500,257.5",
        "1600,15500,272.5",
        "1800,-2500,77.5",
        "1800,-1500,62.5",
        "1800,-500,47.5",
        "1800,500,47.5",
        "1800,1500,62.5",
        "1800,2500,77.5",
        "1800,3500,92.5",
        "1800,4500,107.5",
        "1800,5500,122.5",
        "1800,6500,137.5",
        "1800,7500,152.5",
        "1800,8500,167.5",
        "1800,9500,182.5",
        "1800,10500,197.5",
        "1800,11500,212.5",
        "1800,12500,227.5",
        "1800,13500,242.5",
        "1800,14500,257.5",
        "1800,15500,272.5",
        "2000,-2500,77.5",
        "2000,-1500,62.5",
        "2000,-500,47.5",
        "2000,500,47.5",
        "2000,1500,62.5",
        "2000,2500,77.5",
        "2000,3500,92.5",
        "2000,4500,107.5",
        "2000,5500,122.5",
        "2000,6500,137.5",
        "2000,7500,152.5",
        "2000,8500,167.5",
        "2000,9500,182.5",
        "2000,10500,197.5",
        "2000,11500,212.5",
        "2000,12500,227.5",
        "2000,13500,242.5",
        "2000,14500,257.5",
        "2000,15500,272.5",
        "2200,-2500,77.5",
        "2200,-1500,62.5",
        "2200,-500,47.5",
        "2200,500,47.5",
        "2200,1500,62.5",
        "2200,2500,77.5",
        "2200,3500,92.5",
        "2200,4500,107.5",
        "2200,5500,122.5",
        "2200,6500,137.5",
        "2200,7500,152.5",
        "2200,8500,167.5",
        "2200,9500,182.5",
        "2200,10500,197.5",
        "2200,11500,212.5",
        "2200,12500,227.5",
        "2200,13500,242.5",
        "2200,14500,257.5",
        "2200,15500,272.5",
        "2400,-2500,77.5",
        "2400,-1500,62.5",
        "2400,-500,47.5",
        "2400,500,47.5",
        "2400,1500,62.5",
        "2400,2500,77.5",
        "2400,3500,92.5",
        "2400,4500,107.5",
        "2400,5500,122.5",
        "2400,6500,137.5",
        "2400,7500,152.5",
        "2400,8500,167.5",
        "2400,9500,182.5",
        "2400,10500,197.5",
        "2400,11500,212.5",
        "2400,12500,227.5",
        "2400,13500,242.5",
        "2400,14500,257.5",
        "2400,15500,272.5",
        "2600,-2500,77.5",
        "2600,-1500,62.5",
        "2600,-500,47.5",
        "2600,500,47.5",
        "2600,1500,62.5",
        "2600,2500,77.5",
        "2600,3500,92.5",
        "2600,4500,107.5",
        "2600,5500,122.5",
        "2600,6500,137.5",
        "2600,7500,152.5",
        "2600,8500,167.5",
        "2600,9500,182.5",
        "2600,10500,197.5",
        "2600,11500,212.5",
        "2600,12500,227.5",
        "2600,13500,242.5",
        "2600,14500,257.5",
        "2600,15500,272.5",
        "3600,-2500,77.5",
        "3600,-1500,62.5",
        "3600,-500,47.5",
        "3600,500,47.5",
        "3600,1500,62.5",
        "3600,2500,77.5",
        "3600,3500,92.5",
        "3600,4500,107.5",
        "3600,5500,122.5",
        "3600,6500,137.5",
        "3600,7500,152.5",
        "3600,8500,167.5",
        "3600,9500,182.5",
        "3600,10500,197.5",
        "3600,11500,212.5",
        "3600,12500,227.5",
        "3600,13500,242.5",
        "3600,14500,257.5",
        "3600,15500,272.5",
    };

    const string GbxMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm],Eff [-]";

	private readonly string[] GbxMapData = new[] {
		"0,-650,18.06,",
		"0,-850,22.06,",
		"0,-1050,26.06,",
		"0,-1250,30.06,",
		"0,-1450,34.06,",
		"0,-1650,38.06,",
		"0,-1850,42.06,",
		"0,-2050,46.06,",
		"0,-2250,50.06,",
		"0,-2450,54.06,",
		"0,-350,12.06,",
		"0,-150,8.06,",
		"0,50,6.06,",
		"0,250,10.06,",
		"0,450,14.06,",
		"0,650,18.06,",
		"0,850,22.06,",
		"0,1050,26.06,",
		"0,1250,30.06,",
		"0,1450,34.06,",
		"0,1650,38.06,",
		"0,1850,42.06,",
		"0,2050,46.06,",
		"0,2250,50.06,",
		"0,2450,54.06,",
		"200,-650,19.072,",
		"200,-850,23.072,",
		"200,-1050,27.072,",
		"200,-1250,31.072,",
		"200,-1450,35.072,",
		"200,-1650,39.072,",
		"200,-1850,43.072,",
		"200,-2050,47.072,",
		"200,-2250,51.072,",
		"200,-2450,55.072,",
		"200,-350,13.072,",
		"200,-150,9.072,",
		"200,50,7.072,",
		"200,250,11.072,",
		"200,450,15.072,",
		"200,650,19.072,",
		"200,850,23.072,",
		"200,1050,27.072,",
		"200,1250,31.072,",
		"200,1450,35.072,",
		"200,1650,39.072,",
		"200,1850,43.072,",
		"200,2050,47.072,",
		"200,2250,51.072,",
		"200,2450,55.072,",
		"400,-650,20.084,",
		"400,-850,24.084,",
		"400,-1050,28.084,",
		"400,-1250,32.084,",
		"400,-1450,36.084,",
		"400,-1650,40.084,",
		"400,-1850,44.084,",
		"400,-2050,48.084,",
		"400,-2250,52.084,",
		"400,-2450,56.084,",
		"400,-350,14.084,",
		"400,-150,10.084,",
		"400,50,8.084,",
		"400,250,12.084,",
		"400,450,16.084,",
		"400,650,20.084,",
		"400,850,24.084,",
		"400,1050,28.084,",
		"400,1250,32.084,",
		"400,1450,36.084,",
		"400,1650,40.084,",
		"400,1850,44.084,",
		"400,2050,48.084,",
		"400,2250,52.084,",
		"400,2450,56.084,",
		"600,-650,21.096,",
		"600,-850,25.096,",
		"600,-1050,29.096,",
		"600,-1250,33.096,",
		"600,-1450,37.096,",
		"600,-1650,41.096,",
		"600,-1850,45.096,",
		"600,-2050,49.096,",
		"600,-2250,53.096,",
		"600,-350,15.096,",
		"600,-150,11.096,",
		"600,50,9.096,",
		"600,250,13.096,",
		"600,450,17.096,",
		"600,650,21.096,",
		"600,850,25.096,",
		"600,1050,29.096,",
		"600,1250,33.096,",
		"600,1450,37.096,",
		"600,1650,41.096,",
		"600,1850,45.096,",
		"600,2050,49.096,",
		"600,2250,53.096,",
		"600,2450,57.096,",
		"800,-650,22.108,",
		"800,-850,26.108,",
		"800,-1050,30.108,",
		"800,-1250,34.108,",
		"800,-1450,38.108,",
		"800,-1650,42.108,",
		"800,-1850,46.108,",
		"800,-2050,50.108,",
		"800,-2250,54.108,",
		"800,-2450,58.108,",
		"800,-350,16.108,",
		"800,-150,12.108,",
		"800,50,10.108,",
		"800,250,14.108,",
		"800,450,18.108,",
		"800,650,22.108,",
		"800,850,26.108,",
		"800,1050,30.108,",
		"800,1250,34.108,",
		"800,1450,38.108,",
		"800,1650,42.108,",
		"800,1850,46.108,",
		"800,2050,50.108,",
		"800,2250,54.108,",
		"800,2450,58.108,",
		"1000,-650,23.12,",
		"1000,-850,27.12,",
		"1000,-1050,31.12,",
		"1000,-1250,35.12,",
		"1000,-1450,39.12,",
		"1000,-1650,43.12,",
		"1000,-1850,47.12,",
		"1000,-2050,51.12,",
		"1000,-2250,55.12,",
		"1000,-2450,59.12,",
		"1000,-350,17.12,",
		"1000,-150,13.12,",
		"1000,50,11.12,",
		"1000,250,15.12,",
		"1000,450,19.12,",
		"1000,650,23.12,",
		"1000,850,27.12,",
		"1000,1050,31.12,",
		"1000,1250,35.12,",
		"1000,1450,39.12,",
		"1000,1650,43.12,",
		"1000,1850,47.12,",
		"1000,2050,51.12,",
		"1000,2250,55.12,",
		"1000,2450,59.12,",
		"1200,-650,24.132,",
		"1200,-850,28.132,",
		"1200,-1050,32.132,",
		"1200,-1250,36.132,",
		"1200,-1450,40.132,",
		"1200,-1650,44.132,",
		"1200,-1850,48.132,",
		"1200,-2050,52.132,",
		"1200,-2250,56.132,",
		"1200,-2450,60.132,",
		"1200,-350,18.132,",
		"1200,-150,14.132,",
		"1200,50,12.132,",
		"1200,250,16.132,",
		"1200,450,20.132,",
		"1200,650,24.132,",
		"1200,850,28.132,",
		"1200,1050,32.132,",
		"1200,1250,36.132,",
		"1200,1450,40.132,",
		"1200,1650,44.132,",
		"1200,1850,48.132,",
		"1200,2050,52.132,",
		"1200,2250,56.132,",
		"1200,2450,60.132,",
		"1400,-650,25.144,",
		"1400,-850,29.144,",
		"1400,-1050,33.144,",
		"1400,-1250,37.144,",
		"1400,-1450,41.144,",
		"1400,-1650,45.144,",
		"1400,-1850,49.144,",
		"1400,-2050,53.144,",
		"1400,-2250,57.144,",
		"1400,-2450,61.144,",
		"1400,-350,19.144,",
		"1400,-150,15.144,",
		"1400,50,13.144,",
		"1400,250,17.144,",
		"1400,450,21.144,",
		"1400,650,25.144,",
		"1400,850,29.144,",
		"1400,1050,33.144,",
		"1400,1250,37.144,",
		"1400,1450,41.144,",
		"1400,1650,45.144,",
		"1400,1850,49.144,",
		"1400,2050,53.144,",
		"1400,2250,57.144,",
		"1400,2450,61.144,",
		"1600,-650,26.156,",
		"1600,-850,30.156,",
		"1600,-1050,34.156,",
		"1600,-1250,38.156,",
		"1600,-1450,42.156,",
		"1600,-1650,46.156,",
		"1600,-1850,50.156,",
		"1600,-2050,54.156,",
		"1600,-2250,58.156,",
		"1600,-2450,62.156,",
		"1600,-350,20.156,",
		"1600,-150,16.156,",
		"1600,50,14.156,",
		"1600,250,18.156,",
		"1600,450,22.156,",
		"1600,650,26.156,",
		"1600,850,30.156,",
		"1600,1050,34.156,",
		"1600,1250,38.156,",
		"1600,1450,42.156,",
		"1600,1650,46.156,",
		"1600,1850,50.156,",
		"1600,2050,54.156,",
		"1600,2250,58.156,",
		"1600,2450,62.156,",
		"1800,-650,27.168,",
		"1800,-850,31.168,",
		"1800,-1050,35.168,",
		"1800,-1250,39.168,",
		"1800,-1450,43.168,",
		"1800,-1650,47.168,",
		"1800,-1850,51.168,",
		"1800,-2050,55.168,",
		"1800,-2250,59.168,",
		"1800,-2450,63.168,",
		"1800,-350,21.168,",
		"1800,-150,17.168,",
		"1800,50,15.168,",
		"1800,250,19.168,",
		"1800,450,23.168,",
		"1800,650,27.168,",
		"1800,850,31.168,",
		"1800,1050,35.168,",
		"1800,1250,39.168,",
		"1800,1450,43.168,",
		"1800,1650,47.168,",
		"1800,1850,51.168,",
		"1800,2050,55.168,",
		"1800,2250,59.168,",
		"1800,2450,63.168,",
		"2000,-650,28.18,",
		"2000,-850,32.18,",
		"2000,-1050,36.18,",
		"2000,-1250,40.18,",
		"2000,-1450,44.18,",
		"2000,-1650,48.18,",
		"2000,-1850,52.18,",
		"2000,-2050,56.18,",
		"2000,-2250,60.18,",
		"2000,-2450,64.18,",
		"2000,-350,22.18,",
		"2000,-150,18.18,",
		"2000,50,16.18,",
		"2000,250,20.18,",
		"2000,450,24.18,",
		"2000,650,28.18,",
		"2000,850,32.18,",
		"2000,1050,36.18,",
		"2000,1250,40.18,",
		"2000,1450,44.18,",
		"2000,1650,48.18,",
		"2000,1850,52.18,",
		"2000,2050,56.18,",
		"2000,2250,60.18,",
		"2000,2450,64.18,",
		"3000,-650,28.18,",
		"3000,-850,32.18,",
		"3000,-1050,36.18,",
		"3000,-1250,40.18,",
		"3000,-1450,44.18,",
		"3000,-1650,48.18,",
		"3000,-1850,52.18,",
		"3000,-2050,56.18,",
		"3000,-2250,60.18,",
		"3000,-2450,64.18,",
		"3000,-350,22.18,",
		"3000,-150,18.18,",
		"3000,50,16.18,",
		"3000,250,20.18,",
		"3000,450,24.18,",
		"3000,650,28.18,",
		"3000,850,32.18,",
		"3000,1050,36.18,",
		"3000,1250,40.18,",
		"3000,1450,44.18,",
		"3000,1650,48.18,",
		"3000,1850,52.18,",
		"3000,2050,56.18,",
		"3000,2250,60.18,",
		"3000,2450,64.18,",
	};

}
