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
	public interface IM5_SmartAlternatorSetGeneration : IAbstractModule
	{

		/// <summary>
		/// 	Alternators Generation Power At Crank Idle (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AlternatorsGenerationPowerAtCrankIdle();

		/// <summary>
		/// 	Alternators Generation Power At Crank Traction On  (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AlternatorsGenerationPowerAtCrankTractionOn();

		/// <summary>
		/// 	Alternators Generation Power At Crank Overrun  (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AlternatorsGenerationPowerAtCrankOverrun();
	}
}
