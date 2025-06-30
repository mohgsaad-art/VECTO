using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils;

public class MockEngine : VectoSimulationComponent, IEngineInfo, IEngineControl
{
	public MockEngine(IVehicleContainer container) : base(container) { }

	public PerSecond EngineSpeed { get; set; }
	public NewtonMeter EngineTorque { get; set; }

	public Watt EngineStationaryFullPower(PerSecond angularSpeed)
	{
		throw new System.NotImplementedException();
	}

	public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
	{
		throw new System.NotImplementedException();
	}

	public Watt EngineDragPower(PerSecond angularSpeed)
	{
		throw new System.NotImplementedException();
	}

	public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
	{
		throw new System.NotImplementedException();
	}

	public PerSecond EngineIdleSpeed { get; set; }
	public PerSecond EngineRatedSpeed { get; set; }
	public PerSecond EngineN95hSpeed { get; set; }
	public PerSecond EngineN80hSpeed { get; set; }

	public bool EngineOn { get; set; }

	protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
	{
		container[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
		container[ModalResultField.P_ice_out] = 0.SI<Watt>();
		container[ModalResultField.P_ice_inertia] = 0.SI<Watt>();

		container[ModalResultField.n_ice_avg] = 0.SI<PerSecond>();
		container[ModalResultField.T_ice_fcmap] = 0.SI<NewtonMeter>();

		container[ModalResultField.P_ice_full] = 0.SI<Watt>();
		container[ModalResultField.P_ice_drag] = 0.SI<Watt>();
		container[ModalResultField.T_ice_full] = 0.SI<NewtonMeter>();
		container[ModalResultField.T_ice_drag] = 0.SI<NewtonMeter>();

		container[ModalResultField.FCMap] = 0.SI<KilogramPerSecond>();
		container[ModalResultField.FCNCVc] = 0.SI<KilogramPerSecond>();
		container[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
		//container[ModalResultField.FCAAUX] = 0.SI<KilogramPerSecond>();
		container[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();
	}

	protected override void DoCommitSimulationStep(Second time, Second simulationInterval) { }

	#region Implementation of IEngineControl

	public bool CombustionEngineOn { get; set; }

	#endregion

	protected override bool DoUpdateFrom(object other) => false;

}