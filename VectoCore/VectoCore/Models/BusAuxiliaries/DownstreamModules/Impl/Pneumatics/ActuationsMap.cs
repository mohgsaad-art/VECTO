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

using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class ActuationsMap : IActuationsMap
	{
		private Dictionary<MissionType, IActuations> _map;

		public ActuationsMap(Dictionary<MissionType, IActuations> map)
		{
			_map = map;
			
		}


		#region Implementation of IActuationsMap

		public IActuations Lookup(MissionType missionType)
		{
			return _map[missionType.GetNonEMSMissionType()];
		}

		#endregion
	}
}
