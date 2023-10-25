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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Container for simulation jobs.
	/// </summary>
	public class JobContainer : LoggingObject
	{
		internal readonly List<RunEntry> Runs = new List<RunEntry>();
		private readonly SummaryDataContainer _sumWriter;

		private static int _jobNumber;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="JobContainer"/> class.
		/// </summary>
		/// <param name="sumWriter">The sum writer.</param>
		public JobContainer(SummaryDataContainer sumWriter)
		{
			_sumWriter = sumWriter;
		}

		public void AddRun(IVectoRun run)
		{
			Interlocked.Increment(ref _jobNumber);
			Runs.Add(new RunEntry { Run = run, JobContainer = this });
		}

		public struct CycleTypeDescription
		{
			public string Name;
			public CycleType CycleType;
		}

		public IEnumerable<CycleTypeDescription> GetCycleTypes()
		{
			return
				Runs.Select(
					r => new CycleTypeDescription { Name = r.Run.CycleName, CycleType = r.Run.GetContainer().RunData.Cycle?.CycleType ?? CycleType.None })
					.Distinct();
		}

		/// <summary>
		/// Adds the runs from the factory to the job container.
		/// </summary>
		/// <returns>A List of Run-Identifiers (unique), int</returns>
		public List<int> AddRuns(ISimulatorFactory factory)
		{
			var runIDs = new List<int>();

			factory.SumData = _sumWriter;
			factory.JobNumber = Interlocked.Increment(ref _jobNumber);

			foreach (var run in factory.SimulationRuns()) {
				if (run.CycleName == "LongHaul")
				{
					var entry = new RunEntry { Run = run, JobContainer = this };
					Runs.Add(entry);
					runIDs.Add(entry.Run.RunIdentifier);
				}
				//break;
			}
			return runIDs;
		}

		/// <summary>
		/// Execute all runs, waits until finished.
		/// </summary>
		public void Execute(bool multithreaded = true)
		{
			Log.Info("VectoRun started running. Executing Runs.");
			if (multithreaded) {
				Runs.ForEach(r => r.RunWorkerAsync());
			} else {
				var first = new Task(() => { });
				var task = first;
				// ReSharper disable once LoopCanBeConvertedToQuery
				foreach (var run in Runs) {
					var r = run;
					task = task.ContinueWith(t => r.RunWorkerAsync().Wait(), TaskContinuationOptions.OnlyOnRanToCompletion);
				}
				first.Start();
			}
		}

		public void Cancel()
		{
			foreach (var job in Runs) {
				job.CancelAsync();
			}
			WaitFinished();
		}

		public void CancelCurrent()
		{
			foreach (var job in Runs) {
				job.CancelAsync();
			}
		}

		public void WaitFinished()
		{
			Task.WaitAll(Runs.Select(r => r.RunTask).ToArray());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void JobCompleted()
		{
			if (AllCompleted) {
				_sumWriter.Finish();
			}
		}

		public bool AllCompleted
		{
			get { return Runs.All(r => r.Done); }
		}

		public Dictionary<int, ProgressEntry> GetProgress()
		{
			return Runs.ToDictionary(
				r => r.Run.RunIdentifier,
				r => new ProgressEntry {
					RunId = r.Run.RunIdentifier,
					JobRunId = r.Run.JobRunIdentifier,
					RunName = r.Run.RunName,
					CycleName = r.Run.CycleName,
					RunSuffix = r.Run.RunSuffix,
					Progress = r.Run.Progress,
					Done = r.Done,
					ExecTime = r.ExecTime,
					Success = r.Success,
					Canceled = r.Canceled,
					Error = r.ExecException
				});
		}

		public class ProgressEntry
		{
			// unique identifier of the simulation run
			public int RunId;
			// job-local identifier of the simulation run
			public int JobRunId;

			public string RunName;

			public double Progress;

			public double ExecTime;

			public Exception Error;

			public bool Canceled;

			public bool Success;

			public bool Done;

			public string CycleName;

			public string RunSuffix;
		}

		[DebuggerDisplay("{Run.RunIdentifier}: {Run.RunName}, {Run.CycleName}")]
		internal class RunEntry : LoggingObject
		{
			public IVectoRun Run;
			public JobContainer JobContainer;
			public bool Done;
			public bool Success;
			public bool Canceled;
			public double ExecTime;
			public Exception ExecException;
			public readonly Task RunTask;

			public RunEntry()
			{
				RunTask = new Task(() => {
					var stopWatch = Stopwatch.StartNew();
					try {
						Run.Run();
					} catch (Exception ex) {
						Log.Error(ex, "Error during simulation run!");
						ExecException = ex;
						throw;
					} finally {
						stopWatch.Stop();
						Success = Run.FinishedWithoutErrors && ExecException == null;
						Done = true;
						ExecTime = stopWatch.Elapsed.TotalMilliseconds;
						JobContainer.JobCompleted();
					}
				});
			}

			public Task RunWorkerAsync()
			{
				RunTask.Start();
				return RunTask;
			}

			public void CancelAsync()
			{
				Run.Cancel();
				Canceled = true;
			}
		}
	}
}