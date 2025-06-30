using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

[TestFixture]
public class ATShiftStrategyOptimizedTests
{
	[TestCase(0, 100, 1)]
	[TestCase(0, 200, 1)]
	[TestCase(5, 100, 1)]
	[TestCase(5, 300, 1)]
	[TestCase(5, 600, 1)]
	[TestCase(15, 100, 3)]
	[TestCase(15, 300, 3)]
	[TestCase(15, 600, 3)]
	[TestCase(40, 100, 6)]
	[TestCase(40, 300, 6)]
	[TestCase(40, 600, 6)]
	[TestCase(70, 100, 6)]
	[TestCase(70, 300, 6)]
	[TestCase(70, 600, 6)]
	public void TestATGearInitialize(double vehicleSpeed, double torque, int expectedGear)
	{
		var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
		var vehicleContainer = GetMockVehicleContainer(
			speedKmh: vehicleSpeed,
			driverBehavior: DrivingBehavior.Accelerating,
			gearRatios: gearRatios,
			inputData: out _,
			runData: out var runData,
			simplePt: out _);

		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx, out _);
		var angularVelocity = GetAngularVelocityBySpeed(vehicleSpeed, runData);
		var response = shiftStrategy.InitGear(
			0.SI<Second>(),
			1.SI<Second>(),
			torque.SI<NewtonMeter>(),
			angularVelocity);

