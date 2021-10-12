using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	public class Table4Row
	{
		public PerSecond RPM;
		public double Efficiency;

		public Table4Row(PerSecond rpm, double eff)
		{
			RPM = rpm;
			Efficiency = eff;
		}
	}

	// Used By Combined Alternator.
	// Model based on CombinedALTS_V02_Editable.xlsx
	public interface IAlternator
	{
		// D6
		string AlternatorName { get; set; }

		// G6
		double PulleyRatio { get; set; }

		
		// S10
		double GetEfficiency(PerSecond crankSpeed, Ampere currentDemand);

		// C10-D15
		List<AltUserInput<Ampere>> InputTable2000 { get; set; }

		// F10-G15
		List<AltUserInput<Ampere>> InputTable4000 { get; set; }

		// I10-J15
		List<AltUserInput<Ampere>> InputTable6000 { get; set; }

		// M10-N15
		List<Table4Row> RangeTable { get; set; }

		// Test Equality
		bool IsEqualTo(IAlternator other);
	}
}
