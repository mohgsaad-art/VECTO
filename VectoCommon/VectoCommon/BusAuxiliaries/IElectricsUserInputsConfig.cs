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


using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IElectricsUserInputsConfig
	{
		/// <summary>
		/// 	Power Net Voltage - The supply voltage used on the vehilce.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Volt PowerNetVoltage { get; }

		/// <summary>
		/// 	The Path for the Alternator map
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		//IList<ICombinedAlternatorMapRow> AlternatorMap { get; }
		IAlternatorMap AlternatorMap { get; }

		/// <summary>
		/// 	Alternator Gear Efficiency
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double AlternatorGearEfficiency { get;  }

		Ampere AverageCurrentDemandInclBaseLoad(bool engineOff, bool vehicleStopped);

		Ampere AverageCurrentDemandWithoutBaseLoad(bool engineOff, bool vehicleStopped);

		//Ampere AverageCurrentDemandInclBaseLoad { get; }

		//Ampere AverageCurrentDemandWithoutBaseLoad { get; }

		/// <summary>
		/// 	Door Actuation Time In Seconds ( Time Taken to Open/Close the door )
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Second DoorActuationTimeSecond { get; }

		/// <summary>
		/// 	Result Card Taken During Idle.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardIdle { get; }

		/// <summary>
		/// 	Result Card Taken During Traction
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardTraction { get; }

		/// <summary>
		/// 	Result Card Taken During Overrun
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardOverrun { get; }

		/// <summary>
		/// 	Smart Electrical System
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>True For Smart Electrical Systems/ False For non Smart.</returns>
		/// 	<remarks></remarks>
		AlternatorType AlternatorType { get; }

		/// <summary>
		/// 	Stored Energy Efficiency
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>Stored Energy Efficiency</returns>
		/// 	<remarks></remarks>
		double StoredEnergyEfficiency { get; }

		Watt MaxAlternatorPower { get; }

		WattSecond ElectricStorageCapacity { get; }

		bool ConnectESToREESS { get; }

		double DCDCEfficiency { get; }
	}
}
