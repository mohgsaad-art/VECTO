using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusPassengerCountWriter_V2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
	{
		public CompletedBusPassengerCountWriter_V2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }




		public static XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle, XNamespace writerNamespace)
		{
			if (vehicle.IsAnyNull(vehicle.NumberPassengersStandingLowerDeck, vehicle.NumberPassengersStandingUpperDeck,
				vehicle.NumberPassengerSeatsLowerDeck, vehicle.NumberPassengerSeatsUpperDeck)) {
				return new XElement[0];
			}
			return new XElement[] {
				new XElement(writerNamespace + XMLNames.Bus_NumberPassengerSeatsLowerDeck,
					vehicle.NumberPassengerSeatsLowerDeck),

				new XElement(writerNamespace + XMLNames.Bus_NumberPassengersStandingLowerDeck,
					vehicle.NumberPassengersStandingLowerDeck),

				new XElement(writerNamespace + XMLNames.Bus_NumberPassengerSeatsUpperDeck,
					vehicle.NumberPassengerSeatsUpperDeck),

				new XElement(writerNamespace + XMLNames.Bus_NumberPassengersStandingUpperDeck,
					vehicle.NumberPassengersStandingUpperDeck),
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
