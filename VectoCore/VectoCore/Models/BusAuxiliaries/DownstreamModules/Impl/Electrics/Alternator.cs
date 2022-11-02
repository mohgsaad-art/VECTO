using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	// Model based on CombinedALTS_V02_Editable.xlsx
	public class Alternator : IAlternator
	{
		//private ICombinedAlternatorSignals signals;

		// Constructors

		public Alternator(List<ICombinedAlternatorMapRow> inputs)
		{
			AlternatorName = inputs.First().AlternatorName;
			PulleyRatio = inputs.First().PulleyRatio;

			var values2k = inputs.Where(x => x.RPM.AsRPM.IsEqual(2000))
								.Select(x => new KeyValuePair<Ampere, double>(x.Amps, x.Efficiency))
								.ToDictionary(x => x.Key, x => x.Value);
			var values4k = inputs.Where(x => x.RPM.AsRPM.IsEqual(4000))
								.Select(x => new KeyValuePair<Ampere, double>(x.Amps, x.Efficiency))
								.ToDictionary(x => x.Key, x => x.Value);
			var values6k = inputs.Where(x => x.RPM.AsRPM.IsEqual(6000))
								.Select(x => new KeyValuePair<Ampere, double>(x.Amps, x.Efficiency))
								.ToDictionary(x => x.Key, x => x.Value);

			BuildInputTable(values2k, InputTable2000);
			BuildInputTable(values4k, InputTable4000);
			BuildInputTable(values6k, InputTable6000);

			CreateRangeTable();
		}

		// D6
		public string AlternatorName { get; set; }

		// G6
		public double PulleyRatio { get; set; }

		// C10-D15
		public List<AltUserInput<Ampere>> InputTable2000 { get; set; } = new List<AltUserInput<Ampere>>();

		// F10-G15
		public List<AltUserInput<Ampere>> InputTable4000 { get; set; } = new List<AltUserInput<Ampere>>();

		// I10-J15
		public List<AltUserInput<Ampere>> InputTable6000 { get; set; } = new List<AltUserInput<Ampere>>();

		// M10-N15
		public List<Table4Row> RangeTable { get; set; } = new List<Table4Row>();

		// S9
		//public PerSecond SpindleSpeed
		//{
		//	get { return signals.CrankRPM * PulleyRatio; }
		//}

		// S10
		public double GetEfficiency(PerSecond crankSpeed, Ampere currentDemand)
		{
			// First build RangeTable, table 4
				InitialiseRangeTable();
				CalculateRangeTable(currentDemand);

				// Calculate ( Interpolate ) Efficiency
				var range = RangeTable.Select(s => new AltUserInput<PerSecond>(s.RPM, s.Efficiency)).ToList();

				return Iterpolate(range, crankSpeed * PulleyRatio);
			
		}


		

		public static double Iterpolate<T>(List<AltUserInput<T>> values, T x) where T:SI
		{
			var lowestX = values.Min(m => m.Amps);
			var highestX = values.Max(m => m.Amps);

			// Out of range, returns efficiency for lowest
			if (x < lowestX)
				return values.First(f => f.Amps == lowestX).Eff;

			// Out of range, efficiency for highest
			if (x > highestX)
				return values.First(f => f.Amps == highestX).Eff;

			// On Bounds check
			if (values.Count(w => w.Amps == x) == 1)
				return values.First(w => w.Amps == x).Eff;

			// OK, we need to interpolate.
			var preKey = values.Last(l => l.Amps < x).Amps;
			var postKey = values.First(l => l.Amps > x).Amps;
			var preEff = values.First(f => f.Amps == preKey).Eff;
			var postEff = values.First(f => f.Amps == postKey).Eff;

			var deltaX = postKey - preKey;
			var deltaEff = postEff - preEff;

			
			var retVal = ((x - preKey) / deltaX).Value() * deltaEff + preEff;

			return retVal;
		}

		private void CalculateRangeTable(Ampere currentDemand)
		{
			// M10=Row0-Rpm - N10=Row0-Eff
			// M11=Row1-Rpm - N11=Row1-Eff
			// M12=Row2-Rpm - N12=Row2-Eff - 2000
			// M13=Row3-Rpm - N13=Row3-Eff - 4000
			// M14=Row4-Rpm - N14=Row4-Eff - 6000
			// M15=Row5-Rpm - N15=Row5-Eff
			// M16=Row6-Rpm - N16=Row6-Eff

			// EFFICIENCY

			// 2000
			var N12 = Iterpolate(InputTable2000, currentDemand);
			RangeTable[2].Efficiency = N12;

			// 4000
			var N13 = Iterpolate(InputTable4000, currentDemand);
			RangeTable[3].Efficiency = N13;

			// 6000
			var N14 = Iterpolate(InputTable6000, currentDemand);
			RangeTable[4].Efficiency = N14;

			// Row0 & Row1 Efficiency  =IF(N13>N12,0,MAX(N12:N14)) - Example Alt 1 N13=
			var N11 = N13 > N12 ? 0 : Math.Max(Math.Max(N12, N13), N14);
			RangeTable[1].Efficiency = N11;
			var N10 = N11;
			RangeTable[0].Efficiency = N10;

			// Row 5 Efficiency
			var N15 = N13 > N14 ? 0 : Math.Max(Math.Max(N12, N13), N14);
			RangeTable[5].Efficiency = N15;

			// Row 6 - Efficiency
			var N16 = N15;
			RangeTable[6].Efficiency = N16;

			// RPM

			// 2000 Row 2 - RPM
			var M12 = 2000.RPMtoRad();
			RangeTable[2].RPM = M12;

			// 4000 Row 3 - RPM
			var M13 = 4000.RPMtoRad();
			RangeTable[3].RPM = M13;

			// 6000 Row 4 - RPM
			var M14 = 6000.RPMtoRad();
			RangeTable[4].RPM = M14;

			// Row 1 - RPM
			// NOTE: Update to reflect CombineALternatorSchematicV02 20150429
			// IF(M12=IF(N12>N13,M12-((M12-M13)/(N12-N13))*(N12-N11),M12-((M12-M13)/(N12-N13))*(N12-N11)), M12-0.01, IF(N12>N13,M12-((M12-M13)/(N12-N13))*(N12-N11),M12-((M12-M13)/(N12-N13))*(N12-N11)))
			var M11 = N12 - N13 == 0
						? 0.RPMtoRad()
						: (
							M12 == (N12 > N13
								? M12 - (M12 - M13) / (N12 - N13) * (N12 - N11)
								: M12 - (M12 - M13) / (N12 - N13) * (N12 - N11))
								? M12 - 0.01.RPMtoRad()
								: (N12 > N13
									? M12 - (M12 - M13) / (N12 - N13) * (N12 - N11)
									: M12 - (M12 - M13) / (N12 - N13) * (N12 - N11)));

			RangeTable[1].RPM = M11;

			// Row 0 - RPM
			var M10 = M11 < 1500 ? M11 - 1.RPMtoRad() : 1500.RPMtoRad();
			RangeTable[0].RPM = M10;

			// Row 5 - RPM
			var M15 = M14 == (N14 == 0 || N14 == N13
						? M14 + 1.RPMtoRad()
						: (N13 > N14 ? (M14 - M13) / (N13 - N14) * N14 + M14 : (M14 - M13) / (N13 - N14) * (N14 - N15) + M14)
					)
						? M14 + 0.01.RPMtoRad()
						: (N14 == 0 || N14 == N13
							? M14 + 1.RPMtoRad()
							: (N13 > N14 ? (M14 - M13) / (N13 - N14) * N14 + M14 : (M14 - M13) / (N13 - N14) * (N14 - N15) + M14));

			RangeTable[5].RPM = M15;

			// Row 6 - RPM
			var M16 = M15 > 10000 ? M15 + 1.RPMtoRad() : 10000.RPMtoRad();
			RangeTable[6].RPM = M16;
		}

		private void InitialiseRangeTable()
		{
			RangeTable[0].RPM = 0.RPMtoRad();
			RangeTable[0].Efficiency = 0;
			RangeTable[1].RPM = 0.RPMtoRad();
			RangeTable[1].Efficiency = 0;
			RangeTable[2].RPM = 2000.RPMtoRad();
			RangeTable[2].Efficiency = 0;
			RangeTable[3].RPM = 4000.RPMtoRad();
			RangeTable[3].Efficiency = 0;
			RangeTable[4].RPM = 6000.RPMtoRad();
			RangeTable[4].Efficiency = 0;
			RangeTable[5].RPM = 0.RPMtoRad();
			RangeTable[5].Efficiency = 0;
			RangeTable[6].RPM = 0.RPMtoRad();
			RangeTable[6].Efficiency = 0;
		}

		private void CreateRangeTable()
		{
			RangeTable.Clear();

			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
			RangeTable.Add(new Table4Row(0.RPMtoRad(), 0));
		}

		public void BuildInputTable(Dictionary<Ampere, double> inputs, List<AltUserInput<Ampere>> targetTable)
		{
			targetTable.Clear();

			// Row0
			var D14 = 0.0;
			targetTable.Add(new AltUserInput<Ampere>(0.SI<Ampere>(), D14));

			// Row1
			targetTable.Add(new AltUserInput<Ampere>(inputs.OrderBy(x => x.Key).First().Key, inputs.OrderBy(x => x.Key).First().Value));

			// Row2
			targetTable.Add(
				new AltUserInput<Ampere>(inputs.OrderBy(x => x.Key).Skip(1).First().Key, inputs.OrderBy(x => x.Key).Skip(1).First().Value));

			// Row3
			targetTable.Add(
				new AltUserInput<Ampere>(inputs.OrderBy(x => x.Key).Skip(2).First().Key, inputs.OrderBy(x => x.Key).Skip(2).First().Value));

			var C11 = targetTable[1].Amps;
			var C12 = targetTable[2].Amps;
			var C13 = targetTable[3].Amps;

			var D11 = targetTable[1].Eff;
			var D12 = targetTable[2].Eff;
			var D13 = targetTable[3].Eff;

			D14 = D12 > D13 ? 0 : Math.Max(Math.Max(D11, D12), D13);

			// Row4  - Eff
			targetTable.Add(new AltUserInput<Ampere>(0.SI<Ampere>(), D14));

			// Row4  - Amps
			// Should probably refactor this into some sort of helper/extension method
			var numarray = new[] { D11, D12, D13 };
			var maxD11_D13 = numarray.Max();

			// =IF(OR(D13=0,D13=D12),C13+1,IF(D12>D13,((((C13-C12)/(D12-D13))*D13)+C13),((((C13-C12)/(D12-D13))*(D13-D14))+C13)))
			var C14 = (D13 == 0 || D13 == D12 || D13 == maxD11_D13)
				? C13 + 1.SI<Ampere>()
				: D12 > D13
					? ((((C13 - C12) / (D12 - D13)) * D13) + C13)
					: ((((C13 - C12) / (D12 - D13)) * (D13 - D14)) + C13);
			targetTable[4].Amps = C14;

			// Row5 
			var C15 = C14 > 200 ? C14 + 1.SI<Ampere>() : 200.SI<Ampere>();
			var D15 = D14;
			targetTable.Add(new AltUserInput<Ampere>(C15, D15));

			// Row0
			targetTable[0].Eff = D11;
		}

		public bool IsEqualTo(IAlternator other)
		{
			if (AlternatorName != other.AlternatorName) {
				return false;
			}
			if (PulleyRatio != other.PulleyRatio) {
				return false;
			}

			for (var i = 1; i <= 3; i++) {
				if (InputTable2000[i].Eff != other.InputTable2000[i].Eff)
					return false;
				if (InputTable4000[i].Eff != other.InputTable4000[i].Eff)
					return false;
				if (InputTable6000[i].Eff != other.InputTable6000[i].Eff)
					return false;
			}

			return true;
		}
	}
}
