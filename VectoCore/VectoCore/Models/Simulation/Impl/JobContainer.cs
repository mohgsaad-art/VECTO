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
using System.Collections.Concurrent;
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
		private class RunContainer
		{
			private readonly ISimulatorFactory _simulatorFactory;
			private IOutputDataWriter _outputWriter;

			public IOutputDataWriter OutputWriter => _outputWriter;

			private bool followUpSimulatorFactoryFetched = false;

			private readonly ConcurrentDictionary<int, byte> _unfinishedRuns = new ConcurrentDictionary<int, byte>();
			//private readonly HashSet<int> _unfinishedRuns = new HashSet<int>();
			private readonly ReaderWriterLockSlim _unfinishedRunsRwLock = new ReaderWriterLockSlim();


			public RunContainer(ISimulatorFactory simulatorFactory, IList<int> runIds)
			{
				_simulatorFactory = simulatorFactory;
				foreach (var runId in runIds) {
					_unfinishedRuns[runId] = Byte.MinValue;
				}
			}

			public void JobCompleted(int runId)
			{
				try {
					_unfinishedRunsRwLock.EnterWriteLock();
					_unfinishedRuns.TryRemove(runId, out var tmpByte);
					if (AllCompletedUnsafe()) {
						_outputWriter = _simulatorFactory.ReportWriter;
					}
				} finally {
					_unfinishedRunsRwLock.ExitWriteLock();
				}
			}

			private bool AllCompletedUnsafe()
			{
				return _unfinishedRuns.Count == 0;
			}

			private bool AllCompleted()
			{
				try {
					_unfinishedRunsRwLock.EnterReadLock();
					return AllCompletedUnsafe();
				} finally {
					_unfinishedRunsRwLock.ExitReadLock();
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public ISimulatorFactory GetFollowUpSimulatorFactory()
			{
				try {
					if (followUpSimulatorFactoryFetched || !AllCompleted()) {
						return null;
					}

					followUpSimulatorFactoryFetched = true;
					return _simulatorFactory.FollowUpSimulatorFactory;
				} catch (Exception e){
					LogManager.GetLogger(typeof(JobContainer).FullName).Error(e);
					throw;
				}finally {

				}
			}
		}

		

		private readonly SummaryDataContainer _sumWriter;
		internal readonly List<RunEntry> Runs = new List<RunEntry>();
		private readonly ConcurrentDictionary<int, byte> _unfinishedRuns = new ConcurrentDictionary<int, byte>(); //only key is used 
		private ReaderWriterLockSlim _runsRwLock = new ReaderWriterLockSlim();
		private ConcurrentDictionary<int, RunContainer> _runContainerMap  = new ConcurrentDictionary<int, RunContainer>();
		private static int _jobNumber;
		private bool _multithreaded = true;
		private bool _canceled = false;
		private ReaderWriterLockSlim _cancelLock = new ReaderWriterLockSlim();

		private ConcurrentDictionary<int, ProgressEntry> _progressDictionary =
			new ConcurrentDictionary<int, ProgressEntry>();


		/// <summary>
		/// Initializes a new empty instance of the <see cref="JobContainer"/> class.
		/// </summary>
		/// <param name="sumWriter">The sum writer.</param>
		public JobContainer(SummaryDataContainer sumWriter)
		{
			_sumWriter = sumWriter;
		}

		
		public IEnumerable<IOutputDataWriter> GetOutputDataWriters()
		{
			IList<IOutputDataWriter> outputDataWriters = new List<IOutputDataWriter>();
			foreach (var runContainer in _runContainerMap.Values) {
				if (runContainer.OutputWriter != null) {
					outputDataWriters.Add(runContainer.OutputWriter);
				}
			}

			return outputDataWriters.Distinct();
		}

		public void AddRun(IVectoRun run)
		{
			try {
				_cancelLock.EnterReadLock();
				if (_canceled) {
					return;
				}

				Interlocked.Increment(ref _jobNumber);

				try {
					_runsRwLock.EnterWriteLock();
					Runs.Add(new RunEntry(run, this));
					_unfinishedRuns[run.RunIdentifier] = Byte.MinValue;
				} finally {
					_runsRwLock.ExitWriteLock();
				}
			} finally {
				_cancelLock.ExitReadLock();
			}

		}

		public struct CycleTypeDescription
		{
			public string Name;
			public CycleType CycleType;
		}

		public IEnumerable<CycleTypeDescription> GetCycleTypes(){
			try {
				_runsRwLock.EnterReadLock();
				return Runs.Select(
						r => new CycleTypeDescription {
							Name = r.Run.CycleName,
							CycleType = r.Run.GetContainer().RunData.Cycle?.CycleType ?? CycleType.None
						})
					.Distinct();
			} finally {
				_runsRwLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Adds the runs from the factory to the job container.
		/// </summary>
		/// <returns>A List of Run-Identifiers (unique), int</returns>
		public List<int> AddRuns(ISimulatorFactory factory)
		{
			try {
				_cancelLock.EnterReadLock();
				var runIDs = new List<int>();
				if (_canceled) {
					return runIDs;
				}

				factory.SumData = _sumWriter;
				factory.JobNumber = Interlocked.Increment(ref _jobNumber);

				try
				{
					_runsRwLock.EnterWriteLock();
					foreach (var run in factory.SimulationRuns())
					{
						var entry = new RunEntry(run, this, factory.JobNumber);
						Runs.Add(entry);
						_unfinishedRuns[run.RunIdentifier] = Byte.MinValue;
						//_unfinishedRuns.Add(run.RunIdentifier);
						runIDs.Add(entry.RunId);
					}
				}
				finally
				{
					_runsRwLock.ExitWriteLock();
				}

				var added = _runContainerMap.TryAdd(factory.JobNumber, new RunContainer(factory, runIDs));
				System.Diagnostics.Debug.Assert(added);
				return runIDs;
			}
			finally {
				_cancelLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Execute all runs, waits until finished.
		/// </summary>
		public void Execute(bool multithreaded = true)
		{
			try {
				_cancelLock.EnterReadLock();
				_multithreaded = multithreaded;
				Log.Info("VectoRun started running. Executing Runs.");
				if (_canceled) {
					Log.Info("JobContainer already cancelled\n");
					return;
				}


				try {
					_runsRwLock.EnterWriteLock();
					if (multithreaded) {
						Runs.ForEach(r => { r.RunWorkerAsync(); });
					} else {
						var first = new Task(() => { });
						var task = first;
						// ReSharper disable once LoopCanBeConvertedToQuery
						foreach (var run in Runs) {
							var r = run;
							task = task.ContinueWith(t => r.RunWorkerAsync().Wait(),
								TaskContinuationOptions.OnlyOnRanToCompletion);
						}

						first.Start();
					}
				} finally {
					_runsRwLock.ExitWriteLock();
				}


			} finally {
				_cancelLock.ExitReadLock();
			}
			
		}

		public void Cancel()
		{
			CancelCurrent();
			WaitFinished();
		}

		public void CancelCurrent()
		{
			try
			{
				_cancelLock.EnterWriteLock();
				_canceled = true;
				_runsRwLock.EnterReadLock();
				foreach (var job in Runs)
				{
					job.CancelAsync();
				}
			}
			finally
			{
				_runsRwLock.ExitReadLock();
				_cancelLock.ExitWriteLock();
			}
		}

		public void WaitFinished()
		{
			Task[] prevTasks;
			Task[] tasks = {};
			for (var i = 0; i < 5; i++) {
				try {
					prevTasks = tasks;
					_runsRwLock.EnterReadLock();
					tasks = Runs.Select(r => r.RunTask).ToArray();
				} finally {
					_runsRwLock.ExitReadLock();
				}

				if (prevTasks.Length != tasks.Length) {
					Task.WaitAll(tasks);
				} else {
					return;
				}
			}
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		private void JobCompleted(int runId, int runContainerId)
		{
			try {
				_cancelLock.EnterReadLock();
				if (_canceled) {
					return;
				}
			} finally {
				_cancelLock.ExitReadLock();
			}

			_runContainerMap.TryGetValue(runContainerId, out var runContainer);
			
			runContainer?.JobCompleted(runId);
			AddFollowUpSimulatorFactories(runContainerId);

			try {
				_runsRwLock.EnterWriteLock();
				//_unfinishedRuns.Remove(runId);
				_unfinishedRuns.TryRemove(runId, out var tmpVal);
				if (AllCompletedUnsafe()) {
					_sumWriter.Finish();
				}
			} finally {
				_runsRwLock.ExitWriteLock();
			}
		}

		private void AddFollowUpSimulatorFactories(int runContainerId)
		{
			try {
				_runContainerMap.TryGetValue(runContainerId, out var runContainer);
				var additionalSimulatorFactory = runContainer?.GetFollowUpSimulatorFactory();
				if (additionalSimulatorFactory == null)
					return;

				AddRuns(additionalSimulatorFactory);
				Execute(_multithreaded);
			} catch (Exception ex) {
				Log.Error(ex.Message);
			}
			
		}

		private bool AllCompletedUnsafe()
		{
			return _unfinishedRuns.Count == 0;
		}

		public bool AllCompleted
		{
			get
			{
				return _unfinishedRuns.Count == 0;

			}
		}

		public IDictionary<int, ProgressEntry> GetProgress()
		{
			try {
				_runsRwLock.EnterReadLock();
				foreach (var runEntry in Runs) {
					var key = runEntry.Run.RunIdentifier;
					//Update existing entry
					if(_progressDictionary.ContainsKey(key))
					{
						var entry = _progressDictionary[key];
						entry.Progress = runEntry.Run.Progress;
						entry.Done = runEntry.Done;
						entry.ExecTime = runEntry.ExecTime;
						entry.Success = runEntry.Success;
						entry.Canceled = runEntry.Canceled;
						entry.Error = runEntry.ExecException;
					} else {
						var progressEntry = new ProgressEntry {
							RunId = runEntry.Run.RunIdentifier,
							JobRunId = runEntry.Run.JobRunIdentifier,
							RunName = runEntry.Run.RunName,
							CycleName = runEntry.Run.CycleName,
							RunSuffix = runEntry.Run.RunSuffix,
							Progress = runEntry.Run.Progress,
							Done = runEntry.Done,
							ExecTime = runEntry.ExecTime,
							Success = runEntry.Success,
							Canceled = runEntry.Canceled,
							Error = runEntry.ExecException
						};
						_progressDictionary[key] = progressEntry;
					}
				}
				return _progressDictionary;
				//return Runs.ToDictionary(
				//	r => r.Run.RunIdentifier,
				//	r => new ProgressEntry
				//	{
				//		RunId = r.Run.RunIdentifier,
				//		JobRunId = r.Run.JobRunIdentifier,
				//		RunName = r.Run.RunName,
				//		CycleName = r.Run.CycleName,
				//		RunSuffix = r.Run.RunSuffix,
				//		Progress = r.Run.Progress,
				//		Done = r.Done,
				//		ExecTime = r.ExecTime,
				//		Success = r.Success,
				//		Canceled = r.Canceled,
				//		Error = r.ExecException
				//	});
			} finally {
				_runsRwLock.ExitReadLock();
			}
			

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
			private readonly ReaderWriterLockSlim _doneLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			private bool _done = false;
			public bool Done => _done;

			volatile public bool Running;
			public bool Success;
			public bool Started = false;
			public bool Canceled;
			public double ExecTime;
			public Exception ExecException;
			public readonly Task RunTask;
			private readonly int _runContainerId;

			public int RunId => Run.RunIdentifier;
			public int JobRunId => Run.JobRunIdentifier;

			public RunEntry(IVectoRun run, JobContainer jobContainer, int runContainerId = -1)
			{
				Run = run;
				JobContainer = jobContainer;
				_runContainerId = runContainerId;
				RunTask = new Task(() => {
					var stopWatch = Stopwatch.StartNew();
					try {
						Running = true;
						Run.Run();
					} catch (Exception ex) {
						Log.Error(ex, "Error during simulation run!");
						ExecException = ex;
						throw;
					} finally {
						stopWatch.Stop();
						Success = Run.FinishedWithoutErrors && ExecException == null;
						ExecTime = stopWatch.Elapsed.TotalMilliseconds;
						JobContainer.JobCompleted(RunId, _runContainerId);
						_done = true;
					}
				}, TaskCreationOptions.LongRunning);
			}

			public Task RunWorkerAsync()
			{
				
				if (!Running && !Started) {
					Started = true;
					RunTask.Start(TaskScheduler.Current);
				}
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