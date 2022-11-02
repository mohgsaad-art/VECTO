using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public class XMLManufacturerReportExemptedCompletedBus : XMLManufacturerReportCompletedBus
	{
		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleExemptedCompletedBusType"),
				GetPrimaryVehicleInformation(),
				new XElement(
					tns + "CompletedVehicle",
					new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
					new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress,
						modelData.VehicleData.ManufacturerAddress),
					new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
					new XElement(tns + XMLNames.Vehicle_LegislativeCategory,
						modelData.VehicleData.LegislativeClass.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup,
						modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Vehicle_RegisteredClass,
						modelData.VehicleData.RegisteredClass.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_BodyworkCode, modelData.VehicleData.VehicleCode.ToXMLFormat()),
					new XElement(tns + XMLNames.CorrectedActualMass,
						XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
					new XElement(tns + XMLNames.TPMLM,
						XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 2)),

					new XElement(tns + XMLNames.Bus_NumberPassengersLowerDeck,
						modelData.VehicleData.InputData.NumberPassengerSeatsLowerDeck),
					new XElement(tns + XMLNames.Bus_NumberPassengersUpperDeck,
						modelData.VehicleData.InputData.NumberPassengerSeatsUpperDeck),

					new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
					new XElement(tns + XMLNames.Bus_LowEntry, modelData.VehicleData.InputData.LowEntry),
					new XElement(tns + XMLNames.Bus_HeightIntegratedBody,
						modelData.VehicleData.InputData.Height.ToXMLFormat(3)),

					GetInputDataSignature(modelData)
				)
			);

			Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));
		}


		public override void WriteResult(XMLDeclarationReport.ResultEntry genericResult,
			XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{

			// no results to write...
		}
	}
}