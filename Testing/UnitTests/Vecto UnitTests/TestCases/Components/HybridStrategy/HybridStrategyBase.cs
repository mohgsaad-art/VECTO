using Moq;
using TUGraz.Vecto.UnitTests.TestCases.Utils.ExtensionMethods;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
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
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

public partial class HybridStrategyBase
{
	protected Second _dt = 1.SI<Second>();
	protected Second _t = 0.SI<Second>();


	// const string EngineFldMapHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm]";
		// private static readonly string[] EngineFldMapData = new[] {
		// 	"600,1188,-138",
		// 	"608,1206.92,-138.2",
		// 	"616,1225.84,-138.4",
		// 	"624,1244.76,-138.6",
		// 	"632,1263.68,-138.8",
		// 	"640,1282.6,-139",
		// 	"648,1301.52,-139.2",
		// 	"656,1320.44,-139.4",
		// 	"664,1339.36,-139.6",
		// 	"672,1358.28,-139.8",
		// 	"680,1377.2,-140",
		// 	"688,1396.12,-140.2",
		// 	"696,1415.04,-140.4",
		// 	"704,1433.96,-140.6",
		// 	"712,1452.88,-140.8",
		// 	"720,1471.8,-141",
		// 	"728,1490.72,-141.2",
		// 	"736,1509.64,-141.4",
		// 	"744,1528.56,-141.6",
		// 	"752,1547.48,-141.8",
		// 	"760,1566.4,-142",
		// 	"768,1585.32,-142.2",
		// 	"776,1604.24,-142.4",
		// 	"784,1623.16,-142.6",
		// 	"792,1642.08,-142.8",
		// 	"800,1661,-143",
		// 	"808,1679.92,-143.36",
		// 	"816,1698.84,-143.72",
		// 	"824,1717.76,-144.08",
		// 	"832,1736.68,-144.44",
		// 	"840,1755.6,-144.8",
		// 	"848,1774.52,-145.16",
		// 	"856,1793.44,-145.52",
		// 	"864,1812.36,-145.88",
		// 	"872,1831.28,-146.24",
		// 	"880,1850.2,-146.6",
		// 	"888,1869.12,-146.96",
		// 	"896,1888.04,-147.32",
		// 	"904,1906.96,-147.68",
		// 	"912,1925.88,-148.04",
		// 	"920,1944.8,-148.4",
		// 	"928,1963.72,-148.76",
		// 	"936,1982.64,-149.12",
		// 	"944,2001.56,-149.48",
		// 	"952,2020.48,-149.84",
		// 	"960,2039.4,-150.2",
		// 	"968,2058.32,-150.56",
		// 	"976,2077.24,-150.92",
		// 	"984,2096.16,-151.28",
		// 	"992,2115.08,-151.64",
		// 	"1000,2134,-152",
		// 	"1008,2134,-152.52",
		// 	"1016,2134,-153.04",
		// 	"1024,2134,-153.56",
		// 	"1032,2134,-154.08",
		// 	"1040,2134,-154.6",
		// 	"1048,2134,-155.12",
		// 	"1056,2134,-155.64",
		// 	"1064,2134,-156.16",
		// 	"1072,2134,-156.68",
		// 	"1080,2134,-157.2",
		// 	"1088,2134,-157.72",
		// 	"1096,2134,-158.24",
		// 	"1104,2134,-158.76",
		// 	"1112,2134,-159.28",
		// 	"1120,2134,-159.8",
		// 	"1128,2134,-160.32",
		// 	"1136,2134,-160.84",
		// 	"1144,2134,-161.36",
		// 	"1152,2134,-161.88",
		// 	"1160,2134,-162.4",
		// 	"1168,2134,-162.92",
		// 	"1176,2134,-163.44",
		// 	"1184,2134,-163.96",
		// 	"1192,2134,-164.48",
		// 	"1200,2134,-165",
		// 	"1208,2134,-165.88",
		// 	"1216,2134,-166.76",
		// 	"1224,2134,-167.64",
		// 	"1232,2134,-168.52",
		// 	"1240,2134,-169.4",
		// 	"1248,2134,-170.28",
		// 	"1256,2134,-171.16",
		// 	"1264,2134,-172.04",
		// 	"1272,2134,-172.92",
		// 	"1280,2134,-173.8",
		// 	"1288,2134,-174.68",
		// 	"1296,2134,-175.56",
		// 	"1304,2134,-176.44",
		// 	"1312,2134,-177.32",
		// 	"1320,2134,-178.2",
		// 	"1328,2134,-179.08",
		// 	"1336,2134,-179.96",
		// 	"1344,2134,-180.84",
		// 	"1352,2134,-181.72",
		// 	"1360,2134,-182.6",
		// 	"1368,2134,-183.48",
		// 	"1376,2134,-184.36",
		// 	"1384,2134,-185.24",
		// 	"1392,2134,-186.12",
		// 	"1400,2134,-187",
		// 	"1408,2125.76,-188.2",
		// 	"1416,2117.52,-189.4",
		// 	"1424,2109.28,-190.6",
		// 	"1432,2101.04,-191.8",
		// 	"1440,2092.8,-193",
		// 	"1448,2084.56,-194.2",
		// 	"1456,2076.32,-195.4",
		// 	"1464,2068.08,-196.6",
		// 	"1472,2059.84,-197.8",
		// 	"1480,2051.6,-199",
		// 	"1488,2043.36,-200.2",
		// 	"1496,2035.12,-201.3911",
		// 	"1504,2026.88,-202.5793",
		// 	"1512,2018.64,-203.7675",
		// 	"1520,2010.4,-204.9557",
		// 	"1528,2002.16,-206.1439",
		// 	"1536,1993.92,-207.3321",
		// 	"1544,1985.68,-208.5203",
		// 	"1552,1977.44,-209.7085",
		// 	"1560,1969.2,-210.8967",
		// 	"1568,1960.96,-212.0849",
		// 	"1576,1952.72,-213.2731",
		// 	"1584,1944.48,-214.4613",
		// 	"1592,1936.24,-215.6495",
		// 	"1600,1928,-216.8377",
		// 	"1608,1919.76,-218.0259",
		// 	"1616,1911.52,-219.16",
		// 	"1624,1903.28,-220.24",
		// 	"1632,1895.04,-221.32",
		// 	"1640,1886.8,-222.4",
		// 	"1648,1878.56,-223.48",
		// 	"1656,1870.32,-224.56",
		// 	"1664,1862.08,-225.64",
		// 	"1672,1853.84,-226.72",
		// 	"1680,1845.6,-227.8",
		// 	"1688,1837.36,-228.88",
		// 	"1696,1829.12,-229.96",
		// 	"1704,1820.88,-231.04",
		// 	"1712,1812.64,-232.12",
		// 	"1720,1804.4,-233.2",
		// 	"1728,1796.16,-234.28",
		// 	"1736,1787.92,-235.36",
		// 	"1744,1779.68,-236.44",
		// 	"1752,1771.44,-237.52",
		// 	"1760,1763.2,-238.6",
		// 	"1768,1754.96,-239.68",
		// 	"1776,1746.72,-240.76",
		// 	"1784,1738.48,-241.84",
		// 	"1792,1730.24,-242.92",
		// 	"1800,1722,-244",
		// 	"1808,1703.24,-245.36",
		// 	"1816,1684.48,-246.72",
		// 	"1824,1665.72,-248.08",
		// 	"1832,1646.96,-249.44",
		// 	"1840,1628.2,-250.8",
		// 	"1848,1609.44,-252.16",
		// 	"1856,1590.68,-253.52",
		// 	"1864,1571.92,-254.88",
		// 	"1872,1553.16,-256.24",
		// 	"1880,1534.4,-257.6",
		// 	"1888,1515.64,-258.96",
		// 	"1896,1496.88,-260.32",
		// 	"1904,1478.12,-261.68",
		// 	"1912,1459.36,-263.04",
		// 	"1920,1440.6,-264.4",
		// 	"1928,1421.84,-265.76",
		// 	"1936,1403.08,-267.12",
		// 	"1944,1384.32,-268.48",
		// 	"1952,1365.56,-269.84",
		// 	"1960,1346.8,-271.2",
		// 	"1968,1328.04,-272.56",
		// 	"1976,1309.28,-273.92",
		// 	"1984,1290.52,-275.28",
		// 	"1992,1271.76,-276.64",
		// 	"2000,1253,-278",
		// 	"2008,1234.28,-279.36",
		// 	"2016,1215.56,-280.72",
		// 	"2024,1196.84,-282.08",
		// 	"2032,1178.12,-283.44",
		// 	"2040,1159.4,-284.8",
		// 	"2048,1140.68,-286.16",
		// 	"2056,1121.96,-287.52",
		// 	"2064,1103.24,-288.88",
		// 	"2072,1084.52,-290.24",
		// 	"2080,1065.8,-291.6",
		// 	"2088,1047.08,-292.96",
		// 	"2096,1028.36,-294.32",
		// 	"2104,978.24,-295.68",
		// 	"2112,896.72,-297.04",
		// 	"2120,815.2,-298.4",
		// 	"2128,733.68,-299.76",
		// 	"2136,652.16,-301.12",
		// 	"2144,570.64,-302.48",
		// 	"2152,489.12,-303.84",
		// 	"2160,407.6,-305.2",
		// 	"2168,326.08,-306.56",
		// 	"2176,244.56,-307.92",
		// 	"2184,163.04,-309.28",
		// 	"2192,81.52,-310.64",
		// 	"2200,0,-312"
		// };

