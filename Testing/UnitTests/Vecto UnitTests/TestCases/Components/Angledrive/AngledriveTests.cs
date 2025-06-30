using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.AngleDrive;

public class AngledriveTests
{
	[TestCase(520, 20.320, 279698.4, 2283.1522, 3.240355)]
	public void Angledrive_Losses(double rdyn, double speed, double power, double expectedTqRequest, double ratio)
	{
		// convert to SI
		var angSpeed = CalculationHelper.SpeedToAngularSpeed(speed, rdyn);
		var PvD = power.SI<Watt>();
		var torqueToWheels = PvD / angSpeed;
		var expectedTq = expectedTqRequest.SI<NewtonMeter>();
		var expectedRpm = angSpeed * ratio;

        // setup components
		var vehicle = new Mock<IVehicleContainer>().Object;
		var angledriveData = new AngledriveData {
			Angledrive = new TransmissionData {
				LossMap = TransmissionLossMapReader.Create(InputDataHelper.InputDataAsTableData(AngleLossHdr, AnleLossMap), ratio, "Angledrive"),
				Ratio = ratio
			}
		};
		var mockPort = new Mock<ITnOutPort>();
		NewtonMeter tqRequest = null;
		PerSecond rpmRequest = null;
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.Is<bool>(b => !b))).Returns((Second _, Second _, NewtonMeter tq, PerSecond rpm, bool _) => {
			tqRequest = tq;
			rpmRequest = rpm;
			return new ResponseSuccess(this);
		});
        var angledrive = new Angledrive(vehicle, angledriveData);
		angledrive.InPort().Connect(mockPort.Object);

		// issue request
		angledrive.Request(0.SI<Second>(), 1.SI<Second>(), torqueToWheels, angSpeed);

		// test
		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(tqRequest);
		Assert.IsNotNull(rpmRequest);
		Assert.AreEqual(expectedTq.Value(), tqRequest.Value(), 0.01, "Torque Engine Side");
		Assert.AreEqual(expectedRpm.Value(), rpmRequest.Value(), 0.01, "AngularVelocity Engine Side");
    }

	protected const string AngleLossHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
	protected static readonly string[] AnleLossMap = new[] {
		"0,-2500,77.5",
		"0,-1500,62.5",
		"0,-500,47.5",
		"0,500,47.5",
		"0,1500,62.5",
		"0,2500,77.5",
		"0,3500,92.5",
		"0,4500,107.5",
		"0,5500,122.5",
		"0,6500,137.5",
		"0,7500,152.5",
		"0,8500,167.5",
		"0,9500,182.5",
		"0,10500,197.5",
		"0,11500,212.5",
		"0,12500,227.5",
		"0,13500,242.5",
		"0,14500,257.5",
		"0,15500,272.5",
		"3600,-2500,77.5",
		"3600,-1500,62.5",
		"3600,-500,47.5",
		"3600,500,47.5",
		"3600,1500,62.5",
		"3600,2500,77.5",
		"3600,3500,92.5",
		"3600,4500,107.5",
		"3600,5500,122.5",
		"3600,6500,137.5",
		"3600,7500,152.5",
		"3600,8500,167.5",
		"3600,9500,182.5",
		"3600,10500,197.5",
		"3600,11500,212.5",
		"3600,12500,227.5",
		"3600,13500,242.5",
		"3600,14500,257.5",
		"3600,15500,272.5",
	};
}