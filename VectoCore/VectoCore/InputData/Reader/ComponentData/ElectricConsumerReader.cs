using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ElectricConsumerReader
	{
		public static readonly string[] Header = new[] {
			Fields.Category, Fields.Consumer, Fields.BaseVehicle, Fields.PhaseIdle, Fields.NuminVehicle
		};

		public static ElectricalConsumerList ReadStream(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str));
		}

		public static ElectricalConsumerList Create(TableData data)
		{
			if (!HeaderValid(data.Columns)) {
				throw new VectoException("Invalid header. Expected: {0}, got: {1}",
					string.Join(", ", Header),
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			var retVal = new List<ElectricalConsumer>();
			foreach (DataRow row in data.Rows) {
				var consumer = new ElectricalConsumer() {
					Category = row.Field<string>(Fields.Category),
					ConsumerName = row.Field<string>(Fields.Consumer),
					BaseVehicle = row.ParseBoolean(Fields.BaseVehicle),
					DefaultConsumer = row.ParseBoolean(Fields.DefaultConsumer),
					Bonus = row.ParseBoolean(Fields.Bonus),
					PhaseIdleTractionOn = row.ParseDouble(Fields.PhaseIdle),
					NumberInActualVehicle = row.Field<string>(Fields.NuminVehicle)
				};
				foreach (var mission in EnumHelper.GetValues<MissionType>()) {
					if (data.Columns.Contains(mission.GetLabel())) {
						consumer[mission] = row.ParseDouble(mission.GetLabel()).SI<Ampere>();
					}
				}
				retVal.Add(consumer);
			}

			return new ElectricalConsumerList( retVal);
		}

		private static bool HeaderValid(DataColumnCollection dataColumns)
		{
			return Header.All(h => dataColumns.Contains(h));
		}

		public static class Fields
		{
			public const string Category = "Category";
			public const string Consumer = "Consumer";
			public const string BaseVehicle = "Base Vehicle";
			public const string DefaultConsumer = "Default Consumer";
			public const string Bonus = "Bonus";
			//public const string NominalAmps = "Nominal Amps";
			public const string PhaseIdle = "PhaseIdle";
			public const string NuminVehicle = "Number in Vehicle";
			//public const string Info = "Info";

		}
	}
}
