using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public class LookupDataReader<TKey, TValue>
	{
		private string[] _columns;
		private int _minRows;
		private string _name;

		public LookupDataReader(string name, string[] columns, int minRows = 2)
		{
			_name = name;
			_minRows = minRows;
			_columns = columns;
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> Create(TableData data, Func<DataRow, TKey> xFunc, Func<DataRow, TValue> yFunc)
		{
			if (data.Columns.Count != _columns.Length) {
				throw new VectoException("{0} lookup data must consist of {1} columns", _name, _columns.Length);
			}

			if (data.Rows.Count < _minRows) {
				throw new VectoException("{0} lookup data must consist of at least {1} rows.", _name, _minRows);
			}

			if (!HeaderIsValid(data.Columns)) {
				LoggingObject.Logger<ShareIdleLowLookup>().Warn(
					"{0} lookup: Header line is not valid. Expected: '{1}', Got: {2}",
					_name,
					_columns.Join(),
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				for (var i = 0; i < Math.Min(_columns.Length, data.Columns.Count); i++) {
					data.Columns[i].Caption = _columns[i];
				}
			}
			return data.Rows.Cast<DataRow>()
					.Select(
						r => new KeyValuePair<TKey, TValue>(
							xFunc(r),
							yFunc(r)))
					;
		}

		private bool HeaderIsValid(DataColumnCollection columns)
		{
			return _columns.All(x => columns.Contains(x));
		}
	}
}
