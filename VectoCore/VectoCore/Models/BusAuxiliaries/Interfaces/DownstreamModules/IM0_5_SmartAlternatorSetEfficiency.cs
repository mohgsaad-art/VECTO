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

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
{
	public interface IM0_5_SmartAlternatorSetEfficiency : IAbstractModule
	{

		/// <summary>
		/// Smart Idle Current (A)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Ampere SmartIdleCurrent { get; }

		/// <summary>
		/// Alternators Efficiency In Idle ( Fraction )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		double AlternatorsEfficiencyIdleResultCard { get; }

		/// <summary>
		/// Smart Traction Current (A)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Ampere SmartTractionCurrent { get; }

		/// <summary>
		/// Alternators Efficiency In Traction ( Fraction )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		double AlternatorsEfficiencyTractionOnResultCard { get; }

		/// <summary>
		/// Smart Overrrun Current (A)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Ampere SmartOverrunCurrent { get; }

		/// <summary>
		/// Alternators Efficiency In Overrun ( Fraction )
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		double AlternatorsEfficiencyOverrunResultCard { get; }
	}
}
