using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using MockDrivingCycle = TUGraz.VectoCore.Tests.Utils.MockDrivingCycle;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Auxiliaries;

public class LorryAuxTests
{
    [TestCase(2358, 500, 1200),
	TestCase(2358, 1500, 1200),
        TestCase(1500, 1500, 1200)
	]
    public void AuxConstant(double rpm, double tq, double auxPwrDemand)
    {
		var speed = rpm.RPMtoRad();
		var torque = tq.SI<NewtonMeter>();
		var t = 0.SI<Second>();

        var dataWriter = new MockModalDataContainer();
		var container = new Mock<IVehicleContainer>().Object;
		var eng = new Mock<IEngineInfo>();
		var engCtl = eng.As<IEngineControl>();
		eng.Setup(e => e.EngineOn).Returns(true);
		eng.Setup(e => e.EngineSpeed).Returns(() => speed);
		engCtl.Setup(e => e.CombustionEngineOn).Returns(true);
		Mock.Get(container).Setup(c => c.EngineInfo).Returns(eng.Object);
		Mock.Get(container).Setup(c => c.EngineCtl).Returns(engCtl.Object);
		var aux = new EngineAuxiliary(container);

        var constPower = auxPwrDemand.SI<Watt>();
        aux.AddConstant("CONSTANT", constPower);

        aux.Initialize(torque, speed);
        var auxDemand = aux.TorqueDemand(t, t, torque, speed);
        AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);

    }

    [TestCase]
    public void AuxDirect()
    {
		var speed = 2358.RPMtoRad();
		var torque = 500.SI<NewtonMeter>();

        var dataWriter = new MockModalDataContainer();
		var container = new Mock<IVehicleContainer>().Object;
		var eng = new Mock<IEngineInfo>();
		var engCtl = eng.As<IEngineControl>();
		eng.Setup(e => e.EngineOn).Returns(true);
		eng.Setup(e => e.EngineSpeed).Returns(() => speed);
		engCtl.Setup(e => e.CombustionEngineOn).Returns(true);
		Mock.Get(container).Setup(c => c.EngineInfo).Returns(eng.Object);
		Mock.Get(container).Setup(c => c.EngineCtl).Returns(engCtl.Object);
		
		var data = DrivingCycleDataReader.ReadFromStream(InputDataHelper.InputDataAsStream(Header, CycleData),
			CycleType.MeasuredSpeed, "TestCycle", false);
        var cycle = new MockDrivingCycle(container, data);

		Mock.Get(container).Setup(c => c.DrivingCycleInfo).Returns(cycle);

        var aux = new EngineAuxiliary(container);
		
        aux.AddCycle("CYCLE");
        container.AddAuxiliary("CYCLE");

        var t = 0.SI<Second>();

        var expected = new[] { 6100, 3100, 2300, 4500, 6100 };
        foreach (var e in expected) {
            aux.Initialize(torque, speed);
            var auxDemand = aux.TorqueDemand(t, t, torque, speed);

            AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());
            cycle.CommitSimulationStep(t, t, null);
        }
    }

    [TestCase]
    public void AuxAllCombined()
    {
        var dataWriter = new MockModalDataContainer();
        dataWriter.AddAuxiliary("CONSTANT");

		var speed = 578.22461991.RPMtoRad(); // = 2358 (nAuxiliary) * ratio
		var torque = 500.SI<NewtonMeter>();

        var container = new Mock<IVehicleContainer>().Object;
		var eng = new Mock<IEngineInfo>();
		var engCtl = eng.As<IEngineControl>();
		eng.Setup(e => e.EngineOn).Returns(true);
		eng.Setup(e => e.EngineSpeed).Returns(() => speed);
		engCtl.Setup(e => e.CombustionEngineOn).Returns(true);
		Mock.Get(container).Setup(c => c.EngineInfo).Returns(eng.Object);
		Mock.Get(container).Setup(c => c.EngineCtl).Returns(engCtl.Object);
		
		var data = DrivingCycleDataReader.ReadFromStream(InputDataHelper.InputDataAsStream(Header, CycleData),
            CycleType.MeasuredSpeed, "TestCycle", false);
        // cycle ALT1 is set to values to equal the first few fixed points in the auxiliary file.
        // ALT1.aux file: nAuxiliary speed 2358: 0, 0.38, 0.49, 0.64, ...
        // ALT1 in cycle file: 0, 0.3724 (=0.38*0.96), 0.4802 (=0.49*0.96), 0.6272 (0.64*0.96), ...

        var cycle = new MockDrivingCycle(container, data);

		Mock.Get(container).Setup(c => c.DrivingCycleInfo).Returns(cycle);

        var aux = new EngineAuxiliary(container);
        new MockEngine(container);

        aux.AddCycle("CYCLE");
        var constPower = 1200.SI<Watt>();
        aux.AddConstant("CONSTANT", constPower);

        var t = 0.SI<Second>();
        // MQ 2021-03-03: updated expected values - mapping auxiliary is no longer supported
        var expected = new[] {
                1200 + 6100, // + 72.9166666666667,
				// = 1000 * 0.07 (nAuxiliary=2358 and psupply=0) / 0.98 (efficiency_supply)
				1200 + 3100, // + 677.083333333333,
				// = 1000 * 0.65 (nAuxiliary=2358 and psupply=0.38) / 0.98 (efficiency_supply)
				1200 + 2300, // + 822.916666666667,
				// = 1000 * 0.79 (nAuxiliary=2358 and psupply=0.49) / 0.98 (efficiency_supply)
				1200 + 4500, // + 1031.25, // = ...
				1200 + 6100, // + 1166.66666666667,
				1200 + 6100, // + 1656.25,
				1200 + 6100, // + 2072.91666666667,
				1200 + 6100, // + 2510.41666666667,
				1200 + 6100, // + 2979.16666666667,
				1200 + 6100, // + 3322.91666666667,
				1200 + 6100, // + 3656.25
			};

        foreach (var e in expected) {
            aux.Initialize(torque, speed);
            var auxDemand = aux.TorqueDemand(t, t, torque, speed);

            AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());

            cycle.CommitSimulationStep(t, t, null);
        }
    }

	protected const string Header = "<t> , <v> , <grad>       , <Padd> , <Aux_ALT1> , <Aux_ALT2> , <Aux_ALT3>";

	protected readonly string[] CycleData = new[] {
		"0   , 0   , -0.020237973 , 6.1    , 0          , 0.25       , 0.25",
		"1   , 64  , -0.020237973 , 3.1    , 0.3724     , 0.25       , 0.25",
		"2   , 64  , -0.020237973 , 2.3    , 0.4802     , 0.25       , 0.25",
		"3   , 64  , -0.020237973 , 4.5    , 0.6272     , 0.25       , 0.25",
		"4   , 64  , -0.020237973 , 6.1    , 0.735      , 0.25       , 0.25",
		"5   , 64  , -0.020237973 , 6.1    , 1.0976     , 0.25       , 0.25",
		"6   , 64  , -0.020237973 , 6.1    , 1.3916     , 0.25       , 0.25",
		"7   , 64  , -0.020237973 , 6.1    , 1.6464     , 0.25       , 0.25",
		"8   , 64  , -0.020237973 , 6.1    , 1.911      , 0.25       , 0.25",
		"9   , 64  , -0.020237973 , 6.1    , 2.0776     , 0.25       , 0.25",
		"10  , 64  , -0.020237973 , 6.1    , 2.254      , 0.25       , 0.25",
	};
}