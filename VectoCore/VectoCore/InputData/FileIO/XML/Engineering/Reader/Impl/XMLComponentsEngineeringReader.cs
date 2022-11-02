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
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal class XMLComponentsEngineeringReaderV07 : AbstractExternalResourceReader, IXMLComponentsReader, IXMLAxlesReader, IXMLAxleReader, IXMLGearboxReader, IXMLAuxiliaryReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string COMPONENTS_XSD_TYPE = "VehicleComponentsType";
		public const string AXLES_XSD_TYPE = "AxleWheelsDataEngineeringType";
		public const string AXLE_XSD_TYPE = "AxleDataEngineeringType";
		public const string GEARBOX_XSD_TYPE = "GearboxDataEngineeringType";
		public const string AUXILIARY_XSD_TYPE = "AuxiliariesDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE_COMPONENTS = XMLHelper.CombineNamespace(NAMESPACE_URI, COMPONENTS_XSD_TYPE);
		public static readonly string QUALIFIED_XSD_TYPE_AXLES = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_XSD_TYPE);
		public static readonly string QUALIFIED_XSD_TYPE_AXLE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_XSD_TYPE);
		public static readonly string QUALIFIED_XSD_TYPE_GEARBOX = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_XSD_TYPE);
		public static readonly string QUALIFIED_XSD_TYPE_AUXILIARY = XMLHelper.CombineNamespace(NAMESPACE_URI, AUXILIARY_XSD_TYPE);

		//protected string Version;
		protected IXMLEngineeringVehicleData Vehicle;

		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angularGearInputData;
		protected IEngineEngineeringInputData _engineInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAuxiliariesEngineeringInputData _auxiliaryData;
		protected IGearboxEngineeringInputData _gearboxData;
		protected IAirdragEngineeringInputData _airdragInputData;
		protected ITorqueConverterEngineeringInputData _torqueConverterInputData;
		protected IAxlesEngineeringInputData _axlesInputData;
		protected ITyreEngineeringInputData _tyre;
		protected IVehicleComponentsEngineering _components;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLComponentsEngineeringReaderV07(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode) :
			base(vehicle, componentsNode)
		{
			if (componentsNode == null) {
				throw new VectoException("component node must not be null!");
			}

			BaseNode = componentsNode;
			Vehicle = vehicle;
		}

		#region Implementation of IEngineeringComponentsFactory

		public virtual IAxleGearInputData AxleGearInputData => _axleGearInputData ?? (_axleGearInputData = CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator));

		public virtual IAngledriveInputData AngularGearInputData =>
			_angularGearInputData ?? (_angularGearInputData = CreateComponent(
				XMLNames.Component_Angledrive, AngledriveCreator, false, true));

		public virtual IEngineEngineeringInputData EngineInputData => _engineInputData ?? (_engineInputData = CreateComponent(XMLNames.Component_Engine, EngineCreator));

		public virtual IRetarderInputData RetarderInputData =>
			_retarderInputData ?? (_retarderInputData = CreateComponent(
				XMLNames.Component_Retarder, RetarderCreator, false, true));

		public virtual IAuxiliariesEngineeringInputData AuxiliaryData => _auxiliaryData ?? (_auxiliaryData = CreateComponent(XMLNames.Component_Auxiliaries, AuxiliariesCreator));

		public virtual IGearboxEngineeringInputData GearboxData => _gearboxData ?? (_gearboxData = CreateComponent(XMLNames.Component_Gearbox, GearboxCreator));

		public virtual IPTOTransmissionInputData PTOData => Vehicle;

		public virtual IAirdragEngineeringInputData AirdragInputData => _airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator));

		public virtual IAxlesEngineeringInputData AxlesEngineeringInputData => _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxlesCreator));

		public virtual ITorqueConverterEngineeringInputData TorqueConverter =>
			_torqueConverterInputData ?? (_torqueConverterInputData = CreateComponent(
				XMLNames.Component_TorqueConverter, TorqueConverterCreator, true, true));

		public virtual ITyreEngineeringInputData Tyre => _tyre ?? (_tyre = CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator));

		public virtual IVehicleComponentsEngineering ComponentInputData => _components ?? (_components = CreateComponent(XMLNames.Vehicle_Components, ComponentsCreator, requireDataNode:false));


		public IAxleEngineeringInputData CreateAxle(XmlNode axleNode)
		{
			var version = XMLHelper.GetXsdType(axleNode.SchemaInfo.SchemaType);
			try {
				var axle = Factory.CreateAxleData(version, axleNode, Vehicle);
				axle.Reader = Factory.CreateAxleReader(version, Vehicle, axleNode);
				return axle;
			} catch (Exception e) {
				var axleNumber = axleNode.Attributes?.GetNamedItem(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Axle: {1} Version: {2}", e, axleNode.LocalName, axleNumber, version);
			}
		}

		public ITransmissionInputData CreateGear(XmlNode gearNode)
		{
			var version = XMLHelper.GetXsdType(gearNode.SchemaInfo.SchemaType);
			try {
				return Factory.CreateGearData(version, gearNode, (Vehicle as IXMLResource).DataSource.SourcePath);
			} catch (Exception e) {
				var gearNumber = gearNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Gear: {1} Version: {2}", e, gearNode.LocalName, gearNumber, version);
			}
		}

		public IAuxiliaryEngineeringInputData CreateAuxiliary(XmlNode auxNode)
		{
			var version = XMLHelper.GetXsdType(auxNode.SchemaInfo.SchemaType);
			try {
				return Factory.CreateAuxData(version, auxNode, (Vehicle as IXMLResource).DataSource.SourcePath);
			} catch (Exception e) {
				throw new VectoException("Unsupported XML Version! Node: {0} Version: {2}", e, auxNode.LocalName, version);
			}
		}

		#endregion

		#region FactoryMethods

		protected IVehicleComponentsEngineering ComponentsCreator(string version, XmlNode componentNode, string sourcefile)
		{
			var components = Factory.CreateComponentData(version, Vehicle, componentNode, sourcefile);
			components.ComponentReader = Factory.CreateComponentReader(version, Vehicle, componentNode);
			return components;
		}

		protected virtual IAxleGearInputData AxlegearCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAxlegearData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAngledriveInputData AngledriveCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringAngledriveDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateAngledriveData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IEngineEngineeringInputData EngineCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateEngineData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IRetarderInputData RetarderCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringRetarderDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateRetarderData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAuxiliariesEngineeringInputData AuxiliariesCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var aux = Factory.CreateAuxiliariesData(version, Vehicle, componentNode, sourceFile);
			aux.Reader = Factory.CreatAuxiliariesReader(version, Vehicle, componentNode);
			return aux;
		}

		protected virtual IGearboxEngineeringInputData GearboxCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var gbx = Factory.CreateGearboxData(version, Vehicle, componentNode, sourceFile);
			gbx.Reader = Factory.CreateGearboxReader(version, Vehicle, componentNode);
			return gbx;
		}

		protected virtual IAirdragEngineeringInputData AirdragCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAirdragData(version, Vehicle, componentNode, sourceFile);
		}

		protected ITorqueConverterEngineeringInputData TorqueConverterCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringTorqueConverterDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateTorqueconverterData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAxlesEngineeringInputData AxlesCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var axles = Factory.CreateAxlesData(version, Vehicle, componentNode, sourceFile);
			axles.Reader = Factory.CreateAxlesReader(version, Vehicle, componentNode);
			return axles;
		}

		protected ITyreEngineeringInputData TyreCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateTyre(version, Vehicle, componentNode, sourceFile);
		}

		#endregion
	}

	internal class XMLComponentsEngineeringReaderV10 : XMLComponentsEngineeringReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string COMPONENTS_XSD_TYPE = "VehicleComponentsType";
		//public new const string AXLES_XSD_TYPE = "AxleWheelsComponentEngineeringType";
		//public new const string AXLE_XSD_TYPE = "AxleDataEngineeringType";
		//public new const string GEARBOX_XSD_TYPE = "GearboxComponentsEngineeringType";
		//public new const string AUXILIARY_XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE_COMPONENTS = XMLHelper.CombineNamespace(NAMESPACE_URI, COMPONENTS_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AXLES = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AXLE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_GEARBOX = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AUXILIARY = XMLHelper.CombineNamespace(NAMESPACE_URI, AUXILIARY_XSD_TYPE);

		public XMLComponentsEngineeringReaderV10(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode) :
			base(vehicle, componentsNode) { }
	}

	internal class XMLComponentsEngineeringReaderV10TEST : XMLComponentsEngineeringReaderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		//public new const string COMPONENTS_XSD_TYPE = "VehicleComponentsType";
		//public new const string AXLES_XSD_TYPE = "AxleWheelsComponentEngineeringType";
		//public new const string AXLE_XSD_TYPE = "AxleDataEngineeringType";
		//public new const string GEARBOX_XSD_TYPE = "GearboxComponentsEngineeringType";
		//public new const string AUXILIARY_XSD_TYPE = "AuxiliariesComponentEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE_COMPONENTS = XMLHelper.CombineNamespace(NAMESPACE_URI, COMPONENTS_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AXLES = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AXLE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_GEARBOX = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_XSD_TYPE);
		public new static readonly string QUALIFIED_XSD_TYPE_AUXILIARY = XMLHelper.CombineNamespace(NAMESPACE_URI, AUXILIARY_XSD_TYPE);

		public XMLComponentsEngineeringReaderV10TEST(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }

		public override ITyreEngineeringInputData Tyre => _tyre ?? (_tyre = CreateComponent("Tire", TyreCreator));
	}
}
