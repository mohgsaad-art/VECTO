
using FsCheck;
using FsCheck.Fluent;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy.PropertyBased;

using FsCheck.NUnit;

[TestFixture]
public class GeneratorTests()
{

	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest1(VehicleState state)
	{
		return true.ToProperty();
	}

	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest2(DriverState state)
	{
		return true.ToProperty();
	}
	
	
	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest3(GeneralState state)
	{
		return true.ToProperty();
	}
	
	
	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest4(HybridControllerState state)
	{
		return true.ToProperty();
	}
	
	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest5(PreviousState state)
	{
		return true.ToProperty();
	}
	
	[Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 1, QuietOnSuccess = false)]
	public Property GenTest6(Request state)
	{
		return true.ToProperty();
	}
	
}

public static class Generators
{
	internal record CommonResultProperties(bool ResponseGearEngaged);

	private static int _maxIgnoreReason =
		Enum.GetValues<HybridConfigurationIgnoreReason>().Select(e => (int)e).Max();

	internal static Gen<CommonResultProperties> CommonResultPropertiesGen =>
		(from engaged in ArbMap.Default.ArbFor<bool>().Generator
			select new CommonResultProperties(engaged));

	internal static Arbitrary<TestData> TestData
	{
		get
		{
			var gen =
				from entries in HybridResultEntries.Generator
				from vehicleState in VehicleState
				from driverState in DriverState.Generator
				from generalState in GeneralState.Generator
				select (new TestData(entries, vehicleState, driverState, generalState));
			return gen.ToArbitrary();
		}
	}


	internal static Arbitrary<HybridResultEntry> SingleEntry(
		CommonResultProperties commonProperties,
		HybridConfigurationIgnoreReason? ignoreReason = null)
	{
		var entry =
			from fuelCost in ArbMap.Default.GeneratorFor<double>().Where(d => !double.IsInfinity(d))
			from _ignoreReason in Gen.Elements(Enum.GetValues<HybridConfigurationIgnoreReason>())

			from mechanicalAssistPowerTorque in ArbMap.Default.GeneratorFor<NormalFloat>()
			from mechanicalAssistPowerSpeed in ArbMap.Default.GeneratorFor<NormalFloat>().Where(n => n.Get >= 0) //No NaN or inf
			from reessPowerDemand in ArbMap.Default.GeneratorFor<NormalFloat>() //No NaN or inf
			from gear in Generators.GearshiftPositionArb.Generator
			from gearResponsePos  in Gen.Choose(commonProperties.ResponseGearEngaged ? 1 : 0, 12)
			select new HybridResultEntry() {
				FuelCosts = fuelCost,
				IgnoreReason = ignoreReason ?? _ignoreReason,
				Gear = gear,
				Setting = new HybridStrategyResponse() {
					MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
						{
							PowertrainPosition.HybridPositionNotSet,
							Tuple.Create(mechanicalAssistPowerSpeed.Get.SI<PerSecond>(),
								mechanicalAssistPowerTorque.Get.SI<NewtonMeter>())
						}
					},
				},
				Response = new ResponseDryRun("null") {
					ElectricSystem = new ElectricSystemDryRunResponse("null") {
						RESSPowerDemand = reessPowerDemand.Get.SI<Watt>()
					},
					Gearbox = {
						Gear = new GearshiftPosition((uint) gearResponsePos, commonProperties.ResponseGearEngaged)
					}
				}
			};
		return entry.ToArbitrary();
		
		
		
	}

	
	internal static Arbitrary<List<HybridResultEntry>> HybridResultEntries
	{
		get
		{
			var gen =
				from commonResponseProperties in CommonResultPropertiesGen
				from numberOfElements in Gen.Choose(1, 12)
				from entries in HybridResultEntry(commonResponseProperties).ScaleSize(s 
					=> {
					return s;
				}).ListOf(numberOfElements)
				select entries;
			return gen.ToArbitrary();
		}
	}
	

	internal static Gen<HybridResultEntry> HybridResultEntry(CommonResultProperties commonProperties, 
		Func<HybridConfigurationIgnoreReason, bool>? ignoreReasonFilter = null,
		Func<HybridResultEntry, HybridResultEntry>? modifyEntry = null)
	{		
		var usedIgnoreReasonFilter = ignoreReasonFilter ?? ((r) => true);
		var ignoreReasons = Enumerable.Range(0, _maxIgnoreReason + 1).Select(i => (HybridConfigurationIgnoreReason)i).Where(usedIgnoreReasonFilter).ToList();
		var usedModifyEntry = (modifyEntry ?? ((e) => e));
		if (ignoreReasons.Count == 0) {
			throw new Exception("Invalid filter");
		}
		
		var entry =
			from fuelCost in ArbMap.Default.GeneratorFor<double>().Where(d => !double.IsInfinity(d))
			from ignoreReason in Gen.Elements(ignoreReasons.AsEnumerable())
			from mechanicalAssistPowerTorque in ArbMap.Default.GeneratorFor<NormalFloat>()
			from mechanicalAssistPowerSpeed in ArbMap.Default.GeneratorFor<NormalFloat>().Where(n => n.Get >= 0) //No NaN or inf
			from reessPowerDemand in ArbMap.Default.GeneratorFor<NormalFloat>() //No NaN or inf
			from gear in GearshiftPositionArb.Generator
			from gearResponsePos  in Gen.Choose(commonProperties.ResponseGearEngaged ? 1 : 0, 12)
			select usedModifyEntry(new HybridResultEntry() {
				FuelCosts = fuelCost,
				IgnoreReason = ignoreReason,
				Gear = gear,
				Setting = new HybridStrategyResponse() {
					MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
						{
							PowertrainPosition.HybridPositionNotSet,
							Tuple.Create(mechanicalAssistPowerSpeed.Get.SI<PerSecond>(),
								mechanicalAssistPowerTorque.Get.SI<NewtonMeter>())
						}
					},
				},
				Response = new ResponseDryRun("null") {
					ElectricSystem = new ElectricSystemDryRunResponse("null") {
						RESSPowerDemand = reessPowerDemand.Get.SI<Watt>()
					},
					Gearbox = {
						Gear = new GearshiftPosition((uint) gearResponsePos, commonProperties.ResponseGearEngaged)
					}
				}
			});
		return entry;
	}
	// internal static Arbitrary<HybridResultEntry> HybridResultEntry(CommonResultProperties commonProperties)
	// {
	// 	var entry =
	// 		from fuelCost in ArbMap.Default.GeneratorFor<double>().Where(d => !double.IsInfinity(d))
	// 		from ignoreReason in Gen.Elements(Enum.GetValues<HybridConfigurationIgnoreReason>())
	//
	// 			from mechanicalAssistPowerTorque in ArbMap.Default.GeneratorFor<NormalFloat>()
	// 			from mechanicalAssistPowerSpeed in ArbMap.Default.GeneratorFor<NormalFloat>().Where(n => n.Get >= 0) //No NaN or inf
	// 			from reessPowerDemand in ArbMap.Default.GeneratorFor<NormalFloat>() //No NaN or inf
	// 			from gear in Generators.GearshiftPositionArb.Generator
	// 			from gearResponsePos  in Gen.Choose(commonProperties.ResponseGearEngaged ? 1 : 0, 12)
	// 			select new HybridResultEntry() {
	// 				FuelCosts = fuelCost,
	// 				IgnoreReason = ignoreReason,
	// 				Gear = gear,
	// 				Setting = new HybridStrategyResponse() {
	// 					MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
	// 						{
	// 							PowertrainPosition.HybridPositionNotSet,
	// 							Tuple.Create(mechanicalAssistPowerSpeed.Get.SI<PerSecond>(),
	// 								mechanicalAssistPowerTorque.Get.SI<NewtonMeter>())
	// 						}
	// 					},
	// 				},
	// 				Response = new ResponseDryRun("null") {
	// 					ElectricSystem = new ElectricSystemDryRunResponse("null") {
	// 						RESSPowerDemand = reessPowerDemand.Get.SI<Watt>()
	// 					},
	// 					Gearbox = {
	// 						Gear = new GearshiftPosition((uint) gearResponsePos, commonProperties.ResponseGearEngaged)
	// 					}
	// 				}
	// 			};
	// 		return entry.ToArbitrary();
	// }
	
	internal static Arbitrary<GearshiftPosition> GearshiftPositionArb
	{
		get
		{
			var gen = from pos in Gen.Choose(1, 12)
				select new GearshiftPosition((uint)pos, null);
			return gen.ToArbitrary();
		}
	}

	internal static Arbitrary<HybridControllerState> HybridControllerState =>
		(from gear in Generators.GearshiftPositionArb.Generator
			select new HybridControllerState(gear)).ToArbitrary();

	internal static Arbitrary<VehicleState> VehicleStateArb => Generators.VehicleState.ToArbitrary();
	internal static Gen<VehicleState> VehicleState {
		get {
			var entry =
				from speed in Gen.Choose(0, 85)
				from tractionForce in ArbMap.Default.GeneratorFor<NormalFloat>()
				from gearboxState in GearboxState.Generator
				from braking in ArbMap.Default.GeneratorFor<bool>()
				select new VehicleState(
					speed.KMPHtoMeterPerSecond(),
					tractionForce.Get.SI<NewtonMeter>(),
					braking ? 100.SI<Watt>() : 0.SI<Watt>(),
					gearboxState);

			return entry;
		}
	}

	internal static Arbitrary<GearboxState> GearboxState =>
	(
		from currentGear in GearshiftPositionArb.Generator
		from gearEngaged in Gen.Frequency((1, Gen.Constant(true)), (2, Gen.Constant(false)))
		select new GearboxState(currentGear, gearEngaged)
	).ToArbitrary();

	private static Gen<double> Acceleration(DrivingAction drivingAction)
	{
		
		switch (drivingAction) {
			case DrivingAction.Accelerate:
				return from a in Gen.Choose(1, 10).Select(v => (double)v / 10)
					select (a / 10);
			case DrivingAction.Coast:
				return from a in Gen.Choose(-1, 1).Select(v => (double)v / 10)
					select (a / 10);
			case DrivingAction.Halt:
				return from a in Gen.Choose(0,0).Select(v => (double)v / 10)
					select (a / 10);
			case DrivingAction.Brake:
				return from a in Gen.Choose(-10,-1).Select(v => (double)v / 10)
					select (a / 10);
			case DrivingAction.Roll:
				return from a in Gen.Choose(-1, 1).Select(v => (double)v / 10)
					select (a / 10);
			default: throw new NotImplementedException();
		}
	}
	
	internal static Arbitrary<DriverState> DriverState
	{
		get
		{
			var gen =
				from action in Gen.Elements(Enum.GetValues<DrivingAction>())
				from acc in Acceleration(action)
				select new DriverState(action, acc.SI<MeterPerSquareSecond>());
			return gen.ToArbitrary();
		}
	}

	internal static Arbitrary<GeneralState> GeneralState
	{
		get
		{
			var gen = from targetSpeed in Gen.Choose(0, 85)
				select new GeneralState(targetSpeed.KMPHtoMeterPerSecond());
			return gen.ToArbitrary();
		}
	}

	internal static Arbitrary<Request> Request =>
	(
		from outTorque in ArbMap.Default.GeneratorFor<NormalFloat>()
		from outSpeed in ArbMap.Default.GeneratorFor<NormalFloat>()
		from dryRun in ArbMap.Default.GeneratorFor<bool>()
		select new Request(
			1.SI<Second>(),
			0.5.SI<Second>(),
			outTorque.Get.SI<NewtonMeter>(),
			outSpeed.Get.SI<PerSecond>(),
			dryRun)
	).ToArbitrary();

	internal static Arbitrary<PreviousState> PreviousState => 
	(
		from outTorque in ArbMap.Default.GeneratorFor<NormalFloat>()
		from outSpeed in ArbMap.Default.GeneratorFor<NormalFloat>()
		select new PreviousState(outTorque.Get.SI<NewtonMeter>(), outTorque.Get.SI<PerSecond>())
	).ToArbitrary();
}