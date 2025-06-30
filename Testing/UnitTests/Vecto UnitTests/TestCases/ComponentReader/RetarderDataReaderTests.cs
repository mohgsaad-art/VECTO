using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class RetarderDataReaderTests
{
	private const double Tolerance = 0.0001;

	protected ISimplePowertrainBuilder powertrainBuilder;

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		var kernel = new StandardKernel(new VectoNinjectModule());
		powertrainBuilder = kernel.Get<ISimplePowertrainBuilder>();
	}

    [TestCase]
    public void RetarderDataSorting()
    {
        var retarderEntries = new[] {
            "100,10.02",
            "0,10",
            "200,10.08",
            "500,10.5",
            "300,10.18",
            "400,10.32",
        };
        var retarderTbl =  InputDataHelper.InputDataAsTableData("Retarder Speed [rpm],Loss Torque [Nm]",
                retarderEntries);
        var vehicle = new Mock<IVehicleContainer>().Object;
        var retarderData = RetarderLossMapReader.Create(retarderTbl);
        var retarder = new Retarder(vehicle, retarderData, 2.0);

		var mockPort = new Mock<ITnOutPort>();
		NewtonMeter tqRequest = null;
		PerSecond rpmRequest = null;
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.Is<bool>(b => !b))).Returns((Second _, Second _, NewtonMeter tq, PerSecond rpm, bool _) => {
			tqRequest = tq;
			rpmRequest = rpm;
			return new ResponseSuccess(this);
		});

        retarder.InPort().Connect(mockPort.Object);
        var outPort = retarder.OutPort();

        var absTime = 0.SI<Second>();
        var dt = 0.SI<Second>();

		var cardanSpeed = 125.RPMtoRad();
		var cardanTorque = 100.SI<NewtonMeter>();
		var expectedRetarderLoss = 20.26.SI<NewtonMeter>();
        // --------
        outPort.Initialize(cardanTorque, cardanSpeed);
        outPort.Request(absTime, dt, cardanTorque, cardanSpeed, false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(rpmRequest);
		Assert.IsNotNull(tqRequest);
		Assert.AreEqual(cardanSpeed.AsRPM, rpmRequest.AsRPM, Tolerance);
		Assert.AreEqual((cardanTorque + expectedRetarderLoss).Value(), tqRequest.Value(), Tolerance);

        //Assert.AreEqual(125.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
        //Assert.AreEqual(100 + 20.26, nextRequest.Torque.Value(), Delta);
    }

}