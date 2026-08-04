using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueSplitter : StatefulVectoSimulationComponent<SimpleComponentState>, ITnInPort, ITnOutPort, ITorqueSplitter
    {
		protected List<ITnOutPort> _nextComponents = new List<ITnOutPort>();
		private IElectricPowerJunctionBox _junctionBox;

		public TorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox) : 
			base(container, Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			_junctionBox = junctionBox;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);

			Dictionary<ITnOutPort, IResponse> nextCompResponses = new Dictionary<ITnOutPort, IResponse>();

			_junctionBox.ClearResults();

			_nextComponents.ForEach(t => {
				_junctionBox.CurrentTorqueSplitterNextComponent = t;
				var response = t.Request(absTime, dt, outTorque / _nextComponents.Count, outAngularVelocity, dryRun);
				nextCompResponses[t] = response;
			});

			while (nextCompResponses.Values.Count(x => x is ResponseElectricSystemNotReady) > 0)
			{
				nextCompResponses
					.Where(x => x.Value is ResponseElectricSystemNotReady)
					.ToList().ForEach(t => {

						_junctionBox.CurrentTorqueSplitterNextComponent = t.Key;
						var response = t.Key.Request(absTime, dt, outTorque / _nextComponents.Count, outAngularVelocity, dryRun);
						nextCompResponses[t.Key] = response;
				});
			}

			var responses = nextCompResponses.Values.ToList();

			var first = responses.First();

			if ((first is ResponseSuccess) || (first is ResponseOverload) || (first is ResponseUnderload)) 
			{
				first.ElectricMotor.AngularVelocity = responses.Average(x => x.ElectricMotor.AngularVelocity?.Value() ?? 0).SI<PerSecond>();
				first.ElectricMotor.AvgDrivetrainSpeed = responses.Average(x => x.ElectricMotor.AvgDrivetrainSpeed?.Value() ?? 0).SI<PerSecond>();

				first.ElectricMotor.TorqueRequest = responses.Sum(x => x.ElectricMotor.TorqueRequest?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.TorqueRequestEmMap = responses.Sum(x => x.ElectricMotor.TorqueRequestEmMap?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.TotalTorqueDemand = responses.Sum(x => x.ElectricMotor.TotalTorqueDemand?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.MaxDriveTorque = responses.Sum(x => x.ElectricMotor.MaxDriveTorque?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.MaxDriveTorqueEM = responses.Sum(x => x.ElectricMotor.MaxDriveTorqueEM?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.InertiaTorque = responses.Sum(x => x.ElectricMotor.InertiaTorque?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.MaxRecuperationTorque = responses.Sum(x => x.ElectricMotor.MaxRecuperationTorque?.Value() ?? 0).SI<NewtonMeter>();
				first.ElectricMotor.MaxRecuperationTorqueEM = responses.Sum(x => x.ElectricMotor.MaxRecuperationTorqueEM?.Value() ?? 0).SI<NewtonMeter>();

				first.ElectricMotor.ElectricMotorPowerMech = responses.Sum(x => x.ElectricMotor.ElectricMotorPowerMech?.Value() ?? 0).SI<Watt>();
				first.ElectricMotor.InertiaPowerDemand = responses.Sum(x => x.ElectricMotor.InertiaPowerDemand?.Value() ?? 0).SI<Watt>();
				first.ElectricMotor.PowerRequest = responses.Sum(x => x.ElectricMotor.PowerRequest?.Value() ?? 0).SI<Watt>();

				if (first.Gearbox != null)
				{
					first.Gearbox.PowerRequest = responses.Any(x => x.Gearbox != null) ? responses.Sum(x => x.Gearbox?.PowerRequest?.Value() ?? 0).SI<Watt>() : null;
                }
            }

			return first;
		}

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);

			var responses = _nextComponents.Select(x => x.Initialize(outTorque, outAngularVelocity));

			return responses.First();
		}

		public virtual void Connect(ITnOutPort other)
		{
			if (!_nextComponents.Contains(other))
			{
				_nextComponents.Add(other);
				_junctionBox.AddTorqueSplitterNextComponent(other);
			}
		}

		public virtual ITnInPort InPort()
		{
			return this;
		}

		public virtual ITnOutPort OutPort()
		{
			return this;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is TorqueSplitter ts)
			{
				PreviousState = ts.PreviousState.Clone();
				return true;
			}

			return false;
		}

	}
}
