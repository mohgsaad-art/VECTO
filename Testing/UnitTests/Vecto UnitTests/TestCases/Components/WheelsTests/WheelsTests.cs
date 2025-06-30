using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Assert = NUnit.Framework.Assert;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.WheelsTests;

public class WheelsTests
{
    [TestCase(5000, 20, 2600, 38.4615384),
	TestCase(5000, 10, 2600, 19.230769),
	TestCase(1000, 20, 520, 38.4615384),
    ]
    public void WheelsRequestTest(double force, double velocity, double expTorque, double expAngVel)
    {
        var container = new Mock<IVehicleContainer>();
        var vehicleData = new VehicleData()
        {
            DynamicTyreRadius = 0.520.SI<Meter>(),
            CurbMass = 15700.SI<Kilogram>(),
            Loading = 3300.SI<Kilogram>(),
            AxleData = new List<Axle>() {
                new Axle() {
                    AxleWeightShare = 0.4375,
                    TwinTyres = false,
                    Inertia = 21.666666666.SI<KilogramSquareMeter>(),
                    TyreTestLoad = 62538.75.SI<Newton>(),
                    //RollResistanceCoefficient = 0.0055,
				},
                new Axle() {
                    AxleWeightShare = 0.375,
                    TwinTyres = true,
                    Inertia = 10.8333333.SI<KilogramSquareMeter>(),
					TyreTestLoad = 52532.55.SI<Newton>(),
				},
                new Axle() {
                    AxleWeightShare = 0.1875,
                    TwinTyres = false,
                    Inertia = 21.6666666.SI<KilogramSquareMeter>(),
					TyreTestLoad = 62538.75.SI<Newton>(),
				}
            }
        };

        NewtonMeter reqTq = null;
        PerSecond reqN = null;

        IWheels wheels = new Wheels(container.Object, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia);
        var mockPort = new Mock<ITnOutPort>();
        mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
            It.IsAny<PerSecond>(), It.IsAny<bool>())).Returns(
            (Second t, Second dt, NewtonMeter tq, PerSecond n, bool dryRun) =>
            {
                reqTq = tq;
                reqN = n;
                return new ResponseSuccess(this);
            });

        wheels.InPort().Connect(mockPort.Object);

        var requestPort = wheels.OutPort();

        var absTime = 0.SI<Second>();
        var dt = 1.SI<Second>();


        requestPort.Initialize(force.SI<Newton>(), velocity.SI<MeterPerSecond>());

        var retVal = requestPort.Request(absTime, dt, force.SI<Newton>(), velocity.SI<MeterPerSecond>(), false);

        Assert.AreEqual(expTorque, reqTq.Value(), 0.0001);
        Assert.AreEqual(expAngVel, reqN.Value(), 0.0001);
    }
}