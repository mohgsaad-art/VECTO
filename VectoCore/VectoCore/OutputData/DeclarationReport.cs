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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.OutputData
{
	public interface IDeclarationReport
	{
		/**
		 * allow to initialize the report
		 * read the configuration of the powertrain to be used during the simulation.
		 * This methodd is called once befor creating the simulation runs with a temporary 
		 * VectoRunData instance
		 */
		void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);

		/**
		 * called when creating the simulation run (before starting the simulations)
		 * Hence, the report class knows which and how many results to expect after the simulation
		 * (calls to AddResult)
		 */
		void PrepareResult(LoadingType loading, Mission mission, int fuelMode, VectoRunData runData);

		/**
		 * called after the simulation run providing the modal data of the simulation 
		 * for the given configuration
		 */
		void AddResult(
			LoadingType loadingType, Mission mission, int fuelMode, VectoRunData runData, IModalDataContainer modData);

	}

	public interface IResultEntry
	{
		MissionType Mission { get; set; }

		LoadingType LoadingType { get; set; }

		int FuelMode { get; set; }
		IList<IFuelProperties> FuelData { get; set; }
		Kilogram Payload { get; set; }
		Kilogram TotalVehicleMass { get; set; }
		CubicMeter CargoVolume { get; set; }

		double? PassengerCount { get; set; }
		VehicleClass VehicleClass { get; set; }

		void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor);
	}

	/// <summary>
	/// Class for creating a declaration report.
	/// </summary>
	public abstract class DeclarationReport<T> : IDeclarationReport where T : IResultEntry, new()
	{
		public class ResultContainer<TEntry>
		{
			public MissionType Mission;

			public Dictionary<LoadingType, TEntry> ResultEntry;
		}


		/// <summary>
		/// Dictionary of MissionTypes and their corresponding results.
		/// </summary>
		//protected readonly Dictionary<MissionType, ResultContainer<T>> Missions =
		//	new Dictionary<MissionType, ResultContainer<T>>();
		//protected readonly Dictionary<int, Dictionary<MissionType, ResultContainer<T>>> Missions =
		//new Dictionary<int, Dictionary<MissionType, ResultContainer<T>>>();
		protected readonly List<T> Results = new List<T>();

		/// <summary>
		/// The full load curve.
		/// </summary>
		//internal Dictionary<uint, EngineFullLoadCurve> Flc { get; set; }

		///// <summary>
		///// The declaration segment from the segment table
		///// </summary>
		//internal Segment? Segment { get; set; }
		/// <summary>
		/// The result count determines how many results must be given before the report gets written.
		/// </summary>
		private int _resultCount;

		protected readonly IReportWriter Writer;

		protected DeclarationReport(IReportWriter writer)
		{
			Writer = writer;
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public void PrepareResult(LoadingType loading, Mission mission, int fuelMode, VectoRunData runData)
		{
			_resultCount++;
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		protected void WriteResults()
		{
			_resultCount--;
			if (_resultCount == 0) {
				DoWriteReport();
			}
		}

		public void AddResult(
			LoadingType loadingType, Mission mission, int fuelMode, VectoRunData runData,
			IModalDataContainer modData)
		{
			if (mission.MissionType != MissionType.ExemptedMission) {
				var entry = new T {
					Mission = mission.MissionType,
					LoadingType = loadingType,
					FuelMode = fuelMode,
					FuelData = runData.EngineData.Fuels.Select(x => x.FuelData).ToList(),
					Payload = runData.VehicleData.Loading,
					TotalVehicleMass = runData.VehicleData.TotalVehicleMass,
					CargoVolume = runData.VehicleData.CargoVolume,
					VehicleClass = runData.Mission?.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass,
					//runData.VehicleData.VehicleClass,
					PassengerCount = runData.VehicleData.PassengerCount
				};

				Results.Add(entry);
				DoStoreResult(entry, runData, modData);
			}

			WriteResults();
		}

		protected virtual IEnumerable<T> OrderedResults
		{
			get {
				return Results.OrderBy(x => x.VehicleClass).ThenBy(x => x.FuelMode).ThenBy(x => x.Mission)
							.ThenBy(x => x.LoadingType);
			}
		}

		/// <summary>
		/// Adds the result of one run for the specific mission and loading. 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="runData"></param>
		/// <param name="modData">The mod data.</param>
		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected abstract void DoStoreResult(T entry, VectoRunData runData, IModalDataContainer modData);


		protected internal virtual void DoWriteReport()
		{
			foreach (var result in OrderedResults) {
				WriteResult(result);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}

		protected abstract void OutputReports();

		protected abstract void GenerateReports();

		protected abstract void WriteResult(T result);

		public abstract void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);
	}
}
