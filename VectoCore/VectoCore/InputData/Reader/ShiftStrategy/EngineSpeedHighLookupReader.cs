using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public class EngineSpeedHighLookupReader
	{
		public static IEngineSpeedHighFactorLookup ReadFromStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static IEngineSpeedHighFactorLookup Create(TableData data)
		{
			if (data == null) {
				return null;
			}
			return new EngineSpeedHighFactorLookup(
				new LookupDataReader<double, double>("EngineSpeedHighFactor", new[] { Fields.TorqueRatio, Fields.SpeedFactor })
					.Create(
						data,
						x => x.ParseDouble(Fields.TorqueRatio), y => y.ParseDouble(Fields.SpeedFactor)).OrderBy(x => x.Key).ToArray());
		}

		private class Fields
		{
			public static string TorqueRatio = "T_card_dem/T_card_max";
			public static string SpeedFactor = "ratio_w_eng_max";
		}
	}
}