		private record TnPort
	{
		public NewtonMeter T
		{
			get;
		}

		public PerSecond n
		{
			get;
		}
		public TnPort(NewtonMeter torque, PerSecond velocity)
		{
			T = torque;
			n = velocity;
		}

		/// <summary>
		/// Translates with a ratio, power remains constant
		/// From Out to In
		/// </summary>
		/// <param name="ratio"></param>
		/// <returns>TnComponent on the input side</returns>
		public TnPort Apply(double ratio)
		{
			return new TnPort(T / ratio, n * ratio);
		}
		
		
	}

	protected void SetVehicleSpeed(IVehicleContainer container, MeterPerSecond vehicleSpeed)
	{
		var mock = Mock.Get(container);
		mock.Setup(m => m.VehicleInfo.VehicleSpeed).Returns(vehicleSpeed);
	}
	
	protected void SetDrivingCycleTargetSpeed(IVehicleContainer container, MeterPerSecond targetSpeed)
	{
		var mock = Mock.Get(container);
		mock.Setup(m => m.DrivingCycleInfo.TargetSpeed).Returns(targetSpeed);
	}

	protected void SetBrakePower(IVehicleContainer container, Watt brakePower)
	{
		var mock = Mock.Get(container);
		mock.Setup(m => m.Brakes.BrakePower).Returns(brakePower);
	}
	
	

