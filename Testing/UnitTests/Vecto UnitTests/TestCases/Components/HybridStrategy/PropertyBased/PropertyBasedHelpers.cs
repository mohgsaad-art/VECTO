using System.Runtime.CompilerServices;
using System.Text;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using Moq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy.PropertyBased;




public record VehicleState(
	MeterPerSecond Speed,
	NewtonMeter TractionForce,
	Watt BrakePower,
	GearboxState GearboxState)
{
	/// <summary>
	/// Current Vehicle Speed
	/// </summary>
	public MeterPerSecond Speed { get; init; } = Speed;
	
	/// <summary>
	/// Current Traction Force
	/// </summary>
	public NewtonMeter TractionForce { get; init; } = TractionForce;

}

public record TestData(List<HybridResultEntry> Entries, VehicleState VehicleState, DriverState DriverState, GeneralState GeneralState)
{
	#region Overrides of TestData

	protected virtual bool PrintMembers(StringBuilder builder)
	{
		builder.AppendLine();
		builder.AppendJoin("\n", Entries);
		builder.AppendLine();
		builder.AppendLine(VehicleState.ToString());
		builder.AppendLine(DriverState.ToString());
		builder.AppendLine(GeneralState.ToString());
		return true;
	}

	#endregion
}

public record GearboxState(GearshiftPosition Gear, bool GearEngaged);
public record HybridControllerState(GearshiftPosition NextGear);
public record DriverState(DrivingAction Action, MeterPerSquareSecond DriverAcceleration);
public record GeneralState(MeterPerSecond CycleTargetSpeed);
public record Request(Second AbsTime, Second Dt, NewtonMeter OutTorque, PerSecond OutAngularVelocity, bool DryRun);
public record PreviousState(NewtonMeter OutTorque, PerSecond OutAngularVelocity);


internal static class ContainerExtensions
{	
	internal static void SetVehicleState(this Mock<IVehicleContainer> container, VehicleState vehicleState)
	{
		container.Setup(g => g.VehicleInfo.VehicleSpeed).Returns(vehicleState.Speed);
		
		container.Setup(g => g.Brakes.BrakePower).Returns(vehicleState.BrakePower);
		
		container.SetGearboxState(vehicleState.GearboxState);
	}
	internal static void SetDriverState(this Mock<IVehicleContainer> container, DriverState driverState)
	{
		container.Setup(v => v.DriverInfo.DrivingAction).Returns(driverState.Action);
		container.Setup(v => v.DriverInfo.DriverAcceleration).Returns(driverState.DriverAcceleration);
	}
	
	internal static void SetGeneralState(this Mock<IVehicleContainer> container, GeneralState generalState)
	{
		container.Setup(c => c.DrivingCycleInfo.TargetSpeed).Returns(generalState.CycleTargetSpeed);
	}

	internal static void SetHybridController(this VectoCore.Models.SimulationComponent.Strategies.HybridStrategy hybridStrategy,
		HybridControllerState hybridControllerState,
		PreviousState previousState)
	{
		var controller = new Mock<IHybridController>(MockBehavior.Strict);
		hybridStrategy.Controller = controller.Object;
		
		var shiftStrategy = new Mock<IShiftStrategy>(MockBehavior.Strict);
		controller.Setup(c => c.ShiftStrategy).Returns(shiftStrategy.Object);
		
		shiftStrategy.Setup(s => s.NextGear).Returns(hybridControllerState.NextGear);

		var outTorque = previousState.OutTorque;
		var outAngularVelocity = previousState.OutAngularVelocity;
		
		var controllerState = new SimpleComponentState() {
			OutTorque = outTorque,
			OutAngularVelocity = outAngularVelocity
		};
		//Initialization
		hybridStrategy.Initialize(outTorque, outAngularVelocity);
		controller.Setup(c => c.PreviousState).Returns(() => controllerState);
		
	}

	internal static void SetGearboxState(this Mock<IVehicleContainer> container, GearboxState gearboxState)
	{
		var gbxInfo = container.Object.GearboxesInfo.Single();
		var gbxInfoMock = Mock.Get(gbxInfo);
		gbxInfoMock.Setup(g => g.Gear).Returns(gearboxState.Gear);
		gbxInfoMock.Setup(g => g.GearEngaged(It.IsAny<Second>())).Returns(gearboxState.GearEngaged);
	}

}

public static class ReplayExtensions
{

	public static Config WithReplay(this Config config, string replayString, int size)
	{		
		var split = replayString.Trim('(', ')').Split([","], StringSplitOptions.RemoveEmptyEntries);
		var seed = UInt64.Parse(split[0]);
		var gamma = UInt64.Parse(split[1]);
		return Config(config, size, seed, gamma);
	}
	
	
	public static Config WithReplay(this Config config, string replayString)
	{
		//if someone sets this, we want it to throw if it fails
		var split = replayString.Trim('(', ')').Split([","], StringSplitOptions.RemoveEmptyEntries);
		var seed = UInt64.Parse(split[0]);
		var gamma = UInt64.Parse(split[1]);
		int? size = split.Length == 3 ? Convert.ToInt32(UInt32.Parse(split[2])) : null;
		return Config(config, size, seed, gamma);
	}

	private static Config Config(Config config, int? size, ulong seed, ulong gamma)
	{
		FSharpOption<int> sizeOption;
		if (size.HasValue) {
			sizeOption = FSharpOption<int>.Some(size.Value);
		} else {
			sizeOption = FSharpOption<int>.None;
		}
		var replay = new Replay(new Rnd(seed, gamma), sizeOption);
		var cfg = config.WithReplay(replay);
		return cfg;
	}
}



internal static class BoolExtensions
{
	public static Property MyImplies(this bool a, Func<bool> b)
	{
		return b.ToProperty().When(a);
		// //
		// return (!a).ToProperty().Or(b);
	}
	
	public static Property MyImplies(this bool a, bool b)
	{
		// return b.ToProperty().When(a);
		//
		return (!a).ToProperty().Or(b);
	}


	public static Property MyClassify(this Property p, bool condition, [CallerArgumentExpression(nameof(condition))] string? variableName = null)
	{
		return p.Classify(condition, variableName);
	}
	
	
}

internal static class PropertyHelpers
{
	public static Property Label(this Property p, IList<HybridResultEntry> hybridResultEntries,
		HybridResultEntry selectedEntry)
	{
		p.Label($"Selected {selectedEntry?.ToString() ?? "-"} from {string.Join("\n",hybridResultEntries)}");

		return p;
	}
}