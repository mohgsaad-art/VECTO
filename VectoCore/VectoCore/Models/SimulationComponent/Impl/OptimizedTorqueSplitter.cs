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

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class OptimizedTorqueSplitter : TorqueSplitter
    {
        private List<List<double>> _torqueRatioCombinations;
        private readonly ITestPowertrain _testPowertrain;

        public OptimizedTorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox) :
            base(container, junctionBox)
        {
            _testPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false, true);

            CreateTorqueRatioCombinations();
        }

        private void CreateTorqueRatioCombinations()
        {
            _torqueRatioCombinations = new List<List<double>>()
            {
                new List<double>() { 0.9, 0.1 },
                new List<double>() { 0.5, 0.5 }
            };
        }

        protected override Dictionary<ITnOutPort, double> ConfigureTorqueRatio(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            var same = DataBus.Powertrains.First().Value.AxlePowertrainData == DataBus.Powertrains.Last().Value.AxlePowertrainData;

            var bestTorqueRatio = (_torqueRatioCombinations.Count() == 1)
                ? _torqueRatioCombinations.First()
                : SelectBestTorqueRatio(PerformRequestOnTestPowertrainForEachTorqueRatioCombination(absTime, dt, outTorque, outAngularVelocity, dryRun));

            return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => bestTorqueRatio[x.index]);
        }

        private List<Tuple<List<IResponse>, List<double>>> PerformRequestOnTestPowertrainForEachTorqueRatioCombination(
            Second absTime, 
            Second dt, 
            NewtonMeter outTorque, 
            PerSecond outAngularVelocity,  
            bool dryRun)
        {
            var responses = new List<Tuple<List<IResponse>, List<double>>>();

            foreach (var torqueRatio in _torqueRatioCombinations)
            {
                UpdateTestPowertrain(torqueRatio);

                var powertrainResponses = _testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);

                responses.Add(new Tuple<List<IResponse>, List<double>>(powertrainResponses, torqueRatio));
            }

            return responses;
        }

        private void UpdateTestPowertrain(List<double> torqueRatio)
        {
            _testPowertrain.JunctionBox.ClearResults();

            _testPowertrain.UpdateComponents();

            _testPowertrain.TorqueSplitter.TorqueRatio = torqueRatio;
        }

        private List<double> SelectBestTorqueRatio(List<Tuple<List<IResponse>, List<double>>> ratioCombinationResponses)
        {
            List<double> bestRatioCombination = null;

            if (ratioCombinationResponses.Any(x => x.Item1.All(r => (r is ResponseSuccess) || (r is ResponseDryRun))))
            {
                var successes = ratioCombinationResponses.Where(x => x.Item1.All(r => (r is ResponseSuccess) || (r is ResponseDryRun)));

                var sortedCombinations = (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake)
                    ? successes.OrderByDescending(x => VectoMath.Abs(x.Item1.First().ElectricSystem.ConsumerPower))
                    : successes.OrderBy(x => VectoMath.Abs(x.Item1.First().ElectricSystem.ConsumerPower));

                bestRatioCombination = sortedCombinations.First().Item2;
            }
            else
            {
                bestRatioCombination = ratioCombinationResponses
                    .Where(x => x.Item1.Any(r => r is IOverUnderResponse))
                    .OrderBy(x => VectoMath.Abs(x.Item1.Where(r => r is IOverUnderResponse).Sum(r => (r as IOverUnderResponse).Delta)))
                    .First()
                    .Item2;
            }

            return bestRatioCombination ?? ratioCombinationResponses.First().Item2;
        }

        protected override bool DoUpdateFrom(object other)
        {
            if (other is OptimizedTorqueSplitter ts)
            {
                PreviousState = ts.PreviousState.Clone();
                return true;
            }

            return false;
        }
    }
}
