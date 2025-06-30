#define TRACE_FC

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using NLog.LayoutRenderers;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public interface IFuelCellPreRunInfo
    {
        Meter WindowSize { get; }
        int BinarySearchIterations { get; }
        double StartSoC { get; }

		FuelCellPreRunPostprocessor.SearchResult[] AcceptedSearchResults { get; }
        FuelCellPreRunPostprocessor.SearchResult[] RejectedSearchResults { get; }
    }

    public partial class FuelCellPreRunPostprocessor : IFuelCellPreRunInfo
    {
		public string WriterBasePath { get; set; }

		#region PreRunInfo

		private string RunName;
		private PreRunEntry[] _preRunResults;
		private Meter _totalDistance;
		private bool _writeModalResults = true;

        public Meter TotalDistance
		{
			get => _totalDistance ?? (_totalDistance =
				_preRunResults.Last().s - (_preRunResults.First().s - _preRunResults.First().ds));
		}
		private Second _totalDuration;
        public Second TotalDuration { get => _totalDuration ??
											(_totalDuration = _preRunResults.Last().t - _preRunResults.First().t + _preRunResults.First().dt / 2 + _preRunResults.Last().dt / 2); }

        #endregion


        #region BinarySearch
        public SearchResult[] AcceptedSearchResults { get; private set; }
        public SearchResult[] RejectedSearchResults { get; private set; }
		public int BinarySearchIterations { get; private set; }
        #endregion



        public double StartSoC { get; private set; }

        public Meter WindowSize { get; private set; }

        private static object fileLock = new object();



		public class SearchResult
        {
			public SearchResult(bool success, double initSoc, Meter distance, FCCalcEntry[] entries, string reason)
			{
				_success = success;
				_initSoc = initSoc;
				_distance = distance;
				_entries = entries;
				_reason = reason;
			}

			private readonly FCCalcEntry[] _entries;
			public FCCalcEntry[] Entries => _entries;

			private readonly double _initSoc;
			public double InitSoc => _initSoc;

			private readonly bool _success;
			public bool Success => _success;

			private readonly Meter _distance;
			private readonly string _reason;
			public Meter Distance => _distance;
		}


        public FuelCellPreRunPostprocessor(IModalDataContainer modData)
		{
			FillPreRunResults(modData);
			RunName = modData?.RunName ?? "";
		}

		private void FillPreRunResults(IModalDataContainer modData)
		{
			var data = modData?.Data;
			var timeCol = data?.Columns[ModalResultField.time.GetShortCaption()];
			var dtCol = data?.Columns[ModalResultField.simulationInterval.GetName()];
			var distCol = data?.Columns[ModalResultField.dist.GetShortCaption()];
			var dsCol = data?.Columns[ModalResultField.simulationDistance.GetName()];
			var esTCol = data?.Columns[ModalResultField.P_terminal_ES.GetName()];
			var elAuxCol = data?.Columns[ModalResultField.P_Aux_el_HV.GetName()];

			_preRunResults = data?.AsEnumerable().Select(m => new PreRunEntry() {
				P_es_T = (Watt)m[esTCol],
				P_el_aux = (Watt)m[elAuxCol],
				t = (Second)m[timeCol],
				dt = (Second)m[dtCol],
				s = (Meter)m[distCol],
                ds = (Meter)m[dsCol],
			}).ToArray();
		}

		private ElectricSystem GetElectricSystem(IElectricEnergyStorage batSystem, BatterySystemData batData)
		{

			var es = new ElectricSystem(null, batData);
			es.Connect(batSystem);

			return es;
		}


		private ElectricSystem GetElectricSystem(BatterySystemData batData, double initSoc, out BatterySystem batSystem)
		{

			batSystem = new BatterySystem(null, batData);

			batSystem.Initialize(initSoc);
			var es = GetElectricSystem(batSystem, batData);

			return es;
		}

		public SearchResult CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData, bool writeModalResults)
		{
			_writeModalResults = writeModalResults;
			return CalculateFuelCellPowerDemand(fcData, batData);
		}

		/// <summary>
		/// Entry
		/// </summary>
		/// <param name="fcData"></param>
		/// <param name="batData"></param>
		/// <returns></returns>
		public SearchResult CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{
			batData = batData.Clone();
			var es = GetElectricSystem(batData, batData.InitialSoC, out _);
			var maxWindowSize = fcData.MaxWindowSize ?? TotalDistance;

			List<FCCalcEntry> rawFcCalcEntries = new List<FCCalcEntry>();
			rawFcCalcEntries.AddRange(FcCalcEntries(maxWindowSize, 0.SI<Watt>(), fcData.MaxElectricPower, es, _preRunResults, TotalDistance, TotalDuration));

			ApplyBatterySafetyMargin(batData, rawFcCalcEntries, maxWindowSize);

			if (TryWithFullDistance(fcData, ref batData, out var result)) {
				StartSoC = result.InitSoc;
				WindowSize = maxWindowSize;
				BinarySearchIterations = 0;
				return result;
			}
			return BinarySearchFuelCellPowerDemand(fcData, batData);
		}

		private SearchResult BinarySearchFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{
			double searchThreshold = 0.05;
			var minWindowSize = 5.SI<Meter>();
            var maxWindowSize = fcData.MaxWindowSize ?? TotalDistance;
            //Start binary search
            var accepted = new List<SearchResult>();
			// we can add the full distance already beforehand because the search starts only if the full distance is not
			// feasible.
			var rejected = new List<SearchResult>() {
				new SearchResult(false, batData.InitialSoC, maxWindowSize, Array.Empty<FCCalcEntry>(), "init assumption for binary search")
			};
			var iterationCount = 0;
			try {
				SearchAlgorithm.BinarySearch(0.SI<Meter>(), maxWindowSize,
					evaluateFunction:
					distance => {
						CalculateFuelCellPowerDemandForWindowSize(distance, fcData, batData, out var result);
						return result;
					},
					acceptFunction:
					(distance, result) => {
						var searchResult = (SearchResult)result;
						if (searchResult.Success) {
							accepted.Add(searchResult);
						} else {
							rejected.Add(searchResult);
						}

						return searchResult.Success;
					},
					abortCriterion: (d, o) => {
						if (d < minWindowSize) {
							throw new VectoSearchAbortedException(
								$"Window size < {minWindowSize}, battery is too small");
						}

						if (!accepted.Any() || !rejected.Any()) {
							return false;
						}

						var lastAccepted = accepted.Last();
						var lastRejected = rejected.Last();
						var deviation = VectoMath.Abs((lastRejected.Distance - lastAccepted.Distance)) /
							(lastRejected.Distance + lastAccepted.Distance) * 2;
						if (deviation < searchThreshold) {

						}

						return deviation < searchThreshold;
					},
					ref iterationCount,
					searcher: this
				);
			} finally {
				BinarySearchIterations = iterationCount;
				WindowSize = accepted.LastOrDefault()?.Distance ?? maxWindowSize;
				RejectedSearchResults = rejected.ToArray();
				AcceptedSearchResults = accepted.ToArray();
				StartSoC = accepted.LastOrDefault()?.InitSoc ?? 0.0;
			}

			return accepted.Last();
		}

		private bool TryWithFullDistance(FuelCellSystemData fcData, ref BatterySystemData batData,
			out SearchResult result)
		{
			result = null;
			var fcSufficient = CheckFCPower(fcData, batData, out var msg);
			if (!fcSufficient) {
				throw new VectoException($"FuelCell power not sufficient {msg}");
			}

            var maxWindowSize = fcData.MaxWindowSize ?? TotalDistance;
            return CalculateFuelCellPowerDemandForWindowSize(maxWindowSize, fcData, batData, out result);
		}

		public bool TryShiftInitialSoC(double batMinSoc, double batMaxSoc, double initSoc, double minSocTrace,
            double maxSocTrace, out double shiftedSoc)
        {
            shiftedSoc = -1;
            if ((batMaxSoc - batMinSoc).IsSmaller(maxSocTrace - minSocTrace, 1E-03))
            {
                return false;
            }
			//check difference
            var initSoc_min = batMinSoc + (initSoc - minSocTrace);
            var initSoc_max = batMaxSoc - (maxSocTrace - initSoc);

            shiftedSoc = (initSoc_min + initSoc_max) / 2;

			if (shiftedSoc.IsSmaller(batMinSoc) || shiftedSoc.IsGreater(batMaxSoc)) {
				throw new VectoException($"Calculated SoC {shiftedSoc} is out of range!");
			}

			return true;
        }


        public bool CalculateFuelCellPowerDemandForWindowSize(Meter windowSize, FuelCellSystemData fcData,
            BatterySystemData batData, out SearchResult result)
        {
            ///For a given window size as first step we calculate the fuel cell power, try to simulate it with a tracing infinity battery and check if the SoC limits are violated
			//var minFcPower = fcData.MinElectricPower;
			var minFcPower = 0.SI<Watt>();
			//var maxFcPower = fcData.MaxElectricPower;

			batData = batData.Clone();
			//Calculate Raw Fuel CellDemand
			//var batSystem = new BatterySystem(null, batData);
			var initSoc = batData.InitialSoC;
			var es = GetElectricSystem(batData, batData.InitialSoC, out var batSystem);


			var fcCalcEntries = GetRawFuelCellPowerDemand(windowSize: windowSize, minFcPower: minFcPower,
				maxFcPower: fcData.MaxElectricPower, es, preRunResults: _preRunResults);

			var processedFcCalcEntries = new List<FCCalcEntry>(_preRunResults.Length); //Holds the processed fcCalcEntries to avoid multiple enumerations
			//var socLimits = ApplyBatterySafetyMargin(batData, fcCalcEntries, batSystem);
			var usableRange = batSystem.MaxSoC - batSystem.MinSoC;


			//Determine battery energy losses ---------------------------------------------------------------------------------
			//and check for SoC violations
			var infBatData = batData.Clone();
            var tracingInfinityBat = new TracingInfinityBatterySystem(infBatData);
			tracingInfinityBat.Initialize(batData.InitialSoC);
			var infEs = GetElectricSystem(tracingInfinityBat, infBatData);



			var deltaEnergyBatInt = 0.SI<WattSecond>();


			var infinityDummyContainer = new SimpleModDataContainer();




            var correctedPreRunResults = new List<PreRunEntry>();
			var preRunResultsAreCorrected = false;


			foreach (var entry in fcCalcEntries) {
				var correctedPreRunEntry = new PreRunEntry(entry.preRunEntry);
				correctedPreRunResults.Add(correctedPreRunEntry);

				var SoC = RequestFromEs(entry, infEs, infinityDummyContainer, tracingInfinityBat, ref deltaEnergyBatInt);

				processedFcCalcEntries.Add(entry);
				if ((SoC - tracingInfinityBat.MinSoC).IsGreater(usableRange))
				{
					//SoC range violated

					//Recuperating
					if (entry.P_el_dem.IsGreater(0))
					{
						//Stop recuperating
						correctedPreRunEntry.P_es_T = 0.SI<Watt>();
						preRunResultsAreCorrected = true;
					}
					else
					{
						//we are not recuperating but the used SoC range is to large,
						//Window is too large.
						result = new SearchResult(
							success: false,
							initSoc: SoC,
							distance: windowSize,
							entries: processedFcCalcEntries.ToArray(),
							reason: $"SoC Range too large usable: {usableRange}, range: {SoC - tracingInfinityBat.MinSoC}");
						WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, false);
						//Return false to decrease window size
						return false;
					}
				}
			}

			if (preRunResultsAreCorrected) {
				//Throw away old results
                deltaEnergyBatInt = 0.SI<WattSecond>();
				processedFcCalcEntries.Clear();


				tracingInfinityBat = new TracingInfinityBatterySystem(batData);
				tracingInfinityBat.Initialize(StartSoC);
				infEs = GetElectricSystem(tracingInfinityBat, batData);

				//Calculate updated fc demand
				fcCalcEntries = GetRawFuelCellPowerDemand(windowSize, minFcPower, fcData.MaxElectricPower, es,
					correctedPreRunResults);


				infinityDummyContainer = new SimpleModDataContainer();
				foreach (var entry in fcCalcEntries) {


					var SoC = RequestFromEs(entry, infEs, infinityDummyContainer, tracingInfinityBat, ref deltaEnergyBatInt);
					processedFcCalcEntries.Add(entry);
					if ((SoC - tracingInfinityBat.MinSoC).IsGreater(usableRange)) {
						throw new VectoException("Violation of usable SoC should be covered");
						if (entry.P_el_dem.IsGreater(0)) {
							throw new VectoException(
								"Parts where we are recuperating with full battery should not be included");
						} else {

						}
					}
				}
			}



			CompensateBatteryDelta(processedFcCalcEntries, deltaEnergyBatInt);


			//Check infintiy battery again for violated SoC limits
			tracingInfinityBat = new TracingInfinityBatterySystem(batData);
			tracingInfinityBat.Initialize(initSoc);
			infinityDummyContainer = new SimpleModDataContainer();
			infEs = GetElectricSystem(tracingInfinityBat, batData);

			foreach (var entry in processedFcCalcEntries) {
				//Should be limited?
				var response = infEs.Request(entry.t, entry.dt, entry.P_Bat_T_Final, false).RESSResponse;
				if (!(response is RESSResponseSuccess))
				{
					throw new VectoException("Unexpected Response");
				}
                tracingInfinityBat.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);
			}


			var minSocTrace = tracingInfinityBat.MinSocTrace;
			var maxSocTrace = tracingInfinityBat.MaxSocTrace;


			if (TryShiftInitialSoC(
					batMinSoc: batSystem.MinSoC,
					batMaxSoc: batSystem.MaxSoC,
					initSoc, minSocTrace, maxSocTrace, out var shiftedSoc)) {

				initSoc = shiftedSoc;

				if(CalculateWithRealBattery(batData, initSoc, processedFcCalcEntries.ToArray()))
				{
					WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, true);
					result = new SearchResult(
						distance: windowSize,
						entries: processedFcCalcEntries.ToArray(),
						initSoc: initSoc,
						success: true,
						reason:"");
					return true;
				}
			}

			WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, false);
			result = new SearchResult(
				distance: windowSize,
				entries: processedFcCalcEntries.ToArray(),
				initSoc: initSoc,
				success: false,
				reason: "SoC violated");

			return false;
		}

		private static void CompensateBatteryDelta(List<FCCalcEntry> processedFcCalcEntries, WattSecond deltaEnergyBatInt)
		{
			var remainingTime = processedFcCalcEntries.Sum(e => e.CanChangeFCPower ? e.dt.Value() : 0).SI<Second>();
			//Maximum Additional Energy that can be provided by the fuel cell in a timestep
			var maxInc =
				processedFcCalcEntries.Select(e => !e.CanChangeFCPower ? 0.SI<WattSecond>() : VectoMath.Max(e.P_max_fc - e.P_FC_corr, 0.SI<Watt>()) * e.dt).ToArray();

			var weights = new List<double>(maxInc.Length);
			var totalIncPotential = maxInc.Sum();
			for (var i = 0; i < maxInc.Length; i++) {
				var weight = maxInc[i] / totalIncPotential;
				weights.Add(weight);
				var entry = processedFcCalcEntries[i];
				entry.delta_P_FCS = weights[i] * -deltaEnergyBatInt / entry.dt;

				if (entry.FCPowerFinal.IsGreater(entry.P_max_CFCS)) {
					throw new VectoException("Power too high");
				}
			}
		}

		private static double RequestFromEs(FCCalcEntry entry, ElectricSystem infinityEs,
			SimpleModDataContainer infinityDummyContainer, TracingInfinityBatterySystem infBat, ref WattSecond deltaEnergyBatInt)
		{
			//Since we are using the infinity battery, the limit that is applied is only influenced by the properties of the battery but not by the SoC
            var batPower = entry.P_Bat_T.LimitTo
			(infinityEs.MaxDischargePower(entry.dt),
				infinityEs.MaxChargePower(entry.dt));

			//Request determined battery power from infinity battery system
			var batResponse = infinityEs.Request(entry.t, entry.dt, batPower, false).RESSResponse;
			if (!(batResponse is RESSResponseSuccess)) {
				throw new VectoException("Unexpected Response");
			}

			//infinityEs.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);
			infBat.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);

			deltaEnergyBatInt += infinityDummyContainer.P_REES_int * entry.dt;

			entry.P_REESS_int = infinityDummyContainer.P_REES_int;
			var SoC = infinityEs.StateOfCharge; //Current virtual soc
			entry.SoC = SoC;

			return SoC;
		}


		private BatterySystemData ApplyBatterySafetyMargin(BatterySystemData batData, IEnumerable<FCCalcEntry> fcCalcEntries, Meter maxWindowSize)
		{
			var tmpBatSystem = new BatterySystem(null, batData);
			var first = fcCalcEntries.First();
            Debug.Assert(first.WindowSize == maxWindowSize);
            //When window size equals TotalDistance, P_FC_raw == average P_el_dem
            var energy_safety_margin = first.P_FC_raw * 10.SI<Second>();
			var dSocSafety =
				Math.Abs((energy_safety_margin / tmpBatSystem.NominalVoltage / tmpBatSystem.TotalCapacity).Value());
			var minSocSafe = tmpBatSystem.MinSoC + dSocSafety;
			var maxSocSafe = tmpBatSystem.MaxSoC - dSocSafety;

			if ((maxSocSafe - minSocSafe).IsSmallerOrEqual(0)) {
				throw new VectoException(
					"Battery is too small! Usable SOC range would be negative when safety margins are considered.");
			}

			batData.Batteries.ForEach(tuple => {
				tuple.Item2.MinSOC = minSocSafe;
				tuple.Item2.MaxSOC = maxSocSafe;
			});
			tmpBatSystem = new BatterySystem(null, batData);
			tmpBatSystem.Initialize(batData.InitialSoC);

			Debug.Assert(tmpBatSystem.MinSoC.IsEqual(minSocSafe) && tmpBatSystem.MaxSoC.IsEqual(maxSocSafe), "Invalid Soc limits");
			return batData;
		}


		public bool CalculateWithRealBattery(BatterySystemData batData, double SoC, FCCalcEntry[] entries)
        {
            batData = batData.Clone();
            batData.ChargeSustainingBatterySystem = false;
            var batSystem = new BatterySystem(null, batData);
            batSystem.Initialize(SoC);
            var dummyContainer = new SimpleModDataContainer();

			var es = new ElectricSystem(null, batData);
			es.Connect(batSystem);


            var infBattery = new TracingInfinityBatterySystem(batData);
			infBattery.Initialize((batSystem.MaxSoC + batSystem.MinSoC) / 2);



            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
				var maxPower = es.MaxChargePower(entry.dt);
				var minPower = es.MaxDischargePower(entry.dt);




				//We have to limit the power, if we are recuperating in the prerun, it is possible, that the power limits of the battery are violated here,
				//In the real simulation, there will be no recuperation if the power limit is reached
                var batPower = entry.P_Bat_T_Final.LimitTo(minPower,maxPower);


                //Limit batPower
                var batResponse = es.Request(entry.t, entry.dt, batPower, false).RESSResponse;
                if (!(batResponse is RESSResponseSuccess))
                {
					//Assuming no recuperation


					return false;
                }

				entry.Real_SoC = batResponse.StateOfCharge;
                batSystem.CommitSimulationStep(entry.t, entry.dt, dummyContainer);
            }

            return true;
        }

		[Conditional("TRACE_FC")]
		private void WriteEntriesToFile(Meter windowSize, FCCalcEntry[] fcCalcEntries, double initSoc,
            WattSecond deltaEnergyBatInt, bool success)
        {
			lock (fileLock)
            {
				if (!_writeModalResults)
				{
					return;
				}

				var preRunEntriesCSVPath = Path.Combine(
					Path.GetDirectoryName(WriterBasePath) ?? "",
					$"fuelcell_data_{RunName}_{Math.Round(windowSize.Value(), 0)}_soc_{initSoc}_{(success ? "success" : "")}.csv");

				using (var fs = new StreamWriter(preRunEntriesCSVPath))
                {
                    fs.WriteLine($"DeltaEnergyBat: {deltaEnergyBatInt} / {deltaEnergyBatInt.ConvertToKiloWattHour()}");
                    fs.WriteLine(FCCalcEntry.Header);
                    foreach (var entry in fcCalcEntries)
                    {
                        fs.WriteLine(entry.ToString());
                    }
                }
            }

		}

        /// <summary>
        /// Check if fuelcell can supply enough power over the cycle.
        /// </summary>
        /// <returns></returns>
        public bool CheckFCPower(FuelCellSystemData fcData, BatterySystemData batData, out string msg)
        {
            msg = "";
			var minFcPower = 0.SI<Watt>(); //values below min power will be handled with time slicing
			var maxFcPower = fcData.MaxElectricPower;

			var es = GetElectricSystem(batData, batData.InitialSoC, out var _);
			//var tmpBatSystem = new BatterySystem(null, batData);
			//tmpBatSystem.Initialize(batData.InitialSoC);

			var maxWindowSize = fcData.MaxWindowSize ?? TotalDistance;

			var avgPowerDemand = GetRawFuelCellPowerDemand(maxWindowSize, minFcPower, maxFcPower, es, _preRunResults).First().P_FC_raw;

			if (avgPowerDemand > maxFcPower)
            {
                msg = $"average el demand {avgPowerDemand} > max fuel cell power {maxFcPower}";
                return false;
			}

            return true;
		}

        public IEnumerable<FCCalcEntry> GetRawFuelCellPowerDemand(Meter windowSize, Watt minFcPower, Watt maxFcPower, ElectricSystem es, IEnumerable<PreRunEntry> preRunResults)
		{
			foreach (var fcCalcEntry in FcCalcEntries(windowSize, minFcPower, maxFcPower, es, preRunResults, TotalDistance, TotalDuration)) {
				yield return fcCalcEntry;
			}
		}

		public static IEnumerable<FCCalcEntry> FcCalcEntries(
			Meter windowSize,
			Watt minFcPower,
			Watt maxFcPower,
			ElectricSystem es,
			IEnumerable<PreRunEntry> preRunResults,
			Meter totalDistance,
			Second totalDuration)
		{
			List<FCCalcEntry> rv = new List<FCCalcEntry>();
			Debug.Assert(minFcPower < maxFcPower, "min power must be smaller than max power");
			if (windowSize.IsGreater(totalDistance))
			{
				throw new VectoException("Window size must not exceed cycle distance!");
			}

			var maxChargingPower = es.MaxChargePower(0.5.SI<Second>());
			var maxDischargingPower = es.MaxDischargePower(0.5.SI<Second>());
			var preRunEntries = preRunResults as PreRunEntry[] ?? preRunResults.ToArray();
			if (windowSize.IsEqual(totalDistance))
			{
				var P_FC_raw = preRunEntries.TimeIntegral<PreRunEntry, Second, Watt, WattSecond>((e) => e.dt, (e) => e.P_es_T) /
								totalDuration;
				foreach (var r in preRunEntries)
				{
					var entry = new FCCalcEntry(maxChargingPower, maxDischargingPower, r, maxFcPower)
					{
						WindowSize = windowSize,
						P_FC_raw = -P_FC_raw,
						P_el_dem = r.P_es_T,
					};


					entry.P_FC = entry.P_FC_raw.LimitTo(minFcPower, entry.P_max_fc);

					var esResponse = es.Request(entry.t, entry.dt, entry.P_Bat_T, true);
					entry.P_Bat_loss = esResponse.RESSResponse.LossPower;

					if (entry.FCPowerFinal > entry.P_max_fc)
					{
						entry.P_FC -= 2 * entry.P_Bat_loss;
						entry.P_Bat_loss = es.Request(entry.t, entry.dt, entry.P_Bat_T, true).RESSResponse.LossPower;
					}

					rv.Add(entry);
				}

				return rv;
			}

			var wIt = new GeneralizedModDataWindowIterator<PreRunEntry, Meter>(preRunEntries, windowSize,
				entry => entry.s, entry => entry.ds);


			for (; !wIt.EndReached; wIt.MoveNext())
			{
				var entry = new FCCalcEntry(maxChargingPower, maxDischargingPower, preRunEntries[wIt.Position], maxFcPower)
				{
					WindowSize = windowSize,
				};

				var time = 0.SI<Second>();
				var emEnergy = 0.SI<WattSecond>();
				for (; !wIt.WindowEndReached; wIt.NextEntry())
				{
					var dt = preRunEntries[wIt.CurrentIndex].dt;
					time += dt;

					emEnergy += dt * preRunEntries[wIt.CurrentIndex].P_es_T;
				}

				entry.P_el_dem = preRunEntries[wIt.Position].P_es_T;
				entry.P_FC_raw = -emEnergy / time;

				entry.P_FC = entry.P_FC_raw.LimitTo(minFcPower, entry.P_max_fc);
				var esResponse = es.Request(entry.t, entry.dt, entry.P_Bat_T, true);
				entry.P_Bat_loss = esResponse.RESSResponse.LossPower;
				if (entry.FCPowerFinal > entry.P_max_fc)
				{
					entry.P_FC -= 2 * entry.P_Bat_loss; //If P_Bat_T is negative, decreasing the fuel cell power will increase the loss power. Leading to Fuel Cell power that is still too high
					entry.P_Bat_loss = es.Request(entry.t, entry.dt, entry.P_Bat_T, true).RESSResponse.LossPower;
				}

				if (entry.FCPowerFinal > entry.P_max_fc)
				{

				}

				rv.Add(entry);
			}

			return rv;
		}
	}
}