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

using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationComponentsDataProviderV10 : AbstractCommonComponentType, IXMLVehicleComponentsDeclaration
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleComponentsType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IAirdragDeclarationInputData _airdragInputData;
		protected IGearboxDeclarationInputData _gearboxInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineDeclarationInputData _engineInputData;
		protected IAuxiliariesDeclarationInputData _auxInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAxlesDeclarationInputData _axleWheels;
		protected IPTOTransmissionInputData _ptoInputData;
		protected IXMLDeclarationVehicleData _vehicle;
		protected ITorqueConverterDeclarationInputData _torqueconverterInputData;
		protected IElectricMachinesDeclarationInputData _electricMachinesInputData;
		protected IElectricStorageSystemDeclarationInputData _electricStorageSystemInputData;
		protected IIEPCDeclarationInputData _iepcInputData;



		public XMLDeclarationComponentsDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			_vehicle = vehicle;
		}

		#region Implementation of IVehicleComponentsDeclaration

		public virtual IAirdragDeclarationInputData AirdragInputData => _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData);

		public virtual IGearboxDeclarationInputData GearboxInputData => _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxInputData);


		public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData => _torqueconverterInputData ?? (_torqueconverterInputData = ComponentReader.TorqueConverterInputData);

		public virtual IAxleGearInputData AxleGearInputData => _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData);

		public virtual IAngledriveInputData AngledriveInputData => _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngledriveInputData);

		public virtual IEngineDeclarationInputData EngineInputData => _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData);

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData);

		public virtual IRetarderInputData RetarderInputData => _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData);

		public virtual IPTOTransmissionInputData PTOTransmissionInputData => _vehicle.PTOTransmissionInputData;

		public virtual IAxlesDeclarationInputData AxleWheels => _axleWheels ?? (_axleWheels = ComponentReader.AxlesDeclarationInputData);

		public virtual IBusAuxiliariesDeclarationData BusAuxiliaries => null;
		public virtual IElectricStorageSystemDeclarationInputData ElectricStorage =>  _electricStorageSystemInputData  ?? (_electricStorageSystemInputData = ComponentReader.ElectricStorageSystem);
		public virtual IElectricMachinesDeclarationInputData ElectricMachines => _electricMachinesInputData ?? (_electricMachinesInputData = ComponentReader.ElectricMachines);
		public virtual IIEPCDeclarationInputData IEPC => _iepcInputData ?? (_iepcInputData = ComponentReader.IEPCInputData);

		#endregion

		#region Implementation of IXMLVehicleComponentsDeclaration

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		#endregion


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsDataProviderV20 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationComponentsDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsDataProviderV24_Lorry : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Components_Conventional_LorryType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationComponentsDataProviderV24_Lorry(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		//public override IAngledriveInputData AngledriveInputData
		//	=> ElementExists(XMLNames.Component_Angledrive) ? base.AngledriveInputData : null;

		//public override IRetarderInputData RetarderInputData
		//	=> ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		//public override IAirdragDeclarationInputData AirdragInputData
		//	=> ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusComponentsDataProviderV24 : XMLDeclarationComponentsDataProviderV10, IXMLVehicleComponentsDeclaration
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Components_Conventional_PrimaryBusType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationPrimaryBusComponentsDataProviderV24(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }


		public override IAirdragDeclarationInputData AirdragInputData => null;
		
		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries =>
			_busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_Conventional_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => this;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => null;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region IRetarderInputData Interface Implementation

		public RetarderType Type => _vehicle.RetarderType;
		public double Ratio => _vehicle.RetarderRatio;
		public TableData LossMap { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-Px_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => this;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => null;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region IRetarderInputData Interface Implementation

		public RetarderType Type => _vehicle.RetarderType;
		public double Ratio => _vehicle.RetarderRatio;
		public TableData LossMap { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S2_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => this;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => null;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region IRetarderInputData Interface Implementation

		public RetarderType Type => _vehicle.RetarderType;
		public double Ratio => _vehicle.RetarderRatio;
		public TableData LossMap { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV01 :
		XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S3_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV01 :
		XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-S4_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => this;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => null;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region IRetarderInputData Interface Implementation

		public RetarderType Type => _vehicle.RetarderType;
		public double Ratio => _vehicle.RetarderRatio;
		public TableData LossMap { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E2_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		public override IEngineDeclarationInputData EngineInputData => null;

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => this;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => null;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region IRetarderInputData Interface Implementation

		public RetarderType Type => _vehicle.RetarderType;
		public double Ratio => _vehicle.RetarderRatio;
		public TableData LossMap { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV01 :
		XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E3_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV01 :
		XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-E4_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;

		public override IAxleGearInputData AxleGearInputData => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV01 :
		XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Vehicle_PEV-IEPC_ComponentsVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IGearboxDeclarationInputData GearboxInputData => null;

		public override IAxleGearInputData AxleGearInputData => ElementExists(XMLNames.Component_Axlegear) ? base.AxleGearInputData : null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationCompletedBusComponentsDataProviderV24 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public const string XSD_TYPE_CONVENTIONAL = "Components_Conventional_CompletedBusType";
		public const string XSD_TYPE_xEV = "Components_xEV_CompletedBusType";


		public static readonly string QUALIFIED_XSD_TYPE_CONVENTIONAL = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_CONVENTIONAL);
		public static readonly string QUALIFIED_XSD_TYPE_xEV = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_xEV);


		private IBusAuxiliariesDeclarationData _busAuxiliaries;


		public XMLDeclarationCompletedBusComponentsDataProviderV24(IXMLDeclarationVehicleData vehicle,
				XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		public override IAirdragDeclarationInputData AirdragInputData
		{
			get
			{
				if (!ElementExists(XMLNames.Component_AirDrag))
					return null;
				
				return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData);
			}
		}


		public override IGearboxDeclarationInputData GearboxInputData => null;

		public override ITorqueConverterDeclarationInputData TorqueConverterInputData => null;

		public override IAxleGearInputData AxleGearInputData => null;

		public override IAngledriveInputData AngledriveInputData => null;

		public override IEngineDeclarationInputData EngineInputData => null;

		public override IRetarderInputData RetarderInputData => null;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override IAxlesDeclarationInputData AxleWheels => null;


		public override IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get
			{
				if (!ElementExists(XMLNames.Component_Auxiliaries))
					return null;

				return _busAuxiliaries ?? (_busAuxiliaries = GetBusAuxiliaries());
			}
		}

		private IBusAuxiliariesDeclarationData GetBusAuxiliaries()
		{
			var busAux = ComponentReader.BusAuxiliariesInputData;

			if (busAux.ElectricConsumers == null && busAux.HVACAux == null &&
				busAux.PneumaticConsumers == null && busAux.PneumaticSupply == null &&
				busAux.ElectricSupply == null && busAux.FanTechnology == null &&
				busAux.SteeringPumpTechnology == null)
				return null;
			return busAux;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVPxLorryComponentsDataProviderV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-Px_LorryType";
		
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationHEVPxLorryComponentsDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10
		
		//public override IAngledriveInputData AngledriveInputData =>
		//	ElementExists(XMLNames.Component_Angledrive) ? base.AngledriveInputData : null;

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		//public override IAirdragDeclarationInputData AirdragInputData =>
		//	ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationHEVPxLorryComponentsDataProviderV27 : XMLDeclarationHEVPxLorryComponentsDataProviderV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationHEVPxLorryComponentsDataProviderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(vehicle, componentNode, sourceFile) 
		{ }
    }

    // ---------------------------------------------------------------------------------------	

    public class XMLDeclarationHevs2LorryComponentsDataProviderV24 : XMLDeclarationHEVPxLorryComponentsDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S2_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationHevs2LorryComponentsDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IIEPCDeclarationInputData IEPC => null;

		#endregion
	}
	
	// ---------------------------------------------------------------------------------------	

	public class XMLDeclarationHEVSXLorryComponentsDataProviderV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_HEV_S3_TYPE = "Components_HEV-S3_LorryType";
		public const string XSD_HEV_S4_TYPE = "Components_HEV-S4_LorryType";

		public static readonly string QUALIFIED_HEV_S3_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S3_TYPE);
		public static readonly string QUALIFIED_HEV_S4_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S4_TYPE);

		public XMLDeclarationHEVSXLorryComponentsDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IIEPCDeclarationInputData IEPC => null;

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		//public override IAirdragDeclarationInputData AirdragInputData =>
		//	ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-Px_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		protected IBusAuxiliariesDeclarationData _busAuxiliariesDeclarationInputData;

		public XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliariesDeclarationInputData ??
																		(_busAuxiliariesDeclarationInputData = ComponentReader.BusAuxiliariesInputData);
		
		public override IAirdragDeclarationInputData AirdragInputData => null;

		public override IIEPCDeclarationInputData IEPC => null;

		//public override IAngledriveInputData AngledriveInputData =>
		//	ElementExists(XMLNames.Component_Angledrive) ? base.AngledriveInputData : null;

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

        #endregion
    }

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV27 : XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24
    {
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV27(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) 
		{ }
	}
        
	// ---------------------------------------------------------------------------------------

    public class XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24 : XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-S2_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24 : XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_HEV_S3_TYPE = "Components_HEV-S3_PrimaryBusType";
		public const string XSD_HEV_S4_TYPE = "Components_HEV-S4_PrimaryBusType";
		public static readonly string QUALIFIED_HEV_S3_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S3_TYPE);
		public static readonly string QUALIFIED_HEV_S4_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S4_TYPE);

		public XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHeavyLorryHEVIEPCSComponentDataV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-IEPC-S_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLDeclarationHeavyLorryHEVIEPCSComponentDataV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

        #region Overrides of XMLDeclarationComponentsDataProviderV10

        //public override IRetarderInputData RetarderInputData =>
        //	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

        public override IAxleGearInputData AxleGearInputData =>
            ElementExists(XMLNames.Component_Axlegear) ? base.AxleGearInputData : null;

        //public override IAirdragDeclarationInputData AirdragInputData => 
        //	ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

        #endregion
    }

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusHEVIEPCSComponentDataV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_HEV-IEPC-S_PrimaryBus";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxiliariesDeclarationInputData;

		public XMLDeclarationPrimaryBusHEVIEPCSComponentDataV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IAirdragDeclarationInputData AirdragInputData => null;
		
		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliariesDeclarationInputData ??
																		(_busAuxiliariesDeclarationInputData = ComponentReader.BusAuxiliariesInputData);
        //public override IRetarderInputData RetarderInputData =>
        //	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

        public override IAxleGearInputData AxleGearInputData =>
            ElementExists(XMLNames.Component_Axlegear) ? base.AxleGearInputData : null;
		#endregion

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHeavyLorryPEVE2ComponentDataV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E2_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationHeavyLorryPEVE2ComponentDataV24(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		#region Overrides of XMLDeclarationComponentsDataProviderV10

		//public override IAngledriveInputData AngledriveInputData =>
		//	ElementExists(XMLNames.Component_Angledrive) ? base.AngledriveInputData : null;
		
		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		//public override IAirdragDeclarationInputData AirdragInputData =>
		//	ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationHeavyLorryPEVE2ComponentDataV27 : XMLDeclarationHeavyLorryPEVE2ComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationHeavyLorryPEVE2ComponentDataV27(IXMLDeclarationVehicleData vehicle,
            XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) 
		{ }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationHeavyLorryPevExComponentDataV24 : XMLDeclarationHeavyLorryPEVE2ComponentDataV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E3_LorryType";
		public const string XSD_PEV_E4_TYPE = "Components_PEV-E4_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_PEV_E4_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_E4_TYPE);
		public XMLDeclarationHeavyLorryPevExComponentDataV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationHeavyLorryPevExComponentDataV27 : XMLDeclarationHeavyLorryPevExComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        public new static readonly string QUALIFIED_XSD_PEV_E4_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_E4_TYPE);

        public XMLDeclarationHeavyLorryPevExComponentDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationPrimaryBusPEVE2ComponentDataV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E2_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliariesDeclarationInputData;

		public XMLDeclarationPrimaryBusPEVE2ComponentDataV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IEngineDeclarationInputData EngineInputData => null;

		public override IIEPCDeclarationInputData IEPC => null;

		public override IBusAuxiliariesDeclarationData BusAuxiliaries  => _busAuxiliariesDeclarationInputData ??
																 	 (_busAuxiliariesDeclarationInputData = ComponentReader.BusAuxiliariesInputData);

		//public override IAngledriveInputData AngledriveInputData =>
		//	ElementExists(XMLNames.Component_Angledrive) ? base.AngledriveInputData : null;

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

        #endregion
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationPrimaryBusPEVE2ComponentDataV27 : XMLDeclarationPrimaryBusPEVE2ComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationPrimaryBusPEVE2ComponentDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile) 
		{ }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationPrimaryBusPevExComponentDataV24 : XMLDeclarationPrimaryBusPEVE2ComponentDataV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_PEV-E3_PrimaryBusType";
		public const string XSD_PEV_E4_TYPE = "Components_PEV-E4_PrimaryBusType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_PEV_E4_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_E4_TYPE);

		public XMLDeclarationPrimaryBusPevExComponentDataV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base(vehicle, componentNode, sourceFile) { }
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationPrimaryBusPevExComponentDataV27 : XMLDeclarationPrimaryBusPevExComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        public new static readonly string QUALIFIED_XSD_PEV_E4_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_E4_TYPE);

        public XMLDeclarationPrimaryBusPevExComponentDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
            string sourceFile) : base(vehicle, componentNode, sourceFile) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationIEPCHeavyLorryComponentDataV24 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_IEPC_LorryType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLDeclarationIEPCHeavyLorryComponentDataV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		public override IAxleGearInputData AxleGearInputData =>
			ElementExists(XMLNames.Component_Axlegear) ? base.AxleGearInputData : null;

		//public override IAirdragDeclarationInputData AirdragInputData =>
		//	ElementExists(XMLNames.Component_AirDrag) ? base.AirdragInputData : null;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationIEPCHeavyLorryComponentDataV27 : XMLDeclarationIEPCHeavyLorryComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationIEPCHeavyLorryComponentDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile) 
		{ }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationIEPCPrimaryBusComponentDataV24 : XMLDeclarationPrimaryBusComponentsDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Components_IEPC_PrimaryBusType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIEPCPrimaryBusComponentDataV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		//public override IRetarderInputData RetarderInputData =>
		//	ElementExists(XMLNames.Component_Retarder) ? base.RetarderInputData : null;

		public override IAxleGearInputData AxleGearInputData =>
			ElementExists(XMLNames.Component_Axlegear) ? base.AxleGearInputData : null;
		
		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationIEPCPrimaryBusComponentDataV27 : XMLDeclarationIEPCPrimaryBusComponentDataV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationIEPCPrimaryBusComponentDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile) 
		{ }
    }
}