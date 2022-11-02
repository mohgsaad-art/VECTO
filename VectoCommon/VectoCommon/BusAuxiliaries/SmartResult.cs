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
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public class SmartResult : IComparable<SmartResult>
	{
		public Ampere Amps { get;  }
		public Ampere SmartAmps { get;  }

		// Constructors
		
		public SmartResult(Ampere amps, Ampere smartAmps)
		{
			Amps = amps;
			SmartAmps = smartAmps;
		}

		// Comparison
		public int CompareTo(SmartResult other)
		{
			if (other.Amps > Amps) {
				return -1;
			}
			if (other.Amps == Amps) {
				return 0;
			}

			return 1;
		}

		// Comparison Overrides
		public override bool Equals(object obj)
		{
			var other = (SmartResult)obj;

			return Amps == other.Amps;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
