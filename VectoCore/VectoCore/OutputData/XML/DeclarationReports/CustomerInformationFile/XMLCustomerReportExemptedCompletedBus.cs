using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile
{
	public class XMLCustomerReportExemptedCompletedBus : XMLCustomerReportCompletedBus
	{
		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleCompletedExemptedBusType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(
					tns + "PrimaryVehicle",
					new XElement(tns + XMLNames.Component_Manufacturer, PrimaryVehicle.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress, PrimaryVehicle.ManufacturerAddress),
					new XElement(tns + XMLNames.Report_InputDataSignature,
						PrimaryVehicleRecordFile.PrimaryVehicleInputDataHash.ToXML(di)),
					new XElement(tns + "ManufacturerRecordSignature",
						PrimaryVehicleRecordFile.ManufacturerRecordHash.ToXML(di))
				),
				new XElement(
					tns + "CompletedVehicle",

					new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress,
						modelData.VehicleData.ManufacturerAddress)
				),

				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + "VehicleCategory", modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup,
					modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_RegisteredClass,
					modelData.VehicleData.RegisteredClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_VehicleCode, modelData.VehicleData.VehicleCode.ToXMLFormat()),

				new XElement(
					tns + XMLNames.TPMLM,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis,
					XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),

				new XElement(tns + XMLNames.Vehicle_AxleConfiguration,
					modelData.VehicleData.AxleConfiguration.GetName()),
				//new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
				//new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
				new XElement(
					tns + "RegisteredPassengers",
					modelData.VehicleData.InputData.NumberPassengerSeatsLowerDeck +
					modelData.VehicleData.InputData.NumberPassengerSeatsUpperDeck
				),
				new XElement(tns + XMLNames.Bus_LowEntry, modelData.VehicleData.InputData.LowEntry),
				new XElement(tns + XMLNames.Bus_HeightIntegratedBody,
					modelData.VehicleData.InputData.Height.ToXMLFormat(3))
			);
			VehiclePart.Add(
				new XElement(tns + "SumNetPower", XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W))
			);

			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));

			Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));
		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			// no results for exempted
		}

	}
}