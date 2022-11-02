using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class AuxiliariesBusComponentData: IAuxiliariesBus, ITempDataObject<IAuxiliariesViewModel>
	{
		protected const int NotSelected = -1;


		#region IAuxiliariesBus Interface

		public ObservableCollectionEx<AlternatorTechnologyModel> AlternatorTechnologies { get; }
		public List<AlternatorTechnologyModel> OriginAlternatorTechnologies { get; private set; }
		public bool DayrunninglightsLED { get; set; }
		public bool HeadlightsLED { get; set; }
		public bool PositionlightsLED { get; set; }
		public bool BrakelightsLED { get; set; }
		public bool InteriorLightsLED { get; set; }
		public BusHVACSystemConfiguration SystemConfiguration { get; set; }
		public ACCompressorType CompressorTypeDriver { get; set; }
		public ACCompressorType CompressorTypePassenger { get; set; }
		public Watt AuxHeaterPower { get; set; }
		public bool DoubleGlasing { get; set; }
		public bool HeatPump { get; set; }
		public bool AdjustableAuxiliaryHeater { get; set; }
		public bool SeparateAirDistributionDucts { get; set; }
		public Dictionary<string, string> XmlNamesToPropertyMapping { get; private set; }

		#endregion

		public AuxiliariesBusComponentData(IAuxiliariesViewModel viewModel, bool defaultValues)
		{
			AlternatorTechnologies = new ObservableCollectionEx<AlternatorTechnologyModel>();
			if (defaultValues)
				ClearValues(viewModel);

			OriginAlternatorTechnologies = new List<AlternatorTechnologyModel>();
			SetXmlNamesToPropertyMapping();
		}

		public AuxiliariesBusComponentData(IAuxiliariesViewModel viewModel)
		{
			AlternatorTechnologies = new ObservableCollectionEx<AlternatorTechnologyModel>();
			SetValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public void UpdateCurrentValues(IAuxiliariesViewModel viewModel)
		{
			SetValues(viewModel);
		}

		public void ResetToComponentValues(IAuxiliariesViewModel viewModel)
		{
			SetAlternatorTechnology(viewModel.AlternatorTechnologies);
			viewModel.DayrunninglightsLED = DayrunninglightsLED;
			viewModel.HeadlightsLED = HeadlightsLED;
			viewModel.PositionlightsLED = PositionlightsLED;
			viewModel.BrakelightsLED = BrakelightsLED;
			viewModel.InteriorLightsLED = InteriorLightsLED;
			viewModel.SystemConfiguration = SystemConfiguration;
			viewModel.CompressorTypeDriver = CompressorTypeDriver;
			viewModel.CompressorTypePassenger = CompressorTypePassenger;
			viewModel.AuxHeaterPower = AuxHeaterPower;
			viewModel.DoubleGlasing = DoubleGlasing;
			viewModel.HeatPump = HeatPump;
			viewModel.AdjustableAuxiliaryHeater = AdjustableAuxiliaryHeater;
			viewModel.SeparateAirDistributionDucts = SeparateAirDistributionDucts;
		}

		private void SetAlternatorTechnology(ObservableCollectionEx<AlternatorTechnologyModel> res)
		{
			if (OriginAlternatorTechnologies == null)
				return;

			res.Clear();
			for (int i = 0; i < OriginAlternatorTechnologies.Count; i++) {
				res.Add(new AlternatorTechnologyModel{AlternatorTechnology = OriginAlternatorTechnologies[i].AlternatorTechnology});
			}
		}
		
		public void ClearValues(IAuxiliariesViewModel viewModel)
		{
			viewModel.AlternatorTechnologies.Clear(); 
			viewModel.DayrunninglightsLED = default(bool);
			viewModel.HeadlightsLED = default(bool);
			viewModel.PositionlightsLED = default(bool);
			viewModel.BrakelightsLED = default(bool);
			viewModel.InteriorLightsLED = default(bool);
			viewModel.SystemConfiguration = (BusHVACSystemConfiguration)NotSelected;
			viewModel.CompressorTypeDriver = (ACCompressorType)NotSelected;
			viewModel.CompressorTypePassenger = (ACCompressorType)NotSelected;
			viewModel.AuxHeaterPower = default(Watt);
			viewModel.DoubleGlasing = default(bool);
			viewModel.HeatPump = default(bool);
			viewModel.AdjustableAuxiliaryHeater = default(bool);
			viewModel.SeparateAirDistributionDucts = default(bool);
		}
		
		private void SetValues(IAuxiliariesViewModel auxiliaries)
		{
			OriginAlternatorTechnologies = GetOriginAlternatorTechnologies(auxiliaries);
			SetAlternatorTechnology(AlternatorTechnologies);
			DayrunninglightsLED = auxiliaries.DayrunninglightsLED;
			HeadlightsLED = auxiliaries.HeadlightsLED;
			PositionlightsLED = auxiliaries.PositionlightsLED;
			BrakelightsLED = auxiliaries.BrakelightsLED;
			InteriorLightsLED = auxiliaries.InteriorLightsLED;
			SystemConfiguration = auxiliaries.SystemConfiguration;
			CompressorTypeDriver = auxiliaries.CompressorTypeDriver;
			CompressorTypePassenger = auxiliaries.CompressorTypePassenger;
			AuxHeaterPower = auxiliaries.AuxHeaterPower;
			DoubleGlasing = auxiliaries.DoubleGlasing;
			HeatPump = auxiliaries.HeatPump;
			AdjustableAuxiliaryHeater = auxiliaries.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = auxiliaries.SeparateAirDistributionDucts;
		}

		private List<AlternatorTechnologyModel> GetOriginAlternatorTechnologies(IAuxiliariesViewModel auxiliaries)
		{
			if (auxiliaries.AlternatorTechnologies == null)
				return null;

			var result = new List<AlternatorTechnologyModel>();
			for (int i = 0; i < auxiliaries.AlternatorTechnologies.Count; i++) {
				result.Add(new AlternatorTechnologyModel {
					AlternatorTechnology =  auxiliaries.AlternatorTechnologies[i].AlternatorTechnology
				});
			}
			return result;
		}
		
		private void SetXmlNamesToPropertyMapping()
		{
			XmlNamesToPropertyMapping = new Dictionary<string, string> {
				{XMLNames.BusAux_ElectricSystem, nameof(AlternatorTechnologies)},
				{XMLNames.Bus_Dayrunninglights, nameof(DayrunninglightsLED)},
				{XMLNames.Bus_Headlights, nameof(HeadlightsLED)},
				{XMLNames.Bus_Positionlights, nameof(PositionlightsLED)},
				{XMLNames.Bus_Brakelights, nameof(BrakelightsLED)},
				{XMLNames.Bus_Interiorlights, nameof(InteriorLightsLED)},
				{XMLNames.Bus_SystemConfiguration, nameof(SystemConfiguration)},
				{XMLNames.Bus_DriverAC, nameof(CompressorTypeDriver)},
				{XMLNames.Bus_PassengerAC, nameof(CompressorTypePassenger)},
				{XMLNames.Bus_AuxiliaryHeaterPower, nameof(AuxHeaterPower)},
				{XMLNames.Bus_DoubleGlazing, nameof(DoubleGlasing)},
				{XMLNames.Bus_HeatPump, nameof(HeatPump)},
				{XMLNames.Bus_AdjustableAuxiliaryHeater, nameof(AdjustableAuxiliaryHeater)},
				{XMLNames.Bus_SeparateAirDistributionDucts, nameof(SeparateAirDistributionDucts)},
			};
		}
	}
}
