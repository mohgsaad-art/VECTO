using System.Linq;
using System.Xml.Linq;

using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using VECTO3GUI2020.Util.XML.Interfaces;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public interface IXMLBusAuxiliariesWriter : IXMLComponentWriter
	{

	}

	public abstract class XMLBusAuxiliariesWriter : IXMLBusAuxiliariesWriter
	{
		protected readonly IBusAuxiliariesDeclarationData _inputData;
		protected XElement _xElement;

		protected XMLBusAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData)
		{
			_inputData = inputData;
		}

		public XElement GetElement()
		{
			if (_xElement == null)
			{
				Initialize();
				CreateElements();
			}

			return _xElement;
		}

		public XElement GetElement(XNamespace wrapperNamespace)
		{
			throw new System.NotImplementedException();
		}

		public abstract void Initialize();

		public abstract void CreateElements();
	}



	public class XMLBusAuxiliariesWriterMultistage : XMLBusAuxiliariesWriter
	{


		private XNamespace _defaultNamespace;
		private readonly IGroupWriterFactory _groupWriterFactory;

		public XMLBusAuxiliariesWriterMultistage(IBusAuxiliariesDeclarationData inputData,
			IGroupWriterFactory groupWriterFactory) : base(inputData)
		{
			_groupWriterFactory = groupWriterFactory;
		}

		#region Overrides of XMLBusAuxiliariesWriter

		public override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.v2_10_2; 
			_xElement = new XElement(_defaultNamespace + XMLNames.Component_Auxiliaries);
		}

		public override void CreateElements()
		{
			CreateElementsWithGroupWriters();
			return;
			

			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper, 
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, "CompletedVehicleAuxiliaryDataDeclarationType"));
			_xElement.Add(dataElement);


			if (_inputData.ElectricConsumers != null) {
				var electricSystemElement = new XElement(_defaultNamespace + XMLNames.BusAux_ElectricSystem);
				var ledLightsElement = new XElement(_defaultNamespace + "LEDLights");
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Interiorlights, _inputData.ElectricConsumers.InteriorLightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Dayrunninglights, _inputData.ElectricConsumers.DayrunninglightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Positionlights, _inputData.ElectricConsumers.PositionlightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Brakelights, _inputData.ElectricConsumers.BrakelightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Headlights, _inputData.ElectricConsumers.HeadlightsLED));
				electricSystemElement.Add(ledLightsElement);
				dataElement.Add(electricSystemElement);
			}

			if (_inputData.HVACAux != null) {
				var hvacElement = new XElement(_defaultNamespace + "HVAC");
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_SystemConfiguration, _inputData.HVACAux.SystemConfiguration.GetXmlFormat()));

				hvacElement.Add(GetHeatPumpGroupElement(
					xmlNameWrapper:XMLNames.Bus_HeatPumpTypeDriver,
					xmlNameFirstComponent:XMLNames.BusHVACHeatPumpCooling, 
					firstValue:_inputData.HVACAux.HeatPumpTypeCoolingDriverCompartment.GetLabel(),
					xmlNameSecondComponent:XMLNames.BusHVACHeatPumpHeating,
					secondValue:_inputData.HVACAux.HeatPumpTypeHeatingDriverCompartment.GetLabel()));

				hvacElement.Add(GetHeatPumpGroupElement(
					xmlNameWrapper: XMLNames.Bus_HeatPumpTypePassenger,
					xmlNameFirstComponent: XMLNames.BusHVACHeatPumpCooling,
					firstValue: _inputData.HVACAux.HeatPumpTypeCoolingPassengerCompartment.GetLabel(),
					xmlNameSecondComponent: XMLNames.BusHVACHeatPumpHeating,
					secondValue: _inputData.HVACAux.HeatPumpTypeHeatingPassengerCompartment.GetLabel()));


				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AuxiliaryHeaterPower, _inputData.HVACAux.AuxHeaterPower?.ToXMLFormat(0)));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_DoubleGlazing, _inputData.HVACAux.DoubleGlazing));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AdjustableAuxiliaryHeater, _inputData.HVACAux.AdjustableAuxiliaryHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_SeparateAirDistributionDucts, _inputData.HVACAux.SeparateAirDistributionDucts));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_WaterElectricHeater, _inputData.HVACAux.WaterElectricHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AirElectricHeater, _inputData.HVACAux.AirElectricHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_OtherHeatingTechnology, _inputData.HVACAux.OtherHeatingTechnology));
				dataElement.Add(hvacElement);
			}

			dataElement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
		}

		private void CreateElementsWithGroupWriters()
		{

			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper,
				new XAttribute("xmlns" , XMLNamespaces.v2_10_2),
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, "AUX_Conventional_CompletedBusType"));

			if (_inputData.ElectricConsumers != null) {
				var electricSystemElement = new XElement(_defaultNamespace + XMLNames.BusAux_ElectricSystem);
				var ledLightsElement = new XElement(_defaultNamespace + "LEDLights");
				ledLightsElement.Add(
					// ReSharper disable once CoVariantArrayConversion
					_groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxElectricSystemLightsGroup, _defaultNamespace)
						.GetGroupElements(_inputData));
				electricSystemElement.Add(ledLightsElement);
				dataElement.Add(electricSystemElement);

			}

			if (_inputData.HVACAux != null) {
				var hvacElement = new XElement(_defaultNamespace + "HVAC");
				hvacElement.Add(_groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACConventionalSequenceGroup, _defaultNamespace)
						.GetGroupElements(_inputData));
				dataElement.Add(hvacElement);
			}

			_xElement.Add(dataElement);
			
		}

		private XElement GetHeatPumpTypeElement(string xmlName, string value)
		{
			if (value == "~null~")
			{
				value = null;
			}
			return new XElement(_defaultNamespace + xmlName, value);
		}

		private XElement GetHeatPumpGroupElement(string xmlNameWrapper, string xmlNameFirstComponent,
			string xmlNameSecondComponent, string firstValue, string secondValue)
		{
			var element = new XElement(_defaultNamespace + xmlNameWrapper, 
				new XElement(_defaultNamespace + xmlNameFirstComponent, firstValue),
				new XElement(_defaultNamespace + xmlNameSecondComponent, secondValue));

			return element;
		}


		#endregion
	}
}