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
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
    /// <summary>
    /// Defines Methods for adding components, commiting a simulation step and finishing the simulation.
    /// Also defines interfaces for all cockpit access to data.
    /// </summary>
    public interface IVehicleContainer : IDataBus
	{
		IModalDataContainer ModalData { get; }

		[Required, ValidateObject]
		VectoRunData RunData { get; }

		ISimulationOutPort GetCycleOutPort();

		VectoRun.Status RunStatus { get; set; }

		ISimplePowertrainBuilder SimplePowertrainBuilder { get; }

		IAuxInProvider BusAux { get; }

		/// <summary>
        /// Adds a component to the vehicle container.
        /// </summary>
        /// <param name="component"></param>
        void AddComponent(VectoSimulationComponent component);

		void AddAuxiliary(string id, string columnName = null);

		/// <summary>
		/// Commits the current simulation step.
		/// </summary>
		void CommitSimulationStep(Second time, Second simulationInterval);

		/// <summary>
		/// Finishes the simulation.
		/// </summary>
		/// <param name="exception"></param>
		void FinishSimulationRun(Exception exception = null);

		void StartSimulationRun();

		IEnumerable<ISimulationPreprocessor> GetPreprocessingRuns { get; }
		ISumData SumData { get; }
		void AddPreprocessor(ISimulationPreprocessor simulationPreprocessor);
		void ResetComponents();
		void FinishSingleSimulationRun(Exception e = null);

		IReadOnlyList<VectoSimulationComponent> Components { get; }
	}

	public interface IExemptedVehicleContainer : IVehicleContainer
	{

	}

	public interface ISimpleVehicleContainer : IVehicleContainer
	{
		//IDriverDemandOutPort VehiclePort { get; }

		ITnOutPort GearboxOutPort { get; }

		bool HasGearbox { get; }

		VectoSimulationJobType VehicleArchitecutre { get; }

		PowertrainPosition[] ElectricMotorPositions { get; }

        Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors { get; }
		
		IHybridController HybridController { get; }

		IReadOnlyCollection<VectoSimulationComponent> SimulationComponents();

        void UpdateComponents(IDataBus realContainer);

	}
}