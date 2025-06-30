/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
    /// <summary>
    /// Simulator for one vecto simulation job.
    /// </summary>
    public abstract class VectoRun : LoggingObject, IVectoRun
    {
        private static int _runIdCounter;
        protected Second AbsTime = 0.SI<Second>();
        protected Second dt = 1.SI<Second>();
        private bool _cancelled;
        private readonly IFollowUpRunCreator _followUpCreator;
        private readonly IPostMortemAnalyzer _postMortemAnalyzer;
        protected ISimulationOutPort CyclePort { get; set; }

        [Required, ValidateObject]
        protected IVehicleContainer Container { get; set; }

        public bool FinishedWithoutErrors { get; protected set; }

        public int RunIdentifier { get; protected set; }


        public virtual string RunName => Container.RunData.JobName;

        public virtual string CycleName => Container.RunData.Cycle.Name;

        public virtual string RunSuffix => Container.RunData.ModFileSuffix;

        public int JobRunIdentifier => Container.RunData.JobRunId;

        public virtual double Progress => CyclePort.Progress * (PostProcessingDone ? 1.0 : 0.99) * (WritingResultsDone ? 1.0 : 0.99);

        protected VectoRun(IVehicleContainer container, IFollowUpRunCreator followUpCreator = null, IPostMortemAnalyzer postMortem = null)
        {
            Container = container;
            RunIdentifier = Interlocked.Increment(ref _runIdCounter);
            Container.RunStatus = Status.Pending;
            CyclePort = container.GetCycleOutPort();
            PostProcessingDone = false;
            WritingResultsDone = false;
            _followUpCreator = followUpCreator ?? new NoFollowUpRunCreator();
            _postMortemAnalyzer = postMortem ?? new NoPostMortemAnalysis();

        }
        [DebuggerStepThrough]
        public IVehicleContainer GetContainer() => Container;

        public virtual bool CalculateAggregateValues()
        {
            return !(GetContainer().RunData.Exempted || GetContainer().RunData.MultistageRun);
        }
        public void Run()
        {
            if (Container.RunStatus != Status.Pending) {
                return;
            }
            var debug = new DebugData();

            Log.Info("VectoJob preprocessing.");

            foreach (var preprocessing in Container.GetPreprocessingRuns) {
                preprocessing.RunPreprocessing();
            }

            Container.StartSimulationRun();
            Log.Info("VectoJob started running.");

            Container.AbsTime = AbsTime;

            Initialize();
            IResponse response = null;
            var iterationCount = 0;

            try {
                do {
                    response = DoSimulationStep();
                    debug.Add($"[VR.R] ---- ITERATION {iterationCount++} ---- ", response);
                    if (response is ResponseSuccess) {
                        Container.CommitSimulationStep(AbsTime, dt);
                        AbsTime += dt;
                        if (_cancelled) {
                            Log.Error("Run canceled!");
                            Container.RunStatus = Status.Canceled;
                            Container.FinishSimulationRun();
                            return;
                        }
                        Container.AbsTime = AbsTime;
                    }
                } while (response is ResponseSuccess);
                if (CalculateAggregateValues()) {
                    //foreach (var fuel in GetContainer().RunData.EngineData.Fuels) {
                        // calculate vehicleline correction here in local thread context because writing sum-data and
                        // report afterwards is synchronized
                        //var cf = GetContainer().ModalData.VehicleLineCorrectionFactor(fuel.FuelData);
                        GetContainer().ModalData.CalculateAggregateValues();
                    //}
                }

                PostProcessingDone = true;
            } catch (VectoSimulationException vse) {
                if (_postMortemAnalyzer?.AbortSimulation(Container, vse) ?? true) {
                    Log.Error("SIMULATION RUN ABORTED! ========================");
                    Log.Error(vse);
                    Container.RunStatus = Status.Aborted;
                    var ex = new VectoSimulationException(
                        "{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}, f_equiv:{9}",
                        vse, AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed,
                        TryCatch(() => Container.GearboxInfo.Gear), vse.Message, RunIdentifier, CycleName, RunSuffix,
                        TryCatch(() => Container.RunData.HybridStrategyParameters?.EquivalenceFactor));
                    Container.FinishSimulationRun(ex);
                    throw ex;
                }
            } catch (VectoException ve) {
                if (_postMortemAnalyzer?.AbortSimulation(Container, ve) ?? true) {
                    Log.Error("SIMULATION RUN ABORTED! ========================");
                    Log.Error(ve);
                    Container.RunStatus = Status.Aborted;
                    var ex = new VectoSimulationException(
                        "{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}, f_equiv:{9}",
                        ve, AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed,
                        TryCatch(() => Container.GearboxInfo.Gear), ve.Message, RunIdentifier, CycleName, RunSuffix,
                        TryCatch(() => Container.RunData.HybridStrategyParameters?.EquivalenceFactor));
                    try {
                        Container.FinishSimulationRun(ex);
                    } catch (Exception ve2) {
                        ve = new VectoException("Multiple Exceptions occured.",
                            new AggregateException(ve,
                                new VectoException("Exception during finishing Simulation.", ve2)));
                        throw ve;
                    }

                    throw ex;
                }
            } catch (Exception e) {
                if (_postMortemAnalyzer?.AbortSimulation(Container, e) ?? true) {
                    Log.Error("SIMULATION RUN ABORTED! ========================");
                    Log.Error(e);
                    Container.RunStatus = Status.Aborted;

                    var ex = new VectoSimulationException(
                        "{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}, f_equiv:{9}",
                        e, AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed,
                        TryCatch(() => Container.GearboxInfo.Gear), e.Message, RunIdentifier, CycleName, RunSuffix,
                        TryCatch(() => Container.RunData.HybridStrategyParameters?.EquivalenceFactor) ?? "-");
                    Container.FinishSimulationRun(ex);
                    throw ex;
                }
            }

            if (Container.RunStatus == Status.PrimaryBusSimulationIgnore) {
                Container.FinishSimulationRun();
                WritingResultsDone = true;
                FinishedWithoutErrors = true;
                IterationStatistics.FinishSimulation(RunName + CycleName + RunSuffix + RunIdentifier);
                Log.Info("VectoJob finished.");
                return;
            }

            if (CheckCyclePortProgress()) {
                if (CyclePort.Progress < 1) {
                    Container.RunStatus = response is ResponseBatteryEmpty ? Status.REESSEmpty : Status.Aborted;
                } else {
                    Container.RunStatus = Status.Success;
                }
            } else {
                Container.RunStatus = Status.Success;
            }

            var runAgain = _followUpCreator?.RunAgain((data) => {
                    
                    Container.ModalData.Reset(true);
                    Container = _followUpCreator.PowertrainBuilder.Build(data, Container.ModalData, Container.SumData);
                    AbsTime = 0.SI<Second>();
                    Container.AbsTime = AbsTime;
                    CyclePort = Container.GetCycleOutPort();
                    Initialize();
                    Run();
                },
                this,
				(preRunOptions) => {
                    Container.RunData.Report?.PrepareResult(null); //<- increase number of expected results;
					if (preRunOptions.WriteModAndSumData)
					{
                    Container.FinishSingleSimulationRun();
					}
                }) ?? false;

            if (!runAgain) {
                Container.FinishSimulationRun();
                WritingResultsDone = true;
            }
            
            if (Progress.IsSmaller(1, 1e-9)) {
                if (response is ResponseBatteryEmpty) {
                    throw new VectoSimulationException("{3} ({4} {5}) REESS was empty before cycle could be finished. Progress: {6:P1} - absTime: {0:F1}, distance: {1:F1}, dt: {2:F1}",
                        AbsTime, Container.MileageCounter.Distance, dt, RunIdentifier, CycleName, RunSuffix, Progress);

                } else {
                    throw new VectoSimulationException("{5} ({6} {7}) Driving Cycle could not be finished. Progress: {8:P1} - absTime: {0:F1}, distance: {1:F1}, dt: {2:F1}, v: {3:F1}, Gear: {4}, f_equiv:{9}",
                        AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed,
                        TryCatch(() => Container.GearboxInfo.Gear), RunIdentifier, CycleName, RunSuffix, Progress, TryCatch(() => Container.RunData.HybridStrategyParameters.EquivalenceFactor));
                }
            }
            IterationStatistics.FinishSimulation(RunName + CycleName + RunSuffix + RunIdentifier);
            Log.Info("VectoJob finished.");
        }

        protected virtual bool CheckCyclePortProgress()
        {
            return !Container.RunData.Exempted && !Container.RunData.MultistageRun;
        }

        public bool PostProcessingDone { get; protected set; }

        public bool WritingResultsDone { get; protected set; }

        public void Cancel() => _cancelled = true;

        private static object TryCatch(Func<object> action)
        {
            try {
                return action();
            } catch (Exception e) {
                LogManager.GetLogger(typeof(VectoRun).FullName).Info(e);
                return null;
            }
        }

        protected abstract IResponse DoSimulationStep();

        protected abstract IResponse Initialize();

        public enum Status
        {
            Pending,
            Running,
            Success,
            Canceled,
            Aborted,
            REESSEmpty,
            PrimaryBusSimulationIgnore
        }
    }
}