		Assert.AreEqual(expectedGear, response.Gear);
	}
	
	

	
	[TestCase(1, 1, 1000, 1500, 0, Description = "Engage 0-> 1C")]
	public void Gearbox_Engage(int gear, int newGear, double tqNm, double nRPM, double speedKmh)
	{
		var gearRatios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
		
		var vehicleContainer = GetMockVehicleContainer(speedKmh, DrivingBehavior.Accelerating, gearRatios, 
			inputData: out _,
			runData: out var runData, simplePt: out _);
		
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData: runData);
		
		
		
		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		
		
		var expectedN = nRPM.RPMtoRad();
		angularVelocity = expectedN / gearRatios[gear];
		
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);
		
		var expectedT = tqNm.SI<NewtonMeter>();
		var torque = expectedT * gearRatios[gear];
		
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
		
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
		
		var shiftExpected = gear != newGear; //Different Gear
		var disengaged = gbx.Object.Disengaged;
		shiftExpected = shiftRequired || disengaged; //Gearbox was disengaged
		
		Assert.AreEqual(shiftExpected, shiftRequired);
	}

	[TestCase(2, 1, -1000, 1500, 4, DrivingBehavior.Braking, Description = "_ -> 0: disengage before halting")]
	public void Gearbox_Disengange(int gear, int newGear, double tqNm, double nRPM, double speedKmh,
		DrivingBehavior driverBehavior)
	{
		// Assert.Ignore("Work in Progress");
		var gearRatios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
		
		var vehicleContainer = GetMockVehicleContainer(speedKmh, driverBehavior, gearRatios, 
			inputData: out _,
			runData: out var runData, simplePt: out _);
		
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);
		
		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);
		
		var expectedN = nRPM.RPMtoRad();
		angularVelocity = expectedN / gearRatios[gear];
		
		//Called in gbx initialize
		//var gearShiftPosition = shiftStrategy.InitGear(absTime, Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
		//    angularVelocity);
		var engagedPosition = shiftStrategy.Engage(absTime, dt, null, null);
		Assert.IsTrue(engagedPosition.Engaged);
		
		// gbx.CurrentState = new ATGearboxState()
		// {
		// 	Disengaged = gbx.Disengaged,
		// 	Gear = gbx.Gear,
		// };
		
		var expectedT = tqNm.SI<NewtonMeter>();
		var torque = expectedT * gearRatios[gear];
		
		
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
		
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
		
		var shiftExpected = gear != newGear; //Different Gear
		var disengaged = false;
		shiftExpected = shiftRequired || disengaged; //Gearbox was disengaged
		
		Assert.AreEqual(shiftExpected, shiftRequired);
	}

	[TestCase(2, true, 3, true, 100, 1800, 13, Description = "Upshift-TCLocked", TestName="Upshift-TCLocked")]
	[TestCase(1, false, 1, true, 100, 1000, 1, Description = "Upshift-TC", TestName = "Upshift-TC")]
    [TestCase(2, true, 3, true, 100, 800, 13, Description = "Upshift-TCLocked", TestName = "EarlyUpshift")]
    [TestCase(1, false, 1, true, 100, 500, 1, Description = "EarlyUpshift-TC", TestName="EarlyUpshift-TC")]
    public void Gearbox_Upshift(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM, double speedKmh)
	{
		// Assert.Ignore("Work in Progress");
        var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
  
        var vehicleContainer = GetMockVehicleContainer(
			speedKmh, 
			DrivingBehavior.Accelerating, 
			gearRatios, 
			out var inputData, out var runData, out var simplePt);
  
  
		
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);

		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		
		var gbxObj = gbx.Object;
		// var gbxResponse = gbx.Object.Initialize(0.SI<NewtonMeter>(), angularVelocity);

		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		
		
		Assert.AreEqual((uint)gear, initGear.Gear);
		Assert.That(initGear.TorqueConverterLocked, Is.EqualTo(tcLocked));

		SetCurrentGear(gbx, initGear);

  
        // gbx.CurrentState = new ATGearboxState()
        // {
        //     Disengaged = gbx.Disengaged,
        //     Gear = gbx.Gear,
        // };
  
  
		var inAngularVelocity = nRPM.RPMtoRad();
		var outAngularVelocity = inAngularVelocity / gearRatios[gear];
  
        var inTorque = tqNm.SI<NewtonMeter>();
        var outTortque = inTorque * gearRatios[gear];
  
		var response = new ResponseSuccess(this);
		response.Engine.DynamicFullLoadTorque = 50.SI<NewtonMeter>();
		response.Engine.EngineSpeed = inAngularVelocity;
		response.Engine.TorqueOutDemand = inTorque;
  
  
		runData.GearshiftParameters.RatingFactorCurrentGear = 1.1;
  
  
        var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, outTortque, outAngularVelocity, inTorque, inAngularVelocity, initGear, -double.MaxValue.SI<Second>(), response);
  
  
		Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
		Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));
  
        var shiftExpected = gear != newGear; //Different Gear
		
        shiftExpected = shiftRequired || gbx.Object.Disengaged; //Gearbox was disengaged
  
        Assert.AreEqual(shiftExpected, shiftRequired);
    }

	private void SetCurrentGear(Mock<IAPTGearbox> gbx, GearshiftPosition initGear)
	{
		gbx.Setup(g => g.Gear).Returns(initGear);
		gbx.SetupGet(g => g.TorqueConverterLocked).Returns(initGear.TorqueConverterLocked ?? false);
	}

	[TestCase(2, true, 3, true, 100, 1800, 13, Description = "Upshift-TCLocked")]
	[TestCase(1, false, 1, true, 100, 1000, 1, Description = "Upshift-TC")]
	public void Gearbox_EarlyUpshift(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM, double speedKmh)
	{
		var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
		
		var vehicleContainer = GetMockVehicleContainer(
			speedKmh,
			DrivingBehavior.Accelerating,
			gearRatios,
			out var inputData, out var runData, out _);
		
		
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);
				
		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);
		
		Assert.AreEqual((uint)gear, initGear.Gear);
		Assert.That(initGear.TorqueConverterLocked, Is.EqualTo(tcLocked));
		

		
		
		var inAngularVelocity = nRPM.RPMtoRad();
		var outAngularVelocity = inAngularVelocity / gearRatios[gear];
		
		var inTorque = tqNm.SI<NewtonMeter>();
		var outTortque = inTorque * gearRatios[gear];
		
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, outTortque, outAngularVelocity, inTorque, inAngularVelocity, initGear, -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
		
		
		Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
		Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));
		
		var shiftExpected = gear != newGear; //Different Gear
		shiftExpected = shiftRequired || gbx.Object.Disengaged; //Gearbox was disengaged
		
		Assert.AreEqual(shiftExpected, shiftRequired);
	}

//
    [TestCase(1, false, 2, false, 100, 1000, 1, Description = "Upshift-TC")]
    public void Gearbox_Upshift_TC_TC(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM, double speedKmh)
    {
        var gearRatios = new[] { 3.4, 1.3, 1.1, 1.0, 0.7, 0.62 };
  
        var vehicleContainer = GetMockVehicleContainer(
            speedKmh,
            DrivingBehavior.Accelerating,
            gearRatios,
            out var inputData, out var runData, out _);
  
  
        var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
        var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);


		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
				
		Assert.That(runData.GearboxData.GearList.First(p => p.Gear == 2).TorqueConverterLocked, Is.False, "Expected 2nd gear with TC");
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);
		

		
        var inAngularVelocity = nRPM.RPMtoRad();
        var outAngularVelocity = inAngularVelocity / gearRatios[gear];
  
        var inTorque = tqNm.SI<NewtonMeter>();
        var outTortque = inTorque * gearRatios[gear];
  
		var response = new ResponseSuccess(this);
		response.Engine.EngineSpeed = inAngularVelocity;
		response.Engine.TorqueOutDemand = inTorque;
  
  
        var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, outTortque, outAngularVelocity, inTorque, inAngularVelocity, initGear, -double.MaxValue.SI<Second>(), response);
  
  
        Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
        Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));
  
        var shiftExpected = gear != newGear; //Different Gear
        shiftExpected = shiftRequired || gbx.Object.Disengaged; //Gearbox was disengaged
  
        Assert.AreEqual(shiftExpected, shiftRequired);
    }
