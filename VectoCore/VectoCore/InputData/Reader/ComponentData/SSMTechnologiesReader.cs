using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public class SSMTechnologiesReader
	{
		protected static string[] headerCols = new[]
		{
			Fields.Category, Fields.BenefitName,
			Fields.LowFloorC, Fields.LowFloorH, Fields.LowFloorV,
			Fields.SemiLowFloorC, Fields.SemiLowFloorH, Fields.SemiLowFloorV,
			Fields.RaisedFloorC, Fields.RaisedFloorH, Fields.RaisedFloorV,
			Fields.ActiveVC, Fields.ActiveVH, Fields.ActiveVV
		};

		public static List<SSMTechnology> ReadFromFile(string fileName)
		{
			return Create(VectoCSVFile.Read(fileName), fileName);
		}

		public static List<SSMTechnology> ReadFromStream(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str), null);
		}

		public static List<SSMTechnology> Create(DataTable data, string fileName)
		{
			if (!HeaderIsValid(data.Columns)) {
				throw new VectoException("invalid header for techlist file. expected: {0} got: {1}",
					string.Join(", ", headerCols), string.Join(", ",  data.Columns)
					);
			}

			var retVal = new List<SSMTechnology>();
			foreach (DataRow row in data.Rows) {
				retVal.Add(new SSMTechnology()
				{
					Category = row.Field<string>(Fields.Category),
					BenefitName = row.Field<string>(Fields.BenefitName),
					LowFloorH = row.ParseDouble(Fields.LowFloorH),
					LowFloorC = row.ParseDouble(Fields.LowFloorC),
					LowFloorV = row.ParseDouble(Fields.LowFloorV),
					SemiLowFloorH = row.ParseDouble(Fields.SemiLowFloorH),
					SemiLowFloorC = row.ParseDouble(Fields.SemiLowFloorC),
					SemiLowFloorV = row.ParseDouble(Fields.SemiLowFloorV),
					RaisedFloorH = row.ParseDouble(Fields.RaisedFloorH),
					RaisedFloorC = row.ParseDouble(Fields.RaisedFloorC),
					RaisedFloorV = row.ParseDouble(Fields.RaisedFloorV),
					ActiveVH = row.ParseBoolean(Fields.ActiveVH),
					ActiveVV = row.ParseBoolean(Fields.ActiveVV),
					ActiveVC = row.ParseBoolean(Fields.ActiveVC),
				});
			}

			return retVal;
		}

		protected static bool HeaderIsValid(DataColumnCollection columns)
		{
			return headerCols.All(h => columns.Contains(h));
		}

		public class Fields
		{
			public const string Category = "Category";
			public const string BenefitName = "BenefitName";
			public const string LowFloorH = "LowFloorH";
			public const string LowFloorC = "LowFloorC";
			public const string LowFloorV = "LowFloorV";
			public const string SemiLowFloorH = "SemiLowFloorH";
			public const string SemiLowFloorC = "SemiLowFloorC";
			public const string SemiLowFloorV = "SemiLowFloorV";
			public const string RaisedFloorH = "RaisedFloorH";
			public const string RaisedFloorC = "RaisedFloorC";
			public const string RaisedFloorV = "RaisedFloorV";
			public const string ActiveVH = "ActiveVH";
			public const string ActiveVV = "ActiveVV";
			public const string ActiveVC = "ActiveVC";
		}
	}
}
