using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxHVACHeatPumpWriter_v2_10_2 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxHVACHeatPumpWriter_v2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			return new XElement[] {
				GetHeatPumpGroupElement(
					xmlNameWrapper: XMLNames.Bus_HeatPumpTypeDriver,
					xmlNameFirstComponent: XMLNames.BusHVACHeatPumpCooling,
					firstValue: aux.HVACAux.HeatPumpTypeCoolingDriverCompartment.GetLabel(),
					xmlNameSecondComponent: XMLNames.BusHVACHeatPumpHeating,
					secondValue: aux.HVACAux.HeatPumpTypeHeatingDriverCompartment.GetLabel()),

				GetHeatPumpGroupElement(
					xmlNameWrapper: XMLNames.Bus_HeatPumpTypePassenger,
					xmlNameFirstComponent: XMLNames.BusHVACHeatPumpCooling,
					firstValue: aux.HVACAux.HeatPumpTypeCoolingPassengerCompartment.GetLabel(),
					xmlNameSecondComponent: XMLNames.BusHVACHeatPumpHeating,
					secondValue: aux.HVACAux.HeatPumpTypeHeatingPassengerCompartment.GetLabel())
			};

		}

		#endregion

		private XElement GetHeatPumpTypeElement(string xmlName, string value)
		{
			if (value == "~null~")
			{
				value = null;
			}
			return new XElement(_writerNamespace + xmlName, value);
		}

		private XElement GetHeatPumpGroupElement(string xmlNameWrapper, string xmlNameFirstComponent,
			string xmlNameSecondComponent, string firstValue, string secondValue)
		{
			var element = new XElement(_writerNamespace + xmlNameWrapper,
				GetHeatPumpTypeElement(xmlNameFirstComponent, firstValue),
				GetHeatPumpTypeElement(xmlNameSecondComponent, secondValue));

			return element;
		}


	}
}
