// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
{
	//public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

	public interface IBusAuxiliaries
	{
		// Inherits IAuxiliaryEvent

		//event AuxiliaryEventEventHandler AuxiliaryEvent;

		// Information
		//bool Running { get; }

		//string AuxiliaryName { get; }
		//string AuxiliaryVersion { get; }

		// Additional Permenent Monitoring Signals - Required by engineering
		//double AA_NonSmartAlternatorsEfficiency { get; }
		//Ampere AA_SmartIdleCurrent_Amps { get; }
		//double AA_SmartIdleAlternatorsEfficiency { get; }
		//Ampere AA_SmartTractionCurrent_Amps { get; }
		//double AA_SmartTractionAlternatorEfficiency { get; }
		//Ampere AA_SmartOverrunCurrent_Amps { get; }
		//double AA_SmartOverrunAlternatorEfficiency { get; }
		//NormLiterPerSecond AA_CompressorFlowRate_LitrePerSec { get; }
		//bool AA_OverrunFlag { get; }
		//int? AA_EngineIdleFlag { get; }
		//bool AA_CompressorFlag { get; }
		//Kilogram AA_TotalCycleFC_Grams { get; }
		////Liter AA_TotalCycleFC_Litres { get; }
		//Watt AA_AveragePowerDemandCrankHVACMechanicals { get; }
		//Watt AA_AveragePowerDemandCrankHVACElectricals { get; }
		//Watt AA_AveragePowerDemandCrankElectrics { get; }
		//Watt AA_AveragePowerDemandCrankPneumatics { get; }
		//Kilogram AA_TotalCycleFuelConsumptionCompressorOff { get; }
		//Kilogram AA_TotalCycleFuelConsumptionCompressorOn { get; }

		/// <summary>
		/// Total Cycle Fuel In Grams
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		//Kilogram TotalFuel { get; }

		///// <summary>
		///// Total Cycle Fuel in Litres
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks></remarks>
		//Liter TotalFuelLITRES { get; }
		/// <summary>
		/// Total Power Demans At Crank From Auxuliaries (W)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Watt AuxiliaryPowerAtCrankWatts { get; }


		/// <summary>
		/// Vecto Inputs
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		//IVectoInputs VectoInputs { get; set; }
		/// <summary>
		/// Signals From Vecto 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		ISignals Signals { get; set; }

		Watt HVACElectricalPowerConsumer { get; }

		Watt ElectricPowerConsumer { get; }

		Watt ElectricPowerConsumerSum { get; }
		Watt ElectricPowerGenerated { get; }
		Watt ElectricPowerDemandMech { get; }
		NormLiter PSDemandConsumer { get; }
		NormLiter PSAirGenerated { get; }
		NormLiter PSAirGeneratedAlwaysOn { get; }
		//NormLiter PSAirGeneratedDrag { get; }
		Watt PSPowerDemandAirGenerated { get; }
		Watt PSPowerCompressorAlwaysOn { get; }
		Watt PSPowerCompressorDragOnly { get; }
		Watt HVACMechanicalPowerConsumer { get; }
		Watt HVACMechanicalPowerGenerated { get; }
		
		Joule AuxHeaterDemandCalculation(Second cycleTime, Joule engineWasteHeatTotal);


		///// <summary>
		///// Configure Auxuliaries ( Launches Config Form )
		///// </summary>
		///// <param name="filePath"></param>
		///// <param name="vectoFilePath"></param>
		///// <returns></returns>
		///// <remarks></remarks>
		//bool Configure(string filePath, string vectoFilePath);

		///// <summary>
		///// Validate AAUX file path supplied.
		///// </summary>
		///// <param name="filePath"></param>
		///// <param name="message"></param>
		///// <returns></returns>
		///// <remarks></remarks>
		//bool ValidateAAUXFile(string filePath, ref string message);

		// Command
		/// <summary>
		/// Cycle Step - Used to calculate fuelling
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		void CycleStep(Second seconds);

        ///// <summary>
        ///// Initialises AAUX Environment ( Begin Processs )
        ///// </summary>
        ///// <param name="auxcConfig"></param>
        ///// <param name="fuelProperties"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //bool RunStart(IAuxiliaryConfig auxcConfig, IFuelProperties fuelProperties);

        ///// <summary>
        ///// Any Termination Which Needs to be done ( Model depenent )
        ///// </summary>
        ///// <param name="message"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //bool RunStop(ref string message);

        void ResetCalculations();
	}
}
