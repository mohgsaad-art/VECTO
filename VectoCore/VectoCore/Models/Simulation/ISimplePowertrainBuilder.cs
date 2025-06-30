using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Models.Simulation
{
    public interface ISimplePowertrainBuilder
	{
		//ITestPowertrain CreateTestPowertrain(ISimpleVehicleContainer testContainer, IDataBus realContainer, bool createDriver);
		ITestPowertrain CreateTestPowertrain(IVehicleContainer realContainer, bool createDriver, VectoSimulationJobType overrideJobType);
		ITestPowertrain CreateTestPowertrain(IVehicleContainer realContainer, bool createDriver);
        ITestGenset CreateTestGenset(IVehicleContainer realContainer);

        /// <summary>
        /// Builds a simple conventional powertrain.
        /// <code>
        ///(MeasuredSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     ├(Angledrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └ATGearbox or Gearbox
        ///      ├(TransmissionInputRetarder)
        ///      ├(Clutch)
        ///      └CombustionEngine
        ///       └(Aux)
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimplePowertrain(VectoRunData data);

		//ISimpleVehicleContainer BuildSimpleHybridPowertrainGear(VectoRunData data);

        /// <summary>
        /// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
        /// <code>
        /// Vehicle
        /// └Wheels
        ///  └SimpleHybridController
        ///   └Brakes
        ///    │ └Engine E4
        ///    └AxleGear
        ///     │ ├(AxlegearInputRetarder)
        ///     │ └Engine E3
        ///     ├(AngleDrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └Gearbox or APTNGearbox
        ///      ├(TransmissionInputRetarder)
        ///      └Engine E2
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimpleSerialHybridPowertrain(VectoRunData data);

        /// <summary>
        /// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
        /// <code>
        /// Vehicle
        /// └Wheels
        ///  └SimpleHybridController
        ///   └Brakes
        ///    │ └Engine E4
        ///    └AxleGear
        ///     │ ├(AxlegearInputRetarder)
        ///     │ └Engine E3
        ///     ├(AngleDrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └Gearbox or APTNGearbox
        ///      ├(TransmissionInputRetarder)
        ///      └Engine E2
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimpleIEPCHybridPowertrain(VectoRunData data);

        /// <summary>
        /// Builds a simple genset
        /// <code>
        /// Engine Gen
        ///  └CombustionEngine
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimpleGenSet(VectoRunData data);

        /// <summary>
        /// Builds a simple hybrid powertrain.
        ///<code>
        /// (MeasuredSpeedDrivingCycle)
        ///  └Vehicle
        ///   └Wheels
        ///    └SimpleHybridController
        ///     └Brakes
        ///      ├(Engine P4)
        ///      └AxleGear
        ///       ├(Engine P3)
        ///       ├(Angledrive)
        ///       ├(TransmissionOutputRetarder)
        ///       └Gearbox, ATGearbox, or APTNGearbox
        ///        ├(TransmissionInputRetarder)
        ///        ├(Engine P2.5)
        ///        ├(Engine P2)
        ///        ├(SwitchableClutch)
        ///        ├(Engine P1)
        ///        └StopStartCombustionEngine
        ///         └(Aux)
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimpleHybridPowertrain(VectoRunData data);

        /// <summary>
        /// Builds a simple battery electric powertrain for PEVs.
        /// <code>
        /// (Dummy MeasureSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     └ATGearbox or Gearbox
        ///      └Electric Motor
        /// </code>
        /// </summary>
		//ISimpleVehicleContainer BuildSimplePowertrainElectric(VectoRunData data);
	}
}