using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public interface IMrfAirdragType
	{
		XElement GetXmlType(IVehicleDeclarationInputData vehicle, IAirdragDeclarationInputData inputData);


	}
	public class MRFAirdragType : AbstractMrfXmlType, IMrfAirdragType
	{

		public MRFAirdragType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfAirdragType

		public XElement GetXmlType(IVehicleDeclarationInputData vehicle, IAirdragDeclarationInputData inputData)
		{
			if (inputData == null || inputData.AirDragArea == null) {
				Segment? segment = null;
				if (vehicle.VehicleCategory.IsLorry()) {
					segment = DeclarationData.GetTruckSegment(vehicle).Segment;
                }
				if (vehicle.VehicleCategory.IsBus()) {
					segment = vehicle.VehicleCategory.IsCompletedBus()
						? DeclarationData.CompletedBusSegments.Lookup(
							vehicle.AxleConfiguration.NumAxles(), 
							vehicle.VehicleCode, 
							vehicle.RegisteredClass, 
							vehicle.NumberPassengerSeatsLowerDeck,
							vehicle.Height,
							vehicle.LowEntry)
                        : DeclarationData.PrimaryBusSegments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated);
				}

				if (segment == null) {
					return null;
				}
				return new XElement(_mrf + XMLNames.Component_AirDrag,
					new XElement(_mrf + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(_mrf + "CdxA", segment.Value.Missions.First().DefaultCDxA.ToXMLFormat(2)));
            }
			return new XElement(_mrf + XMLNames.Component_AirDrag,
				new XElement(_mrf + XMLNames.Component_Model, inputData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationMethod, inputData.CertificationMethod),
				inputData.CertificationNumber.IsNullOrEmpty()
					? null
					: new XElement(_mrf + XMLNames.Component_CertificationNumber, inputData.CertificationNumber),
				new XElement(_mrf + "CdxA", inputData.AirDragArea.ToXMLFormat(2)),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, inputData.DigestValue?.DigestValue ?? ""));
		}

		#endregion
    }


}
