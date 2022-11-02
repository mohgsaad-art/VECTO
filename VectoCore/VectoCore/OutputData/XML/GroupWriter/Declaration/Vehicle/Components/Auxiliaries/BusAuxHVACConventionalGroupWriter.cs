using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    public class BusAuxHVACConventionalGroupWriter_v2_10_2 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		private readonly IGroupWriterFactory _groupWriterFactory;

		public BusAuxHVACConventionalGroupWriter_v2_10_2(XNamespace writerNamespace,
			IGroupWriterFactory groupWriterFactory) : base(writerNamespace)
		{
			_groupWriterFactory = groupWriterFactory;
		}

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			var elements = new List<XElement>();

			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_SystemConfiguration, 
				aux.HVACAux.SystemConfiguration.GetXmlFormat()));

			elements.AddRange(_groupWriterFactory
				.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACHeatPumpSequenceGroup, _writerNamespace)
				.GetGroupElements(aux));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_AuxiliaryHeaterPower,
				aux.HVACAux.AuxHeaterPower?.ToXMLFormat(0)));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_DoubleGlazing, aux.HVACAux.DoubleGlazing));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_AdjustableAuxiliaryHeater,
				aux.HVACAux.AdjustableAuxiliaryHeater));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_SeparateAirDistributionDucts,
				aux.HVACAux.SeparateAirDistributionDucts));

			return elements.ToArray();
		}

		#endregion
	}
}
