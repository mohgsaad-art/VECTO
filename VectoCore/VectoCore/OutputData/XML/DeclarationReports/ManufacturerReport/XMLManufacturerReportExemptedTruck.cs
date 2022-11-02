using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport 
{
	public class XMLManufacturerReportExemptedTruck : AbstractXMLManufacturerReport
	{
		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleExemptedTruckType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Vehicle_GrossVehicleMass,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),
				ExemptedData(modelData)
			);
			Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));

			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return null;
		}


		private XElement[] ExemptedData(VectoRunData modelData)
		{
			var retVal = new List<XElement>();

			if (modelData.VehicleData.AxleConfiguration != AxleConfiguration.AxleConfig_Undefined) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()));
				retVal.Add(new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()));
			}

			if (modelData.VehicleData.SleeperCab.HasValue) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab.Value));
			}

			if (modelData.VehicleData.MaxNetPower1 != null)
				retVal.Add(new XElement(tns + XMLNames.Vehicle_MaxNetPower1,
					XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)));
			if (modelData.VehicleData.MaxNetPower2 != null) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_MaxNetPower2,
					XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower2, XMLNames.Unit_W)));
			}

			return retVal.ToArray();
		}
	}
}