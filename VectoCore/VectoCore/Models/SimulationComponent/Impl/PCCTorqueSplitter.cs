using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
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

            var coastingTorqueAxlePt1 = CalculateCoastingTorqueForFirstAxlePowertrain(_testPowertrain, absTime, dt, outTorque, outAngularVelocity);

            bestTorqueRatio = new List<double>() { coastingTorqueAxlePt1 / outTorque, 1 - (coastingTorqueAxlePt1 / outTorque) };

            return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => bestTorqueRatio[x.index]);
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
