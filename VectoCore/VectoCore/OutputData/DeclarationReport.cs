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
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
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
		void PrepareResult(LoadingType loading, Mission mission, VectoRunData runData);

		/**
		 * called after the simulation run providing the modal data of the simulation 
		 * for the given configuration
		 */
		void AddResult(LoadingType loadingType, Mission mission, VectoRunData runData, IModalDataContainer modData);
	}

	/// <summary>
	/// Class for creating a declaration report.
	/// </summary>
	public abstract class DeclarationReport<T> : IDeclarationReport where T : new()
	{
		public class ResultContainer<TEntry>
		{
			public MissionType Mission;

			public Dictionary<LoadingType, TEntry> ResultEntry;
		}


		/// <summary>
		/// Dictionary of MissionTypes and their corresponding results.
		/// </summary>
		protected readonly Dictionary<MissionType, ResultContainer<T>> Missions =
			new Dictionary<MissionType, ResultContainer<T>>();

		/// <summary>
		/// The full load curve.
		/// </summary>
		internal Dictionary<uint, EngineFullLoadCurve> Flc { get; set; }

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
		public void PrepareResult(LoadingType loading, Mission mission, VectoRunData runData)
		{
			var missionType = mission.MissionType;
			if (!Missions.ContainsKey(missionType)) {
				Missions[missionType] = new ResultContainer<T>() {
					Mission = missionType,
					ResultEntry = new Dictionary<LoadingType, T>(),
				};
			}
			if (missionType != MissionType.ExemptedMission) {
				Missions[mission.MissionType].ResultEntry[loading] = new T();
			}
			_resultCount++;
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddResult(LoadingType loadingType, Mission mission, VectoRunData runData,
			IModalDataContainer modData)
		{
			if (!Missions.ContainsKey(mission.MissionType)) {
				throw new VectoException("Unknown mission type {0} for generating declaration report", mission.MissionType);
			}
			if (mission.MissionType != MissionType.ExemptedMission && !Missions[mission.MissionType].ResultEntry.ContainsKey(loadingType)) {
				throw new VectoException("Unknown loading type {0} for mission {1}", loadingType, mission.MissionType);
			}
			_resultCount--;

			if (mission.MissionType != MissionType.ExemptedMission) {
				DoAddResult(Missions[mission.MissionType].ResultEntry[loadingType], runData, modData);
			}

			if (_resultCount == 0) {
				DoWriteReport();
				Flc = null;
			}
		}

		/// <summary>
		/// Adds the result of one run for the specific mission and loading. If all runs finished (given by the resultCount) the report will be written.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="runData"></param>
		/// <param name="modData">The mod data.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected abstract void DoAddResult(T entry, VectoRunData runData, IModalDataContainer modData);


		protected internal abstract void DoWriteReport();


		public abstract void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);
	}
}