using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class SimpleHybridController : VectoSimulationComponent, IHybridController, ITnInPort, ITnOutPort
	{

		//private SwitchableClutch clutch;
		private ElectricSystem ElectricSystem;

		protected readonly Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();
		public ITnOutPort NextComponent;

		private Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> _electricMotorTorque = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>();

		public SimpleHybridController(VehicleContainer container, ElectricSystem es) : base(container)
		{
			ElectricSystem = es;
			//this.clutch = clutch;
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			// nothgin to write - this is only used in test-container
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			// nothing to write - this is only used in test-container
		}

		#endregion

		#region Implementation of ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region Implementation of ITnOutProvider

		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region Implementation of IHybridController

		public IShiftStrategy ShiftStrategy => null;

		public SimpleComponentState PreviousState => throw new NotImplementedException();

		public IElectricMotorControl ElectricMotorControl(PowertrainPosition pos)
		{
			return _electricMotorCtl[pos];
		}

		public void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorData)
		{
			if (_electricMotorCtl.ContainsKey(pos)) {
				throw new VectoException("Electric motor already registered as position {0}", pos);
			}

			_electricMotorCtl[pos] = new ElectricMotorController(this, motorData);
		}

		public IHybridControlStrategy Strategy => null;

		//public ResponseDryRun RequestDryRun(
		//	Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
		//	HybridStrategyResponse strategySettings)
		//{
		//	throw new System.NotImplementedException();
		//}

		#endregion

		#region Implementation of ITnInPort

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region Implementation of ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			return NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			//PreviousState.StrategyResponse = Strategy.Initialize(outTorque, outAngularVelocity);
			//_electricMotorTorque = PreviousState.StrategyResponse.MechanicalAssistPower;
			return NextComponent.Initialize(outTorque, outAngularVelocity);
		}

		#endregion

		public void ApplyStrategySettings(HybridStrategyResponse strategySettings)
		{
			if (Gearbox != null) {
				Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;
			}

			if (Engine != null) {
				Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			}
			_electricMotorTorque = strategySettings.MechanicalAssistPower;
		}

		public IHybridControlledGearbox Gearbox { protected get; set; }
		public ICombustionEngine Engine { protected get; set; }

		private NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, bool dryRun)
		{
			return _electricMotorTorque.GetVECTOValueOrDefault(pos)?.Item2;
		}

		///=======================================================================================
		public class ElectricMotorController : IElectricMotorControl
		{
			private SimpleHybridController _controller;
			private ElectricMotorData ElectricMotorData;

			public ElectricMotorController(SimpleHybridController controller, ElectricMotorData motorData)
			{
				_controller = controller;
				ElectricMotorData = motorData;
			}
			#region Implementation of IElectricMotorControl

			public NewtonMeter MechanicalAssistPower(
				Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
				NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
				PowertrainPosition position, bool dryRun)
			{
				return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
														currOutAngularVelocity, dryRun);
			}

			//public NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt)
			//{
			//	var driveTorque = ElectricMotorData.FullLoadCurve.FullLoadDriveTorque(avgSpeed);
			//	var drivePowerElectric = ElectricMotorData.EfficiencyMap.LookupElectricPower(avgSpeed, driveTorque).ElectricalPower;
			//	if (drivePowerElectric >= _controller.ElectricSystem.MaxDischargePower(dt)) {
			//		return driveTorque;
			//	}

			//	drivePowerElectric = _controller.ElectricSystem.MaxDischargePower(dt);
			//	driveTorque = ElectricMotorData.EfficiencyMap.SearchMechanicalPower(drivePowerElectric, avgSpeed).Torque;
			//	return driveTorque;
			//}

			//public NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt)
			//{
			//	return 0.SI<NewtonMeter>();
			//}

			#endregion
		}


		public GearshiftPosition SelectedGear { get; }
		public PerSecond ICESpeed { get; }
		public bool GearboxEngaged { get; }
		public Second SimulationInterval { get; }

		public PerSecond ElectricMotorSpeed(PowertrainPosition pos)
		{
			return null;
		}

		public void RepeatDrivingAction(Second absTime)
		{
			
		}
	}
}