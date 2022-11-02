/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public abstract class LookupData : LoggingObject
	{
		protected abstract string ResourceId { get; }
		protected abstract string ErrorMessage { get; }
		protected abstract void ParseData(DataTable table);

		protected LookupData()
		{
			ReadData();
		}

		protected void ReadData()
		{
			if (!string.IsNullOrWhiteSpace(ResourceId)) {
				var table = ReadCsvResource(ResourceId);
				NormalizeTable(table);
				ParseData(table);
			}
		}

		protected static DataTable ReadCsvResource(string resourceId)
		{
// TODO: MQ 2020-07 Remove in official bus version!
//#if USE_EXTENAL_DECLARATION_DATA
			var tmp = resourceId.Replace(DeclarationData.DeclarationDataResourcePrefix + ".", "");
			var parts = tmp.Split('.');
			var fileName = Path.Combine("Declaration", string.Join(".", parts[parts.Length - 2], parts[parts.Length - 1]));
			if (File.Exists(fileName)) {
				return VectoCSVFile.Read(fileName);
			}

			//#else
			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceId), source: resourceId);
			//#endif
		}

		protected static void NormalizeTable(DataTable table)
		{
			foreach (DataColumn col in table.Columns) {
				table.Columns[col.ColumnName].ColumnName = col.ColumnName.ToLower().RemoveWhitespace();
			}
		}
	}

	public abstract class LookupData<TKey, TValue> : LookupData where TValue : struct
	{
		protected Dictionary<TKey, TValue> Data = new Dictionary<TKey, TValue>();

		protected override string ErrorMessage => "key {0} not found in lookup data";

		public virtual TValue Lookup(TKey key)
		{
			try {
				return Data[key];
			} catch (KeyNotFoundException) {
				throw new VectoException(string.Format(ErrorMessage, key));
			}
		}

		public Dictionary<TKey, TValue> Entries => Data;
	}

	public abstract class LookupData<TKey1, TKey2, TValue> : LookupData where TValue : struct
	{
		protected readonly Dictionary<Tuple<TKey1, TKey2>, TValue> Data = new Dictionary<Tuple<TKey1, TKey2>, TValue>();

		public virtual TValue Lookup(TKey1 key1, TKey2 key2)
		{
			try {
				return Data[Tuple.Create(key1, key2)];
			} catch (KeyNotFoundException) {
				throw new VectoException(string.Format(ErrorMessage, key1, key2));
			}
		}
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TValue> : LookupData where TValue : struct
	{
		protected readonly Dictionary<Tuple<TKey1, TKey2, TKey3>, TValue> Data =
			new Dictionary<Tuple<TKey1, TKey2, TKey3>, TValue>();

		public virtual TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			try {
				return Data[Tuple.Create(key1, key2, key3)];
			} catch (KeyNotFoundException) {
				throw new VectoException(string.Format(ErrorMessage, key1, key2, key3));
			}
		}

		//public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TKey4, TValue> : LookupData where TValue : struct
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TKey4, TKey5, TValue> : LookupData where TValue : struct
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TValue> : LookupData where TValue : struct
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TValue> : LookupData where TValue : struct
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7);
	}
}
