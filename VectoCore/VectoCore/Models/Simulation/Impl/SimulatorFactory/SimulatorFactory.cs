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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModFilter;
using Formatting = Newtonsoft.Json.Formatting;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public abstract class SimulatorFactory : LoggingObject, ISimulatorFactory
	{
		private static int _jobNumberCounter;

		private static object _kernelLock = new object();
		private static IKernel _kernel; //Kernel is only used when the SimulatorFactory is created with the Factory Method.
        
		protected IFollowUpSimulatorFactoryCreator _followUpSimulatorFactoryCreator = null;

		protected bool _simulate = true;

		public bool CreateFollowUpSimulatorFactory { get; set; } = false;

		protected readonly ExecutionMode _mode;

		protected IPowertrainBuilder PowertrainBuilder { get; }

		protected IModalDataFactory ModDataFactory { get; }


		#region Constructors and Factory Methods to instantiate Instances of SimulatorFactory without NInject (should only be used in Testcases that are not updated yet)

		protected SimulatorFactory(ExecutionMode mode, IOutputDataWriter writer, bool validate, IPowertrainBuilder ptBuilder, IModalDataFactory modDataFactory)
		{
			System.Diagnostics.Debug.WriteLine("Created Simulator Factory");
			Log.Info("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			ReportWriter = writer;
			Validate = validate;
			_mode = mode;
			PowertrainBuilder = ptBuilder;
			ModDataFactory = modDataFactory;

            ThreadPool.GetMinThreads(out var workerThreads, out var completionThreads);
			if (workerThreads < 12) {
				workerThreads = 12;
			}
			ThreadPool.SetMinThreads(workerThreads, completionThreads);

		}

        [Obsolete("Creation of new SimulatorFactories should be done with SimulatorFactoryFactory NInject Factory", false)]
		public static ISimulatorFactory CreateSimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer, IDeclarationReport declarationReport = null, IVTPReport vtpReport=null, bool validate = true)
		{
			if (_kernel == null) {
				lock (_kernelLock) {
					if (_kernel == null) {
						_kernel = new StandardKernel(new VectoNinjectModule());
					}
				}
			}
			return _kernel.Get<ISimulatorFactoryFactory>().Factory(mode, dataProvider, writer, declarationReport, vtpReport, validate);
		}

		#endregion

		public ISimulatorFactory FollowUpSimulatorFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries)
		{
			var factory = _followUpSimulatorFactoryCreator?.GetNextFactory(progressEntries);
			if (factory != null) {
				factory.WriteModalResults = this.WriteModalResults;
				//factory.SerializeVectoRunData = this.SerializeVectoRunData;
			}
			return factory;
		}

        public bool Validate { get; set; }

		public IVectoRunDataFactory RunDataFactory { get; protected set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ReportWriter { get; protected set; }

		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }
		public bool ModalResults1Hz { get; set; }
		public bool ActualModalData { get; set; }

		public bool SerializeVectoRunData { get; set; }

		public List<string> MissingInputEntries { get; protected set; } = new List<string>();

		/// <summary>
		/// Only for testing purposes
		/// </summary>
		public Action<VectoRunData> ModifyRunData { get; set; }


		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			bool firstRun = true;
			var warning1Hz = false;
			if (!_simulate) {
				if (MissingInputEntries.Count > 0) {
					throw new VectoException(MissingInputEntries.Select(x => $"<{x}> is bad or missing.").Join("\n"));
				}
				yield break;
			}
			foreach (var data in RunDataFactory.NextRun()) {
				var current = i++;
				data.JobRunId = current;



				if (ModifyRunData != null) {
					ModifyRunData(data);

				}


				yield return (data.Exempted || data.MultistageRun) ? GetExemptedRun(data) : GetNonExemptedRun(data, current, ref warning1Hz, ref firstRun);
			}
		}


		protected virtual IVectoRun GetExemptedRun(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data);
			}

			var container = PowertrainBuilder.BuildExempted(data);
			return new ExemptedRun(container, modData => {
				if (data.Report != null) {
					data.Report.AddResult(data, modData);
				}
			});
        }

		protected virtual IVectoRun GetNonExemptedRun(VectoRunData data, int current, ref bool warning1Hz, ref bool firstRun)
		{
			var addReportResult = PrepareReport(data);
			if (!data.Cycle.CycleType.IsDistanceBased() && ModalResults1Hz && !warning1Hz) {
				Log.Error("Output filter for 1Hz results is only available for distance-based cycles!");
				warning1Hz = true;
			}
			var fuels = data.EngineData != null ? data.EngineData.Fuels.Select(x => x.FuelData).ToList() : new List<IFuelProperties>();
			var modContainer = ModDataFactory.CreateModDataContainer(
				//new ModalDataContainer(
				data, ReportWriter,
				(_mode == ExecutionMode.Declaration) ? addReportResult : null,
				GetModDataFilter(data));
			modContainer.WriteModalResults = _mode != ExecutionMode.Declaration || WriteModalResults;

            // TODO: MQ 20200410 - Remove for official release!
            if (SerializeVectoRunData) {
				var jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                File.WriteAllText(
					Path.Combine(
						(ReportWriter as FileOutputWriter)?.BasePath ?? "", $"{data.JobName}_{data.Cycle.Name}{data.ModFileSuffix}.json"),
					JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings));
			}
			data.JobNumber = JobNumber;
			data.RunNumber = current;
			var run = GetVectoRun(data, modContainer, SumData);

			if (Validate && firstRun) {
				ValidateVectoRunData(
					run, data.JobType, data.ElectricMachinesData.FirstOrDefault()?.Item1, data.GearboxData?.Type,
					data.Mission != null && data.Mission.MissionType.IsEMS());
				firstRun = false;
			}
			return run;
		}

		private IModalDataFilter[] GetModDataFilter(VectoRunData data)
		{
			var modDataFilter = ModalResults1Hz
				? new IModalDataFilter[] { new ModalData1HzFilter() }
				: null;

			if (ActualModalData) {
				modDataFilter = new IModalDataFilter[] { new ActualModalDataFilter(), }; 
			}
			return data.Cycle.CycleType.IsDistanceBased() && ModalResults1Hz || ActualModalData ? modDataFilter : null;
		}
		
		private void ValidateVectoRunData(VectoRun run, VectoSimulationJobType jobType, PowertrainPosition? emPosition, GearboxType? gearboxtype, bool isEms)
		{
			var validationErrors = run.Validate(_mode, jobType, emPosition, gearboxtype, isEms);
			//  TODO cleanup of object cache

			ValidationHelper.ClearValHistory();

			if (validationErrors.Any()) {
				throw new VectoException("Validation of Run-Data Failed: " +
										$"{validationErrors.Select(r => r.ErrorMessage + r.MemberNames.Join("; ")).Join("\n")}");
			}
		}

		private VectoRun GetVectoRun(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			VectoRun run;
			
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					if ((data.SimulationType & SimulationType.DistanceCycle) == 0) {
						throw new VectoException("Distance-based cycle can not be simulated in {0} mode", data.SimulationType);
					}

					var container = PowertrainBuilder.Build(data, modData, sumWriter);
					
					run = new DistanceRun(container, new FollowUpRunCreator(data.IterativeRunStrategy, PowertrainBuilder), new DefaultPostMortemAnalyzer(data.PostMortemStrategy)); 
					break;
				case CycleType.EngineOnly:
					if ((data.SimulationType & SimulationType.EngineOnly) == 0) {
						throw new VectoException("Engine-only cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(PowertrainBuilder.Build(data, modData, sumWriter));
					break;
				case CycleType.VTP:
					if ((data.SimulationType & SimulationType.VerificationTest) == 0) {
						throw new VectoException("VTP-cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(PowertrainBuilder.Build(data, modData, sumWriter));
					break;

				case CycleType.PWheel:
				case CycleType.MeasuredSpeed:
				case CycleType.MeasuredSpeedGear:
					if ((data.SimulationType & (SimulationType.PWheel | SimulationType.MeasuredSpeedCycle)) == 0) {
						throw new VectoException("{1}-cycle can not be simulated in {0} mode", data.SimulationType, data.Cycle.CycleType);
					}
					run = new TimeRun(PowertrainBuilder.Build(data, modData, sumWriter));
					break;
				case CycleType.PTO:
					throw new VectoException("PTO Cycle can not be used as main cycle!");
				default:
					throw new ArgumentOutOfRangeException("CycleType unknown:" + data.Cycle.CycleType);
			}
			return run;
		}

		protected static Action<IModalDataContainer> PrepareReport(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data);
			}
			Action<IModalDataContainer> addReportResult = modData => {
				if (data.Report != null) {
					data.Report.AddResult(data, modData);
				}
			};
			
			return addReportResult;
		}
	}
}

