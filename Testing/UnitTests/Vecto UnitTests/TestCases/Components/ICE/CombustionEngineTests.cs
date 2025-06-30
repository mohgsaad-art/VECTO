using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.ICE;

public class CombustionEngineTests
{
	[TestCase]
	public void TestEngineHasOutPort()
	{
		var engineData = new CombustionEngineData() {
			IdleSpeed = 600.RPMtoRad(),
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{0, new EngineFullLoadCurve(new List<EngineFullLoadCurve.FullLoadCurveEntry>() {
					new() {
						EngineSpeed = 600.RPMtoRad(), 
						TorqueDrag = -20.SI<NewtonMeter>(),
						TorqueFullLoad = 800.SI<NewtonMeter>()
					},
					new() {
						EngineSpeed = 2000.RPMtoRad(),
						TorqueDrag = -20.SI<NewtonMeter>(),
						TorqueFullLoad = 800.SI<NewtonMeter>()
					}
                }, DeclarationData.PT1)}
			}
		};
		var vehicle = new Mock<IVehicleContainer>().Object;
        var engine = new CombustionEngine(vehicle, engineData);

		var port = engine.OutPort();
		Assert.IsNotNull(port);
	}

	[TestCase]
	public void TestOutPortRequestNotFailing()
	{
		var container = GetMockVehicleContainer();
		var inputData = GetMockEngineInputData();

		var dao = new EngineeringDataAdapter();
        var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);

		var port = engine.OutPort();

		var absTime = 0.SI<Second>();
		var dt = 1.SI<Second>();
		var torque = 400.SI<NewtonMeter>();
		var engineSpeed = 1500.RPMtoRad();

		port.Initialize(torque, engineSpeed);
		var response = port.Request(absTime, dt, torque, engineSpeed, false);

		Assert.IsInstanceOf<ResponseSuccess>(response);
		Assert.AreEqual(torque.Value(), response.Engine.TorqueOutDemand.Value());
		Assert.AreEqual(engineSpeed.AsRPM, response.Engine.EngineSpeed.AsRPM);
	}


    [TestCase("Test1Hz", 1000, 50, 50)]
	[TestCase("TestvarHz", 1000, 50, 50)]
	public void TestEngineFullLoadJump(string testName, double rpm, double initialIdleLoad,
            double finalIdleLoad)
    {
		var container = GetMockVehicleContainer();
		var inputData = GetMockEngineInputData();

		var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);

        var expectedResults = InputDataHelper.InputDataAsTableData(ExpectedEngineFldHdr, GetExpectedResults(testName));

        var requestPort = engine.OutPort();

		var modalData = new Mock<IModalDataContainer>().Object;

        var idlePower = initialIdleLoad.SI<Watt>();

        var angularSpeed = rpm.RPMtoRad();

        var t = 0.SI<Second>();
        var dt = 0.1.SI<Second>();
        requestPort.Initialize(Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
        for (; t < 2; t += dt) {
            requestPort.Request(t, dt, Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed, false);
            engine.CommitSimulationStep(t, dt, modalData);
        }

        var i = 0;
        var engineLoadPower = engineData.FullLoadCurves[0].FullLoadStationaryPower(angularSpeed);
        idlePower = finalIdleLoad.SI<Watt>();
        for (; t < 25; t += dt, i++) {
            dt = (expectedResults.Rows[i + 1].ParseDouble(0) - expectedResults.Rows[i].ParseDouble(0)).SI<Second>();
            if (t >= 10.SI<Second>()) {
                engineLoadPower = idlePower;
            }
            var response = requestPort.Request(t, dt, Formulas.PowerToTorque(engineLoadPower, angularSpeed), angularSpeed, false);
            engine.CommitSimulationStep(t, dt, modalData);
            Assert.AreEqual(expectedResults.Rows[i].ParseDouble(0), t.Value(), 0.001, "Time");
            Assert.AreEqual(expectedResults.Rows[i].ParseDouble(1), response.Engine.DynamicFullLoadPower.Value(), 0.1,
                $"Load in timestep {t}");
            modalData.CommitSimulationStep();
        }
    }

	private string[] GetExpectedResults(string testName)
	{
		switch (testName) {
			case "Test1Hz": return ExpectedEngineFldData_1Hz;
			case "TestvarHz": return ExpectedEngineFldData_varHz;
		}

		throw new Exception("Unknown Testcase");
	}

	private IVehicleContainer GetMockVehicleContainer()
	{
		var container = new Mock<IVehicleContainer>();
		var gbx = new Mock<IGearboxInfo>();

		gbx.Setup(g => g.Gear).Returns(new GearshiftPosition(0));

		container.Setup(c => c.GearboxInfo).Returns(gbx.Object);


		return container.Object;
	}

	private IEngineEngineeringInputData GetMockEngineInputData()
	{
		var ice = new Mock<IEngineEngineeringInputData>();
		var mode = new Mock<IEngineModeEngineeringInputData>();
		var fuel = new Mock<IEngineFuelEngineeringInputData>();

		ice.Setup(e => e.Displacement).Returns(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>());
		ice.Setup(e => e.Inertia).Returns(3.8.SI<KilogramSquareMeter>());
		ice.Setup(e => e.EngineModes).Returns(new List<IEngineModeEngineeringInputData>() { mode.Object });

		var decl = ice.As<IEngineDeclarationInputData>();
		decl.Setup(e => e.EngineModes).Returns(new List<IEngineModeDeclarationInputData>() { });

		mode.Setup(m => m.IdleSpeed).Returns(560.RPMtoRad());
		mode.Setup(m => m.Fuels).Returns(new List<IEngineFuelEngineeringInputData>() { fuel.Object });
		mode.Setup(m => m.FullLoadCurve).Returns(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));

		fuel.Setup(f => f.WHTCEngineering).Returns(1.0);
		fuel.Setup(f => f.FuelType).Returns(FuelType.DieselCI);
		fuel.Setup(f => f.FuelConsumptionMap).Returns(InputDataHelper.InputDataAsTableData(IceMapHdr, IceMapData));
		return ice.Object;
	}

	const string IceFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] IceFldData = new[] {
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

	const string IceMapHdr = "engine speed [1/min],torque [Nm],fuel consumption [g/h]";

	private static readonly string[] IceMapData = new[] {
		"560,-149,0",
		"560,0,1256",
		"560,200,3197",
		"560,400,5295",
		"560,600,7615",
		"560,800,9375",
		"560,1000,11239",
		"560,1180,12869",
		"600,-148,0",
		"600,0,1459",
		"600,200,3358",
		"600,400,5498",
		"600,600,8101",
		"600,800,10014",
		"600,1000,12071",
		"600,1200,14201",
		"600,1282,15304",
		"800,-149,0",
		"800,0,1879",
		"800,200,4286",
		"800,400,7021",
		"800,600,10059",
		"800,800,13086",
		"800,1000,16015",
		"800,1200,19239",
		"800,1400,22426",
		"800,1600,25483",
		"800,1791,28905",
		"1000,-160,0",
		"1000,0,2865",
		"1000,200,5963",
		"1000,400,9198",
		"1000,600,12354",
		"1000,800,15965",
		"1000,1000,19864",
		"1000,1200,23530",
		"1000,1400,27202",
		"1000,1600,31165",
		"1000,1800,35103",
		"1000,2000,39360",
		"1000,2200,44120",
		"1000,2300,46836",
		"1200,-179,0",
		"1200,0,3307",
		"1200,200,6897",
		"1200,400,10651",
		"1200,600,14645",
		"1200,800,19115",
		"1200,1000,23677",
		"1200,1200,28180",
		"1200,1400,32431",
		"1200,1600,36698",
		"1200,1800,41691",
		"1200,2000,46915",
		"1200,2200,51783",
		"1200,2300,54932",
		"1400,-203,0",
		"1400,0,4306",
		"1400,200,8143",
		"1400,400,12723",
		"1400,600,17523",
		"1400,800,22288",
		"1400,1000,27093",
		"1400,1200,32536",
		"1400,1400,37746",
		"1400,1600,43194",
		"1400,1800,49453",
		"1400,2000,55830",
		"1400,2200,61072",
		"1400,2300,64377",
		"1600,-235,0",
		"1600,0,5209",
		"1600,200,9669",
		"1600,400,14838",
		"1600,600,20127",
		"1600,800,25894",
		"1600,1000,31631",
		"1600,1200,37248",
		"1600,1400,42826",
		"1600,1600,49752",
		"1600,1800,57020",
		"1600,2000,63914",
		"1600,2079,66520",
		"1800,-264,0",
		"1800,0,6409",
		"1800,200,11777",
		"1800,400,17320",
		"1800,600,23394",
		"1800,800,30501",
		"1800,1000,36378",
		"1800,1200,43079",
		"1800,1400,49796",
		"1800,1600,57436",
		"1800,1800,65157",
		"1800,1857,67574",
		"2000,-301,0",
		"2000,0,9127",
		"2000,200,14822",
		"2000,400,20655",
		"2000,600,27076",
		"2000,800,34188",
		"2000,1000,42837",
		"2000,1200,51018",
		"2000,1352,56618",
		"2100,-320,0",
		"2100,0,10470",
		"2100,200,16332",
		"2100,400,22396",
		"2100,600,28914",
		"2100,800,35717",
		"2100,1000,45643",
		"2100,1100,50653",
		"500,-500,0",
		"3000,1500,0",
		"3000,-500,0",
	};

	const string ExpectedEngineFldHdr = "t,Pe_FullDyn";

	private static readonly string[] ExpectedEngineFldData_1Hz = new[] {
		"2.0,69198.81418",
		"3.0,208433.68870",
		"4.0,234731.75956",
		"5.0,239698.82355",
		"6.0,240637.0",
		"7.0,240814.175",
		"8.0,240847.6436",
		"9.0,240853.9648",
		"10.0,240855.1066",
		"11.0,195373.1647",
		"12.0,195373.1647",
		"13.0,195373.1647",
		"14.0,195373.1647",
		"15.0,195373.1647",
		"16.0,195373.1647",
		"17.0,195373.1647",
		"18.0,195373.1647",
		"19.0,195373.1647",
		"20.0,195373.1647",
		"21.0,195373.1647",
		"22.0,195373.1647",
		"23.0,195373.1647",
		"24.0,195373.1647",
		"25.0,195373.1647",
	};

	private static readonly string[] ExpectedEngineFldData_varHz = new[] {
		"2.00,69198.8142",
		"3.25,219481.7031",
		"3.65,229881.7960",
		"3.85,232992.4796",
		"4.55,238406.8866",
		"5.45,240309.0914",
		"6.15,240685.3031",
		"6.25,240711.4217",
		"6.45,240752.2455",
		"7.25,240828.2358",
		"7.95,240846.9663",
		"8.35,240851.0879",
		"8.65,240852.7990",
		"8.85,240853.5468",
		"9.65,240854.9386",
		"9.95,240855.1346",
		"10.00,240855.1588",
		"10.40,117221.8032",
		"11.20,177379.8128",
		"12.00,177379.8128",
		"12.20,69198.8142",
		"13.20,195373.1647",
		"13.40,69198.8142",
		"14.10,165867.8474",
		"14.20,69198.8142",
		"14.90,165867.8474",
		"15.00,69198.8142",
		"15.90,187124.4811",
		"16.10,69198.8142",
		"16.50,117221.8032",
		"16.70,69198.8142",
		"17.30,152268.0673",
		"18.20,187124.4811",
		"18.60,117221.8032",
		"19.00,117221.8032",
		"19.50,136201.8254",
		"19.60,69198.8142",
		"20.00,117221.8032",
		"20.90,187124.4811",
		"21.70,177379.8128",
		"21.80,69198.8142",
		"21.90,69198.8142",
		"22.60,165867.8474",
		"23.50,187124.4811",
		"24.20,165867.8474",
		"24.90,165867.8474",
		"25,0",
	};
}
