using FsCheck;
using FsCheck.Fluent;
using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy.PropertyBased;

using FsCheck.NUnit;

[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP4)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP4)]
[Ignore("Use tests where the generators can be modified in the test method instead of passing arbitraries, this is not flexible enough")]

// [Category("Mutation")]
public class RequestPropertyBased : HybridStrategyBase
{
	private PowertrainPosition _powertrainPosition;
	private GearboxType _gearboxType;
	public RequestPropertyBased(GearboxType gearboxType, PowertrainPosition powertrainPosition)
	{
		_gearboxType = gearboxType;
		_powertrainPosition = powertrainPosition;
	}

	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1000, QuietOnSuccess = false, Replay = "(10769071830409605146,3377302078599022447,1)")]
	public Property RequestReplay(
		VehicleState vehicleState,
		DriverState driverState,
		GeneralState generalState,
		HybridControllerState hybridControllerState,
		PreviousState previousState,
		Request request)
	{
		return Request(vehicleState, driverState, generalState, hybridControllerState, previousState, request);
	}

	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1000, QuietOnSuccess = false)]
	public Property Request(
		VehicleState vehicleState, 
		DriverState driverState, 
		GeneralState generalState,
		HybridControllerState hybridControllerState,
		PreviousState previousState,
		Request request)
	{
		
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		var container = GetVehicleContainer(runData);
		var powertrainCOntainer = new PowertrainContainer(container.Object);
		container.SetVehicleState(vehicleState);
		container.SetDriverState(driverState);
		container.SetGeneralState(generalState);
		
		

		
		
		var strategy = new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(runData, container.Object, powertrainCOntainer);
		strategy.SetHybridController(hybridControllerState, previousState);

		
		
		
		strategy.Request(
			request.AbsTime, 
			request.Dt, 
			request.OutTorque, 
			request.OutAngularVelocity,
			request.DryRun);
		


		return true.ToProperty();
	}
	
	
}