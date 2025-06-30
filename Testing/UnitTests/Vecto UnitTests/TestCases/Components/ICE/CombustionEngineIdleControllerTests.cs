using System.Data;
using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.ICE;

public class CombustionEngineIdleControllerTests
{

	protected double Tolerance = 1E-3;

    /*
		 * VECTO 2.2
		| time [s] | P_eng_out [kW] | n_eng_avg [1/min] | T_eng_fcmap [Nm] | Gear [-] |
		| 59.5     | 349.981     | 1679.281  | 1990.181    | 8        |
		| 60.5     | 5           | 1679.281  | 28.43269    | 0        |
		| 61.5     | -19.47213   | 1397.271  | -133.0774   | 0        |
		| 62.5     | -18.11888   | 1064.296  | -162.5699   | 0        |
		| 63.5     | -11.11163   | 714.1923  | -148.571    | 0        |
		| 64.5     | -0.5416708  | 560       | -9.236741   | 0        |
		| 65.5     | 5           | 560       | 85.26157    | 0        |
		| 66.5     | 5           | 560       | 85.26157    | 0        |
		| 67.5     | 5           | 560       | 85.26157    | 0        |
		| 68.5     | 5           | 560       | 85.26157    | 0        |
		| 69.5     | 5           | 560       | 85.26157    | 0        |
		| 70.5     | 308.729     | 1284.139  | 2295.815    | 9        |
				*/

    [TestCase]
    public void EngineIdleControllerTest()
	{
		var container = GetMockVehicleContainer();
		var inputData = GetEngineInputData(5.1471.SI<KilogramSquareMeter>());

        var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);
		var aux = new EngineAuxiliary(container);
		aux.AddConstant("CONST", 5000.SI<Watt>());
		engine.Connect(aux.Port());

