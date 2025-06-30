using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.VectoMockup
{

    //public void Run()
    //{
    //	if (!FoundPreviousResults)
    //	{
    //		(GetContainer().ModalData as ModalDataContainer).ModalDataContainerFinished += DummyRun_ModalDataContainerFinished;
    //		_run.Run();
    //	}
    //	else
    //	{
    //		//TODO HM 
    //		Log.Warn($"[DEV ONLY!] Using mod data: {_dummyFilePath}");

    //		var modDataContainer = GetContainer().ModalData as ModalDataContainer;
    //		modDataContainer.ReadDataFromXml(_dummyFilePath);
    //		modDataContainer.Finish(VectoRun.Status.Success);
    //	}


    //}

    //private void DummyRun_ModalDataContainerFinished(object sender, ModalDataContainer.ModalDataContainerFinishedEventArgs e)
    //{
    //	if (FinishedWithoutErrors)
    //	{
    //		e.DataOnFinish.WriteXml(_dummyFilePath, false);
    //		Log.Warn($"[DEV ONLY!] Data written to {_dummyFilePath}:");
    //	}
    //}
    internal class MockupModalDataContainer : IModalDataContainer
    {
		private IModalDataContainer _modalDataContainerImplementation;
		private readonly Action<IModalDataContainer> _addReportResult;
		private Func<Second, Joule, Joule, HeaterDemandResult> _auxHeaterDemandCalc;

		public MockupModalDataContainer(IModalDataContainer modalDataContainer,
			Action<IModalDataContainer> addReportResult)
		{
			_modalDataContainerImplementation = modalDataContainer;
			_addReportResult = addReportResult;

		}

		#region MockupImplementation

		public Second Duration => 1.SI(Unit.SI.Hour).Cast<Second>();
		public Meter Distance => 100.SI(Unit.SI.Kilo.Meter).Cast<Meter>();

		Func<Second, Joule, Joule, HeaterDemandResult> IModalDataContainer.AuxHeaterDemandCalc
		{
			get => _auxHeaterDemandCalc;
			set => _auxHeaterDemandCalc = value;
		}

		#endregion


		#region Implementation of IModalDataContainer

		public bool WriteModalResults { get; set; }

		public object this[ModalResultField key]
		{
			get => _modalDataContainerImplementation[key];
			set => _modalDataContainerImplementation[key] = value;
		}

		public object this[ModalResultField key, IFuelProperties fuel]
		{
			get => _modalDataContainerImplementation[key, fuel];
			set => _modalDataContainerImplementation[key, fuel] = value;
		}

		public object this[ModalResultField key, PowertrainPosition pos]
		{
			get => _modalDataContainerImplementation[key, pos];
			set => _modalDataContainerImplementation[key, pos] = value;
		}

		public object this[ModalResultField key, int? idx]
		{
			get => _modalDataContainerImplementation[key, idx];
			set => _modalDataContainerImplementation[key, idx] = value;
		}

		public object this[ModalResultField key, string arg]
		{
			get => _modalDataContainerImplementation[key, arg];
			set => _modalDataContainerImplementation[key, arg] = value;
		}

		public object this[string auxId]
		{
			get => _modalDataContainerImplementation[auxId];
			set => _modalDataContainerImplementation[auxId] = value;
		}

		public void CommitSimulationStep()
		{
			_modalDataContainerImplementation.CommitSimulationStep();
		}

		public IList<IFuelProperties> FuelData => _modalDataContainerImplementation.FuelData;

		public VectoRun.Status RunStatus => _modalDataContainerImplementation.RunStatus;

		public string Error => _modalDataContainerImplementation.Error;

		public string StackTrace => _modalDataContainerImplementation.StackTrace;

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return _modalDataContainerImplementation.GetValues<T>(key);
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			return _modalDataContainerImplementation.GetValues<T>(col);
		}

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc)
		{
			return _modalDataContainerImplementation.GetValues(selectorFunc);
		}

		public Dictionary<string, DataColumn> Auxiliaries => _modalDataContainerImplementation.Auxiliaries;

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			return _modalDataContainerImplementation.TimeIntegral<T>(field, filter);
		}

		public T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			return _modalDataContainerImplementation.TimeIntegral<T>(field, filter);
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
			_modalDataContainerImplementation.SetDataValue(fieldName, value);
		}

		public void AddAuxiliary(string id, string columnName = null)
		{
			_modalDataContainerImplementation.AddAuxiliary(id, columnName);
		}

		public void Finish(VectoRun.Status runStatus, Exception exception = null)
		{
			_modalDataContainerImplementation.Finish(runStatus, exception);
			if (_addReportResult != null) {
				_addReportResult(this);
			}

		}

		public void FinishSimulation()
		{
			_modalDataContainerImplementation.FinishSimulation();
		}

		public string GetColumnName(IFuelProperties fuelData, ModalResultField mrf)
		{
			return _modalDataContainerImplementation.GetColumnName(fuelData, mrf);
		}

		public void Reset(bool clearColumns = false)
		{
			
		}

		public string GetColumnName(PowertrainPosition pos, ModalResultField mrf)
		{
			return _modalDataContainerImplementation.GetColumnName(pos, mrf);
		}

		public void Reset()
		{
			_modalDataContainerImplementation.Reset();
		}

		

		public Func<Second, Joule, Joule, HeaterDemandResult> AuxHeaterDemandCalc
		{
			get => _modalDataContainerImplementation.AuxHeaterDemandCalc;
			set => _modalDataContainerImplementation.AuxHeaterDemandCalc = value;
		}

		public KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel)
		{
			return _modalDataContainerImplementation.EngineLineCorrectionFactor(fuel);
		}

		public void CalculateAggregateValues()
		{
			_modalDataContainerImplementation.CalculateAggregateValues();
		}

		//public void AddElectricMotor(PowertrainPosition pos)
		//{
		//	_modalDataContainerImplementation.AddElectricMotor(pos);
		//}

		public KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel)
		{
			return _modalDataContainerImplementation.VehicleLineSlope(fuel);
		}

		public bool HasCombustionEngine => _modalDataContainerImplementation.HasCombustionEngine;
		public bool HasGearbox { get; }
		public bool HasAxlegear { get; }

		public WattSecond TotalElectricMotorWorkDrive(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.TotalElectricMotorWorkDrive(emPos);
		}

		public WattSecond TotalElectricMotorWorkRecuperate(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.TotalElectricMotorWorkRecuperate(emPos);
		}

		public WattSecond TotalElectricMotorMotWorkDrive(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.TotalElectricMotorMotWorkDrive(emPos);
		}

		public WattSecond TotalElectricMotorMotWorkRecuperate(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.TotalElectricMotorMotWorkRecuperate(emPos);
		}

		public PerSecond ElectricMotorAverageSpeed(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorAverageSpeed(emPos);
		}

		public double ElectricMotorEfficiencyDrive(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorEfficiencyDrive(emPos);
		}

		public double ElectricMotorEfficiencyGenerate(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorEfficiencyGenerate(emPos);
		}

		public double ElectricMotorMotEfficiencyDrive(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorMotEfficiencyDrive(emPos);
		}

		public double ElectricMotorMotEfficiencyGenerate(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorMotEfficiencyGenerate(emPos);
		}

		public WattSecond ElectricMotorOffLosses(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorOffLosses(emPos);
		}

		public WattSecond ElectricMotorLosses(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorLosses(emPos);
		}

		public WattSecond ElectricMotorMotLosses(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorMotLosses(emPos);
		}

		public WattSecond ElectricMotorTransmissionLosses(PowertrainPosition emPos)
		{
			return _modalDataContainerImplementation.ElectricMotorTransmissionLosses(emPos);
		}

		public double REESSStartSoC()
		{
			return _modalDataContainerImplementation.REESSStartSoC();
		}

		public double REESSEndSoC()
		{
			return _modalDataContainerImplementation.REESSEndSoC();
		}

		public WattSecond REESSLoss()
		{
			return _modalDataContainerImplementation.REESSLoss();
		}

		public ICorrectedModalData CorrectedModalData => _modalDataContainerImplementation.CorrectedModalData;

		public ModalResults Data => throw new NotImplementedException();

		public string RunName => throw new NotImplementedException();

		public IModalDataPostProcessor PostProcessingCorrection
		{
			set => throw new NotImplementedException();
		}

		public KilogramPerWattSecond FuelCellLine => throw new NotImplementedException();

		public bool HasBattery => false;

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
