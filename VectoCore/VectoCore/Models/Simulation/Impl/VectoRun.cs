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
using System.Linq;
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCommon.InputData;

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
                    if (!string.IsNullOrEmpty(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError))
                    {
                        Log.Warn(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError);
                        throw new VectoException(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError, ErrorCodes.FUELCELL_POWERMAP_GENERATION_FAILED);
                    }

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
                } 
                while (response is ResponseSuccess);

                if (CalculateAggregateValues()) 
                {
                    GetContainer().ModalData.CalculateAggregateValues();
                }

				PostProcessingDone = true;
			} 
            catch (Exception e) 
            {
                if (_postMortemAnalyzer?.AbortSimulation(Container, e) ?? true) 
                {
                    Log.Error("SIMULATION RUN ABORTED! ========================");
                    Log.Error(e);

                    Container.RunStatus = Status.Aborted;

                    try
                    {
                        Container.FinishSimulationRun(DecorateExceptionWithRunData(e));
                    }
                    catch (Exception) { }

					throw;
				}
			}

            if (Container.RunStatus == Status.PrimaryBusSimulationIgnore) 
            {
                var exception = string.IsNullOrEmpty(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError)
                    ? null
                    : DecorateExceptionWithRunData(new VectoException(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError, ErrorCodes.FUELCELL_POWERMAP_GENERATION_FAILED));

                Container.FinishSimulationRun(exception);
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
					var isFCHVWithModals = 
						Container.PowertrainInfo.VehicleArchitecture.IsFCHV() &&
						Container.ModalData.WriteModalResults;

					var writeModalsOG = Container.ModalData.WriteModalResults;
					Container.ModalData.WriteModalResults = isFCHVWithModals ? preRunOptions.WriteModAndSumData : Container.ModalData.WriteModalResults;

					// Increase number of expected results
					Container.RunData.Report?.PrepareResult(null);
					Container.FinishSingleSimulationRun();

					Container.ModalData.WriteModalResults = writeModalsOG;

				}) ?? false;

            if (!runAgain) {
                Container.FinishSimulationRun();
                WritingResultsDone = true;
            }
            
            if (Progress.IsSmaller(1, 1e-9)) 
            {
                if (response is ResponseBatteryEmpty) 
                {
                    throw DecorateExceptionWithRunData(new VectoException("REESS was empty before cycle could be finished.", ErrorCodes.REESS_EMPTY_BEFORE_CYCLE_END));
                } 
                else if (string.IsNullOrEmpty(Container.RunData.FuelCellSystemData?.FuelCellPowerMap?.CalculationError))
                {     
                    throw DecorateExceptionWithRunData(new VectoException("Driving Cycle could not be finished.", ErrorCodes.VECTORUN_CYCLE_CANNOT_FINISH));
                }
			}

			IterationStatistics.FinishSimulation(RunName + CycleName + RunSuffix + RunIdentifier);
			Log.Info("VectoJob finished.");
		}

        private Exception DecorateExceptionWithRunData(Exception exception)
        {
            exception.Data[RunInfo.RunIdentifier] = RunIdentifier;
            exception.Data[RunInfo.CycleName] = CycleName;
            exception.Data[RunInfo.RunSuffix] = RunSuffix;
            exception.Data[RunInfo.AbsTime] = AbsTime;
            exception.Data[RunInfo.Distance] = Container.MileageCounter.Distance;
            exception.Data[RunInfo.Speed] = Container.VehicleInfo.VehicleSpeed;
            exception.Data[RunInfo.F_equiv] = Container.RunData.HybridStrategyParameters?.EquivalenceFactor;
            exception.Data[RunInfo.GearInfo] = Container.GearboxesInfo?.ToDictionary(x => x.AxleNumber, x => x.Gear);
            exception.Data[RunInfo.Dt] = dt;
            exception.Data[RunInfo.Progress] = Progress;

            return exception;
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