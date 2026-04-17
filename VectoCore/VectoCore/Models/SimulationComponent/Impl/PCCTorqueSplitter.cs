using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PCCTorqueSplitter : TorqueSplitter
    {
        private readonly ITestPowertrain _testPowertrain;
        
        public PCCTorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox) :
            base(container, junctionBox)
        {
            _testPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false, nameof(TorqueSplitter));
        }

        protected override Dictionary<ITnOutPort, double> ConfigureTorqueRatio(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            var vehicleHasIdenticalAxlePowertrains = DataBus.Powertrains.First().Value.AxlePowertrainData == DataBus.Powertrains.Last().Value.AxlePowertrainData;

            var bestTorqueRatio = new List<double>() { 0.5, 0.5 };

            if (outTorque.IsEqual(0) || vehicleHasIdenticalAxlePowertrains)
            {
                return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => bestTorqueRatio[x.index]);
            }

            var coastingTorqueAxlePt1 = CalculateCoastingTorqueForFirstAxlePowertrain(absTime, dt, outTorque, outAngularVelocity);

            bestTorqueRatio = new List<double>() { coastingTorqueAxlePt1 / outTorque, 1 - (coastingTorqueAxlePt1 / outTorque) };

            return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => bestTorqueRatio[x.index]);
        }

        private NewtonMeter CalculateCoastingTorqueForFirstAxlePowertrain(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            _testPowertrain.UpdateComponents();
            _testPowertrain.TorqueSplitter.TorqueRatio = new List<double>() { 1, 0 };

            foreach (var motor in _testPowertrain.ElectricMotors)
            {
                if (motor.Control is ITestPowertrainElectricMotorControl emCtl)
                {
                    emCtl.EmOff = true;
                }
            }

            var torqueInFirstAxlePowertrain = outTorque * 0.5;
            var initialResponse = _testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, torqueInFirstAxlePowertrain, outAngularVelocity, false).First();
            var delta = initialResponse.Gearbox?.PowerRequest ?? initialResponse.ElectricMotor?.TotalTorqueDemand * initialResponse.ElectricMotor?.AvgDrivetrainSpeed;

            try
            {
                torqueInFirstAxlePowertrain = SearchAlgorithm.Search(
                    torqueInFirstAxlePowertrain, delta, 0.1.SI<NewtonMeter>(),
                    getYValue: response =>
                    {
                        var r = (ResponseDryRun)response;
                        return r.Gearbox?.PowerRequest ?? r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed;
                    },
                    evaluateFunction: torque => _testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, torque, outAngularVelocity, true).First(),
                    criterion: response =>
                    {
                        var r = (ResponseDryRun)response;
                        return (r.Gearbox?.PowerRequest ?? r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed).Value();
                    },
                    searcher: this
                );
            }
            catch (VectoSearchAbortedException) { }

            return torqueInFirstAxlePowertrain;
        }

        protected override bool DoUpdateFrom(object other)
        {
            if (other is PCCTorqueSplitter ts)
            {
                PreviousState = ts.PreviousState.Clone();
                return true;
            }

            return false;
        }
    }
}
