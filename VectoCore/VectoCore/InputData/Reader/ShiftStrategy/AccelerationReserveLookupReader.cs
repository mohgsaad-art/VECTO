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
	public class AccelerationReserveLookupReader
	{
		public static IAccelerationReserveLookup ReadFromStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static IAccelerationReserveLookup ReadFromFile(string filename)
		{
			try {
				var data = VectoCSVFile.Read(filename);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException("Error while reading AccelerationReserve Lookup File: " + e.Message);
			}
		}

		public static IAccelerationReserveLookup Create(TableData data)
		{
			if (data == null) {
				return null;
			}
			return new AccelerationReserveLookup(
				new LookupDataReader<MeterPerSecond, Tuple<MeterPerSquareSecond, MeterPerSquareSecond>>(
						"AcceleationReserve", new[] { Fields.Velocity, Fields.AccelerationReserveLow, Fields.AccelerationReserveHigh }, 3)
					.Create(
						data,
						x => x.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(),
						y => Tuple.Create(
							y.ParseDouble(Fields.AccelerationReserveLow).SI<MeterPerSquareSecond>(),
							y.ParseDouble(Fields.AccelerationReserveHigh).SI<MeterPerSquareSecond>()))
					.OrderBy(x => x.Key.Value()).ToArray());
		}

		public static class Fields
		{
			public const string Velocity = "v";
			public const string AccelerationReserveLow = "a_rsv_low";
			public const string AccelerationReserveHigh = "a_rsv_high";
		}
	}
}
