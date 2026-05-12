using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractInterimVIFGroupWriter : IReportMultistepCompletedBusOutputGroup
	{
		protected readonly IVIFReportInterimFactory _vifReportFactory;
		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";

		protected AbstractInterimVIFGroupWriter(IVIFReportInterimFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IReportMultistepCompletedBusOutputGroup

		public abstract IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider);

		#endregion
	}

	public class CompletedBusGeneralParametersGroup : AbstractInterimVIFGroupWriter
	{
		public CompletedBusGeneralParametersGroup(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractInterimVIFGroupWriter

		public override IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider)
		{
			var vehicleInputData = multiStageInputDataProvider.VehicleInputData;
			return new[] {
				new XElement(_v27 + XMLNames.Component_Manufacturer, vehicleInputData.Manufacturer),
				new XElement(_v27 + XMLNames.Component_ManufacturerAddress, vehicleInputData.ManufacturerAddress),
				new XElement(_v27 + XMLNames.Vehicle_VIN, vehicleInputData.VIN),
				new XElement(_v27 + XMLNames.Component_Date,
					XmlConvert.ToString(vehicleInputData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(_v27 + "SimulationToolLicenseNumber", vehicleInputData.SimulationToolLicenseNumber ?? "N/A"),
			};
		}

		#endregion
	}

	public class GetCompletedBusParametersGroup : AbstractInterimVIFGroupWriter
	{
		public GetCompletedBusParametersGroup(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractInterimVIFGroupWriter

		public override IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider)
		{
			var vehicleInputData = multiStageInputDataProvider.VehicleInputData;
			return new [] {
				vehicleInputData.Model != null
					? new XElement(_v27 + XMLNames.Component_Model, vehicleInputData.Model) : null,
				vehicleInputData.LegislativeClass != null
					? new XElement(_v27 + XMLNames.Vehicle_LegislativeCategory, vehicleInputData.LegislativeClass.ToXMLFormat()) : null,
				vehicleInputData.CurbMassChassis != null
					? new XElement(_v27 + XMLNames.CorrectedActualMass, vehicleInputData.CurbMassChassis.ToXMLFormat(0)) : null,
				vehicleInputData.GrossVehicleMassRating != null
					? new XElement(_v27 + XMLNames.TPMLM, vehicleInputData.GrossVehicleMassRating.ToXMLFormat(0)) : null,
				GetAirdragModifiedMultistageEntry(multiStageInputDataProvider),
				vehicleInputData.RegisteredClass != null
					? new XElement(_v27 + XMLNames.Vehicle_RegisteredClass, vehicleInputData.RegisteredClass.ToXMLFormat()) : null,
			};
		}

		protected XElement GetAirdragModifiedMultistageEntry(IMultistageVIFInputData multiStageInputDataProvider)
		{
			var consolidatedInputData = multiStageInputDataProvider.MultistageJobInputData.JobInputData
				.ConsolidateManufacturingStage;
			var vehicleInputData = multiStageInputDataProvider.VehicleInputData;
			if (consolidatedInputData?.Vehicle?.Components?.AirdragInputData == null)
				return null;

			switch (vehicleInputData.AirdragModifiedMultistep) {
				case null:
					throw new VectoException("AirdragModifiedMultistage must be set if an airdrag component has been set in previous stages.", ErrorCodes.VEHICLE_AIRDRAG_MODIFIED_NOT_SET);
				default:
					return new XElement(_v27 + XMLNames.Bus_AirdragModifiedMultistep, vehicleInputData.AirdragModifiedMultistep);
			}
		}
		#endregion
	}

	public class CompletedBusPassengerCountGroup : AbstractInterimVIFGroupWriter
	{
		public CompletedBusPassengerCountGroup(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractInterimVIFGroupWriter

		public override IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider)
		{
			var vehicleInputData = multiStageInputDataProvider.VehicleInputData;

			if (vehicleInputData.NumberPassengerSeatsLowerDeck == null ||
				vehicleInputData.NumberPassengersStandingLowerDeck == null ||
				vehicleInputData.NumberPassengerSeatsUpperDeck == null ||
				vehicleInputData.NumberPassengersStandingUpperDeck == null) {
				return new XElement[] {};
			}

			return new [] { 
				new XElement(_v27 + XMLNames.Bus_NumberPassengerSeatsLowerDeck, vehicleInputData.NumberPassengerSeatsLowerDeck),
				new XElement(_v27 + XMLNames.Bus_NumberPassengersStandingLowerDeck, vehicleInputData.NumberPassengersStandingLowerDeck),
				new XElement(_v27 + XMLNames.Bus_NumberPassengerSeatsUpperDeck, vehicleInputData.NumberPassengerSeatsUpperDeck),
				new XElement(_v27 + XMLNames.Bus_NumberPassengersStandingUpperDeck, vehicleInputData.NumberPassengersStandingUpperDeck),
			};
		}

		#endregion
	}

	public class CompletedBusDimensionsGroup : AbstractInterimVIFGroupWriter
	{
		public CompletedBusDimensionsGroup(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractInterimVIFGroupWriter

		public override IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider)
		{
			var vehicleInputData = multiStageInputDataProvider.VehicleInputData;

			if (vehicleInputData.Height == null || vehicleInputData.Length == null ||
				vehicleInputData.Width == null || vehicleInputData.EntranceHeight == null) {
				return new XElement[] { };
			}

			return new[] {
				new XElement(_v27 + XMLNames.Bus_HeightIntegratedBody, vehicleInputData.Height.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_v27 + XMLNames.Bus_VehicleLength, vehicleInputData.Length.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_v27 + XMLNames.Bus_VehicleWidth, vehicleInputData.Width.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_v27 + XMLNames.Bus_EntranceHeight, vehicleInputData.EntranceHeight.ConvertToMilliMeter().ToXMLFormat(0)),
			};
		}

		#endregion
	}
}