//
//
//
//
    [TestCase(3, true, 2, true, 900, 600, 15, Description = "Downshift", TestName="Gearbox_DownShift_1")]
    public void Gearbox_Downshift(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM, double speedKmh)
    {
		// Assert.Ignore("Work in Progress");
		
        var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
  
        var vehicleContainer = GetMockVehicleContainer(
            speedKmh,
            DrivingBehavior.Accelerating,
            gearRatios,
            out var inputData,
			out var runData,
			out var simplePt);
  
		vehicleContainer.Setup(v => v.DriverInfo.DrivingAction).Returns(DrivingAction.Accelerate);
  
        var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
        var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);
  
 
		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		
        Assert.AreEqual((uint)gear, initGear.Gear);
        Assert.That(initGear.TorqueConverterLocked, Is.EqualTo(tcLocked));
  
 

		
        var inAngularVelocity = nRPM.RPMtoRad();
        var outAngularVelocity = inAngularVelocity / gearRatios[gear];
  
        var inTorque = tqNm.SI<NewtonMeter>();
        var outTortque = inTorque * gearRatios[gear];
  
		var response = new ResponseSuccess(this);
		response.Engine.EngineSpeed = inAngularVelocity;
		response.Engine.TorqueOutDemand = inTorque;
  
  
		var mockPort = new Mock<ITnOutPort>();
		
		simplePt.Setup(s => s.GearboxOutPort).Returns(mockPort.Object);
		mockPort.Setup(p => p.Request(
			It.IsAny<Second>(),
			It.IsAny<Second>(),
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(),
			true
		)).Returns((Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun) => {
			var response = new ResponseDryRun(this);
			response.Engine.PowerRequest = outTorque * outAngularVelocity;
			return response;
		});
  //
  //
  //
  //
		// var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, outTortque, outAngularVelocity, inTorque, inAngularVelocity, gbx.Gear, -double.MaxValue.SI<Second>(), response);
  //
  //
  //       Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
  //       Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));
  //
  //       var shiftExpected = gear != newGear; //Different Gear
		// var disengaged = false; //gbx.Disengaged;
  //       shiftExpected = shiftRequired || disengaged; //Gearbox was disengaged
  //
  //       Assert.AreEqual(shiftExpected, shiftRequired);
    }
//
	[TestCase(3, true, 2, true, 200, 700, 15, Description = "Downshift_3", TestName = "Gearbox_DownShift_3")]
	public void Gearbox_Downshift_2(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM,
		double speedKmh)
	{
		var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };

		var vehicleContainer = GetMockVehicleContainer(
			speedKmh,
			DrivingBehavior.Accelerating,
			gearRatios,
			out var inputData,
			out var runData,
			out var simplePt);

		vehicleContainer.Setup(v => v.DriverInfo.DrivingAction).Returns(DrivingAction.Accelerate);

		// Condition from ATShiftStrategy
		//if (DataBus.VehicleInfo.VehicleSpeed < DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed - 10.KMPHtoMeterPerSecond() &&
		//	DataBus.DriverInfo.DriverAcceleration < 0.SI<MeterPerSquareSecond>())
		vehicleContainer.Setup(v => v.DrivingCycleInfo.CycleData).Returns(new CycleData() {
			LeftSample = new DrivingCycleData.DrivingCycleEntry() {
				VehicleTargetSpeed = 30.KMPHtoMeterPerSecond()
			}
		});

		vehicleContainer.Setup(v => v.DriverInfo.DriverAcceleration).Returns(-0.1.SI<MeterPerSquareSecond>());


		runData.EngineData.Inertia = 1.SI<KilogramSquareMeter>();
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx, out var gbxNextComponent);
		
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);
		


		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);

		
        Assert.AreEqual((uint)gear, initGear.Gear);
        Assert.That(initGear.TorqueConverterLocked, Is.EqualTo(tcLocked));

		
		var dryRunResponse = new ResponseDryRun(this);
		dryRunResponse.Engine.EngineSpeed =
			vehicleContainer.Object.EngineInfo.EngineN95hSpeed - 1.SI<PerSecond>();
		dryRunResponse.DeltaFullLoad = 40_000.SI<Watt>();
		
		// gbx.Setup(g => g)
		var testPt =
			vehicleContainer.Object.SimplePowertrainBuilder.CreateTestPowertrain(vehicleContainer.Object, false);
		
		var mockGb = Mock.Get(testPt.Gearbox);
		mockGb.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), true)).Returns(dryRunResponse);
		
		
        var inAngularVelocity = nRPM.RPMtoRad();
        var outAngularVelocity = inAngularVelocity / gearRatios[gear];

        var inTorque = tqNm.SI<NewtonMeter>();
        var outTortque = inTorque * gearRatios[gear];

        var shiftRequired = shiftStrategy.ShiftRequired(absTime, 
			dt, 
			outTortque, outAngularVelocity, inTorque, inAngularVelocity, initGear, -double.MaxValue.SI<Second>(), new ResponseSuccess(this));


        Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
        Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));

        var shiftExpected = gear != newGear; //Different Gear
        shiftExpected = shiftRequired || gbx.Object.Disengaged; //Gearbox was disengaged

        Assert.AreEqual(shiftExpected, shiftRequired);
    }

    // [TestCase(3, true, 2, true, 1700, 700, 15, Description = "Downshift", TestName = "Gearbox_Early_DownShift_1")]
	[TestCase(6, true, 5, true, -100, 700, 45, Description = "Downshift", TestName = "Gearbox_Early_DownShift_1")]
    public void Gearbox_Early_Downshift(int gear, bool tcLocked, int newGear, bool newTcLocked, double tqNm, double nRPM, double speedKmh)
	{
		var gearRatios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
		
		var vehicleContainer = GetMockVehicleContainer(
		    speedKmh,
		    DrivingBehavior.Accelerating,
		    gearRatios,
		    out var inputData, out var runData, out var simplePt);
		
		vehicleContainer.Setup(v => v.DriverInfo.DrivingAction).Returns(DrivingAction.Accelerate);
		
		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		var angularVelocity = GetAngularVelocityBySpeed(speedKmh, runData);
		
				
		
		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();
		var initGear = shiftStrategy.InitGear(absTime, dt, 0.SI<NewtonMeter>(), angularVelocity);
		SetCurrentGear(gbx, initGear);
		
		Assert.AreEqual((uint)gear, initGear.Gear);
		Assert.That(initGear.TorqueConverterLocked, Is.EqualTo(tcLocked));

		var gearIdx = gear - 1;
		var inAngularVelocity = nRPM.RPMtoRad();
		var outAngularVelocity = inAngularVelocity / gearRatios[gearIdx];
		
		var inTorque = tqNm.SI<NewtonMeter>();
		var outTortque = inTorque * gearRatios[gearIdx];
		
		var response = new ResponseSuccess(this);
		response.Engine.EngineSpeed = inAngularVelocity;
		response.Engine.TorqueOutDemand = inTorque;
		
		
		var mockPort = new Mock<ITnOutPort>();
		
		// simplePt.Setup(s => s.GearboxOutPort).Returns(mockPort.Object);
		// mockPort.Setup(p => p.Request(
		//     It.IsAny<Second>(),
		//     It.IsAny<Second>(),
		//     It.IsAny<NewtonMeter>(),
		//     It.IsAny<PerSecond>(),
		//     true
		// )).Returns((Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun) => {
		//     var response = new ResponseDryRun(this);
		//     response.Engine.PowerRequest = outTorque * outAngularVelocity;
		//     return response;
		// });
		
		
		runData.GearshiftParameters.RatingFactorCurrentGear = 2;
		
		var shiftRequired = shiftStrategy.ShiftRequired(absTime,
			dt,
		outTortque, 
		outAngularVelocity,
		inTorque, 
		inAngularVelocity,
		initGear, -double.MaxValue.SI<Second>(), response);
		
		
		Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(newGear));
		Assert.That(shiftStrategy.NextGear.TorqueConverterLocked ?? true, Is.EqualTo(newTcLocked));
		
		var shiftExpected = gear != newGear; //Different Gear
		shiftRequired = shiftRequired || gbx.Object.Disengaged; //Gearbox was disengaged
		
		Assert.AreEqual(shiftExpected, shiftRequired);
	}
	
	private Mock<IVehicleContainer> GetMockVehicleContainer(double speedKmh, DrivingBehavior driverBehavior,
		double[] gearRatios,
		out Mock<IVehicleDeclarationInputData> inputData, out VectoRunData runData,
		out Mock<ISimpleVehicleContainer> simplePt)
	{
		var vehicleContainer = new Mock<IVehicleContainer>();

		inputData = GetMockInputData(gearRatios);
		runData = GetDummyVectoRunData(inputData.Object);
		
		vehicleContainer.Setup(c => c.RunData).Returns(runData);

		
		//Testpowertrain
		var testPowertrain = GetMockTestPowertrain(
			runData,
			out simplePt);

		vehicleContainer.Setup(c => c.SimplePowertrainBuilder.CreateTestPowertrain(It.IsAny<IVehicleContainer>(),
			It.IsAny<bool>())).Returns(testPowertrain.Object);

		//VehicleInfo
		vehicleContainer.Setup(c => c.VehicleInfo)
			.Returns(GetVehicleInfo(speedKmh.KMPHtoMeterPerSecond()).Object);

		//EngineInfo
		vehicleContainer.Setup(c => c.EngineInfo).Returns(GetEngineInfo(vehicleContainer, runData));
		
		//AxleGearInfo
		var axleGearInfo = new Mock<IAxlegearInfo>();
		vehicleContainer.Setup(c => c.AxlegearInfo).Returns(axleGearInfo.Object);
		axleGearInfo.Setup(a => a.AxlegearLoss()).Returns(0.SI<Watt>());

		//WheelsInfo
		var wi = new Mock<IWheelsInfo>();
		vehicleContainer.Setup(c => c.WheelsInfo).Returns(wi.Object);
		wi.Setup(w => w.ReducedMassWheels).Returns(0.SI<Kilogram>());

		//Cycle Info
		var cycleInfo = new Mock<IDrivingCycleInfo>();
		vehicleContainer.Setup(c => c.DrivingCycleInfo).Returns(cycleInfo.Object);
		cycleInfo.Setup(c => c.CycleData).Returns(
			GetCycleData());
		cycleInfo.Setup(c => c.RoadGradient).Returns(0.SI<Radian>());


		cycleInfo.Setup(c => c.CycleLookAhead(It.IsAny<Meter>())).Returns(new DrivingCycleData.DrivingCycleEntry() {
			Altitude = 0.SI<Meter>()
		});
		cycleInfo.Setup(c => c.Altitude).Returns(0.SI<Meter>());

		//DriverINfo
		vehicleContainer.Setup(v => v.DriverInfo.DriverBehavior).Returns(driverBehavior);
		var acc = 0.SI<MeterPerSquareSecond>();
		switch (driverBehavior) {
			case DrivingBehavior.Accelerating:
				acc = 1.SI<MeterPerSquareSecond>();
				break;
			case DrivingBehavior.Braking:
				acc = -1.SI<MeterPerSquareSecond>();
				break;
			case DrivingBehavior.Halted:
				acc = 0.SI<MeterPerSquareSecond>();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}





		vehicleContainer.Setup(v => v.DriverInfo.DriverAcceleration).Returns(acc);
		return vehicleContainer;
	}

	private IEngineInfo GetEngineInfo(Mock<IVehicleContainer> vehicleContainer, VectoRunData runData)
	{
		        //EngineInfo
        var engineInfo = new Mock<IEngineInfo>();
		vehicleContainer.Setup(c => c.EngineInfo).Returns(engineInfo.Object);
		engineInfo.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
		engineInfo.Setup(e => e.EngineRatedSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.RatedSpeed);

		engineInfo.Setup(e => e.EngineSpeed).Returns(1400.RPMtoRad());
		engineInfo.Setup(e => e.EngineN95hSpeed).Returns
			(runData.EngineData.FullLoadCurves.First().Value.N95hSpeed);
		engineInfo.Setup(e => e.EngineN80hSpeed).Returns(
			runData.EngineData.FullLoadCurves.First().Value.N80hSpeed);
		engineInfo.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>())).Returns((PerSecond n) =>
			runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));
		return engineInfo.Object;
	}

	private CycleData GetCycleData()
	{
		return new CycleData() {
			LeftSample = new DrivingCycleData.DrivingCycleEntry() {
				PTOActive = PTOActivity.Inactive,
				VehicleTargetSpeed = 10.KMPHtoMeterPerSecond()
			}
		};
	}

	private Mock<IVehicleInfo> GetVehicleInfo(MeterPerSecond speed)
	{
		var vehicleInfo = new Mock<IVehicleInfo>();
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(speed);
		vehicleInfo.Setup(v => v.AirDragResistance(It.IsAny<MeterPerSecond>(), It.IsAny<MeterPerSecond>()))
			.Returns(new AirDragLossResult(0.SI<Watt>(), 0.SI<SquareMeter>(), 0.SI<MeterPerSecond>()));
		vehicleInfo.Setup(v => v.RollingResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vehicleInfo.Setup(v => v.SlopeResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vehicleInfo.Setup(v => v.TotalMass).Returns(12000.SI<Kilogram>());
		return vehicleInfo;
	}

	private Mock<ITestPowertrain> GetMockTestPowertrain(VectoRunData runData,
		out Mock<ISimpleVehicleContainer> simpleContainer)
	{
		var testPt = new Mock<ITestPowertrain>();
		simpleContainer = GetSimplePowertrain(runData,
			out var testGearbox);



		testPt.Setup(t => t.Container).Returns(simpleContainer.Object);
		testPt.Setup(t => t.Gearbox).Returns(testGearbox.Object);

		//Vehicle
		var vehicle = new Mock<ITestPowertrainVehicle>();
		vehicle.Setup(v => v.Initialize(
			It.IsAny<MeterPerSecond>(),
			It.IsAny<Radian>())).Returns(new ResponseSuccess(this));
		testPt.Setup(t => t.Vehicle).Returns(vehicle.Object);


		return testPt;
	}

	private Mock<IVehicleDeclarationInputData> GetMockInputData(double[]? gearRatios)
	{
		var input = new Mock<IVehicleDeclarationInputData>();
		var components = new Mock<IVehicleComponentsDeclaration>();
		input.Setup(i => i.Components).Returns(components.Object);
		var gbx = new Mock<IGearboxDeclarationInputData>();
		var tc = new Mock<ITorqueConverterDeclarationInputData>();

		components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
		components.Setup(c => c.TorqueConverterInputData).Returns(tc.Object);
		gearRatios = gearRatios ?? new double[] { };
		var header = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
		var efficiency = 0.98;
		var data = new List<string>();

		foreach (var speed in new[] { 0, 10000 }) {
			foreach (var tq in new[] { 1e5, -1e5, 0 }) {
				data.Add(FormattableString.Invariant($"{speed:f2}, {tq:f2}, {(1 - efficiency) * Math.Abs(tq)}"));
			}
		}

		var lossmap = InputDataHelper.InputDataAsTableData(header, data.ToArray());
		//lossmap.Columns
		var gears = gearRatios.Select((x, idx) => {
			var gear = new Mock<ITransmissionInputData>();
			gear.Setup(g => g.Ratio).Returns(x);
			gear.Setup(g => g.Gear).Returns(idx + 1);
			//gear.Setup(g => g.Efficiency).Returns(0.98);
			gear.Setup(g => g.LossMap).Returns(lossmap);
			gear.Setup(g => g.MaxInputSpeed).Returns(2000.RPMtoRad());
			return gear.Object;
		}).ToList();
		gbx.Setup(g => g.Type).Returns(GearboxType.ATSerial);
		gbx.Setup(g => g.Gears).Returns(gears);

		var tcData = InputDataHelper.InputDataAsTableData(TcHeader, TcData);
		tc.Setup(t => t.TCData).Returns(tcData);
		return input;
	}

	private static VectoRunData GetDummyVectoRunData(IVehicleDeclarationInputData inputData)
	{
		var nrOfGears = inputData.Components.GearboxInputData.Gears.Count;
		var fldData = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fld = FullLoadCurveReader.Create(fldData);

		// create gearbox data
		var tcDataAdapter = new TorqueConverterDataAdapter();

		// fuel data
		var fuelData = new CombustionEngineFuelData() {
			ConsumptionMap = FuelConsumptionMapReader.Create(
				InputDataHelper.InputDataAsTableData(
					"engine speed [rpm] ,torque [Nm] ,fuel consumption [g/h] ,whr power electrical [W]",
					"500,-131,0,0",
					"500,95.6,1814.959,0",
					"500,573.6,9771.095,0",
					"2453,-209.12,0,0",
					"2453,764.8,39097.94,0"
				)),
			FuelData = DeclarationData.FuelData.Lookup(FuelType.DieselCI),
		};

		var runData = new VectoRunData() {
			VehicleData = new VehicleData() {
				DynamicTyreRadius = 0.465.SI<Meter>(),
				GrossVehicleMass = 12_000.SI<Kilogram>(),
				CurbMass = 10_000.SI<Kilogram>(),
			},
			EngineData = new CombustionEngineData() {
				Inertia = 0.SI<KilogramSquareMeter>(),
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(),
				IdleSpeed = 600.RPMtoRad(),
				RatedSpeedDeclared = 2000.RPMtoRad(),
				Fuels = new List<CombustionEngineFuelData>() {
					fuelData,
				}
			},
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased
			}
		};
		for (uint i = 0; i <= nrOfGears; i++) {
			runData.EngineData.FullLoadCurves[i] = fld;
		}

		var gbxDataAdapter = new GearboxDataAdapter(tcDataAdapter);
		var gbxTypes = new[] {
			GearboxType.ATSerial, GearboxType.ATPowerSplit
		};

		var gearboxData = gbxDataAdapter.CreateGearboxData(inputData, runData,
			new ATShiftStrategyOptimizedPolygonCalculator(), new GearboxType[] {
				GearboxType.ATSerial,
				GearboxType.ATPowerSplit
			});

		var gearShiftParams =
			gbxDataAdapter.CreateGearshiftData(1.0, runData.EngineData.IdleSpeed, GearboxType.ATSerial, nrOfGears);

		runData.GearboxData = gearboxData;
		runData.GearshiftParameters = gearShiftParams;
		return runData;
	}

	private Mock<ISimpleVehicleContainer> GetSimplePowertrain(VectoRunData runData,
		out Mock<ITestPowertrainTransmission> testGearbox)
	{
		var simplePt = new Mock<ISimpleVehicleContainer>();

		testGearbox = GetMockTestGearbox(runData.GearboxData.Gears);


		simplePt.Setup(s => s.GearboxInfo).Returns(testGearbox.Object);
		simplePt.Setup(s => s.GearboxOutPort).Returns(testGearbox.Object);




		//VehiclePort
		return simplePt;
	}

	private Mock<ITestPowertrainTransmission> GetMockTestGearbox(Dictionary<uint, GearData> ratios)
	{
		Mock<IAPTGearbox> amtGearbox = new Mock<IAPTGearbox>();
		amtGearbox.Name = "APT_TestGearbox";
		TestContext.WriteLine(amtGearbox.Name);
		Mock<ITestPowertrainTransmission> gbx = amtGearbox.As<ITestPowertrainTransmission>();



		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());

		GearshiftPosition gear = null;
		gbx.SetupGet(g => g.Gear).Returns(() => { return gear; });
		gbx.SetupSet(g => g.SetGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => { gear = p; });


		GearshiftPosition nextGear = null;
		gbx.SetupGet(g => g.NextGear).Returns(() => nextGear);
		gbx.SetupSet(g => g.SetNextGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => nextGear = p);

		gbx.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
			return new ResponseSuccess(this) {
				Engine = {
					EngineSpeed = rpm,
					PowerRequest = tq * rpm,
				},
			};
		});
		gbx.Setup(p => p.Request(
			It.IsAny<Second>(),
			It.IsAny<Second>(),
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(),
			true)).Returns((
			Second absTime,
			Second dt,
			NewtonMeter t,
			PerSecond n,
			bool dryRun) => {

			var gear = gbx.Object.Gear.Gear;
			var ratio = (gbx.Object.Gear.TorqueConverterLocked ?? false)
				? ratios[gear].Ratio
				: ratios[gear].TorqueConverterRatio;

				
			
			// var ratio =
			// 	gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
			return dryRun
				? new ResponseDryRun(this) {
					Engine = {
						PowerRequest = n * t, EngineSpeed = n * ratio,
						DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						TotalTorqueDemand = t,
						TorqueOutDemand = t,
					},
					Clutch = { PowerRequest = n * t },
					DeltaFullLoad = n * t / 2 * (-1)
				}
				: new ResponseSuccess(this) {
					Engine = {
						PowerRequest = n * t,
						EngineSpeed = n * ratio

					},
					Clutch = { PowerRequest = n * t }
				};
		});
		return gbx;
	}

	private static PerSecond GetAngularVelocityBySpeed(double speedKmh, VectoRunData runData)
	{
		// r_dyn = 0.465m, i_axle = 6.2
		var angularVelocity =
			speedKmh.KMPHtoMeterPerSecond()
			/ runData.VehicleData.DynamicTyreRadius * 6.2;
		return angularVelocity;
	}






	public const string TcHeader = "Speed Ratio, Torque Ratio,MP1000";

	public static readonly string[] TcData = new[] {
		"0.0,1.80,377.80",
		"0.1,1.71,365.21",
		"0.2,1.61,352.62",
		"0.3,1.52,340.02",
		"0.4,1.42,327.43",
		"0.5,1.33,314.84",
		"0.6,1.23,302.24",
		"0.7,1.14,264.46",
		"0.8,1.04,226.68",
		"0.9,1.02,188.90",
		"1.0,1.0,0.00",
		"1.100,0.999,-40.34",
		"1.222,0.998,-80.34",
		"1.375,0.997,-136.11",
		"1.571,0.996,-216.52",
		"1.833,0.995,-335.19",
		"2.200,0.994,-528.77",
		"2.750,0.993,-883.40",
		"4.400,0.992,-2462.17",
		"11.000,0.991,-16540.98",
	};

	public const string EngineFldHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	public static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6",
		"600,1282,-148,0.6",
		"799.9999999,1791,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};

	private ATShiftStrategyOptimized GetShiftStrategyAndGearbox(Mock<IVehicleContainer> vehicleContainer,
		out Mock<IAPTGearbox> gbx)
	{
		return GetShiftStrategyAndGearbox(vehicleContainer, out gbx, out _);
	}

	private ATShiftStrategyOptimized GetShiftStrategyAndGearbox(Mock<IVehicleContainer> vehicleContainer,
		out Mock<IAPTGearbox> gbx,
		out Mock<ITnOutPort> gbxNextComponent)
	{
		var shiftStrategy = new ATShiftStrategyOptimized(vehicleContainer.Object);
		
		gbx = GetGearbox(vehicleContainer.Object.RunData);


		gbxNextComponent = null;


		shiftStrategy.Gearbox = gbx.Object;
		return shiftStrategy;

		// var gbxMock = new Mock<APTGearbox>(vehicleContainer.Object, shiftStrategy) {
		// 	CallBase = true
		// };
		//
		// gbx = gbxMock.Object;
		// shiftStrategy.Gearbox = gbx;
		//
		// var mockPort = new Mock<ITnOutPort>();
		// NewtonMeter tqRequest = null;
		// PerSecond rpmRequest = null;
		//
		// mockPort.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
		// 	It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
		// 	tqRequest = tq;
		// 	rpmRequest = rpm;
		// 	return new ResponseSuccess(this) {
		// 		Engine = {
		// 			EngineSpeed = rpm,
		// 			PowerRequest = tq * rpm,
		// 		},
		// 	};
		// });
		//
		// mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
		// 	It.IsAny<PerSecond>(), true)).Returns(new ResponseDryRun(this));
		//
		// var idleController = new Mock<IIdleController>();
		//
		//
		// gbx.IdleController = idleController.Object;
		// gbx.Connect(mockPort.Object);
		//
		// gbxNextComponent = mockPort;
		// return shiftStrategy;
	}

	private static Mock<IAPTGearbox> GetGearbox(VectoRunData objectRunData)
	{
		Mock<IAPTGearbox> gbx;
		gbx = new Mock<IAPTGearbox>(MockBehavior.Strict);
		gbx.Name = "MockGearbox";
		TestContext.WriteLine(gbx.Name);
		bool disengaged = false;
		gbx.SetupGet(g => g.Disengaged).Returns(() => disengaged);
		gbx.SetupSet(g => g.Disengaged = It.IsAny<bool>()).Callback((bool value) => disengaged = value);

		gbx.Setup(g => g.ComputeShiftLosses(
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(),
			It.IsAny<GearshiftPosition>())).Returns((NewtonMeter outT, PerSecond outN, GearshiftPosition gear) => 
		{
			return 0.SI<WattSecond>();
		});
		gbx.Setup(g => g.EngineInertia).Returns(objectRunData.EngineData.Inertia);
		
		//TorqueConverter
		var tq = new Mock<ITorqueConverter>(MockBehavior.Strict);
		gbx.Setup(g => g.TorqueConverter).Returns(tq.Object);
		tq.Setup(tq => tq.FindOperatingPoint(
			It.IsAny<Second>(),
			It.IsAny<Second>(),
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>())).Returns((Second t, Second dt, NewtonMeter outTorque, PerSecond outSpeed) => {
			var speedRatio = 1.0;
			var torqueRatio = 1.0;
			return new TorqueConverterOperatingPoint() {
				Creeping = false,
				InAngularVelocity = speedRatio * outSpeed,
				OutAngularVelocity = outSpeed,
				InTorque = torqueRatio * outTorque,
				OutTorque = outTorque,
				SpeedRatio = speedRatio,
				TorqueRatio = torqueRatio
			};
		});
		return gbx;
	}
}