	protected Mock<IVehicleContainer> GetVehicleContainer(VectoRunData runData)
	{ 
		var vehicleContainer = new Mock<IVehicleContainer>(MockBehavior.Strict).WithPowertrainDict();

		var simplePtBuilder= GetSimplePowertrainBuilder(runData);
		vehicleContainer.Setup(s => s.SimplePowertrainBuilder).Returns((ISimplePowertrainBuilder)simplePtBuilder.Object);
		var preprocessors = new List<ISimulationPreprocessor>();
		vehicleContainer.Setup(s => s.AddPreprocessor(It.IsAny<ISimulationPreprocessor>()))
			.Callback((ISimulationPreprocessor preprocessor) => preprocessors.Add(preprocessor));
		

		
		//RunData
		vehicleContainer.Setup(v => v.RunData).Returns(runData);
		
		//VehicleInfo
		var speedKmh = 15.KMPHtoMeterPerSecond();
		vehicleContainer.Setup(v => v.VehicleInfo.VehicleSpeed).Returns(speedKmh);
		vehicleContainer.Setup(v => v.VehicleInfo.VehicleStopped)
			.Returns(() => !vehicleContainer.Object.VehicleInfo.VehicleSpeed.IsGreater(0));
		
		//GearboxInfo
		var gbxInfo = GetGearboxInfo(runData).Object;
		vehicleContainer.Setup(v => v.GearboxesInfo).Returns([gbxInfo]);

		//DriverInfo
		vehicleContainer
			.Setup(s => s.DriverInfo.PCCState)
			.Returns(PCCStates.OutsideSegment);
		vehicleContainer
			.Setup(v => v.DriverInfo.NextBrakeTriggerSpeed)
			.Returns(() => null);
		
		//Shiftstrategy
		var controller = new Mock<IHybridController>(MockBehavior.Strict);
		vehicleContainer.Setup(s => s.HybridControllerInfo).Returns(controller.Object);
		
		var shiftStrategy = new Mock<IShiftStrategy>(MockBehavior.Strict);
		controller.Setup(c => c.ShiftStrategy).Returns(shiftStrategy.Object);
		
		
		//PowertrainInfo
		// vehicleContainer
		// 	.Setup(v => v.ElectricMotor)
		// 	.Returns(runData.ElectricMachinesData.Select(t => t.Item).Distinct().ToList());

		//EngineInfo
		vehicleContainer
			.Setup(v => v.EngineInfo.EngineOn)
			.Returns(true);
		vehicleContainer
			.Setup(v => v.EngineInfo.EngineN95hSpeed)
			.Returns(() => {
					return vehicleContainer.Object.RunData.EngineData.FullLoadCurves.First().Value.N95hSpeed;
				}
			);
		
		//CycleInfo
		vehicleContainer
			.Setup(v => v.DrivingCycleInfo.RoadGradient).Returns(0.SI<Radian>());
		
		//ClutchInfo
		var clutchInfo = new Mock<IClutchInfo>(MockBehavior.Strict).Object;
		vehicleContainer
			.Setup(v => v.ClutchesInfo).Returns([clutchInfo]);
		vehicleContainer.Setup(v => v.ClutchesInfo)
			.Returns([clutchInfo]);
		//Brakes
		vehicleContainer
			.Setup(v => v.Brakes).Returns(new Mock<IBrakes>(MockBehavior.Strict).Object);

		//ElectricMotorInfo
		var emInfo = GetElectricMotorInfo(runData).Object;
		vehicleContainer
			.Setup(v => v.ElectricMotorInfo(runData.ElectricMachinesSinglePwt.Single().Item1, It.IsAny<AxleNumber>())).Returns((IElectricMotorInfo)emInfo);
		vehicleContainer.Setup(v => v.ElectricMotorsInfo).Returns([emInfo]);
		
		SetBrakePower(vehicleContainer.Object, 0.SI<Watt>());
		return vehicleContainer;
	}

