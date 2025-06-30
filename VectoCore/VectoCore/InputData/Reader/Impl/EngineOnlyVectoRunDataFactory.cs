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
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineOnlyVectoRunDataFactory : EngineeringModeVectoRunDataFactory
	{
		public const string Name = "EngineOnlyRunDataFactory";


        internal EngineOnlyVectoRunDataFactory(IEngineeringInputDataProvider dataProvider, IPowertrainBuilder ptBuilder, IEngineeringDataAdapter dataAdapter) : base(dataProvider, ptBuilder, dataAdapter) {}

		public override IEnumerable<VectoRunData> NextRun()
		{
			if (InputDataProvider == null) {
				Log.Warn("No valid data provider given");
				yield break;
			}
			for (var modeIdx = 0; modeIdx < InputDataProvider.JobInputData.EngineOnly.EngineModes.Count; modeIdx++) {
				var mode = InputDataProvider.JobInputData.EngineOnly.EngineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var simulationRunData = new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.EngineOnly, mode),
						Cycle = new DrivingCycleProxy(
							DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, false), cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						SimulationType = SimulationType.EngineOnly,
						JobType = VectoSimulationJobType.EngineOnlySimulation,
						ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
						DriverData = CreateDummyDriverData(),
					};
					yield return simulationRunData;
				}
			}
		}

		private DriverData CreateDummyDriverData()
		{
			return new DriverData() {
				EngineStopStart = new DriverData.EngineStopStartData()
					{ EngineOffStandStillActivationDelay = 0.SI<Second>(), MaxEngineOffTimespan = 0.SI<Second>() },
				LookAheadCoasting = new DriverData.LACData() {
					Enabled = false,
					MinSpeed = 0.KMPHtoMeterPerSecond(),
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				AccelerationCurve = new AccelerationCurveData(new[] {
					new KeyValuePair<MeterPerSecond, AccelerationCurveData.AccelerationEntry>(
						0.KMPHtoMeterPerSecond(),
						new AccelerationCurveData.AccelerationEntry() {
							Acceleration = 1.SI<MeterPerSquareSecond>(),
							Deceleration = -1.SI<MeterPerSquareSecond>()
						}),
					new KeyValuePair<MeterPerSecond, AccelerationCurveData.AccelerationEntry>(
						200.KMPHtoMeterPerSecond(),
						new AccelerationCurveData.AccelerationEntry() {
							Acceleration = 1.SI<MeterPerSquareSecond>(),
							Deceleration = -1.SI<MeterPerSquareSecond>()
						}),
				}.ToList()),
				EcoRoll = new DriverData.EcoRollData(),
				OverSpeed = new DriverData.OverSpeedData() {
					MinSpeed = 0.KMPHtoMeterPerSecond(),
					OverSpeed = 0.KMPHtoMeterPerSecond(),
					Enabled = false
				}
			};
		}
	}
}
