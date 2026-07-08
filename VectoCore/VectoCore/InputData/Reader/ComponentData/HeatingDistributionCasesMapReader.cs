using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class HeatingDistributionMapReader
	{
		public static HeatingDistributionMap ReadFile(string fileName)
		{
			if ((string.IsNullOrWhiteSpace(fileName))) {
				return null;
			}

			if (!File.Exists(fileName)) {
				throw new FileNotFoundException(fileName);
			}

			return Create(VectoCSVFile.Read(fileName));
		}

		public static HeatingDistributionMap ReadStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static HeatingDistributionMap Create(TableData data)
		{
			var requiredHeatPumps = EnumHelper.GetValues<HeatPumpType>()
				.Where(x => !x.IsOneOf(HeatPumpType.none, HeatPumpType.not_applicable)).ToList();
			if (!HeaderIsValid(data.Columns, requiredHeatPumps)) {
				throw new VectoException("Invalid Header vor heating distribution map", ErrorCodes.DATATABLE_WRONG_COLUMNS,
					$"Make sure the Heating Distribution Map file contains the required columns: {string.Join(", ", requiredHeatPumps.Select(x => x.GetLabel()))}.");
			}

			var map = new Dictionary<Tuple<HeatingDistributionCase, int>, HeatingDistributionMap.Entry>();
			foreach (DataRow row in data.Rows) {
				var heatPumpContr = new Dictionary<HeatPumpType, double>();
				foreach (var heatPumpType in requiredHeatPumps) {
					var value = row.Field<string>(heatPumpType.GetLabel()).ToDouble(double.NaN) / 100.0;
					if (!double.IsNaN(value)) {
						heatPumpContr.Add(heatPumpType, value);
					}
				}
				var hdCase = HeatingDistributionCaseHelper.Parse(row.Field<string>(Fields.HeatingDistributionCase));
				var envId = row.Field<string>(Fields.EnvironmentalId).ToInt();
				var elHeater = row.Field<string>(Fields.ElectricHeater).ToDouble(double.NaN) / 100.0;
				var fuelHeater = row.Field<string>(Fields.FuelHeater).ToDouble(double.NaN) / 100.0;
				map.Add(Tuple.Create(hdCase, envId), new HeatingDistributionMap.Entry(envId, heatPumpContr, elHeater, fuelHeater));
			}

			return new HeatingDistributionMap(map);
		}

		public static bool HeaderIsValid(DataColumnCollection columns, List<HeatPumpType> requiredHeatPumps)
		{
			var header = requiredHeatPumps.Select(x => x.GetLabel())
				.ToList();
			header.AddRange(new[] { Fields.HeatingDistributionCase, Fields.ElectricHeater, Fields.FuelHeater });
			return header.All(h => columns.Contains(h));
		}

		public static class Fields
		{
			public const string HeatingDistributionCase = "HeatingDistributionCase";
			public const string EnvironmentalId = "Environmental ID";
			public const string ElectricHeater = "electric heater";
			public const string FuelHeater = "fuel heater";
		}
	}

    public static class HeatingDistributionCasesMapReader
	{
		public static HeatingDistributionCasesMap ReadFile(string fileName)
		{
			if ((string.IsNullOrWhiteSpace(fileName))) {
				return null;
			}

			if (!File.Exists(fileName)) {
				throw new FileNotFoundException(fileName);
			}

			return Create(VectoCSVFile.Read(fileName));
		}

		public static HeatingDistributionCasesMap ReadStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static HeatingDistributionCasesMap Create(TableData data)
		{
			var requiredHeatPumps = EnumHelper.GetValues<HeatPumpType>()
				.Where(x => !x.IsOneOf(HeatPumpType.none, HeatPumpType.not_applicable)).ToList();
			if (!HeaderIsValid(data.Columns, requiredHeatPumps)) {
				throw new VectoException("Invalid Header vor heating distribution map", ErrorCodes.DATATABLE_WRONG_COLUMNS,
					$"Make sure the Heating Distribution Map file contains the required columns: {string.Join(", ", requiredHeatPumps.Select(x => x.GetLabel()))}.");
			}

			var map = new List<HeatingDistributionCasesMap.Entry>();
			foreach (DataRow row in data.Rows) {
				var allowedHeatPumps = new List<HeatPumpType>();
				foreach (var heatPumpType in requiredHeatPumps) {
					var allowed = row.Field<string>(heatPumpType.GetLabel()).ToBoolean();
					if (allowed) {
						allowedHeatPumps.Add(heatPumpType);
					}
				}

				if (allowedHeatPumps.Count > 1) {
					throw new VectoException("One row shall not contain multiple heatpump technologies!", ErrorCodes.DATATABLE_INVALID_DATA,
						$"Make sure the Heating Distribution Map file does not contain multiple heatpump technologies in a single row.");
				}

				if (allowedHeatPumps.Count == 0) {
					allowedHeatPumps.Add(HeatPumpType.none);
				}
				map.Add(new HeatingDistributionCasesMap.Entry() {
					HeatingDistributionCase = HeatingDistributionCaseHelper.Parse(row.Field<string>(Fields.HeatingDistributionCase)),
					ElectricHeater = row.Field<string>(Fields.ElectricHeater).ToBoolean(),
					FuelHeater = row.Field<string>(Fields.FuelHeater).ToBoolean(),
					HeatPumpType = allowedHeatPumps.First()
				});
			}

			return new HeatingDistributionCasesMap(map);
		}

		public static bool HeaderIsValid(DataColumnCollection columns, List<HeatPumpType> requiredHeatPumps)
		{
			var header = requiredHeatPumps.Select(x => x.GetLabel())
				.ToList();
			header.AddRange(new[] {Fields.HeatingDistributionCase, Fields.ElectricHeater, Fields.FuelHeater});
			return header.All(h => columns.Contains(h));
		}

		public static class Fields
		{
			public const string HeatingDistributionCase = "HeatingDistributionCase";
			public const string ElectricHeater = "electric heater";
			public const string FuelHeater = "fuel heater";
		}
	}
}