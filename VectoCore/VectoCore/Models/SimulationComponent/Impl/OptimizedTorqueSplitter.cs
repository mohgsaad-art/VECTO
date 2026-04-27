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

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class OptimizedTorqueSplitter : TorqueSplitter
    {
        private List<List<double>> _torqueRatioCombinations;
        private readonly ITestPowertrain _testPowertrain;

        public OptimizedTorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox) :
            base(container, junctionBox)
        {
            _testPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false, nameof(TorqueSplitter));

            CreateTorqueRatioCombinations();
        }

        private void CreateTorqueRatioCombinations()
        {
            _torqueRatioCombinations = new List<List<double>>()
            {
                //new List<double>() { 0.6, 0.4 },
                new List<double>() { 0.5, 0.5 }
            };

            System.Diagnostics.Debug.Assert(_torqueRatioCombinations.All(x => x.Sum().IsEqual(1)));
        }

        protected override Dictionary<ITnOutPort, double> ConfigureTorqueRatio(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            var bestTorqueRatio = (_torqueRatioCombinations.Count() == 1)
                ? _torqueRatioCombinations.First()
                : DataBus.DriverInfo.DrivingAction.IsOneOf(DrivingAction.Coast, DrivingAction.Roll)
                    ? GetTorqueRatio(CalculateCoastingTorqueForFirstAxlePowertrain(_testPowertrain, absTime, dt, outTorque, outAngularVelocity), outTorque)
                    : SelectBestTorqueRatio(PerformRequestOnTestPowertrainForEachTorqueRatioCombination(absTime, dt, outTorque, outAngularVelocity, dryRun), dryRun);

            return _nextComponents.Select((x, i) => new { item = x, index = i }).ToDictionary(x => x.item, x => bestTorqueRatio[x.index]);
        }

        private List<double> GetTorqueRatio(NewtonMeter torqueAxlePt1, NewtonMeter totalTorque)
        {
            return new List<double>() { torqueAxlePt1 / totalTorque, 1 - (torqueAxlePt1 / totalTorque) };
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
                UpdateTestPowertrain(_testPowertrain, torqueRatio, DataBus.DriverInfo.DrivingAction.IsOneOf(DrivingAction.Coast, DrivingAction.Roll));

                var powertrainResponses = _testPowertrain.TorqueSplitter.PerformRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);

                responses.Add(new Tuple<List<IResponse>, List<double>>(powertrainResponses, torqueRatio));
            }

            return responses;
        }

        private List<double> SelectBestTorqueRatio(List<Tuple<List<IResponse>, List<double>>> ratioCombinationResponses, bool dryRun)
        {
            var acceptableResponses = ratioCombinationResponses.Where(x => !x.Item1.Any(r => r is ResponseElectricSystemNotReady));
           
            if (acceptableResponses.Count() == 0)
            {
                return dryRun 
                    ? ratioCombinationResponses
                        .OrderBy(x => VectoMath.Abs(x.Item1.Sum(r => (r as ResponseElectricSystemNotReady).ElectricMotor.ElectricMotorPowerMech)))
                        .First().Item2
                    : throw new VectoException($"OptimizedTorqueSplitter: no acceptable torque ratios found");
            }
            
            if (acceptableResponses.Count() == 1)
            {
                return acceptableResponses.First().Item2;
            }

            List<double> bestRatioCombination = null;

            if (acceptableResponses.Any(x => x.Item1.All(r => (r is ResponseSuccess) || (r is ResponseDryRun))))
            {
                var successes = acceptableResponses.Where(x => x.Item1.All(r => (r is ResponseSuccess) || (r is ResponseDryRun)));

                var sortedCombinations = (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake)
                    ? successes.OrderByDescending(x => VectoMath.Abs(x.Item1.First().ElectricSystem.ConsumerPower))
                    : successes.OrderBy(x => VectoMath.Abs(x.Item1.First().ElectricSystem.ConsumerPower));

                bestRatioCombination = sortedCombinations.First().Item2;
            }
            else
            {
                bestRatioCombination = acceptableResponses
                    .Where(x => x.Item1.Any(r => r is IOverUnderResponse))
                    .OrderBy(x => VectoMath.Abs(x.Item1.Where(r => r is IOverUnderResponse).Sum(r => (r as IOverUnderResponse).Delta)))
                    .FirstOrDefault()
                    ?.Item2;
            }

            return bestRatioCombination ?? acceptableResponses.First().Item2;
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
