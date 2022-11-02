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


using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricalConsumerList // : IElectricalConsumerList
	{
		private readonly List<ElectricalConsumer> _items;
		//private Volt _powernetVoltage;
		
		// Constructor
		public ElectricalConsumerList(List<ElectricalConsumer> consumer)
		{
			_items = consumer;
		}

		// Interface implementation
		
		public IReadOnlyList<ElectricalConsumer> Items => _items;
	}
}
