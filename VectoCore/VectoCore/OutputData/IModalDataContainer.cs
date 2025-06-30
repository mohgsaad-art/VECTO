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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.VectoCore.OutputData
{
    public interface IModalDataFilter
	{
		ModalResults Filter(ModalResults data);
		string ID { get; }
	}

	public interface IModalDataFactory
	{
		IModalDataContainer CreateModDataContainer(VectoRunData runData, IModalDataWriter writer, Action<IModalDataContainer> addReportResult, IModalDataFilter[] filter);
	}

    public interface IModalDataContainer
	{
		bool WriteModalResults { get; set; }

        /// <summary>
        /// Indexer for fields of the DataWriter. Accesses the data of the current step.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[ModalResultField key] { get; set; }

		object this[ModalResultField key, IFuelProperties fuel] { get; set; }

		object this[ModalResultField key, PowertrainPosition pos] { get; set; }

		object this[ModalResultField key, int? idx] { get; set; }

		object this[ModalResultField key, string arg] { get; set; }

		/// <summary>
		/// Indexer for auxiliary fields of the DataWriter.
		/// </summary>
		/// <param name="auxId"></param>
		/// <returns></returns>
		object this[string auxId] { get; set; }

		
		/// <summary>
		/// Commits the data of the current simulation step.
		/// </summary>
		void CommitSimulationStep();

		IList<IFuelProperties> FuelData { get; }

		VectoRun.Status RunStatus { get; }

		string Error { get; }

		string StackTrace { get; }

		IEnumerable<T> GetValues<T>(ModalResultField key);

		IEnumerable<T> GetValues<T>(DataColumn col);

		IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc);

		Dictionary<string, DataColumn> Auxiliaries { get; }

		T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>;
		T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>;


		void SetDataValue(string fieldName, object value);

        void AddAuxiliary(string id, string columnName = null);

        /// <summary>
        /// Finishes the writing of the DataWriter.
        /// </summary>
        void Finish(VectoRun.Status runStatus, Exception exception = null);

		/// <summary>
		/// clear the modal data after the simulation
		/// called after the simulation is finished and the sum-entries have been written
		/// </summary>
		void FinishSimulation();

		string GetColumnName(IFuelProperties fuelData, ModalResultField mrf);

		void Reset(bool clearColumns = false);
		
		string GetColumnName(PowertrainPosition pos, ModalResultField mrf);


		Second Duration { get; }

		Meter Distance { get; }

		Func<Second, Joule, Joule, HeaterDemandResult> AuxHeaterDemandCalc { get; set; }

		KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel);
		void CalculateAggregateValues();
		//void AddElectricMotor(PowertrainPosition pos);
		KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel);
		bool HasCombustionEngine { get; }
		bool HasGearbox { get; }
		bool HasAxlegear { get; }
		WattSecond TotalElectricMotorWorkDrive(PowertrainPosition emPos);
		WattSecond TotalElectricMotorWorkRecuperate(PowertrainPosition emPos);
		WattSecond TotalElectricMotorMotWorkDrive(PowertrainPosition emPos);
		WattSecond TotalElectricMotorMotWorkRecuperate(PowertrainPosition emPos);
		PerSecond ElectricMotorAverageSpeed(PowertrainPosition emPos);
		double ElectricMotorEfficiencyDrive(PowertrainPosition emPos);
		double ElectricMotorEfficiencyGenerate(PowertrainPosition emPos);
		double ElectricMotorMotEfficiencyDrive(PowertrainPosition emPos);
		double ElectricMotorMotEfficiencyGenerate(PowertrainPosition emPos);
		WattSecond ElectricMotorOffLosses(PowertrainPosition emPos);
		WattSecond ElectricMotorLosses(PowertrainPosition emPos);
		WattSecond ElectricMotorMotLosses(PowertrainPosition emPos);
		WattSecond ElectricMotorTransmissionLosses(PowertrainPosition emPos);
		ICorrectedModalData CorrectedModalData { get; } 
		ModalResults Data { get; }
		string RunName { get; }
		IModalDataPostProcessor PostProcessingCorrection { set; }
		KilogramPerWattSecond FuelCellLine { get; }
		bool HasBattery { get; }
		void RegisterComponent(VectoSimulationComponent component);
		bool ContainsColumn(string modalResultField);
	}
}