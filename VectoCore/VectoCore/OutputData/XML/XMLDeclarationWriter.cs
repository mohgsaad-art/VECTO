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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationWriter : AbstractXMLWriter
	{
		private XNamespace componentNamespace;

		public XMLDeclarationWriter(string vendor) : base(null, vendor)
		{
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
			rootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0";
			componentNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0";
			SchemaVersion = "1.0";
		}

		public XDocument GenerateVectoJob(IDeclarationInputDataProvider data)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

			//<tns:VectoInputDeclaration 
			//  xmlns="urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6" 
			//  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" schemaVersion="0.6" 
			//  xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6" 
			//  xsi:schemaLocation="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6 http://markus.quaritsch.at/VECTO/VectoInput.xsd">
			var job = new XDocument();
			job.Add(new XElement(rootNamespace + XMLNames.VectoInputDeclaration,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", rootNamespace),
				new XAttribute(xsi + "schemaLocation",
					$"{rootNamespace} {SchemaLocationBaseUrl}VectoInput.xsd"),
				CreateDeclarationJob(data))
				);
			return job;
		}

		public XDocument GenerateVectoComponent(IGearboxDeclarationInputData data,
			ITorqueConverterDeclarationInputData torqueConverter)
		{
			return GenerateComponentDocument(CreateGearbox(data, torqueConverter, componentNamespace));
		}

		public XDocument GenerateVectoComponent(IAxleGearInputData data)
		{
			return GenerateComponentDocument(CreateAxlegear(data, componentNamespace));
		}

		protected XDocument GenerateComponentDocument(XElement content)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var component = new XDocument();
			component.Add(new XElement(componentNamespace + XMLNames.VectoInputDeclaration,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", componentNamespace),
				new XAttribute(xsi + "schemaLocation",
					$"{componentNamespace} {SchemaLocationBaseUrl}VectoComponent.xsd"),
				content)
				);
			return component;
		}

		protected XElement[] CreateDeclarationJob(IDeclarationInputDataProvider data)
		{
			return new[] {
				CreateVehicle(data)
			};
		}

		protected XElement CreateVehicle(IDeclarationInputDataProvider data)
		{
			var retarder = data.JobInputData.Vehicle.Components.RetarderInputData;
			var gearbox = data.JobInputData.Vehicle.Components.GearboxInputData;
			var vehicle = data.JobInputData.Vehicle;
			var engine = data.JobInputData.Vehicle.Components.EngineInputData;
			var angledrive = data.JobInputData.Vehicle.Components.AngledriveInputData;


			var id = CreateIdString("VEH-" + vehicle.Model);

			return new XElement(tns + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, id),
				GetDefaultComponentElements(vehicle.CertificationNumber, vehicle.Model, Constants.NOT_AVAILABLE),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, "N3"),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, vehicle.VehicleCategory.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, vehicle.CurbMassChassis.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, vehicle.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed,
					vehicle.EngineIdleSpeed != null
						? vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)
						: engine.EngineModes.First().IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, angledrive.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_PTO,
					new XElement(tns + XMLNames.Vehicle_PTO_ShaftsGearWheels, "none"),
					new XElement(tns + XMLNames.Vehicle_PTO_OtherElements, "none")),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, false),
				new XElement(tns+XMLNames.Vehicle_VocationalVehicle, false),
				new XElement(tns + XMLNames.Vehicle_SleeperCab, true),
				new XElement(tns + XMLNames.Vehicle_ADAS,
					new XElement(tns + XMLNames.Vehicle_ADAS_EngineStopStart, false),
					new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, false),
					new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, false),
					new XElement(tns + XMLNames.Vehicle_ADAS_PCC, "none")
				),
				CreateTorqueLimits(vehicle),
				new XElement(tns + XMLNames.Vehicle_Components,
					CreateEngine(engine),
					CreateGearbox(gearbox, gearbox.Type.AutomaticTransmission() ? vehicle.Components.TorqueConverterInputData : null),
					angledrive.Type == AngledriveType.SeparateAngledrive ? CreateAngleDrive(angledrive) : null,
					retarder.Type.IsDedicatedComponent() ? CreateRetarder(retarder) : null,
					CreateAxlegear(vehicle.Components.AxleGearInputData),
					CreateAxleWheels(vehicle.Components.AxleWheels),
					CreateAuxiliaries(vehicle.Components.AuxiliaryInputData),
					CreateAirdrag(vehicle.Components.AirdragInputData)
					)
				);
		}

		protected XElement CreateEngine(IEngineDeclarationInputData data, XNamespace ns = null)
		{
			var id = CreateIdString($"ENG-{data.Model.RemoveWhitespace()}");

			var fld = FullLoadCurveReader.Create(data.EngineModes.First().FullLoadCurve, true);
			return new XElement((ns ?? tns) + XMLNames.Component_Engine,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements($"ENG-{data.Model}", data.Model),
					new XElement(tns + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_IdlingSpeed, data.EngineModes.First().IdleSpeed.AsRPM.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedSpeed, fld.RatedSpeed.AsRPM.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedPower, fld.FullLoadStationaryPower(fld.RatedSpeed).ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_MaxTorque, fld.MaxTorque.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_WHTCUrban, data.EngineModes.First().Fuels.First().WHTCUrban.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_WHTCRural, data.EngineModes.First().Fuels.First().WHTCRural.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_WHTCMotorway, data.EngineModes.First().Fuels.First().WHTCMotorway.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_ColdHotBalancingFactor, data.EngineModes.First().Fuels.First().ColdHotBalancingFactor.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_CorrectionFactor_RegPer, "1.0000"),
					new XElement(tns + XMLNames.Engine_CorrecionFactor_NCV, "1.0000"),
					new XElement(tns + XMLNames.Engine_FuelType, "Diesel CI"),
					new XElement(tns + XMLNames.Engine_FuelConsumptionMap,
						EmbedDataTable(data.EngineModes.First().Fuels.First().FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping)),
					new XElement(tns + XMLNames.Engine_FullLoadAndDragCurve,
						EmbedDataTable(data.EngineModes.First().FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping)
						)
					),
				AddSignatureDummy(id)
				);
		}

		protected XElement CreateGearbox(IGearboxDeclarationInputData gbxData,
			ITorqueConverterDeclarationInputData torqueConverter, XNamespace ns = null)
		{
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;
			foreach (var gearData in gbxData.Gears) {
				var gear = new XElement(tns + XMLNames.Gearbox_Gears_Gear,
					new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, i++),
					new XElement(tns + XMLNames.Gearbox_Gear_Ratio, gearData.Ratio.ToXMLFormat(3)),
					gearData.MaxTorque != null
						? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, gearData.MaxTorque.Value().ToXMLFormat(0))
						: null,
					gearData.MaxInputSpeed != null
						? new XElement(tns + XMLNames.Gearbox_Gear_MaxSpeed, gearData.MaxInputSpeed.AsRPM.ToXMLFormat(0))
						: null,
					new XElement(tns + XMLNames.Gearbox_Gear_TorqueLossMap,
						EmbedDataTable(gearData.LossMap, AttributeMappings.TransmissionLossmapMapping))
					);
				gears.Add(gear);
			}
			var id = CreateIdString($"GBX-{gbxData.Model.RemoveWhitespace()}");


			return new XElement((ns ?? tns) + XMLNames.Component_Gearbox,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements($"GBX-{gbxData.Model}", gbxData.Model),
					new XElement(tns + XMLNames.Gearbox_TransmissionType, gbxData.Type.ToXMLFormat()),
					new XElement(tns + XMLNames.Component_Gearbox_CertificationMethod, "Standard values"),
					gears
					),
				AddSignatureDummy(id),
				gbxData.Type.AutomaticTransmission() ? CreateTorqueConverter(torqueConverter) : null
				);
		}

		private XElement CreateTorqueConverter(ITorqueConverterDeclarationInputData data)
		{
			if (data == null) {
				throw new Exception("Torque Converter is required!");
			}

			var id = CreateIdString($"TC-{data.Model.RemoveWhitespace()}");


			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.CertificationNumber, data.Model),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.TorqueConverter_Characteristics,
						EmbedDataTable(data.TCData, AttributeMappings.TorqueConverterDataMapping,
							precision: new Dictionary<string, uint>() {
								{ TorqueConverterDataReader.Fields.SpeedRatio, 4 }
							})
						)
					),
				AddSignatureDummy(id));
		}

		private XElement CreateAngleDrive(IAngledriveInputData data, XNamespace ns = null)
		{
			var id = CreateIdString($"ANGL-{data.Model.RemoveWhitespace()}");

			return new XElement((ns ?? tns) + XMLNames.Component_Angledrive,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.CertificationNumber, data.Model),
					new XElement(tns + XMLNames.AngleDrive_Ratio, data.Ratio.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.AngleDrive_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping))),
				AddSignatureDummy(id));
		}

		public XElement CreateRetarder(IRetarderInputData data, XNamespace ns = null)
		{
			var id = CreateIdString($"RET-{data.Model.RemoveWhitespace()}");

			return new XElement((ns ?? tns) + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.CertificationNumber, data.Model),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.Retarder_RetarderLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.RetarderLossmapMapping)
						)
					),
				AddSignatureDummy(id)
				);
		}

		public XElement CreateAxlegear(IAxleGearInputData data, XNamespace ns = null)
		{
			var typeId = CreateIdString($"AXLGEAR-{data.Ratio.ToString("F3", CultureInfo.InvariantCulture)}");

			return new XElement((ns ?? tns) + XMLNames.Component_Axlegear,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, typeId),
					GetDefaultComponentElements(typeId, Constants.NOT_AVAILABLE),
					new XElement(tns + "LineType", "Single portal axle"),
					new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.Axlegear_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping))),
				AddSignatureDummy(typeId)
				);
		}

		public XElement CreateAxleWheels(IAxlesDeclarationInputData data, XNamespace ns = null)
		{
			var axleData = data.AxlesDeclaration;
			var numAxles = axleData.Count;
			var axles = new List<XElement>(numAxles);
			for (var i = 0; i < numAxles; i++) {
				var axle = axleData[i];
				axles.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i + 1),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_AxleType_Attr,
						i == 1 ? AxleType.VehicleDriven.ToString() : AxleType.VehicleNonDriven.ToString()),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, axle.TwinTyres),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Steered, i == 0),
					CreateTyre(axle.Tyre)
					));
			}

			return new XElement((ns ?? tns) + XMLNames.Component_AxleWheels,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XElement(tns + XMLNames.AxleWheels_Axles, axles))
				);
		}

		private XElement CreateTyre(ITyreDeclarationInputData tyre)
		{
			var id = CreateIdString($"TYRE-{tyre.Dimension}".Replace("/", "_"));

			return new XElement(tns + "Tyre",
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements($"TYRE-{tyre.Dimension}", tyre.Dimension),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, tyre.Dimension),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCDeclared, tyre.RollResistanceCoefficient.ToXMLFormat(4)),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, tyre.TyreTestLoad.Value().ToXMLFormat(0))
					),
				AddSignatureDummy(id));
		}

		public XElement CreateAuxiliaries(IAuxiliariesDeclarationInputData data)
		{
			var auxList = new Dictionary<AuxiliaryType, XElement>();
			foreach (var auxData in data.Auxiliaries) {
				var entry = new XElement(tns + AuxTypeToXML(auxData.Type),
					auxData.Technology.Select(x => new XElement(tns + XMLNames.Auxiliaries_Auxiliary_Technology, x)).ToArray<object>());
				auxList[auxData.Type] = entry;
			}
			var aux = new XElement(tns + XMLNames.ComponentDataWrapper);
			foreach (
				var key in
					new[] {
						AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
						AuxiliaryType.HVAC
					}) {
				aux.Add(auxList[key]);
			}
			return new XElement(tns + XMLNames.Component_Auxiliaries, aux);
		}

		private XElement CreateAirdrag(IAirdragDeclarationInputData data, XNamespace ns = null)
		{
			var id = CreateIdString($"Airdrag-{data.Model}");

			return new XElement((ns ?? tns) + XMLNames.Component_AirDrag,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.Model, Constants.NOT_AVAILABLE),
					new XElement(tns + "CdxA_0", data.AirDragArea.Value().ToXMLFormat(2)),
					new XElement(tns + "TransferredCdxA", data.AirDragArea.Value().ToXMLFormat(2)),
					new XElement(tns + XMLNames.AirDrag_DeclaredCdxA, data.AirDragArea.Value().ToXMLFormat(2))),
				AddSignatureDummy(id)
				);
		}

		private string AuxTypeToXML(AuxiliaryType type)
		{
			return type.ToString();
		}

		private XElement AddSignatureDummy(string id)
		{
			return new XElement(tns + XMLNames.DI_Signature,
				new XElement(di + XMLNames.DI_Signature_Reference,
					new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, "#" + id),
					new XElement(di + XMLNames.DI_Signature_Reference_Transforms,
						new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr,
								"http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithoutComments")),
						new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "urn:vecto:xml:2017:canonicalization"))
						),
					new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
						new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/04/xmlenc#sha256")),
					new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "")
					)
				);
		}

		protected XElement[] GetDefaultComponentElements(string componentId, string makeAndModel)
		{
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, Vendor),
				new XElement(tns + XMLNames.Component_Model, makeAndModel),
				new XElement(tns + XMLNames.Component_CertificationNumber, componentId),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, "VectoCore"),
			};
		}

		protected XElement[] GetDefaultComponentElements(string vin, string makeAndModel, string address)
		{
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, Vendor),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, address),
				new XElement(tns + XMLNames.Component_Model, makeAndModel),
				new XElement(tns + XMLNames.Vehicle_VIN, vin),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
			};
		}

		private string CreateIdString(string id)
		{
			var regexp = new Regex("[^a-zA-Z0-9_.-]");
			return regexp.Replace(id, "");
		}
	}
}
