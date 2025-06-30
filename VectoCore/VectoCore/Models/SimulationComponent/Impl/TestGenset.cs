using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TestGenset : ITestGenset
	{

		public ISimpleVehicleContainer Container;
		public ITestpowertrainCombustionEngine CombustionEngine { get; }
		public ITestpowertrainElectricMotor ElectricMotor => _em;
		public IGensetMotorController ElectricMotorCtl { get; }

		public Joule EM_ThermalBuffer {
			set { _em.SetThermalBuffer = value; }
		}

		public bool EM_DeRatingActive {
			set { _em.SetDeRatingActive = value; }
		}

		public PerSecond EM_DrivetrainSpeed {
			set { _em.GetPreviousState.DrivetrainSpeed = value; }
		}

		public PerSecond EM_Speed {
			set { _em.GetPreviousState.EMSpeed = value; }
		}

		public IAuxPort EngineAux { get; }

		public IElectricEnergyStorage Battery { get; }
		public IElectricEnergyStorage BatterySystem { get; }
		public IElectricEnergyStorage SuperCap { get; }
		private readonly ITestpowertrainElectricMotor _em;

		public TestGenset(ISimpleVehicleContainer container, IDataBus realContainer)
		{
			Container = container;
			CombustionEngine = Container.EngineInfo as ITestpowertrainCombustionEngine;
			EngineAux = CombustionEngine?.GetEngineAux;
			_em = container.ElectricMotors.FirstOrDefault(x => x.Key == PowertrainPosition.GEN).Value as ITestpowertrainElectricMotor;
			ElectricMotorCtl = _em.Control as IGensetMotorController;

			Battery = Container.BatteryInfo as Battery;
			BatterySystem = container.BatteryInfo as BatterySystem;

			SuperCap = Container.BatteryInfo as SuperCap;
		}

		public NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter tq, bool dryRun)
		{
			return _em.ConvertEmTorqueToDrivetrain(emSpeed, tq, dryRun);
		}

		public PerSecond ConvertEmSpeedToDrivetrain(PerSecond emSpeed)
		{
			return _em.ConvertEmSpeedToDrivetrain(emSpeed);
		}
	}
}