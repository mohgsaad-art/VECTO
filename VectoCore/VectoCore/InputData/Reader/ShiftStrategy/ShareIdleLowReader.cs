using System;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public static class ShareIdleLowReader
	{
		public static IShareIdleLowLookup ReadFromStream(Stream stream)
		{
			var data = VectoCSVFile.ReadStream(stream);
			return Create(data);
		}

		public static IShareIdleLowLookup ReadFromFile(string filename)
		{
			try {
				var data = VectoCSVFile.Read(filename);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException("error while reading ShiftStrategy IdleLow Lookup File: " + e.Message);
			}
		}

		public static ShareIdleLowLookup Create(TableData data)
		{
			if (data == null) {
				return null;
			}
			return new ShareIdleLowLookup(
				new LookupDataReader<MeterPerSecond, double>(
						"ShiftStrategy IdleLow", new[] { Fields.Velocity, Fields.WeightingFactor })
					.Create(data,
						x => x.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(), 
						y => y.ParseDouble(Fields.WeightingFactor))
					.OrderBy(x => x.Key.Value())
					.ToArray());
		}

		
		public static class Fields
		{
			public const string Velocity = "velocity";
			public const string WeightingFactor = "weighting factor";
		}
	}
}
