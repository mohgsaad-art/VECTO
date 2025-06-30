using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.AxlegearTests;

public class AxlegearTests
{
    [TestCase(520, 20.320, 279698.4, 2220.9965722057)]
    public void AxleGearTest(double rdyn, double speed, double power, double expectedTqIn)
	{
		var vehicle = new Mock<IVehicleContainer>().Object;
		var inputData = GetAxleInputData();
		var axleGearData = new AxleGearDataAdapter().CreateAxleGearData(inputData); // MockSimulationDataFactory.CreateAxleGearDataFromFile(GearboxDataFile);
        var axleGear = new AxleGear(vehicle, axleGearData);

		var mockPort = new Mock<ITnOutPort>();
		NewtonMeter tqRequest = null;
		PerSecond rpmRequest = null;
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.Is<bool>(b => !b))).Returns((Second _, Second _, NewtonMeter tq, PerSecond rpm, bool _) => {
			tqRequest = tq;
			rpmRequest = rpm;
			return new ResponseSuccess(this);
		});

        axleGear.InPort().Connect(mockPort.Object);

        var absTime = 0.SI<Second>();
        var dt = 1.SI<Second>();

        var angSpeed = CalculationHelper.SpeedToAngularSpeed(speed, rdyn);
        var PvD = power.SI<Watt>();
        var torqueToWheels = Formulas.PowerToTorque(PvD, angSpeed);

        axleGear.Request(absTime, dt, torqueToWheels, angSpeed, false);

		var expectedTq = expectedTqIn.SI<NewtonMeter>();
		var expectedRpm = angSpeed * axleGearData.AxleGear.Ratio;

        mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(tqRequest);
		Assert.IsNotNull(rpmRequest);
		Assert.AreEqual(expectedTq.Value(), tqRequest.Value(), 0.01, "Torque Engine Side");
		Assert.AreEqual(expectedRpm.Value(), rpmRequest.Value(), 0.01, "AngularVelocity Engine Side");
	}

    

	protected static string AxlMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
	protected static readonly string[] AxlMapData = new[] {
		"0,0,0",
		"0,-100000,100",
		"0,100000,100",
		"5000,-100000,100",
		"5000,100000,100",
    };

    private IAxleGearInputData GetAxleInputData()
	{
		var axl = new Mock<IAxleGearInputData>();
		axl.Setup(a => a.LineType).Returns(AxleLineType.SinglePortalAxle);
		axl.Setup(a => a.Ratio).Returns(3.240355);
		axl.Setup(a => a.LossMap)
			.Returns(VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream(AxlMapHdr, AxlMapData)));
        return axl.Object;
	}
}