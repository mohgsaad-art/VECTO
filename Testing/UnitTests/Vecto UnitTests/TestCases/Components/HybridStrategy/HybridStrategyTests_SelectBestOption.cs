using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.AMT, PowertrainPosition.HybridP4)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP2)]
[TestFixture(GearboxType.ATSerial, PowertrainPosition.HybridP4)]
public class HybridStrategyTests_SelectBestOption : HybridStrategyBase
{
	private PowertrainPosition _powertrainPosition;
	private GearboxType _gearboxType;
	private readonly VectoRunData _rundata;

	public HybridStrategyTests_SelectBestOption(GearboxType gearboxType, PowertrainPosition powertrainPosition)
	{
		_gearboxType = gearboxType;
		_powertrainPosition = powertrainPosition;
		_rundata = GetRunData();
	}

	[TestCase]
	public void EmptyResults()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);


		var entries = new List<HybridResultEntry>();
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
				new GearshiftPosition(1));
		
		Assert.That(result, Is.Null);
	}
	
	
	
	/// <summary>
	/// No special case, just select the entry with the best score that is not ignored
	/// </summary>
	[TestCase]
	public void BestDefault()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		SetDrivingCycleTargetSpeed(container, 5.KMPHtoMeterPerSecond());
		

		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 8_000, //<- This one should be selected
				IgnoreReason = HybridConfigurationIgnoreReason.Evaluated | HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000, 
				IgnoreReason = HybridConfigurationIgnoreReason.Evaluated
			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				IgnoreReason = HybridConfigurationIgnoreReason.NotEvaluated
			}
		};
		
		
		Assume.That(entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(8_000));
		
	}
	
	/// <summary>
	/// 
	/// </summary>
	[TestCase]
	public void BestVehicleSpeedBelowGearshiftStartspeed()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		
		SetVehicleSpeed(container, DeclarationData.GearboxTCU.StartSpeed);
		SetDrivingCycleTargetSpeed(container, 10.KMPHtoMeterPerSecond());
		

		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 8_000,
				IgnoreReason = HybridConfigurationIgnoreReason.Evaluated | HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000,  //<- This one should be selected
				IgnoreReason = HybridConfigurationIgnoreReason.Evaluated
			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				IgnoreReason = HybridConfigurationIgnoreReason.NotEvaluated
			}
		};
		
		
		Assume.That(entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(9_000));
	}
	
	/// <summary>
	/// No special case, just select the entry with the best score that is not ignored
	/// </summary>
	[TestCase]
	public void BestBraking()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		
		SetVehicleSpeed(container, 9.KMPHtoMeterPerSecond());
		SetDrivingCycleTargetSpeed(container, 10.KMPHtoMeterPerSecond());
		SetBrakePower(container, 2.SI<Watt>());
		
		
		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 4_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 6_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 7_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 8_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000,  //<- This one should be selected
				// IgnoreReason = 
			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				IgnoreReason = HybridConfigurationIgnoreReason.NotEvaluated
			}
		};
		
		
		Assume.That(!entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(9_000));
	}
	
	/// <summary>
	/// No special case, just select the entry with the best score that is not ignored
	/// </summary>
	[TestCase]
	public void BestSwitchGears()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		
		SetVehicleSpeed(container, 9.KMPHtoMeterPerSecond());
		SetDrivingCycleTargetSpeed(container, 10.KMPHtoMeterPerSecond());

		var currentGear = new GearshiftPosition(1);
		

		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 4_000, //<- This one should be selected
				Gear = new GearshiftPosition(3),
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh | HybridConfigurationIgnoreReason.EngineSpeedTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 6_000,
				Gear = currentGear,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow | HybridConfigurationIgnoreReason.EngineSpeedTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 7_000,
				Gear = currentGear,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 8_000, 
				Gear= currentGear,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000,  
				Gear = new GearshiftPosition(3),
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh

			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				Gear = new GearshiftPosition(4),
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				Gear = new GearshiftPosition(5),
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooLow
			}
		};
		
		
		Assume.That(!entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(4_000));
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	[TestCase]
	public void BestNoInvalidEngineSpeed()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		
		SetVehicleSpeed(container, 9.KMPHtoMeterPerSecond());
		SetDrivingCycleTargetSpeed(container, 10.KMPHtoMeterPerSecond());
		SetBrakePower(container, 0.SI<Watt>());
		

		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 8_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000,  //<- This one should be selected
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				IgnoreReason = HybridConfigurationIgnoreReason.NotEvaluated
			}
		};
		
		
		Assume.That(!entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(9_000));
	}
	
	/// <summary>
	/// 
	/// </summary>
	[TestCase]
	public void BestAllOverload()
	{
		var container = GetVehicleContainer(_rundata).Object;
		var powertrainContainer = new PowertrainContainer(container);
		var hybridStrategy =
			new VectoCore.Models.SimulationComponent.Strategies.HybridStrategy(_rundata,
				container, powertrainContainer);
		
		SetVehicleSpeed(container, 9.KMPHtoMeterPerSecond());
		SetDrivingCycleTargetSpeed(container, 10.KMPHtoMeterPerSecond());
		SetBrakePower(container, 0.SI<Watt>());
		

		var entries = new List<HybridResultEntry>() {
			new HybridResultEntry() {
				FuelCosts = 8_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineSpeedTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 9_000,  //<- This one should be selected
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
			},
			new HybridResultEntry() {
				FuelCosts = 10_000,
				IgnoreReason = HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow
			},
			new HybridResultEntry() {
				FuelCosts = 11_000,
				IgnoreReason = HybridConfigurationIgnoreReason.NotEvaluated
			}
		};
		
		
		Assume.That(!entries.Any(x => x.IgnoreReason.AllOK()));

		var scores = entries.Select(e => e.Score);
		
		var result = hybridStrategy.DoSelectBestOption(
			entries, 
			1.SI<Second>(),
			0.5.SI<Second>(),
			0.SI<NewtonMeter>(),
			0.SI<PerSecond>(),
			false,
			new GearshiftPosition(1));
		
		Assert.That(result.FuelCosts, Is.EqualTo(9_000));
	}


	private VectoRunData GetRunData()
	{
		return GetRunData(_powertrainPosition, _gearboxType);
	}
}