	private Mock<IElectricMotorInfo> GetElectricMotorInfo(VectoRunData runData)
	{
		var em = new Mock<IElectricMotorInfo>(MockBehavior.Strict);
		em.Setup(e => e.Position)
			.Returns(runData.ElectricMachinesSinglePwt.Single().Item1);

		return em;
	}

	private Mock<IGearboxInfo> GetGearboxInfo(VectoRunData runData)
	{
		var gbx = new Mock<IGearboxInfo>(MockBehavior.Strict).WithDefaultAxleNumber();
		
		gbx.Setup(g => g.GearEngaged(It.IsAny<Second>()))
			.Returns(true);
		gbx.Setup(g => g.GearboxType)
			.Returns(() => runData.GearboxSinglePwt.Type);
		gbx.Setup(g => g.TractionInterruption)
			.Returns(() => runData.GearboxSinglePwt.TractionInterruption);


		gbx.Setup(g => g.LastDownshift)
			.Returns(double.MinValue.SI<Second>());
		gbx.Setup(g => g.LastUpshift)
			.Returns(double.MinValue.SI<Second>());
		gbx.Setup(g => g.LastShift)
			.Returns(() => {
				//Use function in case last up/downshift is overridden in testcase
				var gbxObject = gbx.Object;
				return VectoMath.Max(gbxObject.LastDownshift, gbxObject.LastUpshift);
			});


		var currentGear = new GearshiftPosition(
			1, 
			runData.GearboxSinglePwt.Type.AutomaticTransmission() ? true : null);
		gbx.Setup(g => g.Gear)
			.Returns(currentGear);
		
		return gbx;
	}

	private Mock<ISimplePowertrainBuilder> GetSimplePowertrainBuilder(VectoRunData runData)
	{
		var testPowertrain = new Mock<ITestPowertrain>(MockBehavior.Strict);
		var simplePowertrain = new Mock<ISimplePowertrainContainer>(MockBehavior.Strict);
		var simpleVehicleContainer = new Mock<ISimpleVehicleContainer>(MockBehavior.Strict);
		
		simpleVehicleContainer.Setup(v => v.SimplePowertrains[It.IsAny<AxleNumber>()]).Returns(simplePowertrain.Object);
		
		var vehicle = new Mock<ITestPowertrainVehicle>(MockBehavior.Strict);
		vehicle.Setup(sv => sv.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>()))
			.Returns((MeterPerSecond vehSpeed, Radian gradient) => {
				//Vehicle --> [Out] Wheels [In] -> [Out] AxleGear [In] -> [Out] Gearbox [In] -->  Engine
				var testGbx = testPowertrain.Object.Gearboxes.Single();
				//Wheels
				var wheelsInAngularVelocity = vehSpeed / runData.VehicleData.DynamicTyreRadius;
				//AxleGear
				var axleGearInAngularVelocity = wheelsInAngularVelocity * runData.AxleGearSinglePwt.AxleGear.Ratio;
				//Gearbox
				var gbxResponse = testGbx.Request(_t, _dt, 0.SI<NewtonMeter>(), axleGearInAngularVelocity, true);

				return new ResponseSuccess(testPowertrain) {
				
				};
			});


		simplePowertrain.Setup(v => v.Gearbox).Returns(() => testPowertrain.Object.Gearboxes.Single());

		testPowertrain
			.Setup(testPt => testPt.Container)
			.Returns(simpleVehicleContainer.Object);

		testPowertrain
			.Setup(tp => tp.Vehicle)
			.Returns(vehicle.Object);
		
		testPowertrain
			.Setup(testPt => testPt.UpdateComponents())
			.Callback(() => {});

		testPowertrain
			.Setup(testPt => testPt.Gearboxes).Returns([(ITestPowertrainTransmission)GetTestPowertrainGearbox(runData.GearboxSinglePwt).Object]);
		
		testPowertrain
			.Setup(testPt => testPt.EngineAux)
			.Returns(new Mock<IAuxPort>().Object);
		//HybridController
		testPowertrain
			.Setup(testPt => testPt.HybridController).Returns((ISimpleHybridController)GetSimpleHybridController(testPowertrain.Object, runData).Object);


