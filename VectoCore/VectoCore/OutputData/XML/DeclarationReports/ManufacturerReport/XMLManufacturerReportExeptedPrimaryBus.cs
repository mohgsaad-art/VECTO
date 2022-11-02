using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public class XMLManufacturerReportExeptedPrimaryBus : AbstractXMLManufacturerReport
	{

		public XMLManufacturerReportExeptedPrimaryBus()
		{
            vns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.8";
        }

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleExemptedPrimaryBusType"),
				new XElement(vns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(vns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(vns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(vns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(vns + "LegislativeCategory", modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(vns + XMLNames.ChassisConfiguration, "Bus"),
				new XElement(vns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(vns + XMLNames.Vehicle_Articulated, modelData.VehicleData.InputData.Articulated),
				new XElement(
					vns + XMLNames.Vehicle_TPMLM,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_kg, 1)),
				//new XElement(
				//	tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(vns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(vns + "SumNetPower", XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)),
				new XElement(vns + "Technology", modelData.VehicleData.InputData.ExemptedTechnology)
				
			);
			Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));

			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return null;
		}

	}
}