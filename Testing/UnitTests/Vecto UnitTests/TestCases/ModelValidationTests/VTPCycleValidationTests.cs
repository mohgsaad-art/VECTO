using System.Globalization;
using Moq;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class VTPCycleValidationTests
{
    public static ThreadLocal<List<string>> LogList = new ThreadLocal<List<string>>();

    const string Header = "<t> [s],<v> [km/h],<n_eng> [rpm],<n_fan> [rpm],<tq_wh_left> [Nm],<tq_wh_right> [Nm],<n_wh_left> [rpm],<n_wh_right> [rpm],<fc_Diesel CI> [g/h],<gear>,CO,NMHC,NOx,PN,tq_eng,thc,CO2";


    [TestCase()]
    public void TestWheelSpeedRatioExceeds_Left()
    {
        SetupLogging();

        var wheelSpeed = 100;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				", wheelSpeed, wheelSpeed * DeclarationData.VTPMode.WheelSpeedDifferenceFactor * 1.1);

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>()
		});

        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);

        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Wheel-speed difference rel."));
    }

    [TestCase()]
    public void TestWheelSpeedRatioExceeds_Right()
    {
        SetupLogging();

        var wheelSpeed = 100;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				", wheelSpeed, wheelSpeed * DeclarationData.VTPMode.WheelSpeedDifferenceFactor * 1.1);

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>()
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);

        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Wheel-speed difference rel."));
    }

    [TestCase()]
    public void TestWheelSpeedDifferenceStandstillExceeds_Left()
    {
        SetupLogging();

        var wheelSpeed = 0.95 * DeclarationData.VTPMode.WheelSpeedZeroTolerance.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, 400, 200, 200, {1}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				", wheelSpeed.ToString(CultureInfo.InvariantCulture), (wheelSpeed + DeclarationData.VTPMode.MaxWheelSpeedDifferenceStandstill.AsRPM * 1.1).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>()
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Wheel-speed difference abs."));
    }

    [TestCase()]
    public void TestWheelSpeedDifferenceStandstillExceeds_Right()
    {
        SetupLogging();

        var wheelSpeed = 0.95 * DeclarationData.VTPMode.WheelSpeedZeroTolerance.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3 , 0 , 0 , 0 , 0 , 0 , 0 , 0
				", wheelSpeed.ToString(CultureInfo.InvariantCulture), (wheelSpeed + DeclarationData.VTPMode.MaxWheelSpeedDifferenceStandstill.AsRPM * 1.1).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>()
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);

        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Wheel-speed difference abs."));
    }


    [TestCase()]
    public void TestFanSpeedTooLow()
    {
        SetupLogging();

        var fanSpeed = 1.05 * DeclarationData.VTPMode.MinFanSpeed.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0, 0, 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0, 0, 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3	, 0, 0, 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0, 0, 0 , 0 , 0 , 0 , 0
				", fanSpeed.ToString(CultureInfo.InvariantCulture), (fanSpeed * 0.9).ToString(CultureInfo.InvariantCulture));

		
		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData() {
					ID = Constants.Auxiliaries.IDs.Fan,
					Technology = new List<string>() { "Crankshaft mounted - On/off clutch" }
				}
			}
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Fan speed (non-electric) exceeds range"));
    }

    [TestCase()]
    public void TestFanSpeedTooHigh()
    {
        SetupLogging();

        var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				", fanSpeed, 1.1 * fanSpeed);

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData() {
					ID = Constants.Auxiliaries.IDs.Fan,
					Technology = new List<string>() { "Crankshaft mounted - On/off clutch" }
				}
			}
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.VerifyInputData();

        Assert.AreEqual(1, LogList.Value.Count);
        Assert.IsTrue(LogList.Value[0].Contains("Fan speed (non-electric) exceeds range"));
    }


    [TestCase()]
    public void TestFanSpeedElectricLow()
    {
        SetupLogging();

        var fanSpeed = 1.05 * DeclarationData.VTPMode.MinFanSpeed.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				", fanSpeed.ToString(CultureInfo.InvariantCulture), (fanSpeed * 0.9).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData() {
					ID = Constants.Auxiliaries.IDs.Fan,
					Technology = new List<string>() { "Electrically driven - Electronically controlled" }
                }
			}
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.VerifyInputData();

        Assert.AreEqual(0, LogList.Value.Count);
    }

    [TestCase()]
    public void TestFanSpeedElectricHigh()
    {
        SetupLogging();

        var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

        var cycleEntries = string.Format(
            @"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0
				", fanSpeed, 1.1 * fanSpeed);

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData() {
					ID = Constants.Auxiliaries.IDs.Fan,
					Technology = new List<string>() { "Electrically driven - Electronically controlled" }
				}
			}
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.VerifyInputData();

        Assert.AreEqual(0, LogList.Value.Count);

    }

    [TestCase(), NUnit.Framework.Ignore("FC-Checks disabled (dual fuel)")]
    public void TestFuelConsumptionTooLow()
    {
        SetupLogging();

        // Approach: calculate min. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
        //           construct cycle with slightly decreasing FC so that FC falls below threshold 

        var fcLimit = (DeclarationData.VTPMode.LowerFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

        var cycleEntries = "";
        for (var i = 0; i < 2000; i++)
            cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0 \n", (i / 2.0).ToString(CultureInfo.InvariantCulture), ((fcLimit * 1.01 * (1 - i / 100000.0)).ConvertToGrammPerHour().Value).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>(),
			TorqueDriftLeftWheel = 0.SI<NewtonMeter>(),
			TorqueDriftRightWheel = 0.SI<NewtonMeter>(),
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.PrepareCycleData();
        vtpCycle.VerifyInputData();

        Assert.Greater(LogList.Value.Count, 1);
        Assert.IsTrue(LogList.Value.Any(x => x.StartsWith("Fuel consumption for the previous 10 [min] below threshold")));

    }

    [TestCase()]
    public void TestFuelConsumptionLowOK()
    {
        SetupLogging();

        // Approach: calculate min. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
        //           construct cycle with constant FC slightly above min. FC 

        var fcLimit = (DeclarationData.VTPMode.LowerFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

        var cycleEntries = "";
        for (var i = 0; i < 2000; i++)
            cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0 \n", (i / 2.0).ToString(CultureInfo.InvariantCulture), ((fcLimit * 1.0001).ConvertToGrammPerHour().Value).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>(),
			TorqueDriftLeftWheel = 0.SI<NewtonMeter>(),
			TorqueDriftRightWheel = 0.SI<NewtonMeter>(),
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.PrepareCycleData();
        vtpCycle.VerifyInputData();

        Assert.AreEqual(0, LogList.Value.Count);

    }

    [TestCase(), NUnit.Framework.Ignore("FC-Checks disabled (dual fuel")]
    public void TestFuelConsumptionTooHigh()
    {
        SetupLogging();

        // Approach: calculate max. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
        //           construct cycle with slightly decreasing FC so that FC falls below threshold 

        var fcLimit = (DeclarationData.VTPMode.UpperFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

        var cycleEntries = "";
        for (var i = 0; i < 2000; i++)
            cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3	, 0 , 0 , 0 , 0 , 0 , 0 , 0 \n", (i / 2.0).ToString(CultureInfo.InvariantCulture), ((fcLimit * 0.99 * (1 + i / 100000.0)).ConvertToGrammPerHour().Value).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>(),
			TorqueDriftLeftWheel = 0.SI<NewtonMeter>(),
			TorqueDriftRightWheel = 0.SI<NewtonMeter>(),
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.PrepareCycleData();
        vtpCycle.VerifyInputData();

        Assert.Greater(LogList.Value.Count, 1);
        Assert.IsTrue(LogList.Value.Any(x => x.StartsWith("Fuel consumption for the previous 10 [min] above threshold")));
    }

    [TestCase()]
    public void TestFuelConsumptionHighOK()
    {
        SetupLogging();

        // Approach: calculate max. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
        //           construct cycle with constant FC slightly below max FC 

        var fcLimit = (DeclarationData.VTPMode.UpperFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

        var cycleEntries = "";
        for (var i = 0; i < 2000; i++)
            cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3 	, 0 , 0 , 0 , 0 , 0 , 0 , 0 \n", (i / 2.0).ToString(CultureInfo.InvariantCulture), ((fcLimit * 0.9999).ConvertToGrammPerHour().Value).ToString(CultureInfo.InvariantCulture));

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(new VectoRunData() {
			Aux = new List<VectoRunData.AuxData>(),
			TorqueDriftLeftWheel = 0.SI<NewtonMeter>(),
			TorqueDriftRightWheel = 0.SI<NewtonMeter>(),
		});
        var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
        var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
        var vtpCycle = new VTPCycle(container, cycleData);
        vtpCycle.PrepareCycleData();
        vtpCycle.VerifyInputData();

        Assert.AreEqual(0, LogList.Value.Count);

    }

    private void SetupLogging()
    {
        LogList.Value = new List<string>();
        LogList.Value.Clear();
        var target = new MethodCallTarget {
            ClassName = typeof(VTPCycleValidationTests).AssemblyQualifiedName,
            MethodName = "LogMethod"
        };
        target.Parameters.Add(new MethodCallParameter("${level}"));
        target.Parameters.Add(new MethodCallParameter("${message}"));
        SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Error);
    }


    // ReSharper disable once UnusedMember.Global -- used by logging framework, see SetupLogging method
    public static void LogMethod(string level, string message)
    {
        LogList.Value.Add(message);
    }
}