		testPowertrain
			.Setup(testPt => testPt.Vehicle.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>()))
			.Returns((MeterPerSecond vehSpeed, Radian gradient) => {
				//Vehicle --> [Out] Wheels [In] -> [Out] AxleGear [In] -> [Out] Gearbox [In] -->  Engine
				var testGbx = testPowertrain.Object.Gearboxes.Single();
				//Wheels
				var wheelsInAngularVelocity = vehSpeed / runData.VehicleData.DynamicTyreRadius;
				//AxleGear
				var axleGearInAngularVelocity = wheelsInAngularVelocity * runData.AxleGearSinglePwt.AxleGear.Ratio;
				//Gearbox
				var gbxResponse = testGbx.Request(_t, _dt, 0.SI<NewtonMeter>(), axleGearInAngularVelocity, true);



				return new ResponseSuccess(testPowertrain) {
				
				};
			});

		//ElectricMotors
		var testElectricMotorMock = GetTestElectricMotor(runData.ElectricMachinesSinglePwt);

		
		
		var testEm = testElectricMotorMock.Object;
		testPowertrain.Setup(t => t.ElectricMotors).Returns([(ITestpowertrainElectricMotor)testEm]);
		// testPowertrain.Setup(t => t.ElectricMotors).Returns(
		// 	new Dictionary<PowertrainPosition, ITestpowertrainElectricMotor>() {
		// 		{testElectricMotor.Position, testElectricMotor}
		// 	});
		testPowertrain.Setup(t => t.ElectricMotorsUpstreamTransmission)
			.Returns(
				() => {
					return testPowertrain.Object.ElectricMotors.Where(em => em.Position.IsOneOf(
						PowertrainPosition.HybridP1,
						PowertrainPosition.HybridP2,
						PowertrainPosition.IHPC,
						PowertrainPosition.HybridP2_5,
						PowertrainPosition.HybridP3)).Cast<IElectricMotor>().ToList();
				}
			);
		testPowertrain.Setup(t => t.GetElectricMotor(It.IsAny<AxleNumber>()))
			.Returns(() => testPowertrain.Object.ElectricMotors.Single());
		testPowertrain.Setup(t => t.CombustionEngine.UpdateFrom(It.IsAny<object>())).Returns(true);
		// testPowertrain.Setup(t => t.Gearbox.UpdateFrom(It.IsAny<object>())).Returns(true);
		testPowertrain.Setup(t => t.Clutch.UpdateFrom(It.IsAny<object>())).Returns(true);
		
		//Brakes
		testPowertrain.Setup(t => t.Brakes.UpdateFrom(It.IsAny<object>())).Returns(true);
		
		
		
		
		
		
		
		//Builder
		var simplePtBuilder = new Mock<ISimplePowertrainBuilder>(MockBehavior.Strict);
		simplePtBuilder.Setup(s => s.CreateTestPowertrain(It.IsAny<IVehicleContainer>(),
			It.IsAny<bool>())).Returns(testPowertrain.Object);
			
		return simplePtBuilder;
	}

	private Mock<ITestpowertrainElectricMotor> GetTestElectricMotor(IList<Tuple<PowertrainPosition, ElectricMotorData>> emsData)
	{
		var testEm = new Mock<ITestpowertrainElectricMotor>(MockBehavior.Strict).WithDefaultAxleNumber();
		testEm.Setup(em => em.UpdateFrom(It.IsAny<object>())).Returns(true);
		Assert.That(emsData, Has.Count.EqualTo(1));
		var pos = emsData.First().Item1;
		var emData = emsData.First().Item2;
		testEm.Setup(e => e.Position).Returns(pos);
		
		return testEm;
	}

	private Mock<ITestPowertrainTransmission> GetTestPowertrainGearbox(GearboxData gbxData)
	{
		var gbx = new Mock<ITestPowertrainTransmission>(MockBehavior.Strict).WithDefaultAxleNumber();
		gbx.Setup(g => g.GearboxType).Returns(gbxData.Type);
		gbx.Setup(g => g.UpdateFrom(It.IsAny<object>())).Returns(true);
		//Set by Mock ----
		GearshiftPosition currentGear = null; 
		GearshiftPosition nextGear = null;
		bool? disengaged = null;
		bool? disengageGearbox = null;
		// ----
		
		gbx.SetupSet(g => g.SetGear = It.IsAny<GearshiftPosition>())
			.Callback((GearshiftPosition pos) => currentGear = pos);
		gbx.SetupGet(g => g.Gear).Returns(() => currentGear);
		

		gbx.SetupSet(g => g.SetNextGear = It.IsAny<GearshiftPosition>())
			.Callback((GearshiftPosition pos) => nextGear = pos);
		
		gbx.SetupSet(g => g.SetDisengaged = It.IsAny<bool>())
			.Callback((bool value) => disengaged = value);

		gbx.SetupSet(g => g.SetDisengageGearbox = It.IsAny<bool>())
			.Callback((bool value) => disengaged = value);
		
		gbx.Setup(g => g.UpdateFrom(It.IsAny<object>())).Returns(true);

		gbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny <Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>(), It.IsAny<bool>()))
			.Returns((Second t, Second dt, NewtonMeter outTorque, PerSecond outSpeed, bool dryRun) => {
				
				var ratio = gbxData.Gears[gbx.Object.Gear.Gear].Ratio;
				var inTn = new TnPort(outTorque, outSpeed).Apply(ratio);
				
				var inSpeed = outSpeed * ratio;
				var inTorque = outTorque / ratio;
				//TODO
				Assert.AreEqual(inTn.n, inSpeed);
				Assert.AreEqual(inTn.T, inTorque);
				

				var response = new ResponseSuccess(gbx) {
					Gearbox = {
						InputSpeed = inSpeed,
						InputTorque = inTorque,
						OutputSpeed = outSpeed,
						OutputTorque = outTorque,
						Gear = gbx.Object.Gear,
						
					},
				};
				return response;
			});
		
		return gbx;
	}

	private Mock<ISimpleHybridController> GetSimpleHybridController(ITestPowertrain testPowertrain, VectoRunData runData)
	{
		var testGbx = testPowertrain.Gearboxes.Single();
		var controller = new Mock<ISimpleHybridController>(MockBehavior.Strict);
		HybridStrategyResponse settings = null;
		controller.Setup(c => c.ApplyStrategySettings(It.IsAny<HybridStrategyResponse>()))
			.Callback((HybridStrategyResponse value) => settings = value);
		

		controller.Setup(c => c.Initialize(It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>()))
			.Returns((NewtonMeter outTorque, PerSecond outSpeed) => {
				var response = new ResponseSuccess(controller) { };
				
				return response;
			});


		controller.Setup(c => c.NextComponent.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>(), true))
			.Returns((Second t, Second dt, NewtonMeter outTorque, PerSecond outSpeed, bool dryRun) => {
				// └Brakes  
				///      ├(Engine P4)  
				///      └AxleGear  
				///       ├(Engine P3)  
				///       ├(Angledrive)  
				///       ├(TransmissionOutputRetarder)  
				///       └Gearbox, ATGearbox, or APTNGearbox  
				///        ├(TransmissionInputRetarder)  
				///        ├(Engine P2.5)  
				///        ├(Engine P2)  
				///        ├(SwitchableClutch)  
				///        ├(Engine P1)  
				///        └StopStartCombustionEngine
				
				TnPort outBrakes = new (outTorque, outSpeed);
				//Eng4
				var inEng4 = outBrakes.Apply(1); //TMP
				var inAxleGear = inEng4.Apply(runData.AxleGearSinglePwt.AxleGear.Ratio);
				//Eng3
				var inEng3 = inAxleGear.Apply(1); //TMP

				var gbxResponse = testGbx.Request(t, dt, inEng3.T, inEng3.n, dryRun);
				TnPort gbxin = new (gbxResponse.Gearbox.InputTorque, gbxResponse.Gearbox.InputSpeed);
				
				//Engine P2.5
				//Engine P2
				
				//Clutch
				
				//Engine P1
				var iceOut = gbxin.Apply(1);
				var fld = runData.EngineData.FullLoadCurves.Single().Value;
				var stationaryFullLoadPower = fld.FullLoadStationaryPower(iceOut.n);
				var stationaryFullLoadTorque = fld.FullLoadStationaryTorque(iceOut.n);
				return new ResponseDryRun(
					controller, 
					gbxResponse) 
				{
					Engine = {
						TorqueOutDemand = iceOut.T,
						TotalTorqueDemand = iceOut.T,
						DragPower = fld.DragLoadStationaryPower(iceOut.n),
						EngineSpeed = iceOut.n,
						StationaryFullLoadTorque = stationaryFullLoadTorque,
						//No transient effect for now
						DynamicFullLoadTorque = stationaryFullLoadTorque,
						DynamicFullLoadPower = stationaryFullLoadPower,
					},
					Clutch = {
						OutputSpeed = gbxin.n
					},
				};
			});
		return controller;
	}
}