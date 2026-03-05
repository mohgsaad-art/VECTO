using System.ComponentModel.Design.Serialization;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using Moq;
using Moq.Language;
using NLog.LayoutRenderers.Wrappers;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy.PropertyBased;



using FsCheck.NUnit;
[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP4)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP4)]
[Category("Mutation")]
public class SelectBestOptionPropertyBased : HybridStrategyBase
{
	private PowertrainPosition _powertrainPosition;
	private GearboxType _gearboxType;
	public SelectBestOptionPropertyBased(GearboxType gearboxType, PowertrainPosition powertrainPosition)
	{
		_gearboxType = gearboxType;
		_powertrainPosition = powertrainPosition;
	}
	
	private void UpdateEntries(List<HybridResultEntry> entries)
	{
		entries.ForEach(e => e.Setting.MechanicalAssistPower = e.Setting.MechanicalAssistPower
			.Select(kv => (_powertrainPosition, kv.Value)).ToDictionary(tuple => tuple._powertrainPosition, t => t.Value));
	}

	// [Property(Arbitrary = new[] { typeof(Generators) }, MaxTest = 10000, EndSize=100, StartSize = 100, QuietOnSuccess = false,
	// 	Replay = ("(4352629797490505192,9061317540283626617,21)"))]
	// public Property SelectBestScoreReplay(List<HybridResultEntry> entries, VehicleState vehicleState,
	// 	DriverState driverState,
	// 	GeneralState generalState)
	// {
	// 	return SelectBestScore(entries, vehicleState, driverState, generalState);
	// }
	//
	//
	
	//
	// public Task NullOnZeroEntriesTest()
	// {
	// 	Prop.ForAll(() => {
	// 		from entries in Generators.HybridResultEntries
	// 		
	// 		NullOnZeroEntries(Generators.HybridResultEntries.Generator.Resize(s => s).ToArbitrary())
	// 		
	// 		
	// 		
	// 		
	// 		
	// 	})
	// 	
	// }

	// [Test]
	// public void NullOnZeroEntriesTest()
	// {
	// 	var td = Generators.TestData;
	//
	// 	var commonPropsGen = Generators.CommonResultPropertiesGen;
	// 	var emptyGen = (
	// 		from common in commonPropsGen
	// 		from entry in Generators.HybridResultEntry(common).Generator.ListOf(0)
	// 		select entry);
	// 	
	// 	
	//
	// }
	[Test]
	public void NullOnZeroEntries()
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		var gen = (
			from td in Generators.TestData.Generator
			select td with { Entries = new List<HybridResultEntry>() }).ToArbitrary();

		var prop = Prop.ForAll(gen, (t) => {
			var selected = RunDoSelectBestOption(t, runData);
			return (selected == null);
		});

