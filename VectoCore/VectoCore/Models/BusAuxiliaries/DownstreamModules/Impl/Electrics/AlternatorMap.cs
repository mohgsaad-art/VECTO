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
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class AlternatorMap : IAlternatorMap
	{
		
		private List<MapPoint> _map = new List<MapPoint>();
		private List<Ampere> _yRange;
		private List<PerSecond> _xRange;
		private PerSecond _minX;
		private Ampere _minY;
		private PerSecond _maxX;
		private Ampere _maxY;

		public AlternatorMap(IList<ICombinedAlternatorMapRow> rows, string source)
		{
			Source = source;
			foreach (var row in rows.OrderBy(x => x.AlternatorName).ThenBy(x => x.RPM).ThenBy(x => x.Amps)) {
				_map.Add(new MapPoint(row.Amps, row.RPM, row.Efficiency));
			}

			GetMapRanges();
		}

		public string Source { get; }

		// Required Action Test or Interpolation Type
		public bool OnBoundaryYInterpolatedX(PerSecond x, Ampere y)
		{
			return _yRange.Contains(y) && !_xRange.Contains(x);
		}

		public bool OnBoundaryXInterpolatedY(PerSecond x, Ampere y)
		{
			return !_yRange.Contains(y) && _xRange.Contains(x);
		}

		public bool ONBoundaryXY(PerSecond x, Ampere y)
		{
			return (from sector in _map
					where sector.Y == y && sector.X == x
					select sector).Count() == 1;
		}

		// Determine Value Methods
		private double GetOnBoundaryXY(PerSecond x, Ampere y)
		{
			return (from sector in _map
					where sector.Y == y && sector.X == x
					select sector).First().V;
		}

		private double GetOnBoundaryYInterpolatedX(PerSecond x, Ampere y)
		{
			//double x0, x1, v0, v1, slope, dx;

			var x0 = (from p in _xRange
				  orderby p
				  where p < x
				  select p).Last();
			var x1 = (from p in _xRange
				  orderby p
				  where p > x
				  select p).First();
			var dx = x1 - x0;

			var v0 = GetOnBoundaryXY(x0, y);
			var v1 = GetOnBoundaryXY(x1, y);

			return v0 + (x - x0) * (v1 - v0) / (x1 - x0);
		}

		private double GetOnBoundaryXInterpolatedY(PerSecond x, Ampere y)
		{
			//double y0, y1, v0, v1, dy, v, slope;

			var y0 = (from p in _yRange
				  orderby p
				  where p < y
				  select p).Last();
			var y1 = (from p in _yRange
				  orderby p
				  where p > y
				  select p).First();
			var dy = y1 - y0;

			var v0 = GetOnBoundaryXY(x, y0);
			var v1 = GetOnBoundaryXY(x, y1);

			var v = v0 + (y - y0) * (v1 - v0) / (y1 - y0);

			return v;
		}

		private double GetBiLinearInterpolatedValue(PerSecond x, Ampere y)
		{
			//double q11, q12, q21, q22, x1, x2, y1, y2, r1, r2, p;

			var y1 = (from mapSector in _map
				  where mapSector.Y < y
				  select mapSector).Last().Y;
			var y2 = (from mapSector in _map
				  where mapSector.Y > y
				  select mapSector).First().Y;

			var x1 = (from mapSector in _map
				  where mapSector.X < x
				  select mapSector).Last().X;
			var x2 = (from mapSector in _map
				  where mapSector.X > x
				  select mapSector).First().X;

			var q11 = GetOnBoundaryXY(x1, y1);
			var q12 = GetOnBoundaryXY(x1, y2);

			var q21 = GetOnBoundaryXY(x2, y1);
			var q22 = GetOnBoundaryXY(x2, y2);

			var r1 = ((x2 - x) / (x2 - x1)) * q11 + ((x - x1) / (x2 - x1)).Value() * q21;

			var r2 = ((x2 - x) / (x2 - x1)) * q12 + ((x - x1) / (x2 - x1)).Value() * q22;


			var p = ((y2 - y) / (y2 - y1)).Value() * r1 + ((y - y1) / (y2 - y1)).Value() * r2;


			return p;
		}

		private void GetMapRanges()
		{
			_yRange = _map.Select(x => x.Y).Distinct().OrderBy(x => x).ToList();
			_xRange = _map.Select(x => x.X).Distinct().OrderBy(x => x).ToList();

			_minX = _xRange.First();
			_maxX = _xRange.Last();
			_minY = _yRange.First();
			_maxY = _yRange.Last();
		}

		// Single entry point to determine Value on map
		public double GetValue(PerSecond x, Ampere y)
		{
			if (x < _minX || x > _maxX || y < _minY || y > _maxY) {

				// OnAuxiliaryEvent(String.Format("Alternator Map Limiting : RPM{0}, AMPS{1}",x,y),AdvancedAuxiliaryMessageType.Warning)


				// Limiting
				if (x < _minX) {
					x = _minX;
				}
				if (x > _maxX) {
					x = _maxX;
				}
				if (y < _minY) {
					y = _minY;
				}
				if (y > _maxY) {
					y = _maxY;
				}
			}


			// Satisfies both data points - non interpolated value
			if (ONBoundaryXY(x, y)) {
				return GetOnBoundaryXY(x, y);
			}

			// Satisfies only x or y - single interpolation value
			if (OnBoundaryXInterpolatedY(x, y)) {
				return GetOnBoundaryXInterpolatedY(x, y);
			}
			if (OnBoundaryYInterpolatedX(x, y)) {
				return GetOnBoundaryYInterpolatedX(x, y);
			}

			// satisfies no data points - Bi-Linear interpolation
			return GetBiLinearInterpolatedValue(x, y);
		}

		
		private class MapPoint
		{
			public Ampere Y;
			public PerSecond X;
			public double V;

			public MapPoint(Ampere y, PerSecond x, double v)
			{
				Y = y;
				X = x;
				V = v;
			}

			public override bool Equals(object other)
			{
				var myOther = other as MapPoint;
				if (myOther == null) {
					return false;
				}

				return V.IsEqual(myOther.V) && X.IsEqual(myOther.X) && Y.IsEqual(myOther.Y);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		// Get Alternator Efficiency
		public double GetEfficiency(PerSecond rpm, Ampere currentDemand)
		{
			return GetValue(rpm, currentDemand);
		}

		public IList<string> Technologies => new List<string>();


		// Public Events
		public event AuxiliaryEventEventHandler AuxiliaryEvent;

		//public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		protected void OnAuxiliaryEvent(string message, AdvancedAuxiliaryMessageType messageType)
		{
			object alternatorMap = this;
			AuxiliaryEvent?.Invoke(ref alternatorMap, message, messageType);
		}

		public override bool Equals(object other)
		{
			var myOther = other as AlternatorMap;

			if (_map.Count != myOther?._map.Count) {
				return false;
			}

			return _map.Zip(myOther._map, Tuple.Create).All(tuple => tuple.Item1.Equals(tuple.Item2));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
