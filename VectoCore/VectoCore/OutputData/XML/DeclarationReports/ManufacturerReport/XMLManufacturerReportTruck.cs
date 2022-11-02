/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public class XMLManufacturerReportTruck : AbstractXMLManufacturerReport
	{
		
		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleTruckType"),
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
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
				new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
				new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
						
				GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData.EngineData),
				VehicleComponents(modelData, fuelModes)
					
			);
			
			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			if (modelData.VehicleData.AxleConfiguration.AxlegearIncludedInGearbox()) {
				return new XElement(
					tns + XMLNames.Vehicle_Components,
					new XAttribute(xsi + "type", "ComponentsTruckFWDType"),
					GetEngineDescription(modelData.EngineData, fuelModes),
					GetGearboxDescription(modelData.GearboxData, modelData.AxleGearData),
					GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
					GetRetarderDescription(modelData.Retarder),
					GetAngledriveDescription(modelData.AngledriveData),
					GetAirDragDescription(modelData.AirdragData),
					GetAxleWheelsDescription(modelData.VehicleData),
					GetAuxiliariesDescription(modelData)
				);
			}
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + "type", "ComponentsTruckType"),
				GetEngineDescription(modelData.EngineData, fuelModes),
				GetGearboxDescription(modelData.GearboxData),
				GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
				GetRetarderDescription(modelData.Retarder),
				GetAngledriveDescription(modelData.AngledriveData),
				GetAxlegearDescription(modelData.AxleGearData),
				GetAirDragDescription(modelData.AirdragData),
				GetAxleWheelsDescription(modelData.VehicleData),
				GetAuxiliariesDescription(modelData)
			);
		}

		protected override XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var auxData = modelData.Aux.ToDictionary(a => a.ID);
			var auxList = new[] {
				AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
				AuxiliaryType.HVAC
			};
			var retVal = new XElement(tns + XMLNames.Component_Auxiliaries);
			foreach (var auxId in auxList) {
				if (auxData.TryGetValue(auxId.Key(), out var auxValue)) {
					foreach (var entry in auxValue.Technology) {
						retVal.Add(new XElement(tns + GetTagName(auxId), entry));
					}
				}
			}
			return retVal;
		}

		private string GetTagName(AuxiliaryType auxId)
		{
			return auxId + "Technology";
		}

		

		

		

		

		

		
		

		
	}
}
