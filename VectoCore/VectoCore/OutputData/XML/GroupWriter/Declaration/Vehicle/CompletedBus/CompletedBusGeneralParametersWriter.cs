using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    public class CompletedBusGeneralParametersWriterV2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
    {
		public CompletedBusGeneralParametersWriterV2_10_2(XNamespace writerNamespace) : base(writerNamespace)
		{

		}

		#region Overrides of GroupWriter

		public XElement[] GetGroupElements(IVehicleDeclarationInputData inputData)
		{
			return GetGroupElements(inputData, _writerNamespace);
		}

		public static XElement[] GetGroupElements(IVehicleDeclarationInputData inputData, XNamespace writerNamespace)
		{
			return new XElement[] {
				new XElement(writerNamespace + XMLNames.Component_Manufacturer, inputData.Manufacturer),
				new XElement(writerNamespace + XMLNames.Component_ManufacturerAddress,
					inputData.ManufacturerAddress),
				new XElement(writerNamespace + XMLNames.Vehicle_VIN, inputData.VIN),
				new XElement(writerNamespace + XMLNames.Component_Date, inputData.Date.ToXmlFormat())
			};
		}

		#endregion
	}
}
