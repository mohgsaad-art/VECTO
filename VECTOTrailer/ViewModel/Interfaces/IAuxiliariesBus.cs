using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Helper;
using VECTOTrailer.Model;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface IAuxiliariesBus
	{
		#region Electric System

		ObservableCollectionEx<AlternatorTechnologyModel> AlternatorTechnologies { get; /*set; */ }
		bool DayrunninglightsLED { get; set; }
		bool HeadlightsLED { get; set; }
		bool PositionlightsLED { get; set; }
		bool BrakelightsLED { get; set; }
		bool InteriorLightsLED { get; set; }

		#endregion

		#region Havac

		BusHVACSystemConfiguration SystemConfiguration { get; set; }
		ACCompressorType CompressorTypeDriver { get; set; }
		ACCompressorType CompressorTypePassenger { get; set; }
		Watt AuxHeaterPower { get; set; }
		bool DoubleGlasing { get; set; }
		bool HeatPump { get; set; }
		bool AdjustableAuxiliaryHeater { get; set; }
		bool SeparateAirDistributionDucts { get; set; }

		#endregion

		Dictionary<string, string> XmlNamesToPropertyMapping { get; }
	}
}
