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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLComponentReaderV10 : AbstractComponentReader, IXMLComponentReader, IXMLAxlesReader, IXMLAxleReader, IXMLGearboxReader, IXMLAuxiliaryReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleComponentsType";
		public const string AXLES_READER_TYPE = "AxleWheelsDataDeclarationType";
		public const string AXLE_READER_TYPE = "AxleDeclarationType";
		public const string GEARBOX_READER_TYPE = "GearboxDataDeclarationType";
		public const string AUX_READER_TYPE = "AuxiliariesDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);
		public static readonly string AXLE_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_READER_TYPE);
		public static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);
		public static readonly string AUXILIARIES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_READER_TYPE);

		protected IXMLDeclarationVehicleData Vehicle;

		protected IVehicleComponentsDeclaration _components;
		protected IGearboxDeclarationInputData _gearboxInputData;
		protected IAxleGearInputData _axlegearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineDeclarationInputData _engineInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAxlesDeclarationInputData _axlesInputData;
		protected IAirdragDeclarationInputData _airdragInputData;
		protected IAuxiliariesDeclarationInputData _auxiliaryInputData;
		protected ITorqueConverterDeclarationInputData _torqueConverterInputData;
		protected IElectricMachinesDeclarationInputData _electricMachinesInputData;
		protected IElectricStorageSystemDeclarationInputData _electricStorageSystemInputData;
		protected IIEPCDeclarationInputData _iepcDeclarationInputData;


		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLComponentReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{
			if (componentsNode == null)
			{
				throw new VectoException("component node must not be null!");
			}

			Vehicle = vehicle;
		}

		#region Implementation of IXMLComponentReader

		public virtual IVehicleComponentsDeclaration ComponentInputData => _components ?? (_components = CreateComponent(XMLNames.Vehicle_Components, ComponentsCreator));

		public virtual IAirdragDeclarationInputData AirdragInputData => _airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator, true));

		public virtual IGearboxDeclarationInputData GearboxInputData => _gearboxInputData ?? (_gearboxInputData = CreateComponent(XMLNames.Component_Gearbox, GearboxCreator));

		public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get
			{
				if (_torqueConverterInputData == null && BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Component_TorqueConverter)) == null)
				{
					return null;
				}
				return _torqueConverterInputData ?? (_torqueConverterInputData = CreateComponent(
							XMLNames.Component_TorqueConverter, TorqueConverterCreator));
			}
		}

		public virtual IBusAuxiliariesDeclarationData BusAuxiliariesInputData => null;

		public virtual IElectricMachinesDeclarationInputData ElectricMachines => _electricMachinesInputData ?? 
																				(_electricMachinesInputData = GetElectricMachineEntries());
		public virtual IElectricStorageSystemDeclarationInputData ElectricStorageSystem => _electricStorageSystemInputData ?? 
			(_electricStorageSystemInputData  = GetElectricEnergyStorageEntries());

		public virtual IIEPCDeclarationInputData IEPCInputData => _iepcDeclarationInputData ?? 
														(_iepcDeclarationInputData = GetComponentNode(XMLNames.IEPC_Component) != null ? CreateComponent( XMLNames.IEPC_Component, IEPCCreator) : null);

		public virtual ITransmissionInputData CreateGear(XmlNode gearNode)
		{
			var version = XMLHelper.GetXsdType(gearNode.SchemaInfo.SchemaType);
			try
			{
				return Factory.CreateGearData(version, gearNode, ParentComponent.DataSource.SourceFile);
			}
			catch (Exception e)
			{
				var gearNumber = gearNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Gear: {1} Version: {2}", e, gearNode.LocalName, gearNumber, version);
			}
		}

		public virtual IAuxiliaryDeclarationInputData CreateAuxiliary(XmlNode auxNode)
		{
			var version = XMLHelper.GetXsdType(auxNode.ParentNode.ParentNode.SchemaInfo.SchemaType);
			try
			{
				return Factory.CreateAuxiliaryData(version, auxNode, Vehicle);
			}
			catch (Exception e)
			{
				throw new VectoException("Unsupported XML version! Node: {0} version: {1}", e, auxNode.LocalName, version);
			}
		}

		public virtual IAxleDeclarationInputData CreateAxle(XmlNode axleNode)
		{
			var version = XMLHelper.GetXsdType(axleNode.SchemaInfo.SchemaType);
			try
			{
				var axle = Factory.CreateAxleData(version, Vehicle, axleNode, (Vehicle as IXMLResource).DataSource.SourceFile);
				axle.Reader = Factory.CreateAxleReader(version, Vehicle, axleNode);
				return axle;
			}
			catch (Exception e)
			{
				var axleNumber = axleNode.Attributes?.GetNamedItem(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Axle: {1} Version: {2}", e, axleNode.LocalName, axleNumber, version);
			}
		}

		public virtual IAxleGearInputData AxleGearInputData => _axlegearInputData ?? (_axlegearInputData = CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator));


		public virtual IAngledriveInputData AngledriveInputData =>
			_angledriveInputData ??
			(_angledriveInputData = CreateComponent(XMLNames.Component_Angledrive, AngledriveCreator, true));

		public virtual IEngineDeclarationInputData EngineInputData => _engineInputData ?? (_engineInputData = CreateComponent(XMLNames.Component_Engine, EngineCreator));


		public virtual IAuxiliariesDeclarationInputData AuxiliaryData =>
			_auxiliaryInputData ??
			(_auxiliaryInputData = CreateComponent(XMLNames.Component_Auxiliaries, AuxiliaryCreator));


		public virtual IRetarderInputData RetarderInputData =>
			_retarderInputData ??
			(_retarderInputData = CreateComponent(XMLNames.Component_Retarder, RetarderCreator, true));


		public virtual IAxlesDeclarationInputData AxlesDeclarationInputData => _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxleWheelsCreator));

		public virtual ITyreDeclarationInputData Tyre => CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator);

		#endregion

		protected virtual IAirdragDeclarationInputData AirdragCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null)
			{
				return new XMLDeclarationAirdragDataProviderV10(Vehicle, null, sourceFile);
			}

			return Factory.CreateAirdragData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IGearboxDeclarationInputData GearboxCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var gbx = Factory.CreateGearboxData(version, Vehicle, componentNode, sourceFile);
			gbx.Reader = Factory.CreateGearboxReader(version, Vehicle, componentNode);
			return gbx;
		}

		protected virtual ITorqueConverterDeclarationInputData TorqueConverterCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null)
			{
				return new XMLDeclarationTorqueConverterDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateTorqueconverterData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAxleGearInputData AxlegearCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAxlegearData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAngledriveInputData AngledriveCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null)
			{
				return new XMLDeclarationAngledriveDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateAngledriveData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IEngineDeclarationInputData EngineCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateEngineData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IRetarderInputData RetarderCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null)
			{
				return new XMLDeclarationRetarderDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateRetarderData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAxlesDeclarationInputData AxleWheelsCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var axles = Factory.CreateAxleWheels(version, Vehicle, componentNode, sourceFile);
			axles.Reader = Factory.CreateAxlesReader(version, Vehicle, componentNode);
			return axles;
		}


		protected virtual ITyreDeclarationInputData TyreCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateTyre(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAuxiliariesDeclarationInputData AuxiliaryCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var aux = Factory.CreateAuxiliariesData(version, Vehicle, componentNode, sourceFile);
			aux.Reader = Factory.CreateAuxiliariesReader(version, Vehicle, componentNode);
			return aux;
		}


		protected virtual IVehicleComponentsDeclaration ComponentsCreator(
			string version, XmlNode componentNode, string sourcefile)
		{
			var components = Factory.CreateComponentData(version, Vehicle, componentNode, sourcefile);
			components.ComponentReader = Factory.CreateComponentReader(version, Vehicle, componentNode);
			return components;
		}
		
		protected virtual IElectricMachinesDeclarationInputData GetElectricMachineEntries()
		{
			var electricMachines = new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>>();
			var electricMachine = CreateComponent(XMLNames.Component_ElectricMachine, ElectricMachinesCreator, true);
			if(electricMachine?.Entries?.Any() == true)
				electricMachines.AddRange(electricMachine.Entries);

			var electricMachineGEN = CreateComponent(XMLNames.Component_ElectricMachineGEN, ElectricMachinesCreator, true);
			if(electricMachineGEN?.Entries?.Any() == true)
				electricMachines.AddRange(electricMachineGEN.Entries);
			
			return electricMachines.Any() ? new XMLElectricMachinesDeclarationData(electricMachines) : null;
		}

		protected virtual IElectricMachinesDeclarationInputData ElectricMachinesCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			if (componentNode == null)
				return null;

			var electricMachine = Factory.CreateElectricMachinesData(version, Vehicle, componentNode, sourcefile);
			electricMachine.ElectricMachineSystemReader = Factory.CreateElectricMotorReader(version, Vehicle, componentNode, sourcefile);
			return electricMachine;
		}

		protected virtual IElectricStorageSystemDeclarationInputData GetElectricEnergyStorageEntries()
		{
			return CreateComponent(XMLNames.Component_ElectricEnergyStorage, ElectricEnergyStorageCreator, true);
		}
		
		protected virtual IElectricStorageSystemDeclarationInputData ElectricEnergyStorageCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			if (componentNode == null)
				return null;

			var electricStorage = Factory.CreateElectricStorageSystemData(version, Vehicle, componentNode, sourcefile);
			electricStorage.StorageTypeReader =
				Factory.CreateStorageTypeReader(version, Vehicle, componentNode, sourcefile);
			

			return electricStorage;
		}
		
		protected virtual IIEPCDeclarationInputData IEPCCreator(string version, XmlNode componentNode,
			string sourceFile)
		{
			if (componentNode == null)
				return null;

			return Factory.CreateIEPCData(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV20 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "VehicleComponentsType";
		public new const string AXLE_READER_TYPE = "AxleDataDeclarationType";


		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);
		public new static readonly string AXLE_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_READER_TYPE);
		public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);
		public new static readonly string AUXILIARIES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_READER_TYPE);


		public XMLComponentReaderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV26 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public XMLComponentReaderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) 
		{ }
    }

    public class XMLComponentReaderV27 : XMLComponentReaderV20
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public new static readonly string AXLE_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_READER_TYPE);

        public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

        public XMLComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode)
        { }
    }


    // ---------------------------------------------------------------------------------------

    public class XMLComponentReaderV24_Lorry : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		//public new const string AXLE_READER_TYPE = "AxleDataDeclarationType";
		public new const string AUX_READER_TYPE = "AUX_Conventional_LorryDataType";
		public const string AUX_HEV_P_READER_TYPE = "AUX_HEV-P_LorryDataType";
		public const string AUX_HEV_S2_READER_TYPE = "AUX_HEV-S_LorryDataType";
		public const string AUX_PEV_E2_READER_TYPE = "AUX_PEV_LorryDataType";


		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);
		//public new static readonly string AXLE_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_READER_TYPE);
		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);
		public new static readonly string AUXILIARIES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_READER_TYPE);
		public static readonly string AUXILIARIES_READER_HEV_P_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_HEV_P_READER_TYPE);
		public static readonly string AUXILIARIES_READER_HEV_S2_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_HEV_S2_READER_TYPE);
		public static readonly string AUXILIARIES_READER_PEV_E2_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_PEV_E2_READER_TYPE);


		public XMLComponentReaderV24_Lorry(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }
	}


	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV24_PrimaryBus : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Components_Conventional_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLComponentReaderV24_PrimaryBus(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }

		#region Overrides of XMLComponentReaderV10

		public override IAirdragDeclarationInputData AirdragInputData =>
			_airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreaor));

		protected virtual IAirdragDeclarationInputData AirdragCreaor(string version, XmlNode componentNode,
			string sourceFile)
		{

			return Factory.CreateAirdragData(version, Vehicle, componentNode, sourceFile);
		}

		public override IIEPCDeclarationInputData IEPCInputData => null;

		#endregion


		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => _busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV24_CompletedBus : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public const string XSD_TYPE_CONVENTIONAL = "Components_Conventional_CompletedBusType";
		public const string XSD_TYPE_xEV = "Components_xEV_CompletedBusType";


		public static readonly string QUALIFIED_XSD_TYPE_CONVENTIONAL = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_CONVENTIONAL);
		public static readonly string QUALIFIED_XSD_TYPE_xEV = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_xEV);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLComponentReaderV24_CompletedBus(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode)
		{

		}

		public override IAngledriveInputData AngledriveInputData => null;

		public override IAxleGearInputData AxleGearInputData => null;

		public override IAxlesDeclarationInputData AxlesDeclarationInputData => null;

		public override IEngineDeclarationInputData EngineInputData => null;

		public override IGearboxDeclarationInputData GearboxInputData => null;

		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;

		public override ITyreDeclarationInputData Tyre => null;

        public override IRetarderInputData RetarderInputData => null;

		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;

		public override IAirdragDeclarationInputData AirdragInputData =>
			_airdragInputData ??
			(_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator));

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => _busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}

	}


	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV24_HEV_PxHeavyLorry : XMLComponentReaderV20
	{

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Components_Conventional_LorryType";
		public const string XSD_HEV_Px_TYPE = "Components_HEV-Px_LorryType";
		
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_Px_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_Px_TYPE);

		public XMLComponentReaderV24_HEV_PxHeavyLorry(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }

		
		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => null;
		public override IIEPCDeclarationInputData IEPCInputData => null;
	}

    // ---------------------------------------------------------------------------------------

    public class XMLComponentReaderV27_HEV_PxHeavyLorry : XMLComponentReaderV24_HEV_PxHeavyLorry
    {

        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        public new static readonly string QUALIFIED_XSD_HEV_Px_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_Px_TYPE);

        public XMLComponentReaderV27_HEV_PxHeavyLorry(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLElectricMachineSystemReaderV24 : AbstractComponentReader, IXMLElectricMachineSystemReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricMachineType";
		public const string XSD_GEN_TYPE = "ElectricMachineGENType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLElectricMachineSystemReaderV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base(vehicle, componentNode)
		{

		}

		#region Implementation of IXMLElectricMotorReader
		
		public virtual IElectricMotorDeclarationInputData CreateElectricMachineSystem(XmlNode electricMachineSystem)
		{
			return CreateComponent(XMLNames.ElectricMachineSystem, ElectricMachinesCreator);
		}

		public virtual IADCDeclarationInputData ADCInputData
		{
			get
			{
				return CreateComponent("ADC", ADCCreator);
			}
		}


		protected virtual IADCDeclarationInputData ADCCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			return Factory.CreateADCDeclarationInputData(version, componentNode, sourcefile);
		}

		protected virtual IElectricMotorDeclarationInputData ElectricMachinesCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			return Factory.CreateElectricMotorDeclarationInputData(version, componentNode, sourcefile);
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLElectricMachineSystemReaderV01 : AbstractComponentReader, IXMLElectricMachineSystemReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "ElectricMachineType";
		public const string XSD_GEN_TYPE = "ElectricMachineGENType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLElectricMachineSystemReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base(vehicle, componentNode)
		{

		}

		#region Implementation of IXMLElectricMotorReader

		public virtual IElectricMotorDeclarationInputData CreateElectricMachineSystem(XmlNode electricMachineSystem)
		{
			return CreateComponent(XMLNames.ElectricMachineSystem, ElectricMachinesCreator);
		}

		public virtual IADCDeclarationInputData ADCInputData {
			get {
				return CreateComponent("ADC", ADCCreator);
			}
		}


		protected virtual IADCDeclarationInputData ADCCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			return Factory.CreateADCDeclarationInputData(version, componentNode, sourcefile);
		}

		protected virtual IElectricMotorDeclarationInputData ElectricMachinesCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			return Factory.CreateElectricMotorDeclarationInputData(version, componentNode, sourcefile);
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLREESSReaderV24 : AbstractXMLType, IXMLREESSReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        [Inject] public virtual IDeclarationInjectFactory Factory { protected get; set; }

		public XMLREESSReaderV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base( componentNode) { }

		#region Implementation of IXMLREESSReader

		public virtual IREESSPackInputData CreateREESSInputData(XmlNode reessNode, REESSType reessType)
		{
			var type = reessNode.SchemaInfo.SchemaType;
			var version = XMLHelper.GetXsdType(type);
			if (version == null)
				return null;
			
			switch (reessType) {
				case REESSType.Battery:
					return BatteryPackCreator(version, reessNode, null);
				case REESSType.SuperCap:
					return SuperCapCreator(version, reessNode, null);
				default:
					return null;
			}
		}

		protected virtual IBatteryPackDeclarationInputData BatteryPackCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
			var node = GetNode(XMLNames.REESS, componentNode);
			var dataNode = GetNode(XMLNames.ComponentDataWrapper, node);
			version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);
			return Factory.CreateBatteryPackDeclarationInputData(version, node, sourcefile);
		}

		protected virtual ISuperCapDeclarationInputData SuperCapCreator(string version,
			XmlNode componentNode, string sourcefile)
		{
            var dataNode = GetNode(XMLNames.ComponentDataWrapper, componentNode);
            version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);
			return Factory.CreateSuperCapDeclarationInputData(version, componentNode, sourcefile);
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLREESSReaderV27 : XMLREESSReaderV24
    {
		public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
		public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLREESSReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
            string sourceFile) : base(vehicle, componentNode, sourceFile) { }
    }

    // ---------------------------------------------------------------------------------------
    public class XMLREESSReaderV01 : XMLREESSReaderV24
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		
		public XMLREESSReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base(vehicle, componentNode, sourceFile) { }

	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_Conventional_ComponentsVIFType";
		public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";
		
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);
		public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;

		public XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }

		public override IGearboxDeclarationInputData GearboxInputData => _gearboxInputData ?? (_gearboxInputData = CreateComponent(XMLNames.Component_Transmission, GearboxCreator));

		public override IRetarderInputData RetarderInputData => null;

		public override IAirdragDeclarationInputData AirdragInputData => null;

		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => _busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-Px_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base (vehicle, componentsNode) {}
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S2_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }

	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S3_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S4_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }

		#region Overrides of XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01

		public override IGearboxDeclarationInputData GearboxInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E2_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E3_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E4_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------
	public class XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV01 : XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-IEPC_ComponentsVIFType";
		//public new const string GEARBOX_READER_TYPE = "TransmissionDataVIFType";
		//public new const string AXLES_READER_TYPE = "AxleWheelsDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		//public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		//public new static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);

		public XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLPrimaryBusHEVPxDeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-Px_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;

		public XMLPrimaryBusHEVPxDeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }

		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;
		

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => _busAuxInputData ?? 
			(_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPrimaryBusHEVPxDeclarationComponentReaderV27 : XMLPrimaryBusHEVPxDeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLPrimaryBusHEVPxDeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPrimaryBusHEVS2DeclarationComponentReaderV201 : XMLPrimaryBusHEVPxDeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S2_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLPrimaryBusHEVS2DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLPrimaryBusHEVS3DeclarationComponentReaderV201 : XMLPrimaryBusHEVPxDeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S3_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLPrimaryBusHEVS3DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;
		public override IAngledriveInputData AngledriveInputData => null;
	
		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLPrimaryBusHEVS4DeclarationComponentReaderV201 : XMLPrimaryBusHEVS3DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S4_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLPrimaryBusHEVS4DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}
	
	// ---------------------------------------------------------------------------------------
	
	public class XMLHeavyLorryHEVS2DeclartionComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S2_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLHeavyLorryHEVS2DeclartionComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------
	
	public class XMLHeavyLorryHEVS3DeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S3_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLHeavyLorryHEVS3DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle,
			XmlNode componentsNode) : base(vehicle, componentsNode) { }
		
		#region Overrides of XMLComponentReaderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override IAngledriveInputData AngledriveInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;
		
		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLHeavyLorryHEVS4DeclarationComponentReaderV201 : XMLHeavyLorryHEVS3DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S4_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLHeavyLorryHEVS4DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }
		
		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-IEPC-S_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;

		public override IAngledriveInputData AngledriveInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => null;
		
		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201 : XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-IEPC-S_PrimaryBus";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;

		public XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }
		
		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;
		
		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => _busAuxInputData ??
			(_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLPEVHeavyLorryE2DeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E2_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLPEVHeavyLorryE2DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IEngineDeclarationInputData EngineInputData => null;

		public override IIEPCDeclarationInputData IEPCInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------
    public class XMLPEVHeavyLorryE2DeclarationComponentReaderV27 : XMLPEVHeavyLorryE2DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLPEVHeavyLorryE2DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPEVHeavyLorryE3DeclarationComponentReaderV201 : XMLPEVHeavyLorryE2DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E3_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLPEVHeavyLorryE3DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;
		public override IAngledriveInputData AngledriveInputData => null;
		
		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPEVHeavyLorryE3DeclarationComponentReaderV27 : XMLPEVHeavyLorryE3DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLPEVHeavyLorryE3DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPEVHeavyLorryE4DeclarationComponentReaderV201 : XMLPEVHeavyLorryE3DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E4_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLPEVHeavyLorryE4DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPEVHeavyLorryE4DeclarationComponentReaderV27 : XMLPEVHeavyLorryE4DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLPEVHeavyLorryE4DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE2DeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E2_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;

		public XMLPEVPrimaryBusE2DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;
		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData =>
			_busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}

		public override IAirdragDeclarationInputData AirdragInputData => null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE2DeclarationComponentReaderV27 : XMLPEVPrimaryBusE2DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLPEVPrimaryBusE2DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE3DeclarationComponentReaderV201 : XMLPEVPrimaryBusE2DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E3_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLPEVPrimaryBusE3DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;
		public override IAngledriveInputData AngledriveInputData => null;
		public override IAirdragDeclarationInputData AirdragInputData => null;
		
		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE3DeclarationComponentReaderV27 : XMLPEVPrimaryBusE3DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        
		public XMLPEVPrimaryBusE3DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE4DeclarationComponentReaderV201 : XMLPEVPrimaryBusE3DeclarationComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E4_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLPEVPrimaryBusE4DeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLPEVPrimaryBusE4DeclarationComponentReaderV27 : XMLPEVPrimaryBusE4DeclarationComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        
		public XMLPEVPrimaryBusE4DeclarationComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLIEPCHeavyLorryComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_IEPC_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLIEPCHeavyLorryComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }

		#region Overrides of XMLComponentReaderV10

		public override IEngineDeclarationInputData EngineInputData => null;
		public override IElectricMachinesDeclarationInputData ElectricMachines => null;
		public override IGearboxDeclarationInputData GearboxInputData => null;
		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;
		public override IAngledriveInputData AngledriveInputData => null;
		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData => null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLIEPCHeavyLorryComponentReaderV27 : XMLIEPCHeavyLorryComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLIEPCHeavyLorryComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLIEPCPrimaryBusComponentReaderV201 : XMLIEPCHeavyLorryComponentReaderV201
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_IEPC_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		private IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLIEPCPrimaryBusComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }


		#region Overrides of XMLComponentReaderV10

		public override IAuxiliariesDeclarationInputData AuxiliaryData => null;
		
		#endregion

		#region Overrides of XMLIEPCHeavyLorryComponentReaderV201

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData =>
			_busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator));

		#endregion

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

    // ---------------------------------------------------------------------------------------

    public class XMLIEPCPrimaryBusComponentReaderV27 : XMLIEPCPrimaryBusComponentReaderV201
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLIEPCPrimaryBusComponentReaderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
            : base(vehicle, componentsNode) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLGearboxDeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public new const string GEARBOX_READER_TYPE = "GearboxDataDeclarationType";
		public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		public XMLGearboxDeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLAuxiliaryDeclarationComponentReaderV201 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string AUX_READER_TYPE = "AUX_IEPC_LorryDataType";
		public const string AUX_IEPC_READER_TYPE = "AUX_IEPC_PrimaryBusType";

		public new static readonly string AUXILIARIES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_READER_TYPE);
		public static readonly string AUXILIARIES_READER_IEPC_PRIMARY_QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI, AUX_IEPC_READER_TYPE);

		public XMLAuxiliaryDeclarationComponentReaderV201(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) 
			: base(vehicle, componentsNode) { }
	}
}
