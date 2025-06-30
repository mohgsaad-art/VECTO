using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.RetarderTests;

public class RetarderTests
{

	private const double Tolerance = 0.0001;


    [TestCase(0, 10, 1.0, 10.002),
	TestCase(100, 1000, 1.0, 12),
	TestCase(0, 10, 2.0, 20.008),
	TestCase(100, 1000, 2.0, 36),
    ]
	public void RetarderRequestTest(double cardanTorque, double cardanSpeed, double ratio, double expectedRetarderLoss)
	{
		var vehicle = new Mock<IVehicleContainer>().Object;
		var data = InputDataHelper.InputDataAsTableData(RetarderHdr, RetarderData);
		var retarderData = RetarderLossMapReader.Create(data);
		var retarder = new Retarder(vehicle, retarderData, ratio);

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

		// --------
		outPort.Initialize(cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad());
		outPort.Request(absTime, dt, cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad(), false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(rpmRequest);
		Assert.IsNotNull(tqRequest);
		Assert.AreEqual(cardanSpeed, rpmRequest.AsRPM, Tolerance);
		Assert.AreEqual(cardanTorque + expectedRetarderLoss, tqRequest.Value(), Tolerance);
	}

	[TestCase]
	public void RetarderSubsequentRequestTest()
	{
		var vehicle = new Mock<IVehicleContainer>().Object;
        var data = InputDataHelper.InputDataAsTableData(RetarderHdr, RetarderData);
		var retarderData = RetarderLossMapReader.Create(data);
        var retarder = new Retarder(vehicle, retarderData, 1.0);

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
		// --------
		outPort.Initialize(50.SI<NewtonMeter>(), 650.RPMtoRad());
		outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 1550.RPMtoRad(), false);
		retarder.CommitSimulationStep(absTime, dt, new MockModalDataContainer());

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(rpmRequest);
		Assert.IsNotNull(tqRequest);
        Assert.AreEqual(1550, rpmRequest.AsRPM, Tolerance);

		// (650+1550)/2 = 1100 => 12.42Nm
		Assert.AreEqual(50 + 12.42, tqRequest.Value(), Tolerance);

		//VECTO-307: added an additional request after a commit
		outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 450.RPMtoRad(), false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Exactly(2));
		Assert.AreEqual(450, rpmRequest.AsRPM, Tolerance);
		// avg: (1550+450)/2 = 1000 rpm => 12Nm
		Assert.AreEqual(50 + 12, tqRequest.Value(), Tolerance);
	}

	[TestCase]
	public void RetarderDeclarationNoExtrapolationTest()
	{
		var data = InputDataHelper.InputDataAsTableData(RetarderHdr, RetarderData);
		var retarderData = RetarderLossMapReader.Create(data);
		var declVehicle = new Mock<IVehicleContainer>().Object;
		Mock.Get(declVehicle).Setup(c => c.ExecutionMode).Returns(ExecutionMode.Declaration);
        var retarder = new Retarder(declVehicle, retarderData, 2.0);
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

		outPort.Initialize(50.SI<NewtonMeter>(), 2550.RPMtoRad());
		outPort.Request(0.SI<Second>(), 0.SI<Second>(), 50.SI<NewtonMeter>(), 2550.RPMtoRad(), false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);

        AssertHelper.Exception<VectoException>(() => retarder.CommitSimulationStep(0.SI<Second>(), 0.SI<Second>(), new MockModalDataContainer()),
			"Retarder LossMap data was extrapolated in Declaration mode: range for loss map is not sufficient: n:5100 (min:0, max:2300), ratio:2");
	}

    public const string RetarderHdr = "Retarder Speed [1/min],Torque Loss [Nm]";

	public static readonly string[] RetarderData = new[] {
		"0,10",
		"100,10.02",
		"200,10.08",
		"300,10.18",
		"400,10.32",
		"500,10.5",
		"600,10.72",
		"700,10.98",
		"800,11.28",
		"900,11.62",
		"1000,12",
		"1100,12.42",
		"1200,12.88",
		"1300,13.38",
		"1400,13.92",
		"1500,14.5",
		"1600,15.12",
		"1700,15.78",
		"1800,16.48",
		"1900,17.22",
		"2000,18",
		"2100,18.82",
		"2200,19.68",
		"2300,20.58",
	};
}