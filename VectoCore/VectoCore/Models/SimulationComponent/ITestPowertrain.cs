using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent
{

    public interface ITestPowertrain 
    {
        void UpdateComponents();

		ITestPowertrainVehicle Vehicle { get; }

        IList<ITestPowertrainTransmission> Gearboxes { get; }

        ITestPowertrainTransmission GetGearbox(int axleNumber);

        ITestpowertrainCombustionEngine CombustionEngine { get; }

        ISimpleVehicleContainer Container { get; }

        ISimpleHybridController HybridController { get; }

        IAuxPort EngineAux { get; }

        IClutch Clutch { get; }

        IBrakes Brakes { get; }

        ITestpowertrainElectricMotor GetElectricMotor(int axleNumber);
		IList<ITestpowertrainElectricMotor> ElectricMotors { get; } 
		IList<IElectricMotor> ElectricMotorsUpstreamTransmission { get; }
        IDCDCConverter DCDCConverter { get; }
        IList<ITorqueConverter> TorqueConverters { get; }
		ITestpowertrainGensetChargerAdapter Charger { get; }
		IRESSInfo BatterySystem { get; }

    }

    public interface ITestGenset
    {
		ITestpowertrainCombustionEngine CombustionEngine { get; }

		ITestpowertrainElectricMotor ElectricMotor { get; }

        IGensetMotorController ElectricMotorCtl { get; }

        IElectricEnergyStorage Battery { get; }
		IElectricEnergyStorage BatterySystem { get; }
        IElectricEnergyStorage SuperCap { get; }
        Joule EM_ThermalBuffer { set; }
        bool EM_DeRatingActive { set; }
        PerSecond EM_DrivetrainSpeed { set; }
        PerSecond EM_Speed { set; }

        IAuxPort EngineAux { get; }

        NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter tq, bool dryRun);
        PerSecond ConvertEmSpeedToDrivetrain(PerSecond emSpeed);
    }
}