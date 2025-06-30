using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
	public class SimpleModDataContainer : IModalDataContainer
	{
		protected Dictionary<string, object> _data = new Dictionary<string, object>();



        #region Implementation of IModalDataContainer

        public bool WriteModalResults { get; set; }

        public object this[ModalResultField key]
		{
			get => throw new NotImplementedException();
			set => _data[GetColumnName(key)] = value;
		}

		public object this[ModalResultField key, IFuelProperties fuel]
		{
			get => throw new NotImplementedException();
			set => _data[GetColumnName(fuel, key)] = value;
		}

		public object this[ModalResultField key, PowertrainPosition pos]
		{
			get => throw new NotImplementedException();
			set => _data[GetColumnName(pos, key)] = value;
		}

		public object this[ModalResultField key, int? idx]
		{
			get => throw new NotImplementedException();
			set => _data[GetColumnName(idx, key)] = value;
		}

		

		public object this[ModalResultField key, string arg]
		{
			get => throw new NotImplementedException();
			set => _data[GetColumnName(key, arg)] = value;
		}

		public object this[string auxId]
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public void CommitSimulationStep()
		{
			throw new NotImplementedException();
		}

		public IList<IFuelProperties> FuelData { get; }
		public VectoRun.Status RunStatus { get; }
		public string Error { get; }
		public string StackTrace { get; }
		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; }
		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public T TimeIntegral<T>(ModalResultField field, params object[] formatArgs) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter, object[] formatArgs) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public void SetDataValue(string fieldName, object value)
		{
			throw new NotImplementedException();
		}

		public void AddAuxiliary(string id, string columnName = null)
		{
			throw new NotImplementedException();
		}

		public void Finish(VectoRun.Status runStatus, Exception exception = null)
		{
			throw new NotImplementedException();
		}

		public void FinishSimulation()
		{
			throw new NotImplementedException();
		}


		public void Reset(bool clearColumns = false)
		{
			throw new NotImplementedException();
		}

		protected string GetColumnName(ModalResultField key)
		{
			return key.GetName();
		}

		public string GetColumnName(ModalResultField mrf, string arg)
		{
			return string.Format(mrf.GetCaption(), arg);
		}

		public string GetColumnName(IFuelProperties fuelData, ModalResultField mrf)
		{
			return mrf.GetCaption();
		}

		public string GetColumnName(PowertrainPosition pos, ModalResultField mrf)
		{
			return string.Format(mrf.GetCaption(), pos.GetName());
		}

		protected string GetColumnName(int? idx, ModalResultField key)
		{
			return idx == null ? key.GetName() : $"{key.GetName()}_{idx}";

		}

        public Second Duration { get; }
		public Meter Distance { get; }
		public Func<Second, Joule, Joule, HeaterDemandResult> AuxHeaterDemandCalc { get; set; }
		public KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel)
		{
			throw new NotImplementedException();
		}

		public void CalculateAggregateValues()
		{
			throw new NotImplementedException();
		}

		public KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel)
		{
			throw new NotImplementedException();
		}

		public bool HasCombustionEngine { get; }
		public bool HasGearbox { get; }
		public bool HasAxlegear { get; }
		public WattSecond TotalElectricMotorWorkDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond TotalElectricMotorWorkRecuperate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond TotalElectricMotorMotWorkDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond TotalElectricMotorMotWorkRecuperate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public PerSecond ElectricMotorAverageSpeed(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorEfficiencyDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorEfficiencyGenerate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorMotEfficiencyDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorMotEfficiencyGenerate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorOffLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorMotLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorTransmissionLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public ICorrectedModalData CorrectedModalData { get; }

		public ModalResults Data => throw new NotImplementedException();

		public string RunName => throw new NotImplementedException();

		public IModalDataPostProcessor PostProcessingCorrection
		{
			set => throw new NotImplementedException();
		}

		public KilogramPerWattSecond FuelCellLine => throw new NotImplementedException();

		public bool HasBattery => throw new NotImplementedException();

		public Watt P_REES_int
		{
			get { return (Watt)_data[GetColumnName(ModalResultField.P_reess_int)]; }
		}
		public Scalar SoC
		{
			get { return (Scalar)_data[GetColumnName(ModalResultField.REESSStateOfCharge)]; }
		}

		public void RegisterComponent(VectoSimulationComponent component)
		{
			throw new NotImplementedException();
		}

		public bool ContainsColumn(string modalResultField)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}