		prop.QuickCheckThrowOnFailure();

				
	}

	[TestCase()]
	// [TestCase("(17585389231755444099,17554596621042844791)", 8)]
	public void VehicleSpeedBelowCycleStartspeed(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		
		var slowVehicleGen = (
			from vehicleState in Generators.VehicleState
			from slowSpeed in Gen.Choose(0, (int)runData.GearshiftParametersSinglePwt.StartSpeed.ConvertToKiloMeterPerHour().Value)
			select vehicleState with { Speed = slowSpeed.KMPHtoMeterPerSecond() }
		);
		
		var gen =
			(from entries in Generators.HybridResultEntries.Generator
			from vehicleState in Gen.OneOf(Generators.VehicleState, slowVehicleGen)
			from driverState in Generators.DriverState.Generator
			from generalState in Generators.GeneralState.Generator
			select new TestData(entries, vehicleState, driverState, generalState)).ToArbitrary();

		var prop = Prop.ForAll(gen, (t) => {
			var selected = RunDoSelectBestOption(t, runData);

			var possibleEntries = t.Entries.Where(e => !e.IgnoreReason.EngineSpeedTooHigh() && !e.Score.IsNaN()).ToList();
			var speedBelowPreCondition = t.VehicleState.Speed.IsSmallerOrEqual(runData.GearshiftParametersSinglePwt.StartSpeed)
								&& t.VehicleState.Speed.IsSmaller(t.GeneralState.CycleTargetSpeed,
									1.KMPHtoMeterPerSecond());

			return (selected == possibleEntries.MinBy(e => e.Score))
				.When(possibleEntries.Any() && speedBelowPreCondition).Label($"Selected {selected} \n from {t.Entries.Join("\n")}");
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.QuickCheckThrowOnFailure();
		}
	}
	
	[TestCase()]
	public void EntryWithBestScoreOkEntries(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);

		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from length in Gen.Choose(1, 19)
			from entries in Gen.OneOf(
				Generators.HybridResultEntry(commonProperties, (r) => r.AllOK()), 
				Generators.HybridResultEntry(commonProperties, (r) => true)).ListOf(length)
			select td with { Entries = entries};
			

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);

			var possibleEntries = t.Entries.Where(e => e.IgnoreReason.AllOK() && !e.Score.IsNaN()).ToList();
			var speedBelowPreCondition = t.VehicleState.Speed.IsSmallerOrEqual(runData.GearshiftParametersSinglePwt.StartSpeed)
								&& t.VehicleState.Speed.IsSmaller(t.GeneralState.CycleTargetSpeed,
									1.KMPHtoMeterPerSecond());

			return (selected == possibleEntries.MinBy(e => e.Score))
				.When(possibleEntries.Any() && !speedBelowPreCondition).Label($"Selected {selected} \n from {t.Entries.Join("\n")}");
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.QuickCheckThrowOnFailure();
		}
	}
	
	[TestCase()]
	// [TestCase("(198496771738469811,10245918802050154393)",94)]
	public void EntryWhereTorqueAndSpeedAreOkayDuringBraking(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);

		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from length in Gen.Choose(1, 19)
			from ignoreReason in Gen.OneOf(Gen.Elements(
				Enum.GetValues<HybridConfigurationIgnoreReason>().Where(e => !e.AllOK())
				))
			from entries in Gen.ListOf(Generators.HybridResultEntry(commonProperties, (e) => !e.AllOK()), length )
			select td with { Entries = entries};
			

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);

			var possibleEntriesBraking = t.Entries.Where(e => 
				!e.IgnoreReason.EngineSpeedTooLow()
				&& !e.IgnoreReason.EngineSpeedTooHigh()
				&& e.IgnoreReason.EngineTorqueOK()
				&& !e.Score.IsNaN()).ToList();
			
			var speedBelowPre = t.VehicleState.Speed.IsSmallerOrEqual(runData.GearshiftParametersSinglePwt.StartSpeed)
								&& t.VehicleState.Speed.IsSmaller(t.GeneralState.CycleTargetSpeed,
									1.KMPHtoMeterPerSecond());

			return (selected == possibleEntriesBraking.MinBy(e => e.Score))
				.When(possibleEntriesBraking.Any() && !t.VehicleState.BrakePower.IsEqual(0) && !speedBelowPre);
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.QuickCheckThrowOnFailure();
		}
	}
	
	[TestCase()]
	public void EntryWhereEngineSpeedIsOkWhenNotBraking(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);

		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from length in Gen.Choose(1, 19)
			from entries in Gen.ListOf(
				Gen.OneOf(
					Generators.HybridResultEntry(commonProperties, (e) => !e.AllOK()),
					Generators.HybridResultEntry(commonProperties, (e) => !e.AllOK() && !e.InvalidEngineSpeed())
					)
				, length )
			select td with { Entries = entries};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);

			var possibleEntries = t.Entries.Where(e => 
					!e.IgnoreReason.InvalidEngineSpeed()
					&& !e.Score.IsNaN()).ToList();

			return (selected == possibleEntries.MinBy(e => e.Score))
				.When(possibleEntries.Any() 
					&& t.VehicleState.BrakePower.IsEqual(0) 
					&& t.VehicleState.Speed.IsGreater(VectoMath.Max(runData.GearshiftParametersSinglePwt.StartSpeed, t.GeneralState.CycleTargetSpeed)));
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.QuickCheckThrowOnFailure();
		}
	}
	
	[TestCase()]
	// [TestCase("9444999824428182450,6857404200518031345", 2)]
	// [TestCase("(1451191327976930145,17499268966435901719)",26)]
	// [TestCase("((13161554455205860253,17113508837484764927)", 94)]
	public void BestEntryWhereEngineSpeedIsTooHighButGearDiffers(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);

		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from length in Gen.Choose(1, 19)
			from ignoreReason in Gen.OneOf(Gen.Elements(
				Enum.GetValues<HybridConfigurationIgnoreReason>()
					.Where(e => !e.AllOK())
					.Where(e => e.InvalidEngineSpeed())
			))
			from entries in Gen.ListOf(Generators.HybridResultEntry(commonProperties,r => !r.AllOK() && r.InvalidEngineSpeed()), length)
			select td with {
				Entries = entries,
				VehicleState = td.VehicleState with {BrakePower = 0.SI<Watt>()}
			};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);

			var bestEntry = t.Entries.Where(e => !e.Score.IsNaN()).MinBy(e => e.Score);
			// var possibleEntries = t.Entries.Where(e => 
			// 								!(!e.Gear.Equals(t.VehicleState.Gear) && e.IgnoreReason.EngineSpeedTooHigh())
			// 								&& !e.Score.IsNaN()).ToList();
			
			
			var speedBelowPre = t.VehicleState.Speed.IsSmallerOrEqual(runData.GearshiftParametersSinglePwt.StartSpeed)
								&& t.VehicleState.Speed.IsSmaller(t.GeneralState.CycleTargetSpeed,
									1.KMPHtoMeterPerSecond());
			
			return (bestEntry != null && !(!bestEntry.Gear.Equals(t.VehicleState.GearboxState.Gear) && bestEntry.IgnoreReason.EngineSpeedTooHigh()))
				.MyImplies(() => selected == bestEntry)
				.Label(t.Entries, selected)
				.When(bestEntry != null && !speedBelowPre);
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.VerboseThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.QuickCheckThrowOnFailure();
		}
	}
	
	
	[TestCase()]
	// [TestCase("(3156536313627332916,4162818377255040143)",9)]
	public void BestEntryWhenAllAreOverload(string? replay=null, int? size=null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		
		

		var modifier = (HybridResultEntry entry, GearshiftPosition vehicleGearshiftPosition) => {
			// (best != null && (GearEqual || !Enginespeed too high)
			if (!entry.Gear.Equals(vehicleGearshiftPosition)) {
				entry.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			} else {
				entry.FuelCosts = double.NaN;
			}

			return entry;
		};
		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from drivingAction in Gen.Elements(DrivingAction.Accelerate)//, DrivingAction.Roll)
			from length in Gen.Choose(1, 50)
			from engaged in Gen.Frequency<bool>((1, Gen.Constant(false)), (3, Gen.Constant(true)))
			from entries in Gen.ListOf(
				Gen.OneOf(
					Generators.HybridResultEntry(commonProperties,
						reason => reason.InvalidEngineSpeed() && reason.BatteryDemandExceeded() && reason.EngineSpeedTooLow() && reason.EngineTorqueDemandTooHigh(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					Generators.HybridResultEntry(commonProperties,
						reason => reason.InvalidEngineSpeed() && reason.BatterySoCTooLow() && reason.EngineSpeedTooLow() && reason.EngineTorqueDemandTooHigh(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					Generators.HybridResultEntry(commonProperties, 
						reason => reason.EngineTorqueDemandTooHigh(), (entry => {
							entry.FuelCosts = double.NaN;
							return entry;
						}))
					
					// Generators.HybridResultEntry(commonProperties,
					//
					// 				reason => reason.BatterySoCTooLow() && reason.EngineSpeedTooLow(), entry => {
     //            						if(!entry.Gear.Equals((td.VehicleState.GearboxState with {GearEngaged = engaged}).Gear))
     //            						{
     //            							entry.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
     //            						}
     //            						return entry;
     //            					})), 
			),	length)
			select td with {
				Entries = entries,
				DriverState = td.DriverState with {Action = drivingAction},
				VehicleState = td.VehicleState with {GearboxState = td.VehicleState.GearboxState with {GearEngaged = engaged}}
			};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {

			var selected = RunDoSelectBestOption(t, runData);
			
			var tmpGuard = true;
			
			//Make sure that the best entry doesnt meet the NAND(!differentGear, EnginespeedTooHigh) condition
			var bestEntry = t.Entries.Where(e => !e.Score.IsNaN()).MinBy(e => e.Score);
			var generalPrecondition = !(bestEntry != null && !(!bestEntry.Gear.Equals(t.VehicleState.GearboxState.Gear) &&
									bestEntry.IgnoreReason.EngineSpeedTooHigh()));

			tmpGuard = tmpGuard && generalPrecondition;
			
			var entries = t.Entries;
			var driverState = t.DriverState;
			var vehicleState = t.VehicleState;
//All entries that have a response
			var entriesWhereResponseIsNotNull = entries.Where(e => e.Response != null).ToList();
			var areEntriesAvailable = entriesWhereResponseIsNotNull.Any();
			
			
			var entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow = entriesWhereResponseIsNotNull.Where(e =>
				!(e.IgnoreReason.BatteryDemandExceeded() || e.IgnoreReason.BatterySoCTooLow())).ToList();
			var allOverload = entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow.All(x =>
					x.IgnoreReason.EngineTorqueDemandTooHigh()); //<- Evaluates to true if the list is empty
			var electricMotorCanPropellDuringTractionInterruption =
				_powertrainPosition == PowertrainPosition.HybridP4 ||
				_powertrainPosition == PowertrainPosition.HybridP3;
	
			
			var entriesWhereBatteryDemandIsNotExceeded = entriesWhereResponseIsNotNull.Where(e =>
					!e.IgnoreReason.BatteryDemandExceeded()).ToList();
			
			var overLoadFallback1PreCondition = tmpGuard
								&& (driverState.Action == DrivingAction.Accelerate && allOverload) 
								&& (electricMotorCanPropellDuringTractionInterruption || vehicleState.GearboxState.GearEngaged)
								&& !entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow.Any()
								&& (entriesWhereBatteryDemandIsNotExceeded.Any());
			
			var overloadFallback1 = () => selected == 
										entriesWhereBatteryDemandIsNotExceeded
											.OrderBy(e => Math.Abs(runData.GearboxSinglePwt.GearList.Distance(e.Gear, vehicleState.GearboxState.Gear)))
											.ThenBy(x => -x.Response.ElectricSystem.RESSPowerDemand.Value()).First();
	
			tmpGuard = tmpGuard && !overLoadFallback1PreCondition;
	
			var overLoadFallback1ReturnsNullPreCondition = tmpGuard
												&& (driverState.Action == DrivingAction.Accelerate && allOverload) 
												&& (electricMotorCanPropellDuringTractionInterruption || vehicleState.GearboxState.GearEngaged)
												&& (!entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow.Any())
												&& (!entriesWhereBatteryDemandIsNotExceeded.Any());

			var overloadFallback1ReturnsNull = () => selected == null;

			
			tmpGuard = tmpGuard && !overLoadFallback1ReturnsNullPreCondition;
			
			
			var overloadValidEntries =
				entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow.Where(e =>
					!e.IgnoreReason.InvalidEngineSpeed());
	
			var overloadFallback2Precondition = tmpGuard
												&& (driverState.Action == DrivingAction.Accelerate && allOverload)
												&& (electricMotorCanPropellDuringTractionInterruption ||
													vehicleState.GearboxState.GearEngaged)
												&& (!overloadValidEntries.Any());
			var overloadFallback2 = () => selected ==
																			entriesWhereBatteryDemandIsNotExceededAndSocIsNotTooLow
																				.OrderBy(e =>
																					Math.Abs(runData.GearboxSinglePwt
																						.GearList.Distance(e.Gear,
																							vehicleState.GearboxState.Gear)))
																				.ThenBy(x =>
																					x.Setting.MechanicalAssistPower
																						.Sum(e => e.Value.Item2)).First();



			var defaultCasePrecondition = !(overLoadFallback1PreCondition || overLoadFallback1ReturnsNullPreCondition ||
								overloadFallback2Precondition);

			var defaultCase = Prop.Label(() => {
				return selected == entries.Where(e => {
					var reason = e.IgnoreReason;
					return (!reason.BatteryDemandExceeded() && !reason.BatterySoCTooLow() &&
							!reason.InvalidEngineSpeed());
					})
					.OrderBy(e => Math.Abs(runData.GearboxSinglePwt.GearList.Distance(t.VehicleState.GearboxState.Gear, e.Gear)))
					.ThenBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>())).First();
			}, "default case").When(defaultCasePrecondition).Classify(defaultCasePrecondition, nameof(defaultCasePrecondition));
			
			

			var fallback1 =  Prop.Label(overloadFallback1, nameof(overloadFallback1)).When(overLoadFallback1PreCondition)
				.Classify(overLoadFallback1PreCondition, nameof(overloadFallback1));
			
			var returnsNull =  Prop.Label((Func<bool>)overloadFallback1ReturnsNull, nameof(overloadFallback1ReturnsNull)).When(overLoadFallback1ReturnsNullPreCondition)
				.Classify(overLoadFallback1ReturnsNullPreCondition, nameof(overloadFallback1ReturnsNull));

			var fallback2 = Prop.Label(overloadFallback2, nameof(overloadFallback2)).When(overloadFallback2Precondition)
				.Classify(overloadFallback2Precondition, nameof(overloadFallback2));
			
			
			
			Assert.That(generalPrecondition);


			return (fallback1.Or(returnsNull).Or(fallback2).Or(defaultCase)).When(areEntriesAvailable);

		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.Check(Config.QuickThrowOnFailure.WithMaxTest(100));
		}
	}


	[TestCase()]
	[TestCase("(11556810022805973882,5378051850766989109)",16)]
	public void BestEntryWhenAllAreUnderload(string? replay = null, int? size = null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		
		

		var modifier = (HybridResultEntry entry, GearshiftPosition vehicleGearshiftPosition) => {
			// (best != null && (GearEqual || !Enginespeed too high)
			if (!entry.Gear.Equals(vehicleGearshiftPosition)) {
				entry.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			} else {
				entry.FuelCosts = double.NaN;
			}

			return entry;
		};
		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from drivingAction in Gen.Elements(DrivingAction.Accelerate)//, DrivingAction.Roll)
			from length in Gen.Choose(1, 50)
			from engaged in Gen.Frequency<bool>((1, Gen.Constant(false)), (10, Gen.Constant(true)))
			from entries in Gen.ListOf(
				Gen.OneOf(
					// Generators.HybridResultEntry(commonProperties,
					// 	reason => reason.InvalidEngineSpeed() && !reason.BatteryDemandExceeded() && reason.EngineSpeedTooLow() && reason.EngineTorqueDemandTooLow() && !reason.EngineTorqueDemandTooHigh(),
					// 	(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					Generators.HybridResultEntry(commonProperties,
						reason => reason.InvalidEngineSpeed() && !reason.BatteryDemandExceeded() && reason.EngineSpeedTooLow() && reason.EngineTorqueDemandTooLow() && !reason.EngineTorqueDemandTooHigh(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					Generators.HybridResultEntry(commonProperties, 
						reason => reason.EngineTorqueDemandTooLow() && !reason.EngineTorqueDemandTooHigh(), (entry => {
							entry.FuelCosts = double.NaN;
							return entry;
						}))
			),	length)
			select td with {
				Entries = entries,
				DriverState = td.DriverState with {Action = drivingAction},
				VehicleState = td.VehicleState with {GearboxState = td.VehicleState.GearboxState with {GearEngaged = engaged}}
			};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {

			
			
			
			var selected = RunDoSelectBestOption(t, runData);
			
			
			var entries = t.Entries.Where(e => e.Response != null);
			var allOverload = entries.Where(x => !(x.IgnoreReason.BatteryDemandExceeded() || x.IgnoreReason.BatterySoCTooLow()))
				.All(x => x.IgnoreReason.EngineTorqueDemandTooHigh());
			var electricMotorCanPropellDuringTractionInterruption =
				_powertrainPosition == PowertrainPosition.HybridP4 ||
				_powertrainPosition == PowertrainPosition.HybridP3;
			
			var generalPrecondition = t.DriverState.Action == DrivingAction.Accelerate &&
								entries.All(e => e.IgnoreReason.EngineTorqueDemandTooLow())
								&& (electricMotorCanPropellDuringTractionInterruption || t.VehicleState.GearboxState.GearEngaged) && !allOverload;

			var candidateEntries = t.Entries.Where(e => !e.IgnoreReason.InvalidEngineSpeed() && !e.IgnoreReason.BatteryDemandExceeded());
			var defaultPrecondition = generalPrecondition && candidateEntries.Any();
			
			

			var fallbackEntries = t.Entries.Where(e => !e.IgnoreReason.BatteryDemandExceeded());
			var fallbackPrecondition = !t.Entries.Any() && fallbackEntries.Any() && generalPrecondition;
			
			
			var defaultCase = Prop.Label(() => {
				return selected == candidateEntries.MaxBy(x =>
							x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>())) &&
						selected != null;

			}, "defaultCase").When(generalPrecondition && candidateEntries.Any())
				.Classify(defaultPrecondition, nameof(defaultPrecondition))
				.Classify(!generalPrecondition, $"!{nameof(generalPrecondition)}");

			var fallbackCase = Prop.Label(() => {
				return selected == fallbackEntries.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>())) &&
						selected != null;
				
			}, "fallback").Classify(fallbackPrecondition, nameof(fallbackPrecondition));
			
			







			return defaultCase.Or(fallbackCase);
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.Check(Config.QuickThrowOnFailure.WithMaxTest(100));
		}
	}
	
	
	[TestCase()]
	public void EngagedAndAccelerating(string? replay = null, int? size = null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);

		var modifier = (HybridResultEntry entry, GearshiftPosition vehicleGearshiftPosition) => {
			// (best != null && (GearEqual || !Enginespeed too high)
			if (!entry.Gear.Equals(vehicleGearshiftPosition)) {
				entry.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			} else {
				entry.FuelCosts = double.NaN;
			}

			return entry;
		};
		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from drivingAction in Gen.Elements(Enum.GetValues<DrivingAction>().Where(a => a == DrivingAction.Accelerate))//, DrivingAction.Roll)
			from length in Gen.Choose(1, 50)
			from engaged in Gen.Frequency<bool>((1, Gen.Constant(false)), (10, Gen.Constant(true)))
			from entries in Gen.ListOf(
				Gen.OneOf(
					Generators.HybridResultEntry(commonProperties,
						reason => !reason.AllOK() 
								&& reason.EngineTorqueDemandTooHigh() 
								&& !reason.BatteryDemandExceeded() 
								&& !reason.EngineSpeedTooLow() 
								&& !reason.EngineSpeedTooHigh()
								&& !reason.BatteryBelowMinSoC(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					
					Generators.HybridResultEntry(commonProperties,
						reason => !reason.AllOK() 
								&& reason.EngineTorqueDemandTooHigh() 
								&& !reason.EngineSpeedTooLow() 
								&& !reason.EngineTorqueDemandTooLow()
								&& !reason.BatteryBelowMinSoC(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear))),
					
					Generators.HybridResultEntry(commonProperties,
						reason => !reason.AllOK() 
								&& reason.EngineTorqueDemandTooHigh() 
								&& !reason.BatteryDemandExceeded() 
								&& !reason.EngineSpeedTooLow() 
								&& !reason.EngineSpeedTooHigh()
								&& !reason.EngineTorqueDemandTooLow()
								&& !reason.BatteryBelowMinSoC(),
						(entry => modifier(entry, td.VehicleState.GearboxState.Gear)))
			),	length)
			select td with {
				Entries = entries,
				DriverState = td.DriverState with {Action = drivingAction},
				VehicleState = td.VehicleState with {GearboxState = td.VehicleState.GearboxState with {GearEngaged = engaged}}
			};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);
			
			var batOkEntries = t.Entries.Where(e => !e.IgnoreReason.BatteryDemandExceeded()).ToArray();


			var filterByGear = (IEnumerable<HybridResultEntry> entries) =>
				entries.Where(e => e.Gear.Equals(t.VehicleState.GearboxState.Gear));

			var entriesAllFiltersApplied = filterByGear(t.Entries.Where(e =>
				!e.IgnoreReason.BatteryBelowMinSoC()
				&& !e.IgnoreReason.EngineTorqueDemandTooLow()
				&& !e.IgnoreReason.EngineSpeedTooLow()
				&& !e.IgnoreReason.EngineSpeedTooHigh()
				&& !e.IgnoreReason.BatteryDemandExceeded())).ToList();


			
			var allFiltersAppliedPreCondition = entriesAllFiltersApplied.Any();
			
			var propAllFiltersApplied = Prop.Label(() => {
					if (t.DriverState.Action == DrivingAction.Brake) {
						return selected == entriesAllFiltersApplied.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));	
					}
					return selected == entriesAllFiltersApplied.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				}, "All Filters")
				.When(allFiltersAppliedPreCondition).MyClassify(allFiltersAppliedPreCondition);

			
			
			var entriesBatteryNotBelow = filterByGear(t.Entries.Where(e =>
				!e.IgnoreReason.EngineTorqueDemandTooLow()
				&& !e.IgnoreReason.EngineSpeedTooLow()
				&& !e.IgnoreReason.BatteryBelowMinSoC())).ToList();

			var batteryNotBelowMinSoCPreCondition =
				!allFiltersAppliedPreCondition;

			var propBatteryNotBelowMinSoc =
				Prop.Label(() => {
					if (t.DriverState.Action == DrivingAction.Brake) {
						return selected == entriesBatteryNotBelow.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));	
					}
					return selected == entriesBatteryNotBelow.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				}, "Entries with BatteryBelowMinSoC are included").When(batteryNotBelowMinSoCPreCondition).MyClassify(batteryNotBelowMinSoCPreCondition);

			var braking = t.DriverState.Action == DrivingAction.Brake;

			var entriesTorqueFilterIgnored = filterByGear(t.Entries.Where(e =>
				!e.IgnoreReason.BatteryBelowMinSoC()
				&& !e.IgnoreReason.EngineSpeedTooLow()
				&& !e.IgnoreReason.EngineSpeedTooHigh()
				&& !e.IgnoreReason.BatteryDemandExceeded())).ToList();
			var torqueFilterIgnoredPreCondition = !(allFiltersAppliedPreCondition) && entriesTorqueFilterIgnored.Any();
			var propTorqueFilterIgnored =
				Prop.Label(() => {
					if (t.DriverState.Action == DrivingAction.Brake) {
						return selected == entriesTorqueFilterIgnored.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));	
					}
					return selected == entriesTorqueFilterIgnored.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				}, "Entries with BatteryBelowMinSoC are included").When(torqueFilterIgnoredPreCondition).MyClassify(torqueFilterIgnoredPreCondition);
			
			return (propAllFiltersApplied.Or(propBatteryNotBelowMinSoc).Or(propTorqueFilterIgnored)).MyClassify(braking).Classify(!braking, "not braking");
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.Check(Config.QuickThrowOnFailure.WithMaxTest(100));
		}
	}
	
	[TestCase]
	[TestCase("(14926914363821145547,15187610761312834343)", 2)]
	public void EngagedAndBraking(string? replay = null, int? size = null)
	{
		var runData = GetRunData(_powertrainPosition, _gearboxType);
		var nanModifier = (HybridResultEntry e) => {
			e.FuelCosts = double.NaN; 
			return e;
		};
		var gen =
			from td in Generators.TestData.Generator
			from commonProperties in Generators.CommonResultPropertiesGen
			from drivingAction in Gen.Elements(Enum.GetValues<DrivingAction>().Where(a => a == DrivingAction.Brake))//, DrivingAction.Roll)
			from length in Gen.Choose(1, 50)
			from engaged in Gen.Frequency<bool>((1, Gen.Constant(false)), (10, Gen.Constant(true)))
			from entries in Gen.ListOf(
				Gen.Frequency(	
					(1, Generators.HybridResultEntry(commonProperties,
					reason => !reason.AllOK() 
							&& !reason.BatteryDemandExceeded()
							&& !reason.InvalidEngineSpeed(), nanModifier)),
						(1, Generators.HybridResultEntry(commonProperties,
							reason => !reason.AllOK() 
									&& !reason.BatteryDemandExceeded()
									&& (reason.EngineSpeedTooLow() || reason.EngineSpeedTooHigh()), nanModifier)),
						(1, Generators.HybridResultEntry(commonProperties,
							reason => !reason.AllOK() 
									&& !reason.BatteryDemandExceeded()
									&& !(reason.EngineSpeedTooLow() || reason.EngineSpeedTooHigh()), nanModifier))
					//,
					// (1, Generators.HybridResultEntry(commonProperties,
					// 	reason => !reason.AllOK() 
					// 			&& !reason.BatteryDemandExceeded()
					// 			&& (reason.EngineSpeedTooLow() || reason.EngineSpeedTooHigh()),
					// 	(entry => modifier(entry, td.VehicleState.GearboxState.Gear))))
					),	length)
			select td with {
				Entries = entries,
				DriverState = td.DriverState with {Action = drivingAction},
				VehicleState = td.VehicleState with {GearboxState = td.VehicleState.GearboxState with {GearEngaged = engaged}}
			};

		var prop = Prop.ForAll(gen.ToArbitrary(), (t) => {
			var selected = RunDoSelectBestOption(t, runData);


			var entriesWhereBatteryDemandIsNotExceeded = t.Entries.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).OrderByDescending(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>())).ToList();
			var defaultCasePreCondition = entriesWhereBatteryDemandIsNotExceeded.Any() && !entriesWhereBatteryDemandIsNotExceeded.First().IgnoreReason.InvalidEngineSpeed();
			var propDefaultCase = Prop.Label(() => {
				return !selected.IgnoreReason.InvalidEngineSpeed() && !selected.IgnoreReason.BatteryDemandExceeded()
					&& selected == entriesWhereBatteryDemandIsNotExceeded.MaxBy(x =>
						x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
			}, "Default Case").When(defaultCasePreCondition).MyClassify(defaultCasePreCondition);

			var entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLow = t.Entries.Where(x => !x.IgnoreReason.BatteryDemandExceeded() && !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).OrderByDescending(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>())).ToList();
			var entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLowPreCondition =
				entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLow.Any() &&
				!defaultCasePreCondition;
			var maxOfEntriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNotTooHighOrTooLow = Prop.Label(() => {
				return selected == entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLow.MaxBy(x =>
																		x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
			}, "Default Case").When(entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLowPreCondition).MyClassify(entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLowPreCondition);


			var maxEntriesWhereBatteryDemandIsNotExceeded =
				t.Entries.Where(x => !x.IgnoreReason.BatteryDemandExceeded());
			var maxEntriesWhereBatteryDemandIsNotExceededPreCondition =
				maxEntriesWhereBatteryDemandIsNotExceeded.Any() &&
				!entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLowPreCondition;
			var maxEntriesWhereBatteryDemandIsNotExceededProp = Prop.Label(() => {
				return selected == entriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNeitherTooHighNorTooLow.MaxBy(x =>
					x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
			}, "Default Case").When(maxEntriesWhereBatteryDemandIsNotExceededPreCondition).MyClassify(maxEntriesWhereBatteryDemandIsNotExceededPreCondition);


			
			
			
			
			return propDefaultCase.Or(maxOfEntriesWhereBatteryDemandIsNotExceededAndEngineSpeedIsNotTooHighOrTooLow).Or(maxEntriesWhereBatteryDemandIsNotExceededProp);
		});

		if (replay != null && size.HasValue) {
			prop.Check(Config.QuickThrowOnFailure.WithReplay(replay, size.Value));
		} else {
			prop.Check(Config.QuickThrowOnFailure.WithMaxTest(100));
		}
	}
	
	
	[Test]
	public void TestIgnoreReasons()
	{
		var _maxIgnoreReason =
			Enum.GetValues<HybridConfigurationIgnoreReason>().Select(e => (int)e).Max();
		var ignoreReasons = Enumerable.Range(0, _maxIgnoreReason + 1).Select(i => (HybridConfigurationIgnoreReason)i).Select(r => r.HumanReadable()).ToList();

		TestContext.WriteLine(ignoreReasons.Join("\n"));
	}
	

	private HybridResultEntry? RunDoSelectBestOption(TestData testData, VectoRunData runData)
	{
		
		return RunDoSelectBestOption(testData.Entries, testData.VehicleState, testData.DriverState,
			testData.GeneralState, runData);
	}
	
	
	
	private HybridResultEntry? RunDoSelectBestOption(List<HybridResultEntry> entries, VehicleState vehicleState, DriverState driverState,
		GeneralState generalState, VectoRunData runData)
	{

		var container = GetVehicleContainer(runData);
		var powertrainContainer = new PowertrainContainer(container.Object);
		var strategy = new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(runData, container.Object, powertrainContainer);

		UpdateEntries(entries);
		
		container.SetVehicleState(vehicleState);

		container.SetDriverState(driverState);
		container.SetGeneralState(generalState);
			
		var selected = strategy.DoSelectBestOption(entries, null, null, null, null, false, vehicleState.GearboxState.Gear);
		return selected;
	}
	
	
}

