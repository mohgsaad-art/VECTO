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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
    public class ModalDataContainer : IModalDataContainer
	{
		//private readonly bool _writeEngineOnly;
		private readonly IModalDataFilter[] _filters;
		private readonly Action<ModalDataContainer> _addReportResult;
		public ModalResults Data { get; set; }
		private DataRow CurrentRow { get; set; }

		private readonly IModalDataWriter _writer;
		private readonly List<string> _additionalColumns = new List<string>();
		private Exception SimException;

		private Second _duration;
		private Meter _distance;
		
		//protected Dictionary<int, Dictionary<ModalResultField, DataColumn>> BatteryColumns =
		//	new Dictionary<int, Dictionary<ModalResultField, DataColumn>>();


		private readonly Dictionary<string, SI> _timeIntegrals = new Dictionary<string, SI>();
		
		private readonly Dictionary<FuelType, KilogramPerWattSecond> _engLine = new Dictionary<FuelType, KilogramPerWattSecond>();
		
		private KilogramPerWattSecond _fuelCellLine = null;

		public KilogramPerWattSecond FuelCellLine => _fuelCellLine ?? (_fuelCellLine = GetFuelCellCorrectionFactor());

		//public KilogramPerWattSecond FuelCellLine => _fuelCellLine ?? (_fuelCellLine = GetFuelCellCorrectionFactor());


        private readonly Dictionary<FuelType, KilogramPerWattSecond> _vehLine = new Dictionary<FuelType, KilogramPerWattSecond>();
		
		private Dictionary<PowertrainPosition, WattSecond> _eEmDrive = new Dictionary<PowertrainPosition, WattSecond>();
		private Dictionary<PowertrainPosition, WattSecond> _eEmRecuperate = new Dictionary<PowertrainPosition, WattSecond>();
		private Dictionary<PowertrainPosition, WattSecond> _eEmDriveMot = new Dictionary<PowertrainPosition, WattSecond>();
		private Dictionary<PowertrainPosition, WattSecond> _eEmRecuperateMot = new Dictionary<PowertrainPosition, WattSecond>();

		protected VectoRunData _runData;
		private ICorrectedModalData _correctedModalData;

		public IModalDataPostProcessor PostProcessingCorrection { set; protected get; }

		[Obsolete("Avoid creating ModalDataContainer via constructor - use dependency injection!")]
		public ModalDataContainer(VectoRunData runData, IModalDataWriter writer,
			Action<ModalDataContainer> addReportResult,
			params IModalDataFilter[] filter) : this(runData, writer, addReportResult, filter, null) { }

		
        public ModalDataContainer(VectoRunData runData, IModalDataWriter writer,
			Action<ModalDataContainer> addReportResult,
			IModalDataFilter[] filter, IModalDataPostProcessorFactory postProcessorFactory)
		{
            _runData = runData;
			_writer = writer;

			_filters = filter ?? new IModalDataFilter[0];
			_addReportResult = addReportResult ?? (x => { });

			Auxiliaries = new Dictionary<string, DataColumn>();
			Data = new ModalResults();
			CurrentRow = Data.NewRow();

			if (postProcessorFactory == null) {
				// was not injected but called by the obsolete constructor
				postProcessorFactory =
					new StandardKernel(new VectoNinjectModule()).Get<IModalDataPostProcessorFactory>();
			}
			PostProcessingCorrection = postProcessorFactory.GetPostProcessor(runData.JobType);

            if (runData.EngineData != null) {
				
			}

		}

		public void RegisterComponent(VectoSimulationComponent component)
		{
			Data.RegisterComponent(component, _runData);
		}

		public bool ContainsColumn(string modalResultField)
		{
			return Data.Columns.Contains(modalResultField);
		}

		


		public int JobRunId => _runData.JobRunId;

		public string RunName => _runData.JobName;

		public string CycleName => _runData.Cycle?.Name ?? "";

		public string RunSuffix => _runData.ModFileSuffix;

		public bool WriteModalResults { get; set; }

		public VectoRun.Status RunStatus { get; protected set; }

		public string Error => SimException?.Message;

		public string StackTrace =>
			SimException == null
				? null
				: (SimException.StackTrace ?? SimException.InnerException?.StackTrace);


		public void Reset(bool clearColumns = false)
		{
			Data.Rows.Clear();
			if (clearColumns) {
				_additionalColumns.Clear();
				Auxiliaries.Clear();
				Data.Columns.Clear();
				Data.Reset();
			}
			CurrentRow = Data.NewRow();
			ClearAggregateResults();
		}

		public Second Duration => _duration ?? (_duration = CalcDuration());

		public Meter Distance => _distance ?? (_distance = CalcDistance());

		public Func<Second, Joule, Joule, HeaterDemandResult> AuxHeaterDemandCalc { get; set; }

		public KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel)
		{
			if (_engLine.TryGetValue(fuel.FuelType, out var value)) {
				return value;
			}

			var (k, _) = VectoMath.LeastSquaresFitting(
				GetValues(
					x => x.Field<bool>(ModalResultField.ICEOn.GetName())
						? new Point(
							x.Field<SI>(ModalResultField.P_ice_fcmap.GetName()).Value(),
							x.Field<SI>(GetColumnName(fuel, ModalResultField.FCWHTCc)).Value())
						: null).Where(x => x != null && x.Y > 0).ToArray());
			if (double.IsInfinity(k) || double.IsNaN(k)) {
				LogManager.GetLogger(typeof(ModalDataContainer).FullName).Warn("could not calculate engine correction line - k: {0}", k);
				k = 0;
			}

			_engLine[fuel.FuelType] = k.SI<KilogramPerWattSecond>();

			return _engLine[fuel.FuelType];
		}

		private KilogramPerWattSecond GetFuelCellCorrectionFactor()
		{
			var values = GetValues(
				x => x.Field<SI>(ModalResultField.P_FCSystem.GetName()).IsGreater(0) //FC is on
					? new Point(
						x.Field<SI>(ModalResultField.P_FCSystem.GetName()).Value(),
						x.Field<SI>(ModalResultField.FC_FCSystem.GetName()).Value())
					: null).Where(x => x != null && x.Y > 0).Distinct().OrderBy(p => p.X).ToList();

			//if (_runData.FuelCellSystemData.FuelCells.Count > 1) {
			//	throw new NotImplementedException("Multiple fuel cells are not supported");
			//}

			var fuelCell = _runData.FuelCellSystemData;
			var mid = (int)Math.Floor((values.Count() / 2.0f));
			var lowerOperatingPower = VectoMath.Max(0.9 * values[mid].X, 0);
			var higherOperatingPower = VectoMath.Min(1.1 * values[mid].X, fuelCell.MaxElectricPower.Value());
			if (!(values.First().X.IsSmallerOrEqual(lowerOperatingPower) 
				&& values.Last().X.IsGreaterOrEqual(higherOperatingPower)
				))
			{
				//Insert artificial operating points before calculating the fuel cell line
				var fcShareLow = fuelCell.FuelCellShareMap.Lookup(lowerOperatingPower.SI<Watt>());
				var fcLow = fcShareLow.FuelConsumption;

				var fcShareHigh = fuelCell.FuelCellShareMap.Lookup(higherOperatingPower.SI<Watt>());
				var fcHigh = fcShareHigh.FuelConsumption;
				



				values.Add(new Point(lowerOperatingPower, fcLow.Value()));
				values.Add(new Point(higherOperatingPower, fcHigh.Value()));
				values = values.OrderBy(x => x.X).ToList();
			}

			var (k, _) = VectoMath.LeastSquaresFitting(values);

			if (double.IsInfinity(k) || double.IsNaN(k)) {
				LogManager.GetLogger(typeof(ModalDataContainer).FullName).Warn("could determine not fuel cell correction line - k: {0}", k);
				k = 0;
			}

			return k.SI<KilogramPerWattSecond>();
		}

		public KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel)
		{
			if (_runData.Cycle.CycleType == CycleType.EngineOnly) {
				return null;
			}

			if (!Data.Columns.Contains(ModalResultField.P_wheel_in.GetName())) {
				return null;
			}
			if (_vehLine.TryGetValue(fuel.FuelType, out var value)) {
				return value;
			}

			if (Data.AsEnumerable().Any(r => r.Field<SI>(ModalResultField.P_wheel_in.GetName()) != null)) {
				var (k, _) = VectoMath.LeastSquaresFitting(
					GetValues(row => row.Field<bool>(ModalResultField.ICEOn.GetName())
								? new Point(
									row.Field<SI>(ModalResultField.P_wheel_in.GetName()).Value(),
									row.Field<SI>(GetColumnName(fuel, ModalResultField.FCFinal)).Value())
								: null)
						.Where(x => x != null && x.X > 0 && x.Y > 0).ToArray());
				if (double.IsInfinity(k) || double.IsNaN(k)) {
					LogManager.GetLogger(typeof(ModalDataContainer).FullName).Warn("could not calculate vehicle correction line - k: {0}", k);
					k = 0;
				}

				_vehLine[fuel.FuelType] = k.SI<KilogramPerWattSecond>();
				return _vehLine[fuel.FuelType];
			}

			return null;
		}

		public bool HasCombustionEngine => !(_runData.JobType == VectoSimulationJobType.BatteryElectricVehicle
			|| _runData.JobType == VectoSimulationJobType.IEPC_E
			|| _runData.JobType == VectoSimulationJobType.FCHV
			|| _runData.JobType == VectoSimulationJobType.FCHV_IEPC);

		public bool HasGearbox => _runData.GearboxData != null;

		public bool HasAxlegear => _runData.AxleGearData != null;

		public bool HasBattery => _runData.BatteryData != null;

		public WattSecond TotalElectricMotorWorkDrive(PowertrainPosition emPos)
		{
			var offField = emPos == PowertrainPosition.IEPC ? ModalResultField.IEPC_Off_ : ModalResultField.EM_Off_;
			var mechField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_out_ : ModalResultField.P_EM_mech_;
			var factor = emPos == PowertrainPosition.IEPC ? -1 : 1;
			if (!Data.ElectricMotors.Contains(emPos))
				return null;
			else {
				var eEmDrive = Data.AsEnumerable().Select(r => {
					var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
					return new {
						EM_off = r.Field<Scalar>(string.Format(offField.GetCaption(), emPos.GetName())),
						E_EM = r.Field<Watt>(string.Format(mechField.GetCaption(), emPos.GetName())) *
								dt * factor
					};
				}).Where(x => x.EM_off.IsEqual(0) && x.E_EM.IsSmaller(0)).Sum(x => x.E_EM) ?? 0.SI<WattSecond>();
				return -_eEmDrive.GetOrAdd(emPos, _ => eEmDrive);
			}
		}

		public WattSecond TotalElectricMotorMotWorkDrive(PowertrainPosition emPos) =>
			!Data.ElectricMotors.Contains(emPos) || emPos == PowertrainPosition.IEPC
				? null
				: -_eEmDriveMot.GetOrAdd(emPos, _ => TimeIntegral<WattSecond>(
					string.Format(ModalResultField.P_EM_electricMotor_em_mech_.GetCaption(), emPos.GetName()), x => x < 0));

		public WattSecond TotalElectricMotorWorkRecuperate(PowertrainPosition emPos)
		{
			var offField = emPos == PowertrainPosition.IEPC ? ModalResultField.IEPC_Off_ : ModalResultField.EM_Off_;
			var mechField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_out_ : ModalResultField.P_EM_mech_;
			var factor = emPos == PowertrainPosition.IEPC ? -1 : 1;
			if (!Data.ElectricMotors.Contains(emPos))
				return null;
			else {
				var eEmRecup = Data.AsEnumerable().Select(r => {
					var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
					return new {
						EM_off = r.Field<Scalar>(string.Format(offField.GetCaption(), emPos.GetName())),
						E_EM = r.Field<Watt>(string.Format(mechField.GetCaption(), emPos.GetName())) *
								dt * factor
					};
				}).Where(x => x.EM_off.IsEqual(0) && x.E_EM.IsGreater(0)).Sum(x => x.E_EM) ?? 0.SI<WattSecond>();
				return _eEmRecuperate.GetOrAdd(emPos, _ => eEmRecup);
			}
		}

		public WattSecond TotalElectricMotorMotWorkRecuperate(PowertrainPosition emPos) =>
			!Data.ElectricMotors.Contains(emPos)
				? null
				: _eEmRecuperateMot.GetOrAdd(emPos, _ => TimeIntegral<WattSecond>(
					string.Format(ModalResultField.P_EM_electricMotor_em_mech_.GetCaption(), emPos.GetName()), x => x > 0));


		public double ElectricMotorEfficiencyDrive(PowertrainPosition emPos)
		{
			if (!Data.ElectricMotors.Contains(emPos)) {
				return double.NaN;
			}

			var elPwrField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_el_ : ModalResultField.P_EM_electricMotor_el_;
			var mechPwrField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_int_mech_map_ : ModalResultField.P_EM_mech_;
			var selected = Data.AsEnumerable().Select(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return new {
					P_em = r.Field<Watt>(string.Format(elPwrField.GetCaption(),
						emPos.GetName())),
					E_mech = r.Field<Watt>(string.Format(mechPwrField.GetCaption(),
						emPos.GetName())) * dt,
					E_el = r.Field<Watt>(string.Format(elPwrField.GetCaption(),
						emPos.GetName())) * dt,
				};
			});
			var eMech = 0.SI<WattSecond>();
			var eEl = 0.SI<WattSecond>();
			foreach (var entry in selected.Where(x => x.P_em.IsSmaller(0))) {
				eMech += entry.E_mech;
				eEl += entry.E_el;
			}

			return eMech.Value() / eEl.Value();
		}

		public double ElectricMotorMotEfficiencyDrive(PowertrainPosition emPos)
		{
			if (!Data.ElectricMotors.Contains(emPos)) {
				return double.NaN;
			}
			if (emPos == PowertrainPosition.IEPC) {
				return double.NaN;
			}

			var selected = Data.AsEnumerable().Select(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return new {
					EM_off = r.Field<Scalar>(string.Format(ModalResultField.EM_Off_.GetCaption(), emPos.GetName())),
					P_em = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(),
						emPos.GetName())),
					E_mech = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_em_mech_.GetCaption(),
						emPos.GetName())) * dt,
					E_el = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(),
						emPos.GetName())) * dt,
				};
			});
			var eMech = 0.SI<WattSecond>();
			var eEl = 0.SI<WattSecond>();
			foreach (var entry in selected.Where(x => x.P_em.IsSmaller(0) && x.EM_off.IsEqual(0))) {
				eMech += entry.E_mech;
				eEl += entry.E_el;
			}

			return eMech.Value() / eEl.Value();
		}


		public double ElectricMotorEfficiencyGenerate(PowertrainPosition emPos)
		{
			if (!Data.ElectricMotors.Contains(emPos)) {
				return double.NaN;
			}
			var offField = emPos == PowertrainPosition.IEPC ? ModalResultField.IEPC_Off_ : ModalResultField.EM_Off_;
			var elPwrField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_el_ : ModalResultField.P_EM_electricMotor_el_;
			var mechPwrField = emPos == PowertrainPosition.IEPC ? ModalResultField.P_IEPC_int_mech_map_ : ModalResultField.P_EM_mech_;
			var selected = Data.AsEnumerable().Select(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return new {
					EM_off = r.Field<Scalar>(string.Format(offField.GetCaption(), emPos.GetName())),
					P_em = r.Field<Watt>(string.Format(elPwrField.GetCaption(), 
						emPos.GetName())),
					E_mech = r.Field<Watt>(string.Format(mechPwrField.GetCaption(), 
						emPos.GetName())) * dt,
					E_el = r.Field<Watt>(string.Format(elPwrField.GetCaption(), 
						emPos.GetName())) * dt,
				};
			});
			var eMech = 0.SI<WattSecond>();
			var eEl = 0.SI<WattSecond>();
			foreach (var entry in selected.Where(x => x.P_em.IsGreater(0) && x.EM_off.IsEqual(0))) {
				eMech += entry.E_mech;
				eEl += entry.E_el;
			}

			var eff = eEl.Value() / eMech.Value();
			return eff;
		}

		public double ElectricMotorMotEfficiencyGenerate(PowertrainPosition emPos)
		{
			if (!Data.ElectricMotors.Contains(emPos)) {
				return double.NaN;
			}

			if (emPos == PowertrainPosition.IEPC) {
				return double.NaN;
			}
			var selected = Data.AsEnumerable().Select(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return new {
					P_em = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(),
						emPos.GetName())),
					E_mech = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_em_mech_.GetCaption(),
						emPos.GetName())) * dt,
					E_el = r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(),
						emPos.GetName())) * dt,
				};
			});
			var eMech = 0.SI<WattSecond>();
			var eEl = 0.SI<WattSecond>();
			foreach (var entry in selected.Where(x => x.P_em.IsGreater(0))) {
				eMech += entry.E_mech;
				eEl += entry.E_el;
			}

			var eff = eEl.Value() / eMech.Value();
			return eff;
		}

		public WattSecond ElectricMotorOffLosses(PowertrainPosition emPos)
		{
			if (!Data.ElectricMotors.Contains(emPos)) {
				return null;
			}
			var offField = emPos == PowertrainPosition.IEPC ? ModalResultField.IEPC_Off_ : ModalResultField.EM_Off_;
			var mechField = emPos == PowertrainPosition.IEPC
				? ModalResultField.P_IEPC_out_
				: ModalResultField.P_EM_mech_;
			var selected = Data.AsEnumerable().Select(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return new {
					EM_off = r.Field<Scalar>(string.Format(offField.GetCaption(), emPos.GetName())),
					E_mech = r.Field<Watt>(string.Format(mechField.GetCaption(),
						emPos.GetName())) * dt,
				};
			});
			return selected.Where(x => !x.EM_off.IsEqual(0)).Sum(x => x.E_mech) ?? 0.SI<WattSecond>();
		}

		public WattSecond ElectricMotorLosses(PowertrainPosition emPos)
		{
			var field = emPos == PowertrainPosition.IEPC
				? ModalResultField.P_IEPC_electricMotorLoss_
				: ModalResultField.P_EM_loss_;
			return Data.AsEnumerable().Sum(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return r.Field<Watt>(string.Format(field.GetCaption(),
					emPos.GetName())) * dt;
			});
		}

		public WattSecond ElectricMotorMotLosses(PowertrainPosition emPos)
		{
			if (emPos == PowertrainPosition.IEPC) {
				return null;
			}
			return Data.AsEnumerable().Sum(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return r.Field<Watt>(string.Format(ModalResultField.P_EM_electricMotorLoss_.GetCaption(),
					emPos.GetName())) * dt;
			});
		}

		public WattSecond ElectricMotorTransmissionLosses(PowertrainPosition emPos)
		{
			if (emPos == PowertrainPosition.IEPC) {
				return null;
			}
			return Data.AsEnumerable().Sum(r => {
				var dt = r.Field<Second>(ModalResultField.simulationInterval.GetName());
				return r.Field<Watt>(string.Format(ModalResultField.P_EM_TransmissionLoss_.GetCaption(),
					emPos.GetName())) * dt;
			});
		}

		public PerSecond ElectricMotorAverageSpeed(PowertrainPosition emPos)
		{
			if (Duration == 0.SI<Second>()) {
				return 0.SI<PerSecond>();
			}
			var field = emPos == PowertrainPosition.IEPC
				? ModalResultField.n_IEPC_int_
				: ModalResultField.n_EM_electricMotor_;
			var integral = GetValues(x => x.Field<PerSecond>(string.Format(field.GetCaption(), emPos.GetName())).Value() *
												x.Field<Second>(ModalResultField.simulationInterval.GetName()).Value()).Sum();
			return (integral / Duration.Value()).SI<PerSecond>();
		}

        public ICorrectedModalData CorrectedModalData => _correctedModalData ?? (_correctedModalData = PostProcessingCorrection.ApplyCorrection(this, _runData));


		public void CalculateAggregateValues()
		{
			//var duration = Duration;
			//var distance = _runData.JobType != VectoSimulationJobType.EngineOnlySimulation ? Distance : null;
			//if (distance != null && duration != null && !duration.IsEqual(0)) {
				//var speed = distance / duration;
			//}

			if (HasCombustionEngine) {
				foreach (var fuel in Data.FuelColumns.Keys) {
					EngineLineCorrectionFactor(fuel);
					VehicleLineSlope(fuel);

					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCMap));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCNCVc));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCWHTCc));

					//TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCAAUX));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCMap));
					//TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCICEStopStart));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCFinal));

				}

				TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr);
				TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr);
				TimeIntegral<WattSecond>(ModalResultField.P_aux_ESS_mech_ice_off);
				TimeIntegral<WattSecond>(ModalResultField.P_aux_ESS_mech_ice_on);
				TimeIntegral<WattSecond>(ModalResultField.P_ice_start);
			}

			TimeIntegral<WattSecond>(ModalResultField.P_clutch_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_gbx_shift_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_gbx_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_wheel_in);
			TimeIntegral<WattSecond>(ModalResultField.P_axle_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_ret_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_angle_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_TC_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_brake_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_wheel_inertia);
			TimeIntegral<WattSecond>(ModalResultField.P_veh_inertia);
			TimeIntegral<WattSecond>(ModalResultField.P_aux_mech);
			TimeIntegral<WattSecond>(ModalResultField.P_slope);
			TimeIntegral<WattSecond>(ModalResultField.P_roll);
			TimeIntegral<WattSecond>(ModalResultField.P_air);

		}

		//public void AddElectricMotor(PowertrainPosition pos)
		//{
			
		//}

		public bool HasTorqueConverter => _runData.GearboxData?.TorqueConverterData != null;

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
			ClearAggregateResults();
		}

		protected virtual void ClearAggregateResults()
		{
			_duration = null;
			_distance = null;
			_timeIntegrals.Clear();
			_correctedModalData = null;
		}

		protected virtual Second CalcDuration()
		{
			var time = GetValues<Second>(ModalResultField.time).ToList();
			var dt = GetValues<Second>(ModalResultField.simulationInterval).ToList();
			if (time.Count == 1) {
				return time.First();
			}

			if (time.Count == 0) {
				return 0.SI<Second>();
			}
			return time.Last() - time.First() + dt.First() / 2 + dt.Last() / 2;
		}

		protected virtual Meter CalcDistance()
		{
			if (_runData.Cycle.CycleType == CycleType.EngineOnly) {
				return null;
			}
			var max = GetValues<Meter>(ModalResultField.dist).LastOrDefault() ?? 0.SI<Meter>();
			var first = GetValues(
				r => new {
					dist = r.Field<Meter>(ModalResultField.dist.GetName()),
					vact = r.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
					acc = r.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()),
					dt = r.Field<Second>(ModalResultField.simulationInterval.GetName())
				}).FirstOrDefault();
			var min = 0.SI<Meter>();
			if (first != null && first.vact != null && first.acc != null && first.dt != null) {
				min = first.dist - first.vact * first.dt - first.acc * first.dt * first.dt / 2.0;
			}

			return max == null || min == null ? null : max - min;
		}

		public IList<IFuelProperties> FuelData => Data.FuelColumns.Keys.ToList();

		public void Finish(VectoRun.Status runStatus, Exception exception = null)
		{
			RunStatus = runStatus;
			SimException = exception;

			var dataColumns = GetOutputColumnsOrdered();

			var fileSuffix = RunSuffix;
			if (WriteModalResults) {
				var filteredData = Data;
				foreach (var filter in _filters) {
					fileSuffix += "_" + filter.ID;
					filteredData = filter.Filter(filteredData);
				}

				try {
					_writer.WriteModData(
						JobRunId, RunName, CycleName, fileSuffix,
						new DataView(filteredData).ToTable(false, dataColumns.ToArray()));
				} catch (Exception e) {
					LogManager.GetLogger(typeof(ModalDataContainer).FullName).Error(e.Message);
				}
			}

			if (_addReportResult != null) {
				_addReportResult(this);
			}
			
		}

		private IList<string> GetOutputColumnsOrdered()
		{
			var dataColumns = new List<string>();
			dataColumns.AddRange(
				new[] {
					ModalResultField.time,
					ModalResultField.simulationInterval,
					ModalResultField.dist,
					ModalResultField.v_act,
					ModalResultField.v_targ,
					ModalResultField.acc,
					ModalResultField.grad,
					ModalResultField.altitude,
					ModalResultField.Highway,
					ModalResultField.Gear,
					ModalResultField.TC_Locked,
					// ICE
					ModalResultField.n_ice_avg,
					ModalResultField.T_ice_fcmap,
					ModalResultField.T_ice_full,
					ModalResultField.T_ice_drag,
					ModalResultField.P_ice_fcmap,
					ModalResultField.P_ice_full,
					ModalResultField.P_ice_full_stat,
					ModalResultField.P_ice_drag,
					ModalResultField.P_ice_inertia,
					ModalResultField.P_ice_out,
					// REESS
					ModalResultField.P_terminal_ES,
					ModalResultField.P_reess_terminal,
					ModalResultField.P_reess_int,
					ModalResultField.P_reess_loss,
					ModalResultField.P_ES_Conn_loss,
					ModalResultField.P_reess_charge_max,
					ModalResultField.P_reess_discharge_max,
					ModalResultField.REESSStateOfCharge,
					ModalResultField.U_reess_terminal,
					ModalResultField.U0_reess,
					ModalResultField.I_reess,
				}.Select(x => x.GetName()));





			//Fuel Cell 
			dataColumns.AddRange(Data.FuelCellColumns);
			dataColumns.AddRange(new [] {
				ModalResultField.P_FCSystem,
				ModalResultField.FC_FCSystem,
			}.Select(x => x.GetName()));




			// EMs
			if (Data.ElectricMotors.Count > 0) {
				foreach (var em in Data.ElectricMotors.OrderBy(x => x).Reverse()) {
					var cols = em == PowertrainPosition.IEPC ? ModalResults.IEPCSignals : ModalResults.ElectricMotorSignals;
					dataColumns.AddRange(cols.Select(emCol =>
						string.Format(emCol.GetAttribute().Caption, em.GetName())));
				}
			}
			
			dataColumns.AddRange(
				new[] {
					// TC
					ModalResultField.P_gbx_shift_loss,
					ModalResultField.P_TC_loss,
					ModalResultField.P_TC_out,
					// clutch
					ModalResultField.P_clutch_loss,
					ModalResultField.P_clutch_out,
					// Aux
					ModalResultField.P_aux_mech,
					ModalResultField.P_aux_el,
					ModalResultField.P_Aux_el_HV,
					// Gbx
					ModalResultField.P_gbx_in,
					ModalResultField.P_gbx_loss,
					ModalResultField.P_gbx_inertia,

					ModalResultField.n_gbx_in_avg,
					ModalResultField.n_gbx_out_avg,

					ModalResultField.T_gbx_in,
					ModalResultField.T_gbx_out,

					// retarder
					ModalResultField.P_retarder_in,
					ModalResultField.P_ret_loss,
					// angledrive
					ModalResultField.P_angle_in,
					ModalResultField.P_angle_loss,
					// axlegear
					ModalResultField.P_axle_in,
					ModalResultField.P_axle_loss,
					// wheelEnd
					ModalResultField.P_wheelEnd_in,
					ModalResultField.P_wheelEnd_saving,
					// brakes
					ModalResultField.P_brake_in,
					ModalResultField.P_brake_loss,
					// wheels
					ModalResultField.P_wheel_in,
					ModalResultField.P_wheel_inertia,
					//vehicle
					ModalResultField.P_trac,
					ModalResultField.P_slope,
					ModalResultField.P_air,
					ModalResultField.EffectiveAirDragArea,
					ModalResultField.P_roll,
					ModalResultField.P_veh_inertia,
				}.Select(x => x.GetName()));
			dataColumns.AddRange(Auxiliaries.Values.Where(x => !x.ColumnName.Contains("P_aux_ENG_AUX_")).Select(c => c.ColumnName));
			dataColumns.AddRange(
				new[] {
					// BusAux
					ModalResultField.P_busAux_ES_HVAC,
					ModalResultField.P_busAux_ES_other,
					ModalResultField.P_busAux_ES_consumer_sum,
					ModalResultField.P_busAux_ES_sum_mech,
					ModalResultField.P_busAux_ES_generated,
					ModalResultField.BatterySOC,
					ModalResultField.P_busAux_HVACmech_consumer,
					ModalResultField.P_busAux_HVACmech_gen,
					ModalResultField.Nl_busAux_PS_consumer,
					ModalResultField.Nl_busAux_PS_generated,
					ModalResultField.Nl_busAux_PS_generated_alwaysOn,
					//ModalResultField.Nl_busAux_PS_generated_dragOnly,
					ModalResultField.P_busAux_PS_generated,
					ModalResultField.P_busAux_PS_generated_alwaysOn,
					ModalResultField.P_busAux_PS_generated_dragOnly,
					ModalResultField.P_DCDC_In,
					ModalResultField.P_DCDC_Out,
					ModalResultField.P_DCDC_missing,

					//ModalResultField.SimIntervalCurrent_,
					//ModalResultField.SimIntervalPrev_,
					//ModalResultField.DCDCStateCount_,

					// TC Operating point
					ModalResultField.TorqueConverterSpeedRatio,
					ModalResultField.TorqueConverterTorqueRatio,
					ModalResultField.TC_TorqueOut,
					ModalResultField.TC_angularSpeedOut,
					ModalResultField.TC_TorqueIn,
					ModalResultField.TC_angularSpeedIn,
					// hybrid strategy
					ModalResultField.HybridStrategyScore, 
					ModalResultField.HybridStrategySolution,
					ModalResultField.MaxPropulsionTorqe,
					ModalResultField.HybridStrategyState,
					// WHR
					ModalResultField.P_WHR_el_map, 
					ModalResultField.P_WHR_el_corr, 
					ModalResultField.P_WHR_mech_map,
					ModalResultField.P_WHR_mech_corr, 
					// ESS
					ModalResultField.P_aux_ESS_mech_ice_off, 
					ModalResultField.P_aux_ESS_mech_ice_on,
					ModalResultField.P_ice_start,
					ModalResultField.ICEOn
				}.Select(x => x.GetName()));
			// fuel consumption
			dataColumns.AddRange(Data.FuelColumns.SelectMany(kv => kv.Value.Select(kv2 => kv2.Value.ColumnName)));

			// additional output columns
			dataColumns.AddRange(_additionalColumns);
				
			// every single battery
			foreach (var batKey in Data.BatteryColumns.Keys) {
				dataColumns.AddRange(Data.BatteryColumns[batKey].Keys
					.Where(x => ModalResults.BatterySignals.Contains(x))
					.Select(x => $"{x.GetName()}_{batKey}"));
			}

			return dataColumns.Where(x => Data.Columns.Contains(x)).ToArray();
		}

		[Obsolete]
		public bool HasElectricAuxiliaries
		{
			get => _runData.Aux.Any(aux => aux.ConnectToREESS);
		}

		public IEnumerable<T> GetValues<T>(DataColumn col) => GetValues(x => x.Field<T>(col));

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc) =>
			Data.Rows.Cast<DataRow>().Select(selectorFunc);

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T> =>
			TimeIntegral<T>(field.GetName(), filter);

		public T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			if (!Data.Columns.Contains(field)) {
				return null;
			}
			if (filter == null && _timeIntegrals.TryGetValue(field, out var val)) {
				return (T)val;
			}

			var dt = Data.Columns[ModalResultField.simulationInterval.GetName()];
			var result = 0.0;
			var idx = Data.Columns[field];
			if (idx != null) {
				foreach (DataRow row in Data.Rows) {
					var value = row[idx];
					if (value != null && value != DBNull.Value) {
						var siValue = (SI)value;
						if (filter == null || filter(siValue)) {
							result += siValue.Value() * ((Second)row[dt]).Value();
						}
					}
				}
			}

			var retVal = result.SI<T>();
			if (filter == null) {
				_timeIntegrals[field] = retVal;
			}
			return retVal;
		}

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return GetValues<T>(Data.Columns[key.GetName()]);
		}

		public object this[ModalResultField key]
		{
			get => CurrentRow[key.GetName()];
			set => CurrentRow[key.GetName()] = value;
		}

		public string GetColumnName(ModalResultField mrf, string arg)
		{
			return string.Format(mrf.GetCaption(), arg);
		}

		public string GetColumnName(IFuelProperties fuelData, ModalResultField mrf)
		{
			try {
				return Data.FuelColumns[fuelData][mrf].ColumnName;
			} catch (KeyNotFoundException e) {
				throw new VectoException($"unknown fuel {fuelData.GetLabel()} for key {mrf.GetName()}", e);
			}
		}

		public string GetColumnName(PowertrainPosition pos, ModalResultField mrf)
		{
			return string.Format(mrf.GetCaption(), pos.GetName());
		}

		public object this[ModalResultField key, IFuelProperties fuel]
		{
			get {
				try {
					return CurrentRow[Data.FuelColumns[fuel][key]];
				} catch (KeyNotFoundException e) {
					throw new VectoException($"unknown fuel {fuel.GetLabel()} for key {key.GetName()}", e);
				}
			}
			set {
				try {
					CurrentRow[Data.FuelColumns[fuel][key]] = value;
				} catch (KeyNotFoundException e) {
					throw new VectoException($"unknown fuel {fuel.GetLabel()} for key {key.GetName()}", e);
				}
			}
		}

		public object this[ModalResultField key, PowertrainPosition pos]
		{
			get => CurrentRow[GetColumnName(pos, key)];
			set => CurrentRow[GetColumnName(pos, key)] = value;
		}

		public object this[ModalResultField key, string arg]
		{
			get => CurrentRow[GetColumnName(key, arg)]; 
			set => CurrentRow[GetColumnName(key, arg)] = value;
        }

		public object this[ModalResultField key, int? idx]
		{
			get {
				if (!ModalResults.BatterySignals.Contains(key)) {
					throw new VectoException("ModalResult with index is only supported for REESS fields");
				}

				return idx.HasValue ? CurrentRow[$"{key.GetCaption()}_{idx.Value}"] : CurrentRow[key.GetName()];
			}
			set {
				if (idx == null) {
					CurrentRow[key.GetName()] = value;
				} else {
					if (!ModalResults.BatterySignals.Contains(key)) {
						throw new VectoException("ModalResult with index is only supported for REESS fields");
					}

					var entry = Data.BatteryColumns.GetVECTOValueOrDefault(idx.Value, null);
					var col = entry?.GetVECTOValueOrDefault(key, null);
					if (col == null) {
						throw new VectoException($"Column {key.GetName()} with index {idx} not found");
					}

					CurrentRow[col] = value;
				}
			}
		}

		public object this[string auxId]
		{
			get => CurrentRow[Auxiliaries[auxId]];
			set => CurrentRow[Auxiliaries[auxId]] = value;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetDataValue(string fieldName, object value)
		{
			if (!Data.Columns.Contains(fieldName)) {
				_additionalColumns.Add(fieldName);
				Data.Columns.Add(fieldName);
			}
			if (value is double) {
				CurrentRow[fieldName] = string.Format(CultureInfo.InvariantCulture, "{0}", value);
			} else {
				CurrentRow[fieldName] = value;
			}
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; set; }
		
		/// <summary>
		/// Adds a new auxiliary column into the mod data.
		/// </summary>
		/// <param name="id">The Aux-ID. This is the internal identification for the auxiliary.</param>
		/// <param name="columnName">(Optional) The column name in the mod file. Default: "P_aux_" + id</param>
		public void AddAuxiliary(string id, string columnName = null)
		{
			if (!string.IsNullOrWhiteSpace(id) && !Auxiliaries.ContainsKey(id)) {
				var col = Data.Columns.Add(columnName ?? string.Format(ModalResultField.P_aux_.GetCaption(), id), typeof(SI));

				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
					ModalResultField.P_aux_.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
					ModalResultField.P_aux_.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
					ModalResultField.P_aux_.GetAttribute().ShowUnit;

				Auxiliaries[id] = col;
			}
		}

		public void FinishSimulation()
		{
			//Data.Clear(); //.Rows.Clear();
			Data = null;
			CurrentRow = null;
			Auxiliaries.Clear();
		}
	}
}