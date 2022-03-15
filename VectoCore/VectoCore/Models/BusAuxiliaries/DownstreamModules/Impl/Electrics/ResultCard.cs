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
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class DummyResultCard : IResultCard
	{
		#region Implementation of IResultCard

		public Ampere GetSmartCurrentResult(Ampere amps)
		{
			// MQ: mimic weird behavior of original resultcard implementation - return 10Amps in case less than 2 entries are provided...
			return 10.SI<Ampere>();
		}

		public Dictionary<Ampere, Ampere> Entries => new Dictionary<Ampere, Ampere>();

		#endregion
	}

	public class ResultCard : IResultCard
	{
		private readonly List<SmartResult> _results;

		// Constructor
		public ResultCard(List<SmartResult> results)
		{
			if (results == null) {
				throw new ArgumentException("A list of smart results must be supplied.");
			}

			if (results.Count < 2) {
				throw new ArgumentException("At least two smart result entries must be provided.");
			}
			_results = results.OrderBy(x => x.Amps).ToList();
		}


		// Public class outputs
		public IReadOnlyList<SmartResult> Results => _results;

		public Ampere GetSmartCurrentResult(Ampere Amps)
		{
			return GetOrInterpolate(Amps);
		}

		public Dictionary<Ampere, Ampere> Entries
		{
			get { return _results.ToDictionary(x => x.Amps, x => x.SmartAmps); }
		}


		// Helpers
		/// <summary>
		///         ''' Gets or interpolates value (A)
		///         ''' </summary>
		///         ''' <param name="amps"></param>
		///         ''' <returns></returns>
		///         ''' <remarks></remarks>
		private Ampere GetOrInterpolate(Ampere amps)
		{
			var s = _results.GetSection(x => amps > x.Amps);
			return VectoMath.Interpolate(s.Item1.Amps, s.Item2.Amps, s.Item1.SmartAmps, s.Item2.SmartAmps, amps);

		}

		public override bool Equals(object other)
		{
			var myOther = other as ResultCard;

			if (_results.Count != myOther?._results.Count) {
				return false;
			}

			return _results.Zip(myOther.Results, Tuple.Create).All(
				tuple => tuple.Item1.Amps.IsEqual(tuple.Item2.Amps) && tuple.Item1.SmartAmps.IsEqual(tuple.Item2.SmartAmps));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
