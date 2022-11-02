using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxElectricSystemLightsGroupWriter_v2_10_2 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxElectricSystemLightsGroupWriter_v2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Overrides of GroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			return new XElement[] {
				new XElement(_writerNamespace + XMLNames.Bus_Interiorlights, aux.ElectricConsumers.InteriorLightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Dayrunninglights,
					aux.ElectricConsumers.DayrunninglightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Positionlights, aux.ElectricConsumers.PositionlightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Brakelights, aux.ElectricConsumers.BrakelightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Headlights, aux.ElectricConsumers.HeadlightsLED),
			};
		}

		#endregion
	}
}
