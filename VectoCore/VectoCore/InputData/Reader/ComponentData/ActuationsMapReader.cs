using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ActuationsMapReader
	{
		public static readonly string[] Header = new[] { Fields.CycleName, Fields.Braking, Fields.ParkBrakeAndDoors, Fields.Kneeling, Fields.CycleTime };

		public static IActuationsMap Read(string fileName)
		{
			return Create(VectoCSVFile.Read(fileName), Path.GetFullPath(fileName));
		}

		public static IActuationsMap ReadStream(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str), null);
		}

		public static IActuationsMap Create(DataTable data, string source)
		{
			if (!HeaderIsValid(data.Columns)) {
				throw new VectoException("Invalid header for pneumatic actuations. expected: {0}, got: {1}",
					string.Join(", ", Header),
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			var retVal = new Dictionary<MissionType, IActuations>();
			foreach (DataRow row in data.Rows) {
				var key = row.Field<string>(Fields.CycleName).ParseEnum<MissionType>();
				if (retVal.ContainsKey(key)) {
					throw new VectoException("Duplicate entries in actuations map! {0} / {1}", key.GetLabel());
				}

				var entry = new Actuations {
					Braking = row.Field<string>(Fields.Braking).ToInt(),
					ParkBrakeAndDoors = row.Field<string>(Fields.ParkBrakeAndDoors).ToInt(),
					Kneeling = row.Field<string>(Fields.Kneeling).ToInt(),
					CycleTime = row.Field<string>(Fields.CycleTime).ToInt().SI<Second>(),
				};
				retVal[key] = entry;
			}
			return new ActuationsMap(retVal);
		}

		private static bool HeaderIsValid(DataColumnCollection cols)
		{
			return Header.All(x => cols.Contains(x));
		}

		public class Fields
		{
			public const string CycleName = "CycleName";
			public const string Braking = "Brakes";
			public const string ParkBrakeAndDoors = "Park brake + 2 doors";
			public const string Kneeling = "Kneeling";
			public const string CycleTime = "CycleTime";
		}

	}
}
