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

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IAlternatorMap 
	{

		/// <summary>
		/// 	Returns the alternator efficiency at given rpm
		/// 	</summary>
		/// 	<param name="rpm">alternator rotation speed</param>
		/// <param name="currentDemand"></param>
		/// <returns>Single</returns>
		/// 	<remarks></remarks>
		double GetEfficiency(PerSecond rpm, Ampere currentDemand);

		string Source { get; }
	}
}
