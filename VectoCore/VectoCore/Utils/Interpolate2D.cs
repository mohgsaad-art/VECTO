using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public abstract class Interpolate2D<TKeyX, TKeyY, TValZ, TEntry>
		where TKeyX : SI
		where TKeyY : SI
		where TValZ : SIBase<TValZ>
	{
		private DataTable _data;

		private KeyValuePair<TKeyX, int>[] entriesX;
		private KeyValuePair<TKeyY, int>[] entriesY;

		protected void SetData(TEntry[] entries)
		{
			_data = new DataTable();
			var xEntries = new List<KeyValuePair<TKeyX, int>>();
			var idx = 0;
			foreach (var xValue in GetXValuesSorted(entries).Distinct()) {
				_data.Columns.Add(xValue.ToOutputFormat(), typeof(TValZ));
				xEntries.Add(new KeyValuePair<TKeyX, int>(xValue, idx++));
			}

			entriesX = xEntries.ToArray();

			idx = 0;
			var yEntries = new List<KeyValuePair<TKeyY, int>>();
			foreach (var yValue in GetYValuesSorted(entries).Distinct()) {
				var row = _data.NewRow();
				_data.Rows.Add(row);
				yEntries.Add(new KeyValuePair<TKeyY, int>(yValue, idx++));
			}

			entriesY = yEntries.ToArray();

			foreach (var entry in entries) {
				var col = entriesX.First(x => x.Key.IsEqual(GetXValue(entry)));
				var row = entriesY.First(x => x.Key.IsEqual(GetYValue(entry)));
				_data.Rows[row.Value][col.Value] = GetZValue(entry);
			}
		}

		protected abstract TKeyX GetXValue(TEntry entry);

		protected abstract TKeyY GetYValue(TEntry entry);

		protected abstract TValZ GetZValue(TEntry entry);
		
		protected abstract IEnumerable<TKeyX> GetXValuesSorted(TEntry[] entries);

		protected abstract IEnumerable<TKeyY> GetYValuesSorted(TEntry[] entries);


		public TValZ Interpolate(TKeyX valX, TKeyY valY)
		{
			var speedIdx = -1;
			for (var i = 0; i < entriesX.Length; i++) {
				if (entriesX[i].Key.CompareTo(valX) >= 0) {
					speedIdx = i;
					break;
				}
			}

			if (speedIdx <= 0) {
				speedIdx = 1;
			}

			var gradientIdx = -1;
			for (var i = 0; i < entriesY.Length; i++) {
				if (entriesY[i].Key.CompareTo(valY) >= 0) {
					gradientIdx = i;
					break;
				}
			}

			if (gradientIdx <= 0) {
				gradientIdx = 1;
			}

			var v1 = _data.Rows[entriesY[gradientIdx - 1].Value][entriesX[speedIdx - 1].Value] as TValZ;
			var v2 = _data.Rows[entriesY[gradientIdx - 1].Value][entriesX[speedIdx].Value] as TValZ;
			var v3 = _data.Rows[entriesY[gradientIdx].Value][entriesX[speedIdx - 1].Value] as TValZ;
			var v4 = _data.Rows[entriesY[gradientIdx].Value][entriesX[speedIdx].Value] as TValZ;
			var speed1 = VectoMath.Interpolate(entriesX[speedIdx - 1].Key, entriesX[speedIdx].Key, v1, v2, valX);
			var speed2 = VectoMath.Interpolate(entriesX[speedIdx - 1].Key, entriesX[speedIdx].Key, v3, v4, valX);

			return VectoMath.Interpolate(
				entriesY[gradientIdx - 1].Key, entriesY[gradientIdx].Key, speed1, speed2, valY);
		}
	}
}
