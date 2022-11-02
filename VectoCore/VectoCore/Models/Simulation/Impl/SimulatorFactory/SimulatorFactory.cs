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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModFilter;
using TUGraz.VectoCore.OutputData.XML;
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


		public ISimulatorFactory FollowUpSimulatorFactory
		{
			get
			{
				//if (!CreateFollowUpSimulatorFactory)
				//	return null;

				if (_followUpSimulatorFactoryCreator != null) {
					return _followUpSimulatorFactoryCreator.GetNextFactory();
				} else {
					return null;
				}
				//return _followingSimulatorFactoryCreator?.Invoke();

			}
		}

		public bool CreateFollowUpSimulatorFactory { get; set; } = false;
		protected readonly ExecutionMode _mode;


		#region Constructors and Factory Methods to instantiate Instances of SimulatorFactory without NInject (should only be used in Testcases that are not updated yet)

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

		protected SimulatorFactory(ExecutionMode mode, IOutputDataWriter writer, bool validate = true)
		{
			System.Diagnostics.Debug.WriteLine("Created Simulator Factory");
			Log.Info("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			ReportWriter = writer;
			Validate = validate;
			_mode = mode;

			ThreadPool.GetMinThreads(out var workerThreads, out var completionThreads);
			if (workerThreads < 12) {
				workerThreads = 12;
			}
			ThreadPool.SetMinThreads(workerThreads, completionThreads);

		}


		#endregion






		public bool Validate { get; set; }

		public IVectoRunDataFactory DataReader { get; protected set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ReportWriter { get; protected set; }

		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }
		public bool ModalResults1Hz { get; set; }
		public bool ActualModalData { get; set; }

		public bool SerializeVectoRunData { get; set; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			var warning1Hz = false;
			if (!_simulate) {
				yield break;
			}
			foreach (var data in DataReader.NextRun()) {
				var current = i++;
				var d = data;
				data.JobRunId = current;
				yield return data.Exempted || data.MultistageRun ? GetExemptedRun(data) : GetNonExemptedRun(data, current, d, ref warning1Hz);
			}
		}

		private IVectoRun GetExemptedRun(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data.Loading, data.Mission, data.EngineData?.FuelMode ?? 0, data);
			}
			return new ExemptedRun(new ExemptedRunContainer(data.ExecutionMode) { RunData = data }, modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data.EngineData?.FuelMode ?? 0, data, modData);
				}
			});
		}

		private IVectoRun GetNonExemptedRun(VectoRunData data, int current, VectoRunData d, ref bool warning1Hz)
		{
			var addReportResult = PrepareReport(data);
			if (!data.Cycle.CycleType.IsDistanceBased() && ModalResults1Hz && !warning1Hz) {
				Log.Error("Output filter for 1Hz results is only available for distance-based cycles!");
				warning1Hz = true;
			}
			var fuels = data.EngineData != null ? data.EngineData.Fuels.Select(x => x.FuelData).ToList() : new List<IFuelProperties>();
			IModalDataContainer modContainer =
				new ModalDataContainer(
					data, ReportWriter,
					(_mode == ExecutionMode.Declaration) ? addReportResult : null,
					GetModDataFilter(data)) {
					WriteModalResults = _mode != ExecutionMode.Declaration || WriteModalResults
				};


			// TODO: MQ 20200410 - Remove for official release!
			if (SerializeVectoRunData) {
				File.WriteAllText(
					Path.Combine(
						(ReportWriter as FileOutputWriter)?.BasePath ?? "", $"{data.JobName}_{data.Cycle.Name}{data.ModFileSuffix}.json"),
					JsonConvert.SerializeObject(data, Formatting.Indented));
			}

			var run = GetVectoRun(data, modContainer, modData => {
				if (SumData != null) {
					SumData.Write(modData, JobNumber, current, d);
				}
			});

			if (Validate) {
				ValidateVectoRunData(
					run, data.JobType, data.ElectricMachinesData.FirstOrDefault()?.Item1, data.GearboxData?.Type,
					data.Mission != null && data.Mission.MissionType.IsEMS());
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
			if (validationErrors.Any()) {
				throw new VectoException("Validation of Run-Data Failed: " +
										$"{validationErrors.Select(r => r.ErrorMessage + r.MemberNames.Join("; ")).Join("\n")}");
			}
		}

		private static VectoRun GetVectoRun(VectoRunData data, IModalDataContainer modData, WriteSumData sumWriter)
		{
			VectoRun run;
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					if ((data.SimulationType & SimulationType.DistanceCycle) == 0) {
						throw new VectoException("Distance-based cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new DistanceRun(PowertrainBuilder.Build(data, modData, sumWriter));
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

		private static Action<ModalDataContainer> PrepareReport(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data.Loading, data.Mission, data.EngineData.FuelMode, data);
			}
			Action<ModalDataContainer> addReportResult = modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data.EngineData.FuelMode, data, modData);
				}
			};
			return addReportResult;
		}
	}
}

