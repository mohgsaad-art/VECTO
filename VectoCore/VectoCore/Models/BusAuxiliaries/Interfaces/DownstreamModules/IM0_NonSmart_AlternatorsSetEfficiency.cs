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
	public interface IM0_NonSmart_AlternatorsSetEfficiency : IAbstractModule
	{

		/// <summary>
		///  Gets HVAC Electrical Power Demand (A)
		///  </summary>
		///  <value></value>
		///  <returns></returns>
		///  <remarks></remarks>
		Ampere GetHVACElectricalCurrentDemand { get; }

		/// <summary>
		///  Gets Alternator Efficiency (0-1)
		///  </summary>
		///  <value></value>
		///  <returns></returns>
		///  <remarks></remarks>
		double AlternatorsEfficiency { get; }
	}
}
