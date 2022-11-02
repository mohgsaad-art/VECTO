using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public class XMLExemptedPrimaryBusVehicleReport : XMLPrimaryBusVehicleReport
	{
		#region Implementation of IXMLPrimaryVehicleReport

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + XMLNames.Attr_Type, "VehicleExemptedPrimaryBusType"),
				new XElement(tns + XMLNames.ManufacturerPrimaryVehicle, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.ManufacturerAddressPrimaryVehicle, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(modelData.VehicleData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Vehicle_LegislativeCategory, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.ChassisConfiguration, "Bus"),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_Articulated, modelData.VehicleData.InputData.Articulated),
				new XElement(
					tns + XMLNames.Vehicle_TPMLM,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_kg, 1)),
				//new XElement(
				//	tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Exempted_SumNetPower, XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)),
				new XElement(tns + XMLNames.Exempted_Technology, modelData.VehicleData.InputData.ExemptedTechnology)

			);
			Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));

			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));

		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			// no results for exempted vehicle
		}


		#endregion
	}
}