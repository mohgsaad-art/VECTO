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
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
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
		// ReSharper disable once InconsistentNaming
		protected Second dt = 1.SI<Second>();
		private bool _cancelled;
		protected ISimulationOutPort CyclePort { get; set; }

		[Required, ValidateObject]
		protected IVehicleContainer Container { get; set; }

		public bool FinishedWithoutErrors { get; protected set; }

		public int RunIdentifier { get; protected set; }

		public virtual string RunName
		{
			get { return Container.RunData.JobName; }
		}

		public virtual string CycleName
		{
			get { return Container.RunData.Cycle.Name; }
		}

		public virtual string RunSuffix
		{
			get { return Container.RunData.ModFileSuffix; }
		}

		public int JobRunIdentifier
		{
			get { return Container.RunData.JobRunId; }
		}

		public virtual double Progress
		{
			get { return CyclePort.Progress; }
		}

		protected VectoRun(IVehicleContainer container)
		{
			Container = container;
			RunIdentifier = Interlocked.Increment(ref _runIdCounter);
			Container.RunStatus = Status.Pending;
			CyclePort = container.GetCycleOutPort();
		}

		public IVehicleContainer GetContainer()
		{
			return Container;
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
			try {
				IResponse response;
				do {
					response = DoSimulationStep();
					debug.Add(response);
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
			} catch (VectoSimulationException vse) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(vse);
				Container.RunStatus = Status.Aborted;
				var ex = new VectoSimulationException("{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}",
						vse, AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed, TryCatch(() => Container.GearboxInfo.Gear),
						vse.Message, RunIdentifier, CycleName, RunSuffix);
				Container.FinishSimulationRun(ex);
				throw ex;
			} catch (VectoException ve) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(ve);
				Container.RunStatus = Status.Aborted;
				var ex = new VectoSimulationException("{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}",
						ve,
						AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed, TryCatch(() => Container.GearboxInfo.Gear), ve.Message,
						RunIdentifier, CycleName, RunSuffix);
				try {
					Container.FinishSimulationRun(ex);
				} catch (Exception ve2) {
					ve = new VectoException("Multiple Exceptions occured.",
						new AggregateException(ve, new VectoException("Exception during finishing Simulation.", ve2)));
					throw ve;
				}
				throw ex;
			} catch (Exception e) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(e);
				Container.RunStatus = Status.Aborted;

				var ex = new VectoSimulationException("{6} ({7} {8}) - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}",
						e, AbsTime,
						Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed, TryCatch(() => Container.GearboxInfo.Gear), e.Message,
						RunIdentifier, CycleName, RunSuffix);
				Container.FinishSimulationRun(ex);
				throw ex;
			}
			Container.RunStatus = Progress < 1 ? Status.Aborted : Status.Success;
			Container.FinishSimulationRun();
			if (Progress.IsSmaller(1, 1e-9)) {
				throw new VectoSimulationException(
					"{5} ({6} {7}) Progress: {8} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4}",
					AbsTime, Container.MileageCounter.Distance, dt, Container.VehicleInfo.VehicleSpeed, TryCatch(() => Container.GearboxInfo.Gear), RunIdentifier, CycleName,
					RunSuffix, Progress);
			}
			IterationStatistics.FinishSimulation(RunName + CycleName + RunSuffix + RunIdentifier);

			Log.Info("VectoJob finished.");
		}

		public void Cancel()
		{
			_cancelled = true;
		}

		private static object TryCatch(Func<object> action)
		{
			try {
				return action();
			} catch (VectoException e) {
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
		}
	}
}