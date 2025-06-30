using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Range = System.Range;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy
{
	[TestFixture]
	public class APTNShiftStrategyTests
	{
		[TestCase(500, 80, 1)]
		[TestCase(5, 500, 2)]
		public void InitGear(double torque_Nm, double speed_rpm, int expectedGear)
		{
			// the first element 0.0 is just a placeholder for axlegear, not used in this test
			var ratios = new[] { 0.0, 3.86, 1.93 };

            var container = GetMocks(ratios);

			var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gearbox);

			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			var torque = torque_Nm.SI<NewtonMeter>();
			var speed = speed_rpm.RPMtoRad();

			var result = shiftStrategy.InitGear(absTime, dt, torque, speed);

			var newGear = result.Gear;
			Assert.AreEqual(expectedGear, newGear);



		}
		
		[TestCase(5000, 80, 1)]
		[TestCase(5, 500, 2)]
		public void InitStartGear(double torque_Nm, double speed_rpm, int expectedGear)
		{
			// the first element 0.0 is just a placeholder for axlegear, not used in this test
			var ratios = new[] { 0.0, 3.86, 1.93 };

			var container = GetMocks(ratios);
			container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
			
			var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gearbox);

			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			var torque = torque_Nm.SI<NewtonMeter>();
			var speed = speed_rpm.RPMtoRad();

			var result = shiftStrategy.InitGear(absTime, dt, torque, speed);

			var newGear = result.Gear;
			Assert.AreEqual(expectedGear, newGear);



		}

		[TestCase(
			500,
			80,
			500,
			10000,
			1,
			2, TestName = "Emergency UpShift")]

		[TestCase(
			500,
			80,
			1000,
			1800,
			1,
			2, TestName = "UpShift")]
		[TestCase(
			500,
			80,
			1000,
			800,
			1,
			1, TestName = "No Upshift")]
		public void Upshift(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear)
		{
			CheckUpshift(init_outTorque_Nm, init_outSpeed_rpm, outTorque_Nm, outSpeed_rpm, currentGear, expectedGear, false);
		}
		
		
		public void CheckUpshift(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear, bool effShift)
		{
			// the first element 0.0 is just a placeholder for axlegear, not used in this test
			var ratios = new[] { 0.0, 3.86, 1.93 };
			var ratio = ratios[currentGear];
			var container = GetMocks(ratios);

			if (!effShift) {
				DisableEffshift(container.Object.RunData);
			}

		
			var shiftRequired = currentGear != expectedGear;
			
			var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gearbox);
			// var gear = new GearshiftPosition((uint)currentGear);
			// gearbox.Gear = gear;
			
			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			var init_outTorque = init_outTorque_Nm.SI<NewtonMeter>();
			var init_outAngularVelocity = init_outSpeed_rpm.RPMtoRad();

			
			var outTorque = outTorque_Nm.SI<NewtonMeter>();
			var outAngularVelocity = outSpeed_rpm.RPMtoRad();
			
			var inTorque = outTorque / ratio;
			var inAngularVelocity = outAngularVelocity * ratio;
			
			var lastShiftTime = float.MinValue.SI<Second>();

			var response = new ResponseSuccess(this);
			
			Console.WriteLine(
				$"OutTorque: {outTorque}\n" +
				$"InTorque: {inTorque} \n" +
				$"OutSpeed: {outAngularVelocity.AsRPM} [rpm]\n" + 
				$"InSpeed: {inAngularVelocity.AsRPM} [rpm]\n");
			
			
			//Init gear
			var initResult = shiftStrategy.InitGear(absTime, dt, init_outTorque, init_outAngularVelocity);
			Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(currentGear));
			
			//Check shift required
			var result = shiftStrategy.ShiftRequired(absTime, dt,
				outTorque: outTorque,
				outAngularVelocity: outAngularVelocity,
				inTorque: inTorque,
				inAngularVelocity: inAngularVelocity,
				gear: shiftStrategy.NextGear,
				lastShiftTime: lastShiftTime,
				response: response);

			Assert.AreEqual(shiftRequired, result);
			Assert.AreEqual(expectedGear, shiftStrategy.NextGear.Gear);
			
			
			
		}

		[TestCase(
			500,
			80,
			-10,
			1000,
			1,
			2, TestName = "UpShift")]
		public void EarlyUpshift(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear)
		{
			// the first element 0.0 is just a placeholder for axlegear, not used in this test
			var ratios = new[] { 0.0, 3.86, 1.93 };
			var ratio = ratios[currentGear];
			var container = GetMocks(ratios);
			container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(20.KMPHtoMeterPerSecond());

		
			var shiftRequired = currentGear != expectedGear;
			
			var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gearbox);
			var gear = new GearshiftPosition((uint)currentGear);
			// gearbox.Gear = gear;
			
			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			var init_outTorque = init_outTorque_Nm.SI<NewtonMeter>();
			var init_outAngularVelocity = init_outSpeed_rpm.RPMtoRad();

			
			var outTorque = outTorque_Nm.SI<NewtonMeter>();
			var outAngularVelocity = outSpeed_rpm.RPMtoRad();
			
			var inTorque = outTorque / ratio;
			var inAngularVelocity = outAngularVelocity * ratio;
			
			var lastShiftTime = float.MinValue.SI<Second>();

			var response = new ResponseSuccess(this);
			
			Console.WriteLine(
				$"OutTorque: {outTorque}\n" +
				$"InTorque: {inTorque} \n" +
				$"OutSpeed: {outAngularVelocity.AsRPM} [rpm]\n" + 
				$"InSpeed: {inAngularVelocity.AsRPM} [rpm]\n");
			
			
			//Init gear
			var initResult = shiftStrategy.InitGear(absTime, dt, init_outTorque, init_outAngularVelocity);
			Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(currentGear));
			
			//Check shift required
			var result = shiftStrategy.ShiftRequired(absTime, dt,
				outTorque: outTorque,
				outAngularVelocity: outAngularVelocity,
				inTorque: inTorque,
				inAngularVelocity: inAngularVelocity,
				gear: shiftStrategy.NextGear,
				lastShiftTime: lastShiftTime,
				response: response);

			Assert.AreEqual(shiftRequired, result);
			Assert.AreEqual(expectedGear, shiftStrategy.NextGear.Gear);


		}
		
		[TestCase(
			5,
			200,
			2000,
			200,
			2,
			1, TestName = "Downshift")]
		public void Downshift(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear, int speedKmh=1)
		{
			var ratios = new[] { 0.0,  3.86, 1.93 };
			CheckDownshift(init_outTorque_Nm, init_outSpeed_rpm, outTorque_Nm, outSpeed_rpm, currentGear, expectedGear, ratios, false, speedKmh);
		}
		
		[TestCase(
			5,
			200,
			-500,
			200,
			4,
			2, 
			40, TestName = "EarlyDownshift")]
		public void EarlyDownshift(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear, int speedKmh=1)
		{
			var ratios = new[] { 0.0, 3.86, 3, 2.5, 1.93 };
			CheckDownshift(init_outTorque_Nm, init_outSpeed_rpm, outTorque_Nm, outSpeed_rpm, currentGear, expectedGear, ratios, true, speedKmh);
		}

		[TestCase(
			5,
			200,
			2000,
			40,
			4,
			2, TestName = "Downshift_SkipGear")]
		public void DownshiftSkipGears(double init_outTorque_Nm, double init_outSpeed_rpm, double outTorque_Nm,
			double outSpeed_rpm, int currentGear, int expectedGear)
		{
			var ratios = new[] { 0.0, 3.86, 3,  2.5, 1.93 };
			CheckDownshift(init_outTorque_Nm, init_outSpeed_rpm, outTorque_Nm, outSpeed_rpm, currentGear, expectedGear, ratios, false);
		}

		private void DisableEffshift(VectoRunData runData)
		{
			TestContext.WriteLine("EffShift Disabled");
			runData.GearshiftParameters.AllowedGearRangeFC = 0;
		}

		private void DisableEffshift(Mock<IVehicleContainer> vehicleContainer)
		{
			DisableEffshift(vehicleContainer.Object.RunData);
		}
		
		public void CheckDownshift(
			double init_outTorque_Nm,
			double init_outSpeed_rpm,
			double outTorque_Nm, 
			double outSpeed_rpm, 
			int currentGear,
			int expectedGear, 
			double[] ratios,
			bool effShift = true,
			int vehicleSpeed_kmH = 1)
		{
			// the first element 0.0 is just a placeholder for axlegear, not used in this test
			var ratio = ratios[currentGear];
			var container = GetMocks(ratios);
			container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(vehicleSpeed_kmH.KMPHtoMeterPerSecond());

			
			if (!effShift) {
				DisableEffshift(container.Object.RunData);
			}
			
			var shiftRequired = currentGear != expectedGear;
			
			var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gearbox);
			var gear = new GearshiftPosition((uint)currentGear);
			// gearbox.Gear = gear;
			
			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			var init_outTorque = init_outTorque_Nm.SI<NewtonMeter>();
			var init_outAngularVelocity = init_outSpeed_rpm.RPMtoRad();
			
			var outTorque = outTorque_Nm.SI<NewtonMeter>();
			var outAngularVelocity = outSpeed_rpm.RPMtoRad();
			
			var inTorque = outTorque / ratio;
			var inAngularVelocity = outAngularVelocity * ratio;
			
			var lastShiftTime = float.MinValue.SI<Second>();


			
			Console.WriteLine(
				$"OutTorque: {outTorque}\n" +
				$"InTorque: {inTorque} \n" +
				$"OutSpeed: {outAngularVelocity}\n" + 
				$"InSpeed: {inAngularVelocity}\n");
			
			
			//Init gear
			var initResult = shiftStrategy.InitGear(absTime, dt, init_outTorque, init_outAngularVelocity);
			Assert.That(shiftStrategy.NextGear.Gear, Is.EqualTo(currentGear));
			
			var response = new ResponseSuccess(this) { };
			response.ElectricMotor.AngularVelocity = inAngularVelocity;
			response.ElectricMotor.TorqueRequestEmMap = inTorque;
			response.ElectricMotor.DeRatingActive = false;
			// response.ElectricMotor;
			
			//Check shift required
			var result = shiftStrategy.ShiftRequired(absTime, dt,
				outTorque: outTorque,
				outAngularVelocity: outAngularVelocity,
				inTorque: inTorque,
				inAngularVelocity: inAngularVelocity,
				gear: shiftStrategy.NextGear,
				lastShiftTime: lastShiftTime,
				response: response);

			Assert.AreEqual(shiftRequired, result, "Expected Shift");
			Assert.AreEqual(expectedGear, shiftStrategy.NextGear.Gear);
			
			
			
		}
		
		
		
		
		
		private APTNShiftStrategy GetShiftStrategyAndGearbox(Mock<IVehicleContainer> container, out Mock<IIEPCGearbox> gearbox)
		{
			var shiftStrategy = new APTNShiftStrategy(container.Object);

			var gbx = new Mock<IIEPCGearbox>(MockBehavior.Strict);
			gbx.Name = "MockGearbox";
			gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
			gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());
			
			shiftStrategy.Gearbox = gbx.Object;
			SetVelocityDropLookupData(shiftStrategy);
			container.Setup(c => c.GearboxInfo).Returns(gbx.Object);

			gearbox = gbx;
			return shiftStrategy;
		}

		private Mock<IVehicleContainer> GetMocks(double[] ratios)
		{
			var container = new Mock<IVehicleContainer>();

			//RunData
			var runData = GetRunData(ratios);
			container.Setup(r => r.RunData).Returns(runData);

			//EmInfo
			var em = GetElectricMotor(container.Object, runData.ElectricMachinesData.Single().Item2);
			container.Setup(c => c.ElectricMotorInfo(PowertrainPosition.BatteryElectricE2))
				.Returns(em);
			container.Setup(c => c.PowertrainInfo.ElectricMotorPositions).Returns(new[] {
				PowertrainPosition.BatteryElectricE2
			});
			
			// container.Setup(r => r.GetElectricMotors()).Returns(new List<IElectricMotorInfo>(){em});
			
			container.Setup(c => c.PowertrainInfo.HasCombustionEngine).Returns(false);

			//BatteryInfo
			container.Setup(c => c.BatteryInfo.InternalVoltage).Returns(700.SI<Volt>());
			
			//VehicleInfo
			container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(1.KMPHtoMeterPerSecond());
			container.Setup(c => c.VehicleInfo.VehicleStopped).Returns(false);
			
			//DriverInfo
			container.Setup(c => c.DriverInfo.DriverBehavior).Returns(DrivingBehavior.Accelerating);
			container.Setup(c => c.DriverInfo.DrivingAction).Returns(DrivingAction.Accelerate);
			// container.Set
			
			//PowertrainBuilder, SimplePowertrain
			var testPt = GetTestPowertrain(ratios, container, out _);
			var ptBuilder = new Mock<ISimplePowertrainBuilder>();
			ptBuilder.Setup(c => c.CreateTestPowertrain(
					It.IsAny<IVehicleContainer>(), It.IsAny<bool>()))
				.Returns(testPt.Object);
			ptBuilder.Setup(c => c.CreateTestPowertrain(
					It.IsAny<IVehicleContainer>(), It.IsAny<bool>(), It.IsAny<VectoSimulationJobType>()))
				.Returns(testPt.Object);
			container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

			//DrivingCycleInfo
			container.Setup(c => c.DrivingCycleInfo.RoadGradient).Returns(0.SI<Radian>());

			return container;
		}
		
		private ElectricMotor GetElectricMotor(IVehicleContainer container, ElectricMotorData emData)
		{
			var emControl = new SimpleElectricMotorControl();
			
			var electricMotor = new ElectricMotor(container: container, emData, emControl,
				PowertrainPosition.BatteryElectricE2);

			//Connect Electric System
			var es = GetMockElectricSystem();
			electricMotor.Connect(es.Object);
			return electricMotor;
		}
		
		private TestPowertrainElectricMotor GetTestPowertrainElectricMotor(ISimpleVehicleContainer container,
			ElectricMotorData emData)
		{
			var emControl = new SimpleElectricMotorControl();
			
			var electricMotor = new TestPowertrainElectricMotor(container: container, emData, emControl,
				PowertrainPosition.BatteryElectricE2);

			//Connect Electric System
			var es = GetMockElectricSystem();
			electricMotor.Connect(es.Object);
			return electricMotor;
		}

		private Mock<ITestPowertrain> GetTestPowertrain(double[] ratios, 
			Mock<IVehicleContainer> container,
			out Mock<ISimpleVehicleContainer> simplePt)
		{
			var testPt = new Mock<ITestPowertrain>();
			simplePt = GetSimplePowertrain(ratios, container, out var gbx);
			var simplePtObj = simplePt.Object;
			testPt.Setup(t => t.ElectricMotors).Returns(() => {
				var dict = new Dictionary<PowertrainPosition, ITestpowertrainElectricMotor>();
				foreach (var (k, v) in simplePtObj.ElectricMotors) {
					if (v is ITestpowertrainElectricMotor testEm) {
						dict[k] = testEm;
					} else {
						throw new Exception("Expected TestElectricMotor in simple powertrain");
					}
				}
				return dict;
			});

			testPt.Setup(t => t.Container).Returns(simplePt.Object);
			
			testPt.Setup(t => t.Gearbox).Returns(gbx.Object);
			return testPt;
		}
		
		private Mock<IElectricSystem> GetMockElectricSystem()
		{
			var electricSystem = new Mock<IElectricSystem>();
			
			
			electricSystem
				.Setup(es => es.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<Watt>(), true))
				.Returns((Second t, Second dt, Watt powerDemand, bool dryrun) => {
					
					var response = new Mock<IElectricSystemResponse>();
					response
						.Setup(mr => mr.MaxPowerDrive)
						.Returns(-100E3.SI<Watt>());
					response
						.Setup(mr => mr.MaxPowerDrag)
						.Returns(100E3.SI<Watt>());
					response
						.Setup(mr => mr.RESSPowerDemand).Returns(powerDemand);
						
						
					
					
					return response.Object;
				});


			return electricSystem;
		}
		
		private ElectricMotor GetMockElectricMotor(IVehicleContainer container, ElectricMotorData emData)
		{
			var emControl = new SimpleElectricMotorControl();
			
			var electricMotor = new ElectricMotor(container: container, emData, emControl,
				PowertrainPosition.BatteryElectricE2);

			//Connect Electric System
			var es = GetMockElectricSystem();
			electricMotor.Connect(es.Object);
			return electricMotor;
		}
		
		private Mock<ISimpleVehicleContainer> GetSimplePowertrain(
			double[] ratios, 
			Mock<IVehicleContainer> container, 
			out Mock<ITestPowertrainTransmission> testGbx)
		{
			var simplePt = new Mock<ISimpleVehicleContainer>();
			simplePt.Setup(s => s.IsTestPowertrain).Returns(true);
			
			
			var components = new List<VectoSimulationComponent>();
			
			var runData = container.Object.RunData;
			var em = GetTestPowertrainElectricMotor(simplePt.Object, runData.ElectricMachinesData.Single().Item2);
			var emDict = new Dictionary<PowertrainPosition, IElectricMotorInfo>() {
				{ PowertrainPosition.BatteryElectricE2, em },
			};
			simplePt.Setup(r => r.ElectricMotorInfo(PowertrainPosition.BatteryElectricE2))
				.Returns(em);

			//BatteryInfo
			simplePt.Setup(s => s.BatteryInfo.InternalVoltage).Returns(700.SI<Volt>());

			simplePt.Setup(s => s.ElectricMotors).Returns(emDict);

			simplePt.Setup(s => s.PowertrainInfo.HasCombustionEngine).Returns(false);
			
			simplePt.Setup(s => s.RunData).Returns(runData);

			testGbx = GetTestGearbox(simplePt.Object, em, runData.GearboxData.Gears);
			
			simplePt.Setup(s => s.GearboxCtl).Returns(testGbx.Object);
			simplePt.Setup(s => s.GearboxInfo).Returns(testGbx.Object);
			simplePt.Setup(s => s.GearboxOutPort).Returns(testGbx.Object);
			
			simplePt.Setup(s => s.SimulationComponents()).Returns(components);
			simplePt.Setup(s => s.Brakes).Returns(new Mock<IBrakes>().Object);

			//Take from real powertrain
			simplePt.Setup(s => s.VehicleInfo).Returns(container.Object.VehicleInfo);
			simplePt.Setup(s => s.DriverInfo).Returns(container.Object.DriverInfo);
			
			return simplePt;
		}

		Mock<ITestPowertrainTransmission> GetTestGearbox(
			ISimpleVehicleContainer simpleContainer,
			TestPowertrainElectricMotor em,
			Dictionary<uint, GearData> ratios)
		{
			Mock<IAMTGearbox> amtGearbox = new Mock<IAMTGearbox>();
			amtGearbox.Name = "PEVAMT_TestGearbox";
			Mock<ITestPowertrainTransmission> gbx = amtGearbox.As<ITestPowertrainTransmission>();
			
			gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
			gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());

			GearshiftPosition gear = null;
			gbx.SetupGet(g => g.Gear).Returns(() => gear);
			gbx.SetupSet(g => g.SetGear = It.IsAny<GearshiftPosition>())
				.Callback<GearshiftPosition>(p => {
					gear = p;
				});

		
			GearshiftPosition nextGear = null;
			gbx.SetupGet(g => g.NextGear).Returns(() => nextGear);
			gbx.SetupSet(g => g.SetNextGear = It.IsAny<GearshiftPosition>())
				.Callback<GearshiftPosition>(p => nextGear = p);
				
			
			gbx.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>())).Returns((NewtonMeter outTorque, PerSecond outSpeed) => {
				var ratio =
					gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
				
				var inSpeed = outSpeed * ratio;
				var inTorque = outTorque / ratio;
				
				em.Initialize(inTorque, inSpeed);
				return new ResponseSuccess(this)
				{
					
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
				NewtonMeter outTorque,
				PerSecond outSpeed,
				bool dryRun) => {
		
				var ratio =
					gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;

				var inSpeed = outSpeed * ratio;
				var inTorque = outTorque / ratio;

				var emResponse = em.Request(absTime, dt, inTorque, inSpeed, dryRun);
				return dryRun
					? new ResponseDryRun(this, emResponse) {
						// Engine = {
						// 	PowerRequest = n * t, EngineSpeed = n * ratio,
						// 	DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						// 	TotalTorqueDemand = t,
						// },
						// ElectricMotor = emResponse.ElectricMotor,
						Gearbox = {
							Gear = gbx.Object.Gear,
							InputSpeed = inSpeed,
							InputTorque = inTorque,
							OutputSpeed = outSpeed,
							OutputTorque = outTorque,
							PowerRequest = outSpeed * outTorque,
						},
						Clutch = { PowerRequest = outSpeed * outTorque },
						DeltaFullLoad = outSpeed * outTorque / 2 * (-1)
					}
					: new ResponseSuccess(this) {
						Gearbox = {
							Gear = gbx.Object.Gear,
							InputSpeed = inSpeed,
							InputTorque = inTorque,
						},
						Clutch = { PowerRequest = outSpeed * outTorque }
					};
			});
			return gbx;
		}


		private VectoRunData GetRunData(double[] ratios)
		{
			var runData = new VectoRunData();

			//GearShiftParameters
			var gearshiftParameters = new ShiftStrategyParameters() {
				TorqueReserve = 0.0,
				TimeBetweenGearshifts = 2.SI<Second>(),
				DownshiftAfterUpshiftDelay = 2.SI<Second>(),
				UpshiftAfterDownshiftDelay = 2.SI<Second>(),
				UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>(),
				StartSpeed = 0.KMPHtoMeterPerSecond(),
				RatingFactorCurrentGear = 1,
				AllowedGearRangeFC = 2,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
			};
			runData.GearshiftParameters = gearshiftParameters;

			runData.ElectricMachinesData = GetElectricMachinesData();
			var emData = runData.ElectricMachinesData.Single(i => i.Item1 == PowertrainPosition.BatteryElectricE2)
				.Item2;
			//VehicleData
			var vehicleData = new VehicleData() {
				DynamicTyreRadius = 0.492.SI<Meter>(),
            };
			runData.VehicleData = vehicleData;

			//CycleData 
			var mockCycle = new Mock<IDrivingCycleData>();
			mockCycle.Setup(cd => cd.Entries).Returns(
				new List<DrivingCycleData.DrivingCycleEntry>() {
					new DrivingCycleData.DrivingCycleEntry() {
						RoadGradient = 0.SI<Radian>()
					}
				});
			runData.Cycle = mockCycle.Object;
			

            //Gearboxdata
			var gearboxInputData = new Mock<IGearboxDeclarationInputData>();

			var gearsInputData = new List<Mock<ITransmissionInputData>>();

			gearboxInputData.Setup(
				d => d.Gears)
				.Returns(gearsInputData.Select(m => m.Object)
				.ToList());


			var gearboxData = new GearboxData() {
				Gears = new Dictionary<uint, GearData>(ratios.Length),
				InputData = gearboxInputData.Object,
				Type = GearboxType.AMT,
				TractionInterruption = 1.SI<Second>(),
				Inertia = 0.SI<KilogramSquareMeter>(),
			};
			



			
			runData.GearboxData = gearboxData;
			for (uint i = 1; i < ratios.Length; i++) {
				gearboxData.Gears[i] = new GearData
				{
					Ratio = ratios[i],
					LossMap = TransmissionLossMapReader.Create(0.96, ratios[i], $"Gear {i}"),

				};
			}
			var axlRatio = 3.240355;
			var gearsInput = gearboxData.Gears.Select(x => {
				var r = new Mock<ITransmissionInputData>();
				r.Setup(g => g.Ratio).Returns(x.Value.Ratio);
				r.Setup(g => g.MaxTorque).Returns(x.Value.MaxTorque);
				r.Setup(g => g.MaxInputSpeed).Returns(x.Value.MaxSpeed);
				
				return r.Object;
			}).ToList();
			foreach (var entry in gearboxData.Gears) {
				var gearIdx = (int)entry.Key - 1;
				var dynamicTyreRadius = 0.5.SI<Meter>();

				var shiftPolygon = DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(
					gearIdx,
					emData,
					gearsInput);
					
				entry.Value.ShiftPolygon = shiftPolygon;

			}


			return runData;
		}

		private IList<Tuple<PowertrainPosition, ElectricMotorData>> GetElectricMachinesData()
		{
			//Nr of electric machines
			const int emCount = 2;
			//ElectricMachines
			// var electricMotorData = new ElectricMotorData() {
			// 	
			// };
			var emDataAdapter = new ElectricMachinesDataAdapter();
			var emInputData = new Mock<IElectricMotorDeclarationInputData>();

			var powerMapMock = new Mock<IElectricMotorPowerMap>();
			powerMapMock.Setup(pm => pm.Gear).Returns(0);
			powerMapMock.Setup(pm => pm.PowerMap).Returns(GetEfficiencyMapData());
			
			var voltageLevels = new List<IElectricMotorVoltageLevel>() {
				new ElectricMotorVoltageLevel(){
					VoltageLevel = 100.SI<Volt>(),
					ContinuousTorque = 450.SI<NewtonMeter>(),
					ContinuousTorqueSpeed = 2460.RPMtoRad(),
					OverloadTorque = 485.SI<NewtonMeter>(),
					OverloadTestSpeed = 2460.RPMtoRad(),
					OverloadTime = 120.SI<Second>(),
					FullLoadCurve = GetFullLoadCurveData(),
					PowerMap = new List<IElectricMotorPowerMap>(){powerMapMock.Object}
				},
				new ElectricMotorVoltageLevel(){
					VoltageLevel = 1000.SI<Volt>(),
					ContinuousTorque = 450.SI<NewtonMeter>(),
					ContinuousTorqueSpeed = 2460.RPMtoRad(),
					OverloadTorque = 485.SI<NewtonMeter>(),
					OverloadTestSpeed = 2460.RPMtoRad(),
					OverloadTime = 120.SI<Second>(),
					FullLoadCurve = GetFullLoadCurveData(),
					PowerMap = new List<IElectricMotorPowerMap>(){powerMapMock.Object}
				}
			};
			
			emInputData.Setup(em => em.CertificationMethod).Returns(CertificationMethod.Measured);
			emInputData.Setup(em => em.Inertia).Returns(0.225.SI<KilogramSquareMeter>());
			emInputData.Setup(em => em.R85RatedPower).Returns(250E3.SI<Watt>());
			emInputData.Setup(em => em.ElectricMachineType).Returns(ElectricMachineType.ASM);
			emInputData.Setup(em => em.VoltageLevels).Returns(voltageLevels);
			emInputData.Setup(em => em.DragCurve).Returns(GetDragCurveData);
			
			var adcInputData = new Mock<IADCDeclarationInputData>();
			adcInputData.Setup(adc => adc.Ratio).Returns(1.0f);
			
			var emsInputData = new Mock<IElectricMachinesDeclarationInputData>();
			emsInputData.Setup(em => em.Entries).Returns(
				new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>>() {
					new ElectricMachineEntry<IElectricMotorDeclarationInputData>() {
						Count = emCount,
						Position = PowertrainPosition.BatteryElectricE2,
						ElectricMachine = emInputData.Object,
						ADC = adcInputData.Object,
						RatioADC = 2.0,
						MechanicalTransmissionEfficiency = 0.97,
					}
				});
			
			var avgVoltage = 200.SI<Volt>();
			var torqueLimits = new Dictionary<EMPlacement, IList<Tuple<Volt, TableData>>>();
			
			var electricMachinesData = emDataAdapter.CreateElectricMachines(emsInputData.Object, torqueLimits:torqueLimits, 
				avgVoltage, null);

			
			
			

			return electricMachinesData;
		
		}

		private TableData GetDragCurveData()
		{
			return InputDataHelper.InputDataAsTableData(
				"n [rpm] , T_drag [Nm]",
				"0       , -6.06",
				"7363.77    , -30.31");
		}


		private new List<IElectricMotorLoadCurve> GetFullLoadCurveData()
		{
			var data = new[] {
				#region data

				"0,485,-485",
				"2461.158914,485,-485",
				"2452.135493,485,-485",
				"2466.863034,483.8845,-483.8845",
				"2481.590574,481.010875,-481.010875",
				"2496.318115,478.173625,-478.173625",
				"2503.681885,476.767125,-476.767125",
				"2577.319588,463.138625,-463.138625",
				"2650.95729,450.274,-450.274",
				"2724.594993,438.1005,-438.1005",
				"2798.232695,426.58175,-426.58175",
				"2871.870398,415.645,-415.645",
				"2945.5081,405.253875,-405.253875",
				"3019.145803,395.359875,-395.359875",
				"3092.783505,385.950875,-385.950875",
				"3166.421208,376.978375,-376.978375",
				"3240.05891,368.406,-368.406",
				"3313.696613,360.221625,-360.221625",
				"3387.334315,352.388875,-352.388875",
				"3460.972018,344.895625,-344.895625",
				"3534.60972,337.7055,-337.7055",
				"3608.247423,330.8185,-330.8185",
				"3681.885125,324.19825,-324.19825",
				"3755.522828,317.84475,-317.84475",
				"3829.16053,311.73375,-311.73375",
				"3902.798233,305.853125,-305.853125",
				"3976.435935,300.178625,-300.178625",
				"4050.073638,294.722375,-294.722375",
				"4123.71134,289.460125,-289.460125",
				"4197.349043,284.37975,-284.37975",
				"4270.986745,279.48125,-279.48125",
				"4344.624448,274.740375,-274.740375",
				"4418.26215,270.16925,-270.16925",
				"4491.899853,265.7315,-265.7315",
				"4565.537555,261.451375,-261.451375",
				"4639.175258,257.304625,-257.304625",
				"4712.81296,253.279125,-253.279125",
				"4786.450663,249.387,-249.387",
				"4860.088365,245.604,-245.604",
				"4933.726068,241.94225,-241.94225",
				"5007.36377,238.3775,-238.3775",
				"5081.001473,234.921875,-234.921875",
				"5154.639175,231.575375,-231.575375",
				"5228.276878,228.31375,-228.31375",
				"5301.91458,225.137,-225.137",
				"5375.552283,222.05725,-222.05725",
				"5449.189985,219.05025,-219.05025",
				"5522.827688,216.128125,-216.128125",
				"5596.46539,213.290875,-213.290875",
				"5670.103093,210.51425,-210.51425",
				"5743.740795,207.8225,-207.8225",
				"5817.378498,205.191375,-205.191375",
				"5891.0162,202.620875,-202.620875",
				"5964.653903,200.123125,-200.123125",
				"6038.291605,197.686,-197.686",
				"6111.929308,195.297375,-195.297375",
				"6185.56701,192.969375,-192.969375",
				"6259.204713,190.702,-190.702",
				"6332.842415,188.483125,-188.483125",
				"6406.480118,186.324875,-186.324875",
				"6480.11782,184.203,-184.203",
				"6553.755523,182.129625,-182.129625",
				"6627.393225,180.10475,-180.10475",
				"6701.030928,178.128375,-178.128375",
				"6774.66863,176.2005,-176.2005",
				"6848.306333,174.296875,-174.296875",
				"6921.944035,172.44175,-172.44175",
				"6995.581738,170.635125,-170.635125",
				"7069.21944,168.85275,-168.85275",
				"7142.857143,167.10675,-167.10675",
				"7216.494845,165.40925,-165.40925",
				"7290.132548,163.736,-163.736",
				"7363.77025,162.099125,-162.099125"
			};
				#endregion data

			return new List<IElectricMotorLoadCurve>() {
				new ElectricMotorLoadCurve() {
					Gear = 0,
					LoadCurve = InputDataHelper.InputDataAsTableData("n [rpm] , T_drive [Nm] , T_drag [Nm]", data)
						}
				};
		}

		//private ElectricMotorFullLoadCurve GetFullLoadCurve(int count)
		//{
		//	var inputData = GetFullLoadCurveData();
		//	return ElectricFullLoadCurveReader.Create(inputData, count);
		//}

		private TableData GetEfficiencyMapData()
		{
			return InputDataHelper.InputDataAsTableData(
				"n [rpm] , T [Nm] , P_el [kW]",
				#region entries
				"0, -485, 0.000",
"0, -461, 0.000",
"0, -437, 0.000",
"0, -412, 0.000",
"0, -388, 0.000",
"0, -364, 0.000",
"0, -340, 0.000",
"0, -315, 0.000",
"0, -291, 0.000",
"0, -267, 0.000",
"0, -243, 0.000",
"0, -218, 0.000",
"0, -194, 0.000",
"0, -170, 0.000",
"0, -146, 0.000",
"0, -121, 0.000",
"0, -97, 0.000",
"0, -73, 0.000",
"0, -48, 0.000",
"0, -24, 0.000",
"0, -5, 0.000",
"0, 5, 0.000",
"0, 24, 0.000",
"0, 48, 0.000",
"0, 73, 0.000",
"0, 97, 0.000",
"0, 121, 0.000",
"0, 146, 0.000",
"0, 170, 0.000",
"0, 194, 0.000",
"0, 218, 0.000",
"0, 243, 0.000",
"0, 267, 0.000",
"0, 291, 0.000",
"0, 315, 0.000",
"0, 340, 0.000",
"0, 364, 0.000",
"0, 388, 0.000",
"0, 412, 0.000",
"0, 437, 0.000",
"0, 461, 0.000",
"0, 485, 0.000",
"49, -485, 0.000",
"49, -461, 0.000",
"49, -437, 0.000",
"49, -412, 0.000",
"49, -388, 0.000",
"49, -364, 0.000",
"49, -340, -0.045",
"49, -315, -0.111",
"49, -291, -0.166",
"49, -267, -0.211",
"49, -243, -0.245",
"49, -218, -0.268",
"49, -194, -0.280",
"49, -170, -0.281",
"49, -146, -0.272",
"49, -121, -0.252",
"49, -97, -0.221",
"49, -73, -0.179",
"49, -48, -0.126",
"49, -24, -0.062",
"49, -5, -0.004",
"49, 5, 0.048",
"49, 24, 0.193",
"49, 48, 0.384",
"49, 73, 0.587",
"49, 97, 0.802",
"49, 121, 1.029",
"49, 146, 1.267",
"49, 170, 1.518",
"49, 194, 1.779",
"49, 218, 2.053",
"49, 243, 2.338",
"49, 267, 2.636",
"49, 291, 2.944",
"49, 315, 3.265",
"49, 340, 3.597",
"49, 364, 3.941",
"49, 388, 4.297",
"49, 412, 4.665",
"49, 437, 5.044",
"49, 461, 5.435",
"49, 485, 5.838",
"492, -485, -20.733",
"492, -461, -19.800",
"492, -437, -18.856",
"492, -412, -17.900",
"492, -388, -16.932",
"492, -364, -15.953",
"492, -340, -14.962",
"492, -315, -13.959",
"492, -291, -12.945",
"492, -267, -11.919",
"492, -243, -10.881",
"492, -218, -9.832",
"492, -194, -8.771",
"492, -170, -7.699",
"492, -146, -6.614",
"492, -121, -5.519",
"492, -97, -4.411",
"492, -73, -3.292",
"492, -48, -2.161",
"492, -24, -1.019",
"492, -5, -0.097",
"492, 5, 0.416",
"492, 24, 1.498",
"492, 48, 2.863",
"492, 73, 4.240",
"492, 97, 5.630",
"492, 121, 7.033",
"492, 146, 8.448",
"492, 170, 9.876",
"492, 194, 11.316",
"492, 218, 12.769",
"492, 243, 14.235",
"492, 267, 15.713",
"492, 291, 17.204",
"492, 315, 18.708",
"492, 340, 20.224",
"492, 364, 21.753",
"492, 388, 23.294",
"492, 412, 24.848",
"492, 437, 26.415",
"492, 461, 27.994",
"492, 485, 29.586",
"984, -485, -44.101",
"984, -461, -42.013",
"984, -437, -39.910",
"984, -412, -37.794",
"984, -388, -35.664",
"984, -364, -33.520",
"984, -340, -31.362",
"984, -315, -29.190",
"984, -291, -27.004",
"984, -267, -24.805",
"984, -243, -22.591",
"984, -218, -20.363",
"984, -194, -18.122",
"984, -170, -15.867",
"984, -146, -13.597",
"984, -121, -11.314",
"984, -97, -9.017",
"984, -73, -6.706",
"984, -48, -4.381",
"984, -24, -2.042",
"984, -5, -0.161",
"984, 5, 0.867",
"984, 24, 2.993",
"984, 48, 5.663",
"984, 73, 8.349",
"984, 97, 11.049",
"984, 121, 13.765",
"984, 146, 16.496",
"984, 170, 19.242",
"984, 194, 22.003",
"984, 218, 24.779",
"984, 243, 27.571",
"984, 267, 30.377",
"984, 291, 33.198",
"984, 315, 36.035",
"984, 340, 38.887",
"984, 364, 41.754",
"984, 388, 44.635",
"984, 412, 47.532",
"984, 437, 50.445",
"984, 461, 53.372",
"984, 485, 56.314",
"1477, -485, -67.122",
"1477, -461, -63.906",
"1477, -437, -60.673",
"1477, -412, -57.422",
"1477, -388, -54.153",
"1477, -364, -50.867",
"1477, -340, -47.563",
"1477, -315, -44.241",
"1477, -291, -40.902",
"1477, -267, -37.545",
"1477, -243, -34.170",
"1477, -218, -30.778",
"1477, -194, -27.368",
"1477, -170, -23.941",
"1477, -146, -20.496",
"1477, -121, -17.033",
"1477, -97, -13.552",
"1477, -73, -10.054",
"1477, -48, -6.539",
"1477, -24, -3.005",
"1477, -5, -0.166",
"1477, 5, 1.383",
"1477, 24, 4.552",
"1477, 48, 8.530",
"1477, 73, 12.528",
"1477, 97, 16.545",
"1477, 121, 20.581",
"1477, 146, 24.636",
"1477, 170, 28.710",
"1477, 194, 32.804",
"1477, 218, 36.916",
"1477, 243, 41.048",
"1477, 267, 45.199",
"1477, 291, 49.369",
"1477, 315, 53.558",
"1477, 340, 57.766",
"1477, 364, 61.994",
"1477, 388, 66.240",
"1477, 412, 70.506",
"1477, 437, 74.791",
"1477, 461, 79.095",
"1477, 485, 83.418",
"1969, -485, -89.780",
"1969, -461, -85.464",
"1969, -437, -81.126",
"1969, -412, -76.766",
"1969, -388, -72.382",
"1969, -364, -67.976",
"1969, -340, -63.546",
"1969, -315, -59.094",
"1969, -291, -54.620",
"1969, -267, -50.122",
"1969, -243, -45.602",
"1969, -218, -41.058",
"1969, -194, -36.492",
"1969, -170, -31.904",
"1969, -146, -27.292",
"1969, -121, -22.658",
"1969, -97, -18.000",
"1969, -73, -13.320",
"1969, -48, -8.618",
"1969, -24, -3.892",
"1969, -5, -0.095",
"1969, 5, 1.980",
"1969, 24, 6.193",
"1969, 48, 11.483",
"1969, 73, 16.796",
"1969, 97, 22.135",
"1969, 121, 27.498",
"1969, 146, 32.886",
"1969, 170, 38.299",
"1969, 194, 43.736",
"1969, 218, 49.199",
"1969, 243, 54.686",
"1969, 267, 60.197",
"1969, 291, 65.734",
"1969, 315, 71.295",
"1969, 340, 76.881",
"1969, 364, 82.492",
"1969, 388, 88.127",
"1969, 412, 93.787",
"1969, 437, 99.472",
"1969, 461, 105.182",
"1969, 485, 110.916",
"2461, -485, -112.056",
"2461, -461, -106.670",
"2461, -437, -101.254",
"2461, -412, -95.808",
"2461, -388, -90.334",
"2461, -364, -84.830",
"2461, -340, -79.296",
"2461, -315, -73.733",
"2461, -291, -68.141",
"2461, -267, -62.519",
"2461, -243, -56.868",
"2461, -218, -51.188",
"2461, -194, -45.478",
"2461, -170, -39.738",
"2461, -146, -33.970",
"2461, -121, -28.172",
"2461, -97, -22.344",
"2461, -73, -16.487",
"2461, -48, -10.601",
"2461, -24, -4.685",
"2461, -5, 0.000",
"2461, 5, 2.679",
"2461, 24, 7.937",
"2461, 48, 14.539",
"2461, 73, 21.173",
"2461, 97, 27.839",
"2461, 121, 34.536",
"2461, 146, 41.266",
"2461, 170, 48.027",
"2461, 194, 54.820",
"2461, 218, 61.646",
"2461, 243, 68.503",
"2461, 267, 75.392",
"2461, 291, 82.313",
"2461, 315, 89.265",
"2461, 340, 96.250",
"2461, 364, 103.267",
"2461, 388, 110.315",
"2461, 412, 117.396",
"2461, 437, 124.508",
"2461, 461, 131.652",
"2461, 485, 138.828",
"2953, -485, -133.934",
"2953, -461, -127.504",
"2953, -437, -121.037",
"2953, -412, -114.532",
"2953, -388, -107.990",
"2953, -364, -101.411",
"2953, -340, -94.794",
"2953, -315, -88.140",
"2953, -291, -81.448",
"2953, -267, -74.719",
"2953, -243, -67.952",
"2953, -218, -61.148",
"2953, -194, -54.306",
"2953, -170, -47.427",
"2953, -146, -40.511",
"2953, -121, -33.557",
"2953, -97, -26.566",
"2953, -73, -19.537",
"2953, -48, -12.471",
"2953, -24, -5.367",
"2953, -5, 0.000",
"2953, 5, 3.497",
"2953, 24, 9.801",
"2953, 48, 17.718",
"2953, 73, 25.676",
"2953, 97, 33.674",
"2953, 121, 41.713",
"2953, 146, 49.793",
"2953, 170, 57.913",
"2953, 194, 66.074",
"2953, 218, 74.275",
"2953, 243, 82.517",
"2953, 267, 90.800",
"2953, 291, 99.123",
"2953, 315, 107.487",
"2953, 340, 115.892",
"2953, 364, 124.337",
"2953, 388, 132.823",
"2953, 412, 141.349",
"2953, 437, 149.916",
"2953, 461, 158.524",
"2953, 485, 167.172",
"3446, -485, -155.396",
"3446, -461, -147.951",
"3446, -437, -140.460",
"3446, -412, -132.921",
"3446, -388, -125.335",
"3446, -364, -117.703",
"3446, -340, -110.023",
"3446, -315, -102.297",
"3446, -291, -94.524",
"3446, -267, -86.703",
"3446, -243, -78.836",
"3446, -218, -70.922",
"3446, -194, -62.961",
"3446, -170, -54.953",
"3446, -146, -46.898",
"3446, -121, -38.797",
"3446, -97, -30.648",
"3446, -73, -22.452",
"3446, -48, -14.210",
"3446, -24, -5.921",
"3446, -5, 0.000",
"3446, 5, 4.454",
"3446, 24, 11.805",
"3446, 48, 21.040",
"3446, 73, 30.325",
"3446, 97, 39.661",
"3446, 121, 49.049",
"3446, 146, 58.487",
"3446, 170, 67.976",
"3446, 194, 77.516",
"3446, 218, 87.107",
"3446, 243, 96.749",
"3446, 267, 106.442",
"3446, 291, 116.185",
"3446, 315, 125.980",
"3446, 340, 135.826",
"3446, 364, 145.722",
"3446, 388, 155.669",
"3446, 412, 165.668",
"3446, 437, 175.717",
"3446, 461, 185.817",
"3446, 485, 195.968",
"3938, -485, -176.425",
"3938, -461, -167.994",
"3938, -437, -159.504",
"3938, -412, -150.956",
"3938, -388, -142.351",
"3938, -364, -133.687",
"3938, -340, -124.966",
"3938, -315, -116.187",
"3938, -291, -107.351",
"3938, -267, -98.456",
"3938, -243, -89.503",
"3938, -218, -80.493",
"3938, -194, -71.425",
"3938, -170, -62.299",
"3938, -146, -53.115",
"3938, -121, -43.873",
"3938, -97, -34.574",
"3938, -73, -25.217",
"3938, -48, -15.801",
"3938, -24, -6.328",
"3938, -5, 0.000",
"3938, 5, 5.568",
"3938, 24, 13.967",
"3938, 48, 24.521",
"3938, 73, 35.138",
"3938, 97, 45.818",
"3938, 121, 56.561",
"3938, 146, 67.366",
"3938, 170, 78.235",
"3938, 194, 89.166",
"3938, 218, 100.159",
"3938, 243, 111.216",
"3938, 267, 122.335",
"3938, 291, 133.517",
"3938, 315, 144.762",
"3938, 340, 156.070",
"3938, 364, 167.440",
"3938, 388, 178.873",
"3938, 412, 190.369",
"3938, 437, 201.927",
"3938, 461, 213.549",
"3938, 485, 225.233",
"4430, -485, -197.004",
"4430, -461, -187.614",
"4430, -437, -178.152",
"4430, -412, -168.621",
"4430, -388, -159.020",
"4430, -364, -149.348",
"4430, -340, -139.606",
"4430, -315, -129.794",
"4430, -291, -119.912",
"4430, -267, -109.959",
"4430, -243, -99.936",
"4430, -218, -89.844",
"4430, -194, -79.680",
"4430, -170, -69.447",
"4430, -146, -59.144",
"4430, -121, -48.770",
"4430, -97, -38.326",
"4430, -73, -27.812",
"4430, -48, -17.228",
"4430, -24, -6.573",
"4430, -5, 0.000",
"4430, 5, 6.859",
"4430, 24, 16.305",
"4430, 48, 28.182",
"4430, 73, 40.135",
"4430, 97, 52.164",
"4430, 121, 64.269",
"4430, 146, 76.450",
"4430, 170, 88.707",
"4430, 194, 101.041",
"4430, 218, 113.451",
"4430, 243, 125.937",
"4430, 267, 138.499",
"4430, 291, 151.138",
"4430, 315, 163.852",
"4430, 340, 176.643",
"4430, 364, 189.510",
"4430, 388, 202.453",
"4430, 412, 215.472",
"4430, 437, 228.567",
"4430, 461, 241.739",
"4430, 485, 254.986",
"4922, -485, -217.116",
"4922, -461, -206.794",
"4922, -437, -196.388",
"4922, -412, -185.899",
"4922, -388, -175.325",
"4922, -364, -164.667",
"4922, -340, -153.925",
"4922, -315, -143.099",
"4922, -291, -132.190",
"4922, -267, -121.196",
"4922, -243, -110.118",
"4922, -218, -98.956",
"4922, -194, -87.710",
"4922, -170, -76.381",
"4922, -146, -64.967",
"4922, -121, -53.469",
"4922, -97, -41.887",
"4922, -73, -30.221",
"4922, -48, -18.472",
"4922, -24, -6.638",
"4922, -5, 0.000",
"4922, 5, 8.344",
"4922, 24, 18.839",
"4922, 48, 32.040",
"4922, 73, 45.333",
"4922, 97, 58.716",
"4922, 121, 72.191",
"4922, 146, 85.757",
"4922, 170, 99.413",
"4922, 194, 113.161",
"4922, 218, 127.001",
"4922, 243, 140.931",
"4922, 267, 154.952",
"4922, 291, 169.065",
"4922, 315, 183.269",
"4922, 340, 197.564",
"4922, 364, 211.950",
"4922, 388, 226.427",
"4922, 412, 240.995",
"4922, 437, 255.655",
"4922, 461, 270.406",
"4922, 485, 285.247",
"5415, -485, -236.743",
"5415, -461, -225.518",
"5415, -437, -214.194",
"5415, -412, -202.771",
"5415, -388, -191.249",
"5415, -364, -179.627",
"5415, -340, -167.906",
"5415, -315, -156.086",
"5415, -291, -144.167",
"5415, -267, -132.149",
"5415, -243, -120.031",
"5415, -218, -107.814",
"5415, -194, -95.497",
"5415, -170, -83.082",
"5415, -146, -70.567",
"5415, -121, -57.953",
"5415, -97, -45.240",
"5415, -73, -32.428",
"5415, -48, -19.516",
"5415, -24, -6.505",
"5415, -5, 0.000",
"5415, 5, 10.043",
"5415, 24, 21.588",
"5415, 48, 36.116",
"5415, 73, 50.751",
"5415, 97, 65.495",
"5415, 121, 80.346",
"5415, 146, 95.305",
"5415, 170, 110.371",
"5415, 194, 125.545",
"5415, 218, 140.827",
"5415, 243, 156.217",
"5415, 267, 171.714",
"5415, 291, 187.319",
"5415, 315, 203.031",
"5415, 340, 218.852",
"5415, 364, 234.779",
"5415, 388, 250.815",
"5415, 412, 266.958",
"5415, 437, 283.209",
"5415, 461, 299.568",
"5415, 485, 316.034",
"5907, -485, -255.867",
"5907, -461, -243.768",
"5907, -437, -231.553",
"5907, -412, -219.222",
"5907, -388, -206.774",
"5907, -364, -194.211",
"5907, -340, -181.532",
"5907, -315, -168.738",
"5907, -291, -155.827",
"5907, -267, -142.800",
"5907, -243, -129.657",
"5907, -218, -116.399",
"5907, -194, -103.024",
"5907, -170, -89.534",
"5907, -146, -75.928",
"5907, -121, -62.205",
"5907, -97, -48.367",
"5907, -73, -34.413",
"5907, -48, -20.343",
"5907, -24, -6.157",
"5907, -5, 0.000",
"5907, 5, 11.974",
"5907, 24, 24.569",
"5907, 48, 40.426",
"5907, 73, 56.409",
"5907, 97, 72.518",
"5907, 121, 88.753",
"5907, 146, 105.113",
"5907, 170, 121.599",
"5907, 194, 138.211",
"5907, 218, 154.949",
"5907, 243, 171.813",
"5907, 267, 188.802",
"5907, 291, 205.917",
"5907, 315, 223.158",
"5907, 340, 240.525",
"5907, 364, 258.017",
"5907, 388, 275.635",
"5907, 412, 293.379",
"5907, 437, 311.249",
"5907, 461, 329.245",
"5907, 485, 347.366",
"6399, -485, -274.473",
"6399, -461, -261.527",
"6399, -437, -248.447",
"6399, -412, -235.232",
"6399, -388, -221.884",
"6399, -364, -208.402",
"6399, -340, -194.786",
"6399, -315, -181.036",
"6399, -291, -167.151",
"6399, -267, -153.133",
"6399, -243, -138.981",
"6399, -218, -124.694",
"6399, -194, -110.274",
"6399, -170, -95.719",
"6399, -146, -81.031",
"6399, -121, -66.208",
"6399, -97, -51.252",
"6399, -73, -36.161",
"6399, -48, -20.936",
"6399, -24, -5.577",
"6399, -5, 0.000",
"6399, 5, 14.156",
"6399, 24, 27.802",
"6399, 48, 44.991",
"6399, 73, 62.325",
"6399, 97, 79.805",
"6399, 121, 97.430",
"6399, 146, 115.201",
"6399, 170, 133.117",
"6399, 194, 151.179",
"6399, 218, 169.386",
"6399, 243, 187.738",
"6399, 267, 206.236",
"6399, 291, 224.879",
"6399, 315, 243.668",
"6399, 340, 262.602",
"6399, 364, 281.682",
"6399, 388, 300.907",
"6399, 412, 320.277",
"6399, 437, 339.793",
"6399, 461, 359.455",
"6399, 485, 379.261",
"6891, -485, -292.541",
"6891, -461, -278.777",
"6891, -437, -264.858",
"6891, -412, -250.787",
"6891, -388, -236.561",
"6891, -364, -222.182",
"6891, -340, -207.650",
"6891, -315, -192.963",
"6891, -291, -178.124",
"6891, -267, -163.130",
"6891, -243, -147.983",
"6891, -218, -132.683",
"6891, -194, -117.228",
"6891, -170, -101.621",
"6891, -146, -85.859",
"6891, -121, -69.944",
"6891, -97, -53.876",
"6891, -73, -37.653",
"6891, -48, -21.278",
"6891, -24, -4.748",
"6891, -5, 0.000",
"6891, 5, 16.608",
"6891, 24, 31.306",
"6891, 48, 49.829",
"6891, 73, 68.518",
"6891, 97, 87.374",
"6891, 121, 106.397",
"6891, 146, 125.587",
"6891, 170, 144.943",
"6891, 194, 164.466",
"6891, 218, 184.155",
"6891, 243, 204.011",
"6891, 267, 224.034",
"6891, 291, 244.223",
"6891, 315, 264.580",
"6891, 340, 285.102",
"6891, 364, 305.792",
"6891, 388, 326.648",
"6891, 412, 347.671",
"6891, 437, 368.860",
"6891, 461, 390.216",
"6891, 485, 411.739",
"7383, -485, -310.056",
"7383, -461, -295.501",
"7383, -437, -280.771",
"7383, -412, -265.867",
"7383, -388, -250.788",
"7383, -364, -235.535",
"7383, -340, -220.106",
"7383, -315, -204.504",
"7383, -291, -188.726",
"7383, -267, -172.775",
"7383, -243, -156.648",
"7383, -218, -140.347",
"7383, -194, -123.871",
"7383, -170, -107.221",
"7383, -146, -90.396",
"7383, -121, -73.397",
"7383, -97, -56.222",
"7383, -73, -38.874",
"7383, -48, -21.350",
"7383, -24, -3.653",
"7383, -5, 0.000",
"7383, 5, 19.348",
"7383, 24, 35.099",
"7383, 48, 54.958",
"7383, 73, 75.007",
"7383, 97, 95.245",
"7383, 121, 115.672",
"7383, 146, 136.289",
"7383, 170, 157.095",
"7383, 194, 178.091",
"7383, 218, 199.276",
"7383, 243, 220.651",
"7383, 267, 242.215",
"7383, 291, 263.969",
"7383, 315, 285.912",
"7383, 340, 308.044",
"7383, 364, 330.366",
"7383, 388, 352.878",
"7383, 412, 375.578",
"7383, 437, 398.469",
"7383, 461, 421.549",
"7383, 485, 444.818",
"7876, -485, -327.000",
"7876, -461, -311.682",
"7876, -437, -296.167",
"7876, -412, -280.456",
"7876, -388, -264.547",
"7876, -364, -248.442",
"7876, -340, -232.139",
"7876, -315, -215.639",
"7876, -291, -198.942",
"7876, -267, -182.048",
"7876, -243, -164.958",
"7876, -218, -147.670",
"7876, -194, -130.185",
"7876, -170, -112.503",
"7876, -146, -94.624",
"7876, -121, -76.548",
"7876, -97, -58.274",
"7876, -73, -39.804",
"7876, -48, -21.137",
"7876, -24, -2.273",
"7876, -5, 0.000",
"7876, 5, 22.396",
"7876, 24, 39.201",
"7876, 48, 60.398",
"7876, 73, 81.810",
"7876, 97, 103.435",
"7876, 121, 125.274",
"7876, 146, 147.327",
"7876, 170, 169.593",
"7876, 194, 192.074",
"7876, 218, 214.768",
"7876, 243, 237.676",
"7876, 267, 260.798",
"7876, 291, 284.134",
"7876, 315, 307.683",
"7876, 340, 331.447",
"7876, 364, 355.424",
"7876, 388, 379.615",
"7876, 412, 404.019",
"7876, 437, 428.638",
"7876, 461, 453.470",
"7876, 485, 478.516",
"8368, -485, -343.355",
"8368, -461, -327.303",
"8368, -437, -311.030",
"8368, -412, -294.536",
"8368, -388, -277.822",
"8368, -364, -260.886",
"8368, -340, -243.730",
"8368, -315, -226.352",
"8368, -291, -208.754",
"8368, -267, -190.935",
"8368, -243, -172.895",
"8368, -218, -154.634",
"8368, -194, -136.152",
"8368, -170, -117.449",
"8368, -146, -98.525",
"8368, -121, -79.380",
"8368, -97, -60.014",
"8368, -73, -40.428",
"8368, -48, -20.620",
"8368, -24, -0.592",
"8368, -5, 0.000",
"8368, 5, 25.770",
"8368, 24, 43.629",
"8368, 48, 66.167",
"8368, 73, 88.946",
"8368, 97, 111.964",
"8368, 121, 135.221",
"8368, 146, 158.719",
"8368, 170, 182.456",
"8368, 194, 206.433",
"8368, 218, 230.649",
"8368, 243, 255.106",
"8368, 267, 279.802",
"8368, 291, 304.738",
"8368, 315, 329.913",
"8368, 340, 355.328",
"8368, 364, 380.983",
"8368, 388, 406.878",
"8368, 412, 433.012",
"8368, 437, 459.386",
"8368, 461, 486.000",
"8368, 485, 512.853",
"8860, -485, -359.104",
"8860, -461, -342.346",
"8860, -437, -325.341",
"8860, -412, -308.091",
"8860, -388, -290.594",
"8860, -364, -272.851",
"8860, -340, -254.862",
"8860, -315, -236.626",
"8860, -291, -218.144",
"8860, -267, -199.416",
"8860, -243, -180.442",
"8860, -218, -161.221",
"8860, -194, -141.755",
"8860, -170, -122.042",
"8860, -146, -102.082",
"8860, -121, -81.877",
"8860, -97, -61.425",
"8860, -73, -40.727",
"8860, -48, -19.783",
"8860, -24, 0.000",
"8860, -5, 0.000",
"8860, 5, 29.489",
"8860, 24, 48.402",
"8860, 48, 72.284",
"8860, 73, 96.433",
"8860, 97, 120.849",
"8860, 121, 145.533",
"8860, 146, 170.484",
"8860, 170, 195.701",
"8860, 194, 221.186",
"8860, 218, 246.939",
"8860, 243, 272.958",
"8860, 267, 299.245",
"8860, 291, 325.798",
"8860, 315, 352.619",
"8860, 340, 379.708",
"8860, 364, 407.063",
"8860, 388, 434.685",
"8860, 412, 462.575",
"8860, 437, 490.732",
"8860, 461, 519.156",
"8860, 485, 547.847",
"9352, -485, -374.230",
"9352, -461, -356.794",
"9352, -437, -339.084",
"9352, -412, -321.102",
"9352, -388, -302.847",
"9352, -364, -284.319",
"9352, -340, -265.517",
"9352, -315, -246.443",
"9352, -291, -227.096",
"9352, -267, -207.475",
"9352, -243, -187.582",
"9352, -218, -167.416",
"9352, -194, -146.976",
"9352, -170, -126.264",
"9352, -146, -105.279",
"9352, -121, -84.021",
"9352, -97, -62.489",
"9352, -73, -40.685",
"9352, -48, -18.608",
"9352, -24, 0.000",
"9352, -5, 0.000",
"9352, 5, 33.571",
"9352, 24, 53.540",
"9352, 48, 78.768",
"9352, 73, 104.291",
"9352, 97, 130.111",
"9352, 121, 156.228",
"9352, 146, 182.640",
"9352, 170, 209.349",
"9352, 194, 236.354",
"9352, 218, 263.655",
"9352, 243, 291.252",
"9352, 267, 319.146",
"9352, 291, 347.335",
"9352, 315, 375.821",
"9352, 340, 404.604",
"9352, 364, 433.682",
"9352, 388, 463.057",
"9352, 412, 492.728",
"9352, 437, 522.695",
"9352, 461, 552.958",
"9352, 485, 583.518",
"9845, -485, -388.716",
"9845, -461, -370.630",
"9845, -437, -352.242",
"9845, -412, -333.553",
"9845, -388, -314.563",
"9845, -364, -295.272",
"9845, -340, -275.680",
"9845, -315, -255.786",
"9845, -291, -235.591",
"9845, -267, -215.095",
"9845, -243, -194.298",
"9845, -218, -173.200",
"9845, -194, -151.800",
"9845, -170, -130.099",
"9845, -146, -108.097",
"9845, -121, -85.794",
"9845, -97, -63.190",
"9845, -73, -40.284",
"9845, -48, -17.077",
"9845, -24, 0.000",
"9845, -5, 0.000",
"9845, 5, 38.036",
"9845, 24, 59.061",
"9845, 48, 85.637",
"9845, 73, 112.539",
"9845, 97, 139.768",
"9845, 121, 167.324",
"9845, 146, 195.207",
"9845, 170, 223.417",
"9845, 194, 251.953",
"9845, 218, 280.816",
"9845, 243, 310.007",
"9845, 267, 339.523",
"9845, 291, 369.367",
"9845, 315, 399.538",
"9845, 340, 430.035",
"9845, 364, 460.859",
"9845, 388, 492.010",
"9845, 412, 523.488",
"9845, 437, 555.293",
"9845, 461, 587.424",
"9845, 485, 619.883"
				#endregion data
				).ApplyFactor(ElectricMotorMapReader.Fields.PowerElectrical, 1E3); //Convert from kW to W
		}

		private EfficiencyMap GetEfficiencyMap(int count)
		{
			return ElectricMotorMapReader.Create(GetEfficiencyMapData(), count, ExecutionMode.Declaration);
		}
		private void SetVelocityDropLookupData(PEVAMTShiftStrategy shiftStrategy)
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
}

        