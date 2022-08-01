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
using Newtonsoft.Json;
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
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class SimulatorFactory : LoggingObject, ISimulatorFactory
	{
		private static int _jobNumberCounter;

		private readonly ExecutionMode _mode;

		public SimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer,
			IDeclarationReport declarationReport = null, IVTPReport vtpReport = null, bool validate = true, bool validateHashes = false)
		{
			Log.Info("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			ModWriter = writer;
			Validate = validate;
			ValidateComponentHashes = validateHashes;

			int workerThreads;
			int completionThreads;
			ThreadPool.GetMinThreads(out workerThreads, out completionThreads);
			if (workerThreads < 12) {
				workerThreads = 12;
			}
			ThreadPool.SetMinThreads(workerThreads, completionThreads);

			switch (mode) {
				case ExecutionMode.Declaration:
					CreateDeclarationDataReader(dataProvider, declarationReport, vtpReport);
					break;
				case ExecutionMode.Engineering:
					CreateEngineeringDataReader(dataProvider);
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
		}

		public bool ValidateComponentHashes { get; set; }

		private void CreateDeclarationDataReader(IInputDataProvider dataProvider, IDeclarationReport declarationReport, IVTPReport vtpReport)
		{
			if (dataProvider is IVTPDeclarationInputDataProvider) {
				var vtpProvider = dataProvider as IVTPDeclarationInputDataProvider;
				var report = vtpReport ?? new XMLVTPReport(ModWriter);
				DataReader = new DeclarationVTPModeVectoRunDataFactory(vtpProvider, report);
				return;
			}
			if (dataProvider is IDeclarationInputDataProvider) {
				var declDataProvider = dataProvider as IDeclarationInputDataProvider;
				var report = declarationReport ?? new XMLDeclarationReport(ModWriter);
				DataReader = new DeclarationModeVectoRunDataFactory(declDataProvider, report);
				return;
			}
			throw new VectoException("Unknown InputData for Declaration Mode!");
		}

		private void CreateEngineeringDataReader(IInputDataProvider dataProvider)
		{
			if (dataProvider is IVTPEngineeringInputDataProvider) {
				var vtpProvider = dataProvider as IVTPEngineeringInputDataProvider;
				DataReader = new EngineeringVTPModeVectoRunDataFactory(vtpProvider);
				return;
			}
			if (dataProvider is IEngineeringInputDataProvider) {
				var engDataProvider = dataProvider as IEngineeringInputDataProvider;
				if (engDataProvider.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation) {
					DataReader = new EngineOnlyVectoRunDataFactory(engDataProvider);
				} else {
					DataReader = new EngineeringModeVectoRunDataFactory(engDataProvider);
				}
				return;
			}
			throw  new VectoException("Unknown InputData for Engineering Mode!");
		}

		public bool Validate { get; set; }

		public IVectoRunDataFactory DataReader { get; private set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ModWriter { get; private set; }

		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }
		public bool ModalResults1Hz { get; set; }
		public bool ActualModalData { get; set; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			var warning1Hz = false;

			if (ValidateComponentHashes && DataReader.DataProvider is IDeclarationInputDataProvider declDataProvider) {
				declDataProvider.ValidateComponentHashes();
			}

			foreach (var data in DataReader.NextRun()) {
				var current = i++;
				var d = data;
				data.JobRunId = current;
				yield return data.Exempted ? GetExemptedRun(data) : GetNonExemptedRun(data, current, d, ref warning1Hz);
			}
		}

		private IVectoRun GetExemptedRun(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data.Loading, data.Mission, data);
			}
			return new ExemptedRun(new VehicleContainer(data.ExecutionMode) { RunData = data }, modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data, modData);
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

			IModalDataContainer modContainer =
				new ModalDataContainer(
					data, ModWriter,
					addReportResult: _mode == ExecutionMode.Declaration ? addReportResult : null,
					filter: GetModDataFilter(data)) {
					WriteModalResults = _mode != ExecutionMode.Declaration || WriteModalResults
				};

			var builder = new PowertrainBuilder(
				modContainer, modData => {
					if (SumData != null) {
						SumData.Write(modData, JobNumber, current, d);
					}
				});

			var run = GetVectoRun(data, builder);

			if (Validate) {
				ValidateVectoRunData(
					run, data.GearboxData == null ? (GearboxType?)null : data.GearboxData.Type,
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

		private void ValidateVectoRunData(VectoRun run, GearboxType? gearboxtype, bool isEms)
		{
			var validationErrors = run.Validate(_mode, gearboxtype, isEms);
			if (validationErrors.Any()) {
				throw new VectoException("Validation of Run-Data Failed: " +
										string.Join("\n", validationErrors.Select(r => r.ErrorMessage + string.Join("; ", r.MemberNames))));
			}
		}

		private static VectoRun GetVectoRun(VectoRunData data, PowertrainBuilder builder)
		{
			VectoRun run;
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					if ((data.SimulationType & SimulationType.DistanceCycle) == 0) {
						throw new VectoException("Distance-based cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new DistanceRun(builder.Build(data));
					break;
				case CycleType.EngineOnly:
					if ((data.SimulationType & SimulationType.EngineOnly) == 0) {
						throw new VectoException("Engine-only cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(builder.Build(data));
					break;
				case CycleType.VTP:
					if ((data.SimulationType & SimulationType.VerificationTest) == 0) {
						throw new VectoException("VTP-cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(builder.Build(data));
					break;

				case CycleType.PWheel:
				case CycleType.MeasuredSpeed:
				case CycleType.MeasuredSpeedGear:
					if ((data.SimulationType & (SimulationType.PWheel | SimulationType.MeasuredSpeedCycle)) == 0) {
						throw new VectoException("{1}-cycle can not be simulated in {0} mode", data.SimulationType, data.Cycle.CycleType);
					}
					run = new TimeRun(builder.Build(data));
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
				data.Report.PrepareResult(data.Loading, data.Mission, data);
			}
			Action<ModalDataContainer> addReportResult = modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data, modData);
				}
			};
			return addReportResult;
		}
	}
}
