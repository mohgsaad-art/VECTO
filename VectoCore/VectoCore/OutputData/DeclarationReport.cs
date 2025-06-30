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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

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
		void InitializeReport(VectoRunData modelData);

		/**
		 * called when creating the simulation run (before starting the simulations)
		 * Hence, the report class knows which and how many results to expect after the simulation
		 * (calls to AddResult)
		 */
		void PrepareResult(VectoRunData runData);

		/**
		 * called after the simulation run providing the modal data of the simulation 
		 * for the given configuration
		 */
		void AddResult(VectoRunData runData, IModalDataContainer modData);

	}

	public interface IResultEntry
	{
		void Initialize(VectoRunData vectoRunData);
		
		void Initialize(VectoRunData vectoRunData, IModalDataContainer modalData);

		VectoRunData VectoRunData { get; }

		VectoRun.Status Status { get; }

        MissionType Mission { get; }

		LoadingType LoadingType { get; }

		int FuelMode { get; }
		IList<IFuelProperties> FuelData { get; }

		MeterPerSecond AverageSpeed { get; }

		MeterPerSecond AverageDrivingSpeed { get; }
		MeterPerSecond MaxSpeed { get; }
		MeterPerSecond MinSpeed { get; }
		MeterPerSquareSecond MaxDeceleration { get; }
		MeterPerSquareSecond MaxAcceleration { get; }

		PerSecond EngineSpeedDrivingMin { get; }
		PerSecond EngineSpeedDrivingAvg { get;}
		PerSecond EngineSpeedDrivingMax { get; }
		double AverageGearboxEfficiency { get;  }

		double AverageAxlegearEfficiency { get; }
		Scalar FullLoadPercentage { get; }
		Scalar GearshiftCount { get; }
		Meter Distance { get; }

		IFuelConsumptionCorrection FuelConsumptionFinal(FuelType fuelType);

		WattSecond ElectricEnergyConsumption { get; }

		Kilogram CO2Total { get; }
		Kilogram Payload { get; set; }
		Kilogram TotalVehicleMass { get; }
		CubicMeter CargoVolume { get; }

		double? PassengerCount { get; }
		VehicleClass VehicleClass { get; }
		VehicleClass? PrimaryVehicleClass { get; }

		Watt MaxChargingPower { get; }

		double WeightingFactor { get; }

		Meter ActualChargeDepletingRange { get; }

		Meter EquivalentAllElectricRange { get; }

		Meter ZeroCO2EmissionsRange { get; }

		Meter HydrogenRange { get; }

        DeclarationData.ElectricRangesPEV BeginOfLifeRanges { get; }

        DeclarationData.ElectricRangesPEV EndOfLifeRanges { get; }

        IFuelProperties AuxHeaterFuel { get; }
		Kilogram ZEV_FuelConsumption_AuxHtr { get; }
		Kilogram ZEV_CO2 { get; }

		OvcHevMode OVCMode { get; }
		int OVCIteration { get; }
		double DeltaSoC { get; }

        void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor);

		string Error { get; }

		string StackTrace { get; }

		BatterySystemData BatteryData { get; }

		double BatteryEfficiencyDischarge { get; set; }
		
		void SetResultWeightingFactor(double weightingFactor);

		IResultEntry Clone(OvcHevMode ovcMode);
	}

	public interface IWeightedResult
	{
		VectoRun.Status Status { get; }

		VectoSimulationJobType JobType { get; }

		bool OffVehicleCharging { get; }

		MeterPerSecond AverageSpeed { get; }

		MeterPerSecond AverageDrivingSpeed { get; }

		Meter Distance { get; }

		Kilogram Payload { get; }

		CubicMeter CargoVolume { get; }

		double? PassengerCount { get; }

		IDictionary<IFuelProperties, Kilogram> FuelConsumption { get; }
		
		IDictionary<IFuelProperties, KilogramPerMeter> FuelConsumptionPerMeter { get; }

		WattSecond ElectricEnergyConsumption { get; }

		KilogramPerMeter CO2PerMeter { get; }

		Meter ActualChargeDepletingRange { get; }

		Meter EquivalentAllElectricRange { get; }

		Meter ZeroCO2EmissionsRange { get; }

		Meter HydrogenRange { get; }

		double UtilityFactor { get; }

		IFuelProperties AuxHeaterFuel { get; set; }
		KilogramPerMeter ZEV_FuelConsumption_AuxHtr { get; set; }
		KilogramPerMeter ZEV_CO2 { get; set; }
	}

	public interface IOVCResultEntry 
	{

		IResultEntry ChargeDepletingResult { get; }

		IResultEntry ChargeSustainingResult { get; }

		IWeightedResult Weighted { get; }

	}


	/// <summary>
	/// Class for creating a declaration report.
	/// </summary>
	public abstract class DeclarationReport<T> : IDeclarationReport where T : class, IResultEntry, new()
	{
		
		private readonly List<(MissionType missionType, LoadingType loading, VectoSimulationJobType simulationType, T entry)> ResultsInternal =
			new List<(MissionType missionType, LoadingType loading, VectoSimulationJobType simulationType, T entry)>();

		
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
		public void PrepareResult(VectoRunData runData)
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

		public void AddResult(VectoRunData runData, IModalDataContainer modData)
		{
			//return;
			if (runData.Mission.MissionType != MissionType.ExemptedMission) {
				var entry = new T();
				entry.Initialize(runData, modData);
				lock (ResultsInternal)
				{
					ResultsInternal.Add((entry.Mission, entry.LoadingType, runData.JobType, entry));
				}

				DoStoreResult(entry, runData, modData);
			}

			WriteResults();
		}

		protected virtual IEnumerable<T> Results {
			get
			{
				lock (ResultsInternal) {
					return ResultsInternal.GroupBy(
							r => (r.missionType, r.loading, vehicleClass: r.entry.VehicleClass, ovcMode: r.entry.OVCMode, r.simulationType),
							r => r.entry,
							(group, results) => {
								var simResults = results.ToList();
								var bestResult = DeclarationData.GetDeclarationReportFinalResultEntryIndex(simResults, group.simulationType);
								if (bestResult < 0 || bestResult >= simResults.Count) {
									throw new VectoException($"Invalid index for best result entry. got {bestResult}, max. {simResults.Count}");
								}
								return simResults[bestResult];
							});
				}
            }
		}

		protected virtual IEnumerable<T> OrderedResults
		{
			get
			{
				return Results
					.OrderBy(x => x.VehicleClass)
					.ThenBy(x => x.FuelMode)
					.ThenBy(x => x.Mission)
					.ThenBy(x => x.LoadingType);
            }
		}

		/// <summary>
		/// Adds the result of one run for the specific mission and loading. 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="runData"></param>
		/// <param name="modData">The mod data.</param>
		//[MethodImpl(MethodImplOptions.Synchronized)] //Results are already locked
		protected abstract void DoStoreResult(T entry, VectoRunData runData, IModalDataContainer modData);


		protected internal virtual void DoWriteReport()
		{
			/// Check if LH does not meet LH requierements, i.e. ReferenceLoad and OperationalRange > 350km.
			var RDGroupEntry = Results.SingleOrDefault(e => DeclarationData.EvaluateLHSubgroupConditions(e));

			foreach (var resultEntry in OrderedResults)
			{
				var rdResultEntry = RDGroupEntry != null ? RDGroupEntry : resultEntry;
				var vectoRun = resultEntry.VectoRunData;

				// Set new weighting factors according to new RD group.
				SetWeightingFactors(vectoRun, OrderedResults, rdResultEntry?.ActualChargeDepletingRange?.Value());

				// Update results with newest weighting factors (WFs), i.e. RD WFs if updated otherwise if else.
				WriteResult(resultEntry);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}

		protected abstract void OutputReports();

		protected abstract void GenerateReports();

		protected abstract void WriteResult(T result);

		public abstract void InitializeReport(VectoRunData modelData);

		public abstract void SetWeightingFactors(VectoRunData runData, IEnumerable<IResultEntry> orderedeResults, double? electricRange);
	}
}
