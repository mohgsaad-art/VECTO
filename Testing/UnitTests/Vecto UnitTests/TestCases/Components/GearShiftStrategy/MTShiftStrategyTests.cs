using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

public class MTShiftStrategyTests
{
	[TestCase(1, 2, 1000, 1500)]
	[TestCase(1, 6, 200, 8000)]
	[TestCase(2, 4, 800, 1400)]
    [TestCase(7, 8, 1000, 1400)]
	[TestCase(6, 8, 800, 1400)]
	[TestCase(5, 6, 1000, 1300)]
	[TestCase(4, 5, 1000, 1400)]
	[TestCase(3, 4, 1000, 1400)]
	[TestCase(8, 8, 1000, 1400)]
    public void Gearbox_ShiftUp(int gear, int newGear, double tq_Nm, double n_RPM)
	{
		var shiftExpected = gear != newGear;

		var gearRatios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };


		var container = GetMocks(n_RPM, gearRatios,
			out var runData,
			out var info,
			out var testPt,
			out var ptBuilder);


		var shiftStrategy = GetShiftStrategyAndGearbox(container, out _);



		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();


		var expectedN = n_RPM.RPMtoRad();
		var angularVelocity = expectedN / gearRatios[gear];

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

		var expectedT = tq_Nm.SI<NewtonMeter>();
		var torque = expectedT * gearRatios[gear];
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
        Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
		Assert.AreEqual(shiftExpected, shiftRequired);
    }


	[TestCase(2, 1, 1000, 300)]
	[TestCase(3, 4, 1000, 1400)]
	[TestCase(8, 7, 1800, 750)]
	[TestCase(7, 6, 1800, 750)]
	[TestCase(6, 5, 1800, 750)]
	[TestCase(5, 4, 1800, 750)]
	[TestCase(4, 3, 1800, 750)]
	[TestCase(3, 2, 1800, 750)]
	[TestCase(2, 1, 1900, 750)]
	[TestCase(1, 1, 1200, 700)]
	[TestCase(8, 4, 15000, 200)]
    public void Gearbox_ShiftDown(int gear, int newGear, double tq_Nm, double n_RPM)
	{
		var shiftExpected = gear != newGear;

		var gearRatios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

		var container = GetMocks(n_RPM, gearRatios,
			out var runData,
			out var info,
			out var testPt,
			out var ptBuilder);


		var shiftStrategy = GetShiftStrategyAndGearbox(container, out _);

		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();


		var expectedN = n_RPM.RPMtoRad();
		var angularVelocity = expectedN / gearRatios[gear];

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

		var expectedT = tq_Nm.SI<NewtonMeter>();
		var torque = expectedT * gearRatios[gear];
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
		Assert.AreEqual(shiftExpected, shiftRequired);
    }

	[TestCase(2, 1,2, 1000, 300)]
	[TestCase(3, 2,2, 1000, 1400)]
	[TestCase(8, 7,2, 1800, 750)]
	[TestCase(7, 6,2, 1800, 750)]
	[TestCase(6, 5,2, 1800, 750)]
	[TestCase(5, 4,2, 1800, 750)]
	[TestCase(4, 3,2, 1800, 750)]
	[TestCase(3, 2,2, 1800, 750)]
	[TestCase(2, 2,2, 1900, 750)]
	[TestCase(1, 2,2, 1200, 700)]
	[TestCase(8, 4,2, 15000, 200)]
	[TestCase(2, 2, 2, 300, 1000)]
    public void Gearbox_PTO(int gear, int newGear, int ptoGear, double tq_Nm, double n_RPM)
	{
		var shiftExpected = gear != newGear;

        var gearRatios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
		var container = GetMocks(
			n_RPM: n_RPM,
			gearRatios: gearRatios,
			runData: out var runData,
			info: out _,
			testPt: out _,
			ptBuilder: out _);

		//Cycle Info
		var cycleInfo = new Mock<IDrivingCycleInfo>();
		container.Setup(c => c.DrivingCycleInfo).Returns(cycleInfo.Object);
		cycleInfo.Setup(c => c.CycleData).Returns(
			GetPTOCycleData());

		runData.DriverData = new DriverData() {
			PTODriveRoadsweepingGear = new GearshiftPosition((uint)ptoGear)
		};
		//Recreate Shiftstrategy with updated rundata

		var shiftStrategy = GetShiftStrategyAndGearbox(container, out _);

		
        var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();


		var expectedN = n_RPM.RPMtoRad();
		var angularVelocity = expectedN / gearRatios[gear];

		


		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);



		var expectedT = tq_Nm.SI<NewtonMeter>();
		var torque = expectedT * gearRatios[gear];
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
		Assert.AreEqual(shiftExpected, shiftRequired);
    }


	private MTShiftStrategy GetShiftStrategyAndGearbox(Mock<IVehicleContainer> container,
		out Mock<IGearbox> gearbox)
	{
		gearbox = GetMockGearbox();
		
		var shiftStrategy = new MTShiftStrategy(container.Object) {
			Gearbox = gearbox.Object
		};
		
		SetVelocityDropLookupData(shiftStrategy);
		return shiftStrategy;
	}
	
	

	private Mock<IVehicleContainer> GetMocks(
		double n_RPM, 
		double[] gearRatios, 
		out VectoRunData runData, 
		out Mock<IVehicleInfo> info, 
		out Mock<ITestPowertrain> testPt, 
		out Mock<ISimplePowertrainBuilder> ptBuilder)
	{
		runData = GetRunData(gearRatios);
		var container = GetMockVehicleContainer(runData, n_RPM.RPMtoRad(), out info);

		testPt = GetMockTestPowertrain(runData);
		
		ptBuilder = new Mock<ISimplePowertrainBuilder>(MockBehavior.Strict);
		
		ptBuilder.Setup(p => p.CreateTestPowertrain(It.IsAny<IVehicleContainer>(),It.IsAny<bool>()))
			.Returns(testPt.Object);
		

		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

		// gbx = GetMockGearbox(container.Object);
		//
		// var shiftStrategy = new MTShiftStrategy(container.Object)
		// {
		// 	Gearbox = gbx.Object
		// };
		// SetVelocityDropLookupData(shiftStrategy);
		return container;
	}




    private VectoRunData GetRunData(double[] gearRatios)
	{
		var gearboxData = new GearboxData() {
			Gears = new Dictionary<uint, GearData>()
		};

		for (uint i = 1; i < gearRatios.Length; i++) {
			var gearRatio = gearRatios[i];
			gearboxData.Gears[i] = new GearData() {
				Ratio = gearRatio,
				LossMap = TransmissionLossMapReader.Create(0.96, gearRatio, $"Gear {i}")
			};
		}

		var gearShiftParameters = new ShiftStrategyParameters() {
			StartSpeed = 2.SI<MeterPerSecond>(),
			TimeBetweenGearshifts = 6.SI<Second>(),
			DownshiftAfterUpshiftDelay = 2.SI<Second>(),
			UpshiftAfterDownshiftDelay = 2.SI<Second>(),
			UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>()
		};


		string engineFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

		string[] engineFldData = new[] {
			"560,1180,-149,0.6		   ",
			"600,1282,-148,0.6		   ",
			"799.9999999,1791,-149,0.6 ",
			"1000,2300,-160,0.6		   ",
			"1200,2300,-179,0.6		   ",
			"1400,2300,-203,0.6		   ",
			"1599.999999,2079,-235,0.49",
			"1800,1857,-264,0.25	   ",
			"2000.000001,1352,-301,0.25",
			"2100,1100,-320,0.25	   ",
		};


		var fullLoadCurve =
			FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(engineFldHdr, engineFldData));
		var engineData = new CombustionEngineData() {
			IdleSpeed = 560.RPMtoRad(),
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{
					0u, fullLoadCurve
				}
			}
		};


		var axlRatio = 3.240355;
		var gearsInput = gearboxData.Gears.Select(x => {
			var r = new Mock<ITransmissionInputData>();
			r.Setup(g => g.Ratio).Returns(x.Value.Ratio);
			return r.Object;
		}).ToList();
		foreach (var entry in gearboxData.Gears) {
			var gearIdx = (int)entry.Key - 1;
			var dynamicTyreRadius = 0.5.SI<Meter>();

			entry.Value.ShiftPolygon = 
				DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
				gearIdx, engineData.FullLoadCurves.First().Value,
				gearsInput, engineData, axlRatio, 0.5.SI<Meter>());

			entry.Value.ExtendedShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygonExtended(
				gearIdx, engineData.FullLoadCurves.First().Value, gearsInput, engineData, axlRatio, dynamicTyreRadius);
		}


		var axleGearData = new AxleGearData() {
			AxleGear = new TransmissionData() {
				Ratio = 3.240355,
			}
		};


		var vehicleData = new VehicleData() {
			DynamicTyreRadius = 0.492.SI<Meter>(),
		};


		var mockCycle = new Mock<IDrivingCycleData>();
		mockCycle.Setup(cd => cd.Entries).Returns(
			new List<DrivingCycleData.DrivingCycleEntry>() {
				new DrivingCycleData.DrivingCycleEntry() {
					RoadGradient = 0.SI<Radian>()
				}
			});



		return new VectoRunData() {
			GearboxData = gearboxData,
			GearshiftParameters = gearShiftParameters,
			EngineData = engineData,
			AxleGearData = axleGearData,
			VehicleData = vehicleData,
			Cycle = mockCycle.Object,
		};
	}


	private Mock<IGearbox> GetMockGearbox()
	{
		var mtGbx = new Mock<IMTGearbox>(MockBehavior.Strict);
		var gbx = mtGbx.As<IGearbox>();
		
		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());
		
		return gbx;
	}


	private Mock<ITestPowertrainTransmission> GetMockTestGearbox(Dictionary<uint, GearData> ratios)
	{
		var gbx = new Mock<ITestPowertrainTransmission>();
		
		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());

		GearshiftPosition gear = null;
		gbx.SetupGet(g => g.Gear).Returns(() => {
			
			return gear;
		});
		gbx.SetupSet(g => g.SetGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => {
				gear = p;
			});


		GearshiftPosition nextGear = null;
		gbx.SetupGet(g => g.NextGear).Returns(() => nextGear);
		gbx.SetupSet(g => g.SetNextGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => nextGear = p);
			
		gbx.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
			return new ResponseSuccess(this)
			{
				Engine = {
					EngineSpeed = rpm,
					PowerRequest = tq * rpm,
				},
			};
		});
		
		
		gbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(),
				It.IsAny<bool>()))
			.Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => {
				var ratio = gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
				return dryRun
					? new ResponseDryRun(this) {
						Engine = {
							PowerRequest = n * t, EngineSpeed = n * ratio,
							DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						},
						Clutch = { PowerRequest = n * t }
					}
					: new ResponseSuccess(this) {
						Engine = { PowerRequest = n * t, EngineSpeed = n * ratio },
						Clutch = { PowerRequest = n * t }
					};
			});
		return gbx;
	}
	private Mock<IPowertainInfo> GetPowertrainInfo()
	{
		var powerTrainInfo = new Mock<IPowertainInfo>();
		powerTrainInfo.Setup(p => p.HasCombustionEngine).Returns(true);
		return powerTrainInfo;
	}

	private CycleData GetCycleData()
	{
		return new CycleData() {
			LeftSample = new DrivingCycleData.DrivingCycleEntry() {
				PTOActive = PTOActivity.Inactive
			}
		};
	}

	private CycleData GetPTOCycleData()
	{
		return new CycleData() {
			LeftSample = new DrivingCycleData.DrivingCycleEntry() {
				PTOActive = PTOActivity.PTOActivityRoadSweeping
			}
		};
	}

	private Mock<IVehicleContainer> GetMockVehicleContainer(VectoRunData runData, PerSecond engineSpeed, out Mock<IVehicleInfo> vehicleInfo)
	{
		var vehicleContainer = new Mock<IVehicleContainer>();

		var preProcessingRuns = new List<ISimulationPreprocessor>();

		vehicleContainer.Setup(c => c.RunData).Returns(runData);
		vehicleContainer.Setup(c => c.AddComponent(It.IsAny<VectoSimulationComponent>()));

		vehicleContainer.Setup(c => c.AddPreprocessor(It.IsAny<ISimulationPreprocessor>()))
			.Callback((ISimulationPreprocessor pre) => preProcessingRuns.Add(pre));

		//Vehicle Info
		vehicleInfo = new Mock<IVehicleInfo>();
		vehicleContainer.Setup(c => c.VehicleInfo).Returns(vehicleInfo.Object);
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(1.KMPHtoMeterPerSecond());

		vehicleInfo.Setup(v => v.AirDragResistance(It.IsAny<MeterPerSecond>(), It.IsAny<MeterPerSecond>()))
			.Returns(new AirDragLossResult(0.SI<Watt>(), 0.SI<SquareMeter>(), 0.SI<MeterPerSecond>()));
		vehicleInfo.Setup(v => v.RollingResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vehicleInfo.Setup(v => v.SlopeResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(30.KMPHtoMeterPerSecond());
		vehicleInfo.Setup(v => v.TotalMass).Returns(12000.SI<Kilogram>());

        //WheelsInfo
		vehicleContainer.Setup(c => c.WheelsInfo.ReducedMassWheels).Returns(0.SI<Kilogram>());

        //AxlegearInfo
        var axleGearInfo = new Mock<IAxlegearInfo>();
		vehicleContainer.Setup(c => c.AxlegearInfo).Returns(axleGearInfo.Object);
		axleGearInfo.Setup(a => a.AxlegearLoss()).Returns(0.SI<Watt>());

        //Powertrain Info
        vehicleContainer.Setup(c => c.PowertrainInfo).Returns(GetPowertrainInfo().Object);

		//Engine Info
		var engineInfo = new Mock<IEngineInfo>();
		vehicleContainer.Setup(c => c.EngineInfo).Returns(engineInfo.Object);
		engineInfo.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
		engineInfo.Setup(e => e.EngineRatedSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.RatedSpeed);
		engineInfo.Setup(e => e.EngineSpeed).Returns(engineSpeed);
		engineInfo.Setup(e => e.EngineN95hSpeed).Returns
			(runData.EngineData.FullLoadCurves.First().Value.N95hSpeed);
		engineInfo.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>())).Returns((PerSecond n) =>
			runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));

		//Cycle Info
		var cycleInfo = new Mock<IDrivingCycleInfo>();
		vehicleContainer.Setup(c => c.DrivingCycleInfo).Returns(cycleInfo.Object);
		cycleInfo.Setup(c => c.CycleData).Returns(
			GetCycleData());

		cycleInfo.Setup(c => c.CycleLookAhead(It.IsAny<Meter>())).Returns(new DrivingCycleData.DrivingCycleEntry() {
			Altitude = 0.SI<Meter>()
		});
		cycleInfo.Setup(c => c.Altitude).Returns(0.SI<Meter>());

		var pi = new Mock<IPowertainInfo>();



        //Driver Info
        var driverInfo = new Mock<IDriverInfo>();
		vehicleContainer.Setup(c => c.DriverInfo).Returns(driverInfo.Object);

		driverInfo.Setup(d => d.DriverBehavior).Returns(DrivingBehavior.Accelerating);
		driverInfo.Setup(d => d.DrivingAction).Returns(DrivingAction.Accelerate);

		return vehicleContainer;
	}

	private Mock<ITestPowertrain> GetMockTestPowertrain(VectoRunData runData)
	{
		var testPt = new Mock<ITestPowertrain>(MockBehavior.Strict);
		var simplePt = GetSimplePowertrain(runData, out var gbx);

		testPt.Setup(c => c.Container).Returns(simplePt.Object);
		testPt.Setup(c => c.Gearbox).Returns(gbx.Object);
		testPt.Setup(c => c.UpdateComponents());
		
		
		// var simplePt = GetSimplePowertrain(gearRatios);
		//
		//
		// testContainer.Setup(c => c.RunData).Returns(runData);
		// testContainer.Setup(c => c.PowertrainInfo).Returns(GetPowertrainInfo().Object);
		//
		// testPowertrain.Setup(t => t.Gearbox).Returns(GetMockGearbox(testContainer.Object).Object);
		
		var tEng = new Mock<ITestpowertrainCombustionEngine>();
		testPt.Setup(t => t.CombustionEngine).Returns(tEng.Object);
		tEng.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>()))
			.Returns((PerSecond n) => runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));

		return testPt;
	}

	private Mock<ISimpleVehicleContainer> GetSimplePowertrain(VectoRunData runData, out Mock<ITestPowertrainTransmission> testGearbox)
	{
		var simplePt = new Mock<ISimpleVehicleContainer>();
		
		simplePt.Setup(s => s.RunData).Returns(runData);
		simplePt.Setup(s => s.AddComponent(It.IsAny<VectoSimulationComponent>()));
		simplePt.Setup(s => s.PowertrainInfo).Returns(GetPowertrainInfo().Object);

		testGearbox = GetMockTestGearbox(runData.GearboxData.Gears);
		simplePt.Setup(s => s.GearboxCtl).Returns(testGearbox.Object);


		//Vehicle Info
		var vehicleInfo = new Mock<IVehicleInfo>();
		simplePt.Setup(c => c.VehicleInfo).Returns(vehicleInfo.Object);
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(1.KMPHtoMeterPerSecond());




		//simplePt.Setup(s => s.AddPreprocessor(It.IsAny<ISimulationPreprocessor>()))
		//	.Callback((ISimulationPreprocessor p) => p.RunPreprocessing());
		return simplePt;
	}

	private void SetVelocityDropLookupData(MTShiftStrategy shiftStrategy)
	{
		//"StartVelocity [km/h], Gradient [-], EndVelocity [km/h]"
		var data = new[] {
			new[] { 5.0, -0.0997, 9.061522965237558 },
			new[] { 5.0, -0.0798, 7.76698080079411 },
			new[] { 5.0, -0.0599, 6.46583777913701 },
			new[] { 5.0, -0.0400, 5.1596021788785045 },
			new[] { 5.0, -0.0200, 3.8498121019126907 },
			new[] { 5.0, 0.0000, 2.5380265159468918 },
			new[] { 5.0, 0.0200, 1.2258160477557427 },
			new[] { 5.0, 0.0400, 0.0 },
			new[] { 5.0, 0.0599, 0.0 },
			new[] { 5.0, 0.0798, 0.0 },
			new[] { 5.0, 0.0997, 0.0 },
			new[] { 10.0, -0.0997, 14.807789640888302 },
			new[] { 10.0, -0.0798, 13.474253785811362 },
			new[] { 10.0, -0.0599, 12.133880027250777 },
			new[] { 10.0, -0.0400, 10.788221550084467 },
			new[] { 10.0, -0.0200, 9.438862432338068 },
			new[] { 10.0, 0.0000, 8.087408432114643 },
			new[] { 10.0, 0.0200, 6.735477499784416 },
			new[] { 10.0, 0.0400, 5.384690105076845 },
			new[] { 10.0, 0.0599, 4.036659549786269 },
			new[] { 10.0, 0.0798, 2.6929823705748137 },
			new[] { 10.0, 0.0997, 1.3552289800549435 },
			new[] { 20.0, -0.0997, 25.061542153872097 },
			new[] { 20.0, -0.0798, 23.72002987685605 },
			new[] { 20.0, -0.0599, 22.371630788642932 },
			new[] { 20.0, -0.0400, 21.017907257116025 },
			new[] { 20.0, -0.0200, 19.660452757813403 },
			new[] { 20.0, 0.0000, 18.30088261992287 },
			new[] { 20.0, 0.0200, 16.94082447048767 },
			new[] { 20.0, 0.0400, 15.581908518759507 },
			new[] { 20.0, 0.0599, 14.225757795590708 },
			new[] { 20.0, 0.0798, 12.873978508374211 },
			new[] { 20.0, 0.0997, 11.528150617530041 },
			new[] { 30.000000000000004, -0.0997, 35.091774208140095 },
			new[] { 30.000000000000004, -0.0798, 33.750904327320896 },
			new[] { 30.000000000000004, -0.0599, 32.40315158162777 },
			new[] { 30.000000000000004, -0.0400, 31.050077596965064 },
			new[] { 30.000000000000004, -0.0200, 29.69327509188113 },
			new[] { 30.000000000000004, 0.0000, 28.33435862498606 },
			new[] { 30.000000000000004, 0.0200, 26.974955046566407 },
			new[] { 30.000000000000004, 0.0400, 25.616693776603913 },
			new[] { 30.000000000000004, 0.0599, 24.261197065863925 },
			new[] { 30.000000000000004, 0.0798, 22.91007034016412 },
			new[] { 30.000000000000004, 0.0997, 21.56489279686667 },
			new[] { 40.0, -0.0997, 45.024018797189854 },
			new[] { 40.0, -0.0798, 43.68636622428101 },
			new[] { 40.0, -0.0599, 42.34185052441486 },
			new[] { 40.0, -0.0400, 40.99202962224952 },
			new[] { 40.0, -0.0200, 39.63849244500093 },
			new[] { 40.0, 0.0000, 38.282849690992855 },
			new[] { 40.0, 0.0200, 36.926724305651554 },
			new[] { 40.0, 0.0400, 35.57174178507285 },
			new[] { 40.0, 0.0599, 34.21952045137207 },
			new[] { 40.0, 0.0798, 32.87166183129923 },
			new[] { 40.0, 0.0997, 31.5297412694979 },
			new[] { 50.0, -0.0997, 54.652157586769704 },
			new[] { 50.0, -0.0798, 53.319266704395716 },
			new[] { 50.0, -0.0599, 51.97954186606304 },
			new[] { 50.0, -0.0400, 50.634535516787736 },
			new[] { 50.0, -0.0200, 49.28583097196263 },
			new[] { 50.0, 0.0000, 47.93503321632867 },
			new[] { 50.0, 0.0200, 46.583759417454765 },
			new[] { 50.0, 0.0400, 45.23362925944123 },
			new[] { 50.0, 0.0599, 43.88625525588574 },
			new[] { 50.0, 0.0798, 42.54323315977748 },
			new[] { 50.0, 0.0997, 41.20613261641401 },
			new[] { 60.00000000000001, -0.0997, 64.2503607025971 },
			new[] { 60.00000000000001, -0.0798, 62.91932863058169 },
			new[] { 60.00000000000001, -0.0599, 61.58156853535776 },
			new[] { 60.00000000000001, -0.0400, 60.23862904729555 },
			new[] { 60.00000000000001, -0.0200, 58.89369896300112 },
			new[] { 60.00000000000001, 0.0000, 57.547040626925885 },
			new[] { 60.00000000000001, 0.0200, 56.199911833784675 },
			new[] { 60.00000000000001, 0.0400, 54.85392730356455 },
			new[] { 60.00000000000001, 0.0599, 53.510694584916905 },
			new[] { 60.00000000000001, 0.0798, 52.17180450267632 },
			new[] { 60.00000000000001, 0.0997, 50.83882182989668 },
			new[] { 70.0, -0.0997, 73.78388063954164 },
			new[] { 70.0, -0.0798, 72.45700102808648 },
			new[] { 70.0, -0.0599, 71.12341092304165 },
			new[] { 70.0, -0.0400, 69.78459543842656 },
			new[] { 70.0, -0.0200, 68.44188526693415 },
			new[] { 70.0, 0.0000, 67.09719334997524 },
			new[] { 70.0, 0.0200, 65.75212749798493 },
			new[] { 70.0, 0.0400, 64.40829754257321 },
			new[] { 70.0, 0.0599, 63.06730571969745 },
			new[] { 70.0, 0.0798, 61.73073716025881 },
			new[] { 70.0, 0.0997, 60.40015062055851 },
			new[] { 80.0, -0.0997, 83.25457689135129 },
			new[] { 80.0, -0.0798, 81.93182852399568 },
			new[] { 80.0, -0.0599, 80.60238880559001 },
			new[] { 80.0, -0.0400, 79.2676325992058 },
			new[] { 80.0, -0.0200, 77.92916666616652 },
			new[] { 80.0, 0.0000, 76.58872134590663 },
			new[] { 80.0, 0.0200, 75.24790011046437 },
			new[] { 80.0, 0.0400, 73.90830846424944 },
			new[] { 80.0, 0.0599, 72.57154434889891 },
			new[] { 80.0, 0.0798, 71.23918865436896 },
			new[] { 80.0, 0.0997, 69.91277394305271 },
		};
		var entries = new List<VelocitySpeedGearshiftPreprocessor.Entry>();
		foreach (var d in data) {
			entries.Add(new VelocitySpeedGearshiftPreprocessor.Entry() {
				StartVelocity = d[0].KMPHtoMeterPerSecond(),
				Gradient = d[1].SI<Radian>(),
				EndVelocity = d[2].KMPHtoMeterPerSecond(),
			});
		}

		shiftStrategy.VelocityDropData.Data = entries.ToArray();

	}
}