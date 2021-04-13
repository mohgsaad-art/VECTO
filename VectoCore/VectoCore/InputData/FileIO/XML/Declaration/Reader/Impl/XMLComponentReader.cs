using System;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
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

		public virtual IVehicleComponentsDeclaration ComponentInputData
		{
			get { return _components ?? (_components = CreateComponent(XMLNames.Vehicle_Components, ComponentsCreator)); }
		}

		public virtual IAirdragDeclarationInputData AirdragInputData
		{
			get
			{
				return _airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator, true));
			}
		}

		public virtual IGearboxDeclarationInputData GearboxInputData
		{
			get
			{
				return _gearboxInputData ?? (_gearboxInputData = CreateComponent(XMLNames.Component_Gearbox, GearboxCreator));
			}
		}

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

		public virtual IBusAuxiliariesDeclarationData BusAuxiliariesInputData { get { return null; } }

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


		public virtual IAxleGearInputData AxleGearInputData
		{
			get
			{
				return _axlegearInputData ?? (_axlegearInputData = CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator));
			}
		}


		public virtual IAngledriveInputData AngledriveInputData
		{
			get
			{
				return _angledriveInputData ??
						(_angledriveInputData = CreateComponent(XMLNames.Component_Angledrive, AngledriveCreator, true));
			}
		}

		public virtual IEngineDeclarationInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = CreateComponent(XMLNames.Component_Engine, EngineCreator)); }
		}


		public virtual IAuxiliariesDeclarationInputData AuxiliaryData
		{
			get
			{
				return _auxiliaryInputData ??
						(_auxiliaryInputData = CreateComponent(XMLNames.Component_Auxiliaries, AuxiliaryCreator));
			}
		}


		public virtual IRetarderInputData RetarderInputData
		{
			get
			{
				return _retarderInputData ??
						(_retarderInputData = CreateComponent(XMLNames.Component_Retarder, RetarderCreator, true));
			}
		}


		public virtual IAxlesDeclarationInputData AxlesDeclarationInputData
		{
			get
			{
				return _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxleWheelsCreator));
			}
		}

		public virtual ITyreDeclarationInputData Tyre
		{
			get { return CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator); }
		}

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
			return Factory.CreateTyre(version, componentNode, sourceFile);
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
	}

	// ---------------------------------------------------------------------------------------


	public class XMLComponentReaderV27 : AbstractComponentReader, IXMLTrailerAxlesReader, IXMLTrailerAxleReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public const string XSD_TYPE = "VehicleDeclarationType";
		public const string AXLES_READER_TYPE = "TrailerAxleWheelsDataDeclarationType";
		public const string AXLE_READER_TYPE = "TrailerAxleType";
		//public const string AXLE_READER_TYPE = "AxleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string AXLES_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLES_READER_TYPE);
		public static readonly string AXLE_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, AXLE_READER_TYPE);

		protected IXMLTrailerDeclarationVehicleData Vehicle;
		protected ITrailerAxlesDeclarationInputData _axlesInputData;

		[Inject] public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLComponentReaderV27(IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{
			if (componentsNode == null)
			{
				throw new VectoException("axlewheels node must not be null!");
			}

			Vehicle = vehicle;
		}

		public virtual ITrailerAxlesDeclarationInputData AxlesDeclarationInputData => _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxleWheelsCreator));

		public virtual ITyreDeclarationInputData Tyre => CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator);

		public virtual ITrailerAxleDeclarationInputData CreateAxle(XmlNode axleNode)
		{
			var version = XMLHelper.GetXsdType(axleNode.SchemaInfo.SchemaType);
			try
			{
				var axle = Factory.CreateTrailerAxleData(version, Vehicle, axleNode, ""); //(Vehicle as IXMLResource).DataSource.SourceFile
				axle.Reader = Factory.CreateTrailerAxleReader(version, Vehicle, axleNode);
				return axle;
			}
			catch (Exception e)
			{
				var axleNumber = axleNode.Attributes?.GetNamedItem(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Axle: {1} Version: {2}", e, axleNode.LocalName, axleNumber, version);
			}
		}
		protected virtual IXMLTrailerAxlesDeclarationInputData AxleWheelsCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var axles = Factory.CreateTrailerAxleWheels(version, Vehicle, componentNode, sourceFile);
			axles.Reader = Factory.CreateTrailerAxlesReader(version, Vehicle, componentNode);
			return axles;
		}


		protected virtual ITyreDeclarationInputData TyreCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateTrailerTyre(version, componentNode, sourceFile);
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

	public class XMLAuxiliaryReaderV23 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

		public new const string AUX_READER_TYPE = "AuxiliariesDataDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, AUX_READER_TYPE);


		public XMLAuxiliaryReaderV23(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }
	}


	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderV26 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "PrimaryVehicleComponentsDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public const string COMPLETE_XSD_TYPE = "CompletedVehicleComponentsDeclarationType";
		public static readonly string QUALIFIED_COMPLETE_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, COMPLETE_XSD_TYPE);


		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLComponentReaderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData { get { return _busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator)); } }

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}


	public class XMLPrimaryVehicleBusComponentReaderV01 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public new const string XSD_TYPE = "VehicleComponentsPIFType";
		public new const string GEARBOX_READER_TYPE = "TransmissionDataPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public new static readonly string GEARBOX_READER_QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, GEARBOX_READER_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLPrimaryVehicleBusComponentReaderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode)
			: base(vehicle, componentsNode) { }

		public override IGearboxDeclarationInputData GearboxInputData
		{
			get
			{
				return _gearboxInputData ?? (_gearboxInputData = CreateComponent(XMLNames.Component_Transmission, GearboxCreator));
			}
		}

		public override IRetarderInputData RetarderInputData
		{
			get { return null; }
		}

		public override IAirdragDeclarationInputData AirdragInputData
		{
			get { return null; }
		}

		public override IAuxiliariesDeclarationInputData AuxiliaryData
		{
			get { return null; }
		}

		public override IBusAuxiliariesDeclarationData BusAuxiliariesInputData
		{
			get { return _busAuxInputData ?? (_busAuxInputData = CreateComponent(XMLNames.Component_Auxiliaries, BusAuxCreator)); }
		}

		protected virtual IBusAuxiliariesDeclarationData BusAuxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateBusAuxiliaires(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLComponentReaderNoAxlegearV26 : XMLComponentReaderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
		public new const string XSD_TYPE = "VehicleComponentsNoAxlegearType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLComponentReaderNoAxlegearV26(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }

		#region Overrides of XMLComponentReaderV10

		public override IAxleGearInputData AxleGearInputData { get { return null; } }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLGearboxReaderV26 : XMLComponentReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
		public new const string XSD_TYPE = "GearboxDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IBusAuxiliariesDeclarationData _busAuxInputData;


		public XMLGearboxReaderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode) : base(
			vehicle, componentsNode)
		{ }


	}

}