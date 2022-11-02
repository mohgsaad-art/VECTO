using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusDimensionsWriter_V2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
    {
		public CompletedBusDimensionsWriter_V2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		public static XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle, XNamespace writerNamespace)
		{
			if (vehicle.IsAnyNull(vehicle.Height, vehicle.Length, vehicle.Width, vehicle.EntranceHeight)) {
				return new XElement[0];
			}
			return new XElement[] {
				new XElement(writerNamespace + XMLNames.Bus_HeightIntegratedBody,
					vehicle.Height.ConvertToMilliMeter()),
				new XElement(writerNamespace + XMLNames.Bus_VehicleLength, vehicle.Length.ConvertToMilliMeter()),
				new XElement(writerNamespace + XMLNames.Bus_VehicleWidth, vehicle.Width.ConvertToMilliMeter()),
				new XElement(writerNamespace + XMLNames.Bus_EntranceHeight,
					vehicle.EntranceHeight.ConvertToMilliMeter())
			};
		}

		#region Implementation of IVehicleDeclarationGroupWriter

		public XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle)
		{
			return GetGroupElements(vehicle, _writerNamespace);
		}

		#endregion
	}
}
