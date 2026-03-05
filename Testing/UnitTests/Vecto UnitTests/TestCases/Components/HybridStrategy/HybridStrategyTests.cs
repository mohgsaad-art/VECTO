using System.Linq.Expressions;
using System.Transactions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.VehicleOperation;
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

/// <summary>
/// Unit tests for the ParallelHybrid strategy <see cref="HybridStrategy"/>
/// </summary>
[TestFixture]
[Ignore("Replaced with Property based tests")]
public class HybridStrategyTests : HybridStrategyBase
{

	[TestCase]
	public void Test_Init()
	{
		var runData = GetRunData(PowertrainPosition.HybridP2, GearboxType.AMT);
		var vehicleContainer = GetVehicleContainer(runData);
		var powertrainContainer = new PowertrainContainer(vehicleContainer.Object);
		Assert.DoesNotThrow(() => {
			var hybridStrategy = GetHybridStrategy(runData, 
				vehicleContainer.Object,
				powertrainContainer,
				1,
				1.SI<NewtonMeter>(),
				1.SI<PerSecond>()
			);
		});
	}

	

	[Test,Combinatorial]
	public void Test_Request_Accelerate(
		[Values(true, false)]bool emCanPropellDuringTraction, 
		[Values(true, false)]bool gearEngaged, 
		[Values(true, false)]bool automaticTransmission)
	{
		
		
		
		Assume.That(emCanPropellDuringTraction || gearEngaged || automaticTransmission, "Handle in different testcase");
		
		var runData = GetRunData(
			emCanPropellDuringTraction
				? PowertrainPosition.HybridP4 
				: PowertrainPosition.HybridP2,
			automaticTransmission
				? GearboxType.ATSerial
			    : GearboxType.AMT);
		var vehicleContainer = GetVehicleContainer(runData);
		var powertrainContainer = new PowertrainContainer(vehicleContainer.Object);
		ConsistencyChecks(vehicleContainer);
		
		
		
		var gbxInfo = vehicleContainer.Object.GearboxesInfo.Single();
		var gbxInfoMock = Mock.Get(gbxInfo);
		gbxInfoMock.Setup(g => g.GearboxType).Returns(
			automaticTransmission
				? GearboxType.ATSerial 
				: GearboxType.MT);
		gbxInfoMock.Setup(g => g.GearEngaged(It.IsAny<Second>())).Returns(gearEngaged);
		
		Assert.AreEqual(automaticTransmission, gbxInfo.GearboxType.AutomaticTransmission());
		Assert.AreEqual(gearEngaged, gbxInfo.GearEngaged(_t));
		
		
		
		var hybridStrategy = GetHybridStrategy(runData, 
			vehicleContainer.Object,
			powertrainContainer,
			1,
			1.SI<NewtonMeter>(),
			1.SI<PerSecond>()
			);
		
		
		SetDrivingAction(vehicleContainer, DrivingAction.Accelerate);

		var emOff = !emCanPropellDuringTraction && !gearEngaged && !automaticTransmission;
		
		var response = hybridStrategy.Request(_t, _dt, 1.SI<NewtonMeter>(), 1.RPMtoRad(), false);

		
		
	}

	public void SetDrivingAction(Mock<IVehicleContainer> vehicleContainer, DrivingAction drivingAction)
	{
		vehicleContainer
			.Setup(s => s.DriverInfo.DrivingAction)
			.Returns(DrivingAction.Accelerate);


	}
	


	public VectoCore.Models.SimulationComponent.Strategies.HybridStrategy GetHybridStrategy(VectoRunData runData,
		IVehicleContainer vehicleContainer, IPowertrainContainer powertrainContainer, uint nextGear,
		NewtonMeter outTorque, PerSecond outAngularVelocity)
	{
		var hybridStrategy = new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(runData, vehicleContainer, powertrainContainer);
		
		var controller = new Mock<IHybridController>(MockBehavior.Strict);
		hybridStrategy.Controller = controller.Object;
		
		var shiftStrategy = new Mock<IShiftStrategy>(MockBehavior.Strict);
		controller.Setup(c => c.ShiftStrategy).Returns(shiftStrategy.Object);
		
		var gearshiftPosition = new GearshiftPosition(nextGear);
		shiftStrategy.Setup(s => s.NextGear).Returns(gearshiftPosition);

		var controllerState = new SimpleComponentState() {
			OutTorque = outTorque,
			OutAngularVelocity = outAngularVelocity
		};
		//Initialization
		hybridStrategy.Initialize(outTorque, outAngularVelocity);
		controller.Setup(c => c.PreviousState).Returns(() => controllerState);
		
		return hybridStrategy;
	}


	private void ConsistencyChecks(Mock<IVehicleContainer> mockContainer)
	{
		var container = mockContainer.Object;
		
		Assert.AreEqual(container.RunData.GearboxSinglePwt.Type, container.GearboxesInfo.Single().GearboxType);
		Assert.AreEqual(container.RunData.GearboxSinglePwt.Type, container.SimplePowertrainBuilder.CreateTestPowertrain(container, false).Gearboxes.Single().GearboxType);
		
	}
}