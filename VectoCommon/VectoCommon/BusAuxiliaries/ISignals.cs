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

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface ISignals
	{
		/// <summary>
		/// Pre Existing Aux Power (KW)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		Watt PreExistingAuxPower { get; set; }

		///// <summary>
		///// Engine Motoring Power (KW)
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks>Vecto Input</remarks>
		//Watt EngineMotoringPower { get; set; }

		///// <summary>
		///// Engine Driveline Power (KW)
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks></remarks>
		//Watt EngineDrivelinePower { get; set; }

		/// <summary>
		/// Smart Electrics
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Should be true if fitted to the vehicle - AAUX Input</remarks>
		//bool SmartElectrics { get; set; }

		/// <summary>
		/// Clucth Engaged
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		bool ClutchEngaged { get; set; }

		/// <summary>
		/// Engine Speed 1/NU
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// 	'''
		PerSecond EngineSpeed { get; set; }

		/// <summary>
		/// Smart Pneumatics
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>should be true if fitted to the vehicle- AAux Config Input</remarks>
		//bool SmartPneumatics { get; set; }

		/// <summary>
		/// Total Cycle Time In Seconds
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		//int TotalCycleTimeSeconds { get;  }

		/// <summary>
		/// Current Cycle Time In Seconds 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>( Will Increment during Cycle )</remarks>
		double CurrentCycleTimeInSeconds { get; set; }

		/// <summary>
		/// Engine Driveline Torque
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		NewtonMeter EngineDrivelineTorque { get; set; }

		/// <summary>
		/// Engine Idle
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		bool Idle { get; set; }

		/// <summary>
		/// In Neutral
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Vecto Input</remarks>
		bool InNeutral { get; set; }

		///// <summary>
		///// Auxiliary Event Reporting Level
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks>Can be used by Vecto to choose the level of reporting</remarks>
		//AdvancedAuxiliaryMessageType AuxiliaryEventReportingLevel { get; set; }

		/// <summary>
		/// Engine Stopped 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>'Vecto Input - Used to Cut Fueling in AAux model</remarks>
		bool EngineStopped { get; set; }

		/// <summary>
		/// WHTC ( Correction factor to be applied )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>'Vecto Input</remarks>
		double WHTC { get; set; }

		///// <summary>
		///// Declaration Mode
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks>Vecto Input - Used to decide if to apply WHTC/Possiblye other things in future</remarks>
		//bool DeclarationMode { get; set; }

		/// <summary>
		/// Engine Idle Speed ( Associated with the vehicle bein tested )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		PerSecond EngineIdleSpeed { get; set; }

		
		///// <summary>
		///// Running Calc
		///// </summary>
		///// <value></value>
		///// <returns></returns>
		///// <remarks></remarks>
		//bool RunningCalc { get; set; }

		//Watt InternalEnginePower { get; set; }

		Second SimulationInterval { get; set; }
		Watt ExcessiveDragPower { get; set; }
		bool VehicleStopped { get; set; }
	}
}
