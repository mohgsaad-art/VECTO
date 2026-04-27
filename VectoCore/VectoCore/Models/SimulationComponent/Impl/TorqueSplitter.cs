using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueSplitter : StatefulVectoSimulationComponent<SimpleComponentState>, ITnInPort, ITnOutPort, ITorqueSplitter
    {
		protected IList<ITnOutPort> _nextComponents = new List<ITnOutPort>();
        private IElectricPowerJunctionBox _junctionBox;
        private List<double> _torqueRatio;

        public TorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox) : base(container)
		{
			_junctionBox = junctionBox;
            TorqueRatio = new List<double>() { 0.5, 0.5 };
        }

        public List<double> TorqueRatio 
        { 
            get { return _torqueRatio; }
            set { _torqueRatio = value; System.Diagnostics.Debug.Assert(_torqueRatio.Sum().IsEqual(1)); } 
        }

        public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
            var axlePowertrainResponses = PerformRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);

            return CreateResponse(axlePowertrainResponses);
        }

		public List<IResponse> PerformRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
            CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);

            var torqueRatio = ConfigureTorqueRatio(absTime, dt, outTorque, outAngularVelocity, dryRun);

            System.Diagnostics.Debug.Assert(torqueRatio.Values.Sum().IsEqual(1));

            var axlePowertrainResponses = MakeRequests(absTime, dt, outTorque, outAngularVelocity, dryRun, torqueRatio);

			return axlePowertrainResponses.Values.ToList();
        }

		private Dictionary<ITnOutPort, IResponse> MakeRequests(
			Second absTime, 
			Second dt, 
			NewtonMeter outTorque, 
			PerSecond outAngularVelocity, 
			bool dryRun, 
			Dictionary<ITnOutPort, double> torqueRatio)
		{
            _junctionBox.ClearResults();

            Dictionary<ITnOutPort, IResponse> axlePowertrainResponses = new Dictionary<ITnOutPort, IResponse>();

            foreach (var nextComponent in _nextComponents)
            {
                var response = nextComponent.Request(absTime, dt, outTorque * torqueRatio[nextComponent], outAngularVelocity, dryRun);
                axlePowertrainResponses[nextComponent] = response;
            }

            if (axlePowertrainResponses[_nextComponents.Last()] is ResponseElectricSystemNotReady)
            {
                return dryRun ? axlePowertrainResponses : throw new VectoException($"TorqueSplitter request got ResponseElectricSystemNotReady");
            }

            while (axlePowertrainResponses.Values.Any(x => x is ResponseElectricSystemNotReady))
            {
                axlePowertrainResponses
                    .Where(x => x.Value is ResponseElectricSystemNotReady)
                    .ToList()
					.ForEach(t =>
                    {
                        axlePowertrainResponses[t.Key] = t.Key.Request(absTime, dt, outTorque * torqueRatio[t.Key], outAngularVelocity, dryRun);
                    });
            }

			return axlePowertrainResponses;
        }

        protected void UpdateTestPowertrain(ITestPowertrain testPowertrain, List<double> torqueRatio, bool emOff)
        {
            testPowertrain.JunctionBox.ClearResults();

            testPowertrain.UpdateComponents();

            foreach (var em in testPowertrain.ElectricMotors)
            {
                (em.Control as ITestPowertrainElectricMotorControl).EmOff = emOff;
            }

            testPowertrain.TorqueSplitter.TorqueRatio = torqueRatio;
        }

        protected NewtonMeter CalculateCoastingTorqueForFirstAxlePowertrain(ITestPowertrain testPowertrain, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            UpdateTestPowertrain(testPowertrain, new List<double>() { 1, 0 }, true);

            var torqueInFirstAxlePowertrain = outTorque * 0.5;
            var initialResponse = testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, torqueInFirstAxlePowertrain, outAngularVelocity, false).First();
            var delta = initialResponse.Gearbox?.PowerRequest ?? initialResponse.ElectricMotor?.TotalTorqueDemand * initialResponse.ElectricMotor?.AvgDrivetrainSpeed;

            try
            {
                torqueInFirstAxlePowertrain = SearchAlgorithm.Search(
                    torqueInFirstAxlePowertrain, delta, 0.1.SI<NewtonMeter>(),
                    getYValue: response =>
                    {
                        var r = (ResponseDryRun)response;
                        return r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed;
                    },
                    evaluateFunction: torque => testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, torque, outAngularVelocity, true).First(),
                    criterion: response =>
                    {
                        var r = (ResponseDryRun)response;
                        return (r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed).Value();
                    },
                    searcher: this
                );
            }
            catch (VectoSearchAbortedException) { }

            return torqueInFirstAxlePowertrain;
        }

        private IResponse CreateResponse(List<IResponse> responses)
		{
            var first = responses.First();

            if ((first is ResponseSuccess) || (first is ResponseOverload) || (first is ResponseUnderload) || (first is ResponseDryRun))
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

                if (responses.All(r => r is IOverUnderResponse))
                {
                    (first as IOverUnderResponse).Delta = responses.Sum(r => (r as IOverUnderResponse).Delta);
                }

                if (first.Gearbox != null)
                {
                    first.Gearbox.PowerRequest = responses.Any(x => x.Gearbox?.PowerRequest != null) ? responses.Sum(x => x.Gearbox?.PowerRequest?.Value() ?? 0).SI<Watt>() : null;
                }

                if (responses.All(r => r is ResponseDryRun))
                {
                    var dryRunResponses = responses.Select(r => r as ResponseDryRun);

                    (first as ResponseDryRun).DeltaDragLoad = dryRunResponses.Where(r => r.DeltaDragLoad != null).Sum(r => r.DeltaDragLoad);
                    (first as ResponseDryRun).DeltaFullLoad = dryRunResponses.Where(r => r.DeltaFullLoad != null).Sum(r => r.DeltaFullLoad);
                    (first as ResponseDryRun).DeltaFullLoadTorque = dryRunResponses.Where(r => r.DeltaFullLoadTorque != null).Sum(r => r.DeltaFullLoadTorque);
                    (first as ResponseDryRun).DeltaDragLoadTorque = dryRunResponses.Where(r => r.DeltaDragLoadTorque != null).Sum(r => r.DeltaDragLoadTorque);
                }
            }

            return first;
        }

        protected virtual Dictionary<ITnOutPort, double> ConfigureTorqueRatio(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
            return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => TorqueRatio[x.index]);
        }

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);

            IResponse response = null;

            for (var i = 0; i < _nextComponents.Count(); i++)
            {
                response = _nextComponents[i].Initialize(outTorque * TorqueRatio[i], outAngularVelocity);
            }

            return response;
		}

		public virtual void Connect(ITnOutPort other)
		{
			if (!_nextComponents.Contains(other))
			{
				_nextComponents.Add(other);
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
