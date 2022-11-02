using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONHybridStrategyParametersParallelHybrid : JSONFile, IHybridStrategyParameters
	{
		public JSONHybridStrategyParametersParallelHybrid(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{ }

		public double EquivalenceFactorDischarge =>
			Body["EquivalenceFactor"] == null ? Body.GetEx<double>("EquivalenceFactorDischarge") :
				Body.GetEx<double>("EquivalenceFactor");

		public double EquivalenceFactorCharge =>
			Body["EquivalenceFactor"] == null ? Body.GetEx<double>("EquivalenceFactorCharge") :
				Body.GetEx<double>("EquivalenceFactor");

		public double MinSoC => Body.GetEx<double>("MinSoC") / 100.0;

		public double MaxSoC => Body.GetEx<double>("MaxSoC") / 100.0;

		public double TargetSoC => Body.GetEx<double>("TargetSoC") / 100.0;

		public Second MinimumICEOnTime => Body.GetEx<double>("MinICEOnTime").SI<Second>();

		public Second AuxBufferTime => Body.GetEx<double>("AuxBufferTime").SI<Second>();

		public Second AuxBufferChargeTime => Body.GetEx<double>("AuxBufferChgTime").SI<Second>();

		public double ICEStartPenaltyFactor => Body["ICEStartPenaltyFactor"] == null ? 0 : Body.GetEx<double>("ICEStartPenaltyFactor");

		public double CostFactorSOCExpponent => Body["CostFactorSOCExponent"] == null ? double.NaN : Body.GetEx<double>("CostFactorSOCExponent");
		public double GensetMinOptPowerFactor => double.NaN;
	}

	// ---------------------

	public class JSONHybridStrategyParametersSerialHybrid : JSONFile, IHybridStrategyParameters
	{
		public JSONHybridStrategyParametersSerialHybrid(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		#region Implementation of IHybridStrategyParameters

		public double EquivalenceFactorDischarge => double.NaN;
		public double EquivalenceFactorCharge => double.NaN;
		public double MinSoC => Body.GetEx<double>("MinSoC") / 100.0;

		public double MaxSoC => double.NaN;
		public double TargetSoC => Body.GetEx<double>("TargetSoC") / 100.0;
		public Second MinimumICEOnTime => null;
		public Second AuxBufferTime => null;
		public Second AuxBufferChargeTime => null;
		public double ICEStartPenaltyFactor => double.NaN;
		public double CostFactorSOCExpponent => double.NaN;
		public double GensetMinOptPowerFactor => 0; //Body.GetEx<double>("GensetMinOptPowerFactor");
		
		#endregion
	}
}