		Mock.Get(container).Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>())).Callback(
			(Second absTime, Second dt) => {
				engine.CommitSimulationStep(absTime, dt, null);
				aux.CommitSimulationStep(absTime, dt, null);
			});

        var requestPort = engine.OutPort();

		var idleCtl = engine.IdleController;
		idleCtl.RequestPort = engine;

        var absTime = 0.SI<Second>();
        var dt = Constants.SimulationSettings.TargetTimeInterval;

        var angularVelocity = 1680.RPMtoRad();
        var torque = 345000.SI<Watt>() / angularVelocity;

        var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

        response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
        Assert.AreEqual(350000, response.Engine.PowerRequest.Value(), Tolerance);

		container.CommitSimulationStep(absTime, dt);
        absTime += dt;

        var engineSpeed = new[] {
                1439.5773, 1225.5363, 1026.6696, 834.1936, 641.1360, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560,
                560, 560
            };

        var enginePower = new[] {
                -37334.1588, -27198.2777, -20280.7550, -15216.7221, -11076.6656, -500.7991, 5000, 5000, 5000, 5000, 5000, 5000, 5000,
                5000, 5000, 5000, 5000, 5000, 5000, 5000
            };

        var engSpeedResults = new List<dynamic>();
        for (var i = 0; i < 20; i++) {
            torque = 0.SI<NewtonMeter>();

            response = (ResponseSuccess)idleCtl.Request(absTime, dt, torque, null, false);

			container.CommitSimulationStep(absTime, dt);

            engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
            Assert.AreEqual(engineSpeed[i], engine.PreviousState.EngineSpeed.AsRPM, Tolerance, $"entry {i}");
            Assert.AreEqual(enginePower[i], engine.PreviousState.EnginePower.Value(), Tolerance, $"entry {i}");
            absTime += dt;
        }
        //dataWriter.Finish();
    }

    [TestCase]
    public void EngineIdleControllerTest2()
    {
		var container = GetMockVehicleContainer();
		var inputData = GetEngineInputData(5.1471.SI<KilogramSquareMeter>());

		var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);
		var aux = new EngineAuxiliary(container);
		aux.AddConstant("CONST", 5000.SI<Watt>());
		engine.Connect(aux.Port());

		Mock.Get(container).Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>())).Callback(
			(Second absTime, Second dt) => {
				engine.CommitSimulationStep(absTime, dt, null);
				aux.CommitSimulationStep(absTime, dt, null);
			});

        var requestPort = engine.OutPort();

		var idleCtl = engine.IdleController;
		idleCtl.RequestPort = engine;

        var absTime = 0.SI<Second>();
        var dt = Constants.SimulationSettings.TargetTimeInterval;

        var angularVelocity = 95.5596.SI<PerSecond>();

        var torque = (engine.ModelData.FullLoadCurves[0].DragLoadStationaryPower(angularVelocity) - 5000.SI<Watt>()) /
                    angularVelocity;

        var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

        response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
        Assert.AreEqual(-14829.79713, response.Engine.PowerRequest.Value(), Tolerance);
		container.CommitSimulationStep(absTime, dt);
        absTime += dt;

        var engineSpeed = new[] {
                1680.RPMtoRad(), 1680.RPMtoRad(), 1467.014.RPMtoRad(), 1272.8658.RPMtoRad(), 1090.989.RPMtoRad(),
                915.3533.RPMtoRad(), 738.599.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
                560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
                560.RPMtoRad()
            };

		var engSpeedResults = new List<dynamic>();
        torque = 0.SI<NewtonMeter>();
        for (var i = 0; i < engineSpeed.Length; i++) {
            response = (ResponseSuccess)idleCtl.Request(absTime, dt, torque, null, false);

			container.CommitSimulationStep(absTime, dt);

            engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
            //Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance);
            //Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance);

            absTime += dt;
        }
	}

	[TestCase]
	public void EngineIdleControllerTest3()
	{
		var container = GetMockVehicleContainer();
		var inputData = GetEngineInputData(3.8.SI<KilogramSquareMeter>());

		var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);
		var aux = new EngineAuxiliary(container);
		aux.AddConstant("CONST", 5000.SI<Watt>());
		engine.Connect(aux.Port());

		Mock.Get(container).Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>())).Callback(
			(Second absTime, Second dt) => {
				engine.CommitSimulationStep(absTime, dt, null);
				aux.CommitSimulationStep(absTime, dt, null);
			});

        var requestPort = engine.OutPort();

		var idleCtl = engine.IdleController;
		idleCtl.RequestPort = engine;
        var absTime = 0.SI<Second>();
		var dt = Constants.SimulationSettings.TargetTimeInterval;

		var angularVelocity = 800.RPMtoRad();
		var torque = 100000.SI<Watt>() / angularVelocity;

		var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

		response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
		Assert.AreEqual(105000, response.Engine.PowerRequest.Value(), Tolerance);
		container.CommitSimulationStep(absTime, dt);
		absTime += dt;

		var engineSpeed = new[] { 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad() };
		var enginePower = new[] { -8601.6308.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>() };

		
		for (var i = 0; i < engineSpeed.Length; i++) {
			torque = 0.SI<NewtonMeter>();

			response = (ResponseSuccess)idleCtl.Request(absTime, dt, torque, null, false);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance, "i: {0}", i);
			Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance, "i: {0}", i);
		}
	}

    [TestCase]
    public void EngineIdleJump()
    {
		var container = GetMockVehicleContainer();
		var inputData = GetEngineInputData(3.8.SI<KilogramSquareMeter>());

        var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());

		var engine = new CombustionEngine(container, engineData);
		var aux = new EngineAuxiliary(container);
		aux.AddConstant("CONST", 5000.SI<Watt>());
		engine.Connect(aux.Port());

		var dataWriter = new MockModalDataContainer();
        dataWriter.AddAuxiliary("CONST");

        Mock.Get(container).Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>())).Callback(
			(Second absTime, Second dt) => {
				engine.CommitSimulationStep(absTime, dt, dataWriter);
				aux.CommitSimulationStep(absTime, dt, dataWriter);
                dataWriter.CommitSimulationStep();
			});

        var requestPort = engine.OutPort();
		var idleCtl = engine.IdleController;
		idleCtl.RequestPort = engine;

        var torque = 1200.SI<NewtonMeter>();
        var angularVelocity = 800.RPMtoRad();

        // initialize engine...

        requestPort.Initialize(torque, angularVelocity);

        var absTime = 0.SI<Second>();
        var dt = Constants.SimulationSettings.TargetTimeInterval;

        var response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);

        container.CommitSimulationStep(absTime, dt);

        var row = dataWriter.Data.Rows.Cast<DataRow>().Last();
        Assert.AreEqual(100530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_ice_out.GetName()]).Value());
        Assert.AreEqual(105530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_ice_fcmap.GetName()]).Value());
        Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux_mech.GetName()]);
        Assert.AreEqual(800.RPMtoRad(), row[ModalResultField.n_ice_avg.GetName()]);

        absTime += dt;

        // actual test...

       
        torque = 0.SI<NewtonMeter>();

        response = (ResponseSuccess)idleCtl.Request(absTime, dt, torque, null, false);

        container.CommitSimulationStep(absTime, dt);
        row = dataWriter.Data.Rows.Cast<DataRow>().Last();

        Assert.AreEqual(0.SI<Watt>(), row[ModalResultField.P_ice_out.GetName()]);
        Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux_mech.GetName()]);
        Assert.AreEqual(680.RPMtoRad(), row[ModalResultField.n_ice_avg.GetName()]);
    }

    private IVehicleContainer GetMockVehicleContainer()
	{
		var container = new Mock<IVehicleContainer>();
		var gbx = new Mock<IGearboxInfo>();
		var ice = new Mock<IEngineInfo>();
		var iceCtl = new Mock<IEngineControl>();
		var veh = new Mock<IVehicleInfo>();

		gbx.Setup(g => g.Gear).Returns(new GearshiftPosition(0));
		ice.Setup(i => i.EngineOn).Returns(true);
		iceCtl.Setup(i => i.CombustionEngineOn).Returns(true);
		veh.Setup(v => v.VehicleStopped).Returns(false);

		container.Setup(c => c.GearboxInfo).Returns(gbx.Object);
		container.Setup(c => c.EngineInfo).Returns(ice.Object);
		container.Setup(c => c.EngineCtl).Returns(iceCtl.Object);
		container.Setup(c => c.VehicleInfo).Returns(veh.Object);

		return container.Object;
	}

    private IEngineEngineeringInputData GetEngineInputData(KilogramSquareMeter inertia)
    {
        var ice = new Mock<IEngineEngineeringInputData>();
        var mode = new Mock<IEngineModeEngineeringInputData>();
        var fuel = new Mock<IEngineFuelEngineeringInputData>();

        ice.Setup(e => e.Displacement).Returns(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>());
        ice.Setup(e => e.Inertia).Returns(inertia);
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
        "560,2300,12869",
		"2100,-320,0",
        "2100,0,10470",
		"2100,2300,50653",
	};

}