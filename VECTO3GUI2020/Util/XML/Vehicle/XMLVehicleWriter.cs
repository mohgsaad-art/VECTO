using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.ComponentWriter;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Resources.XML;

namespace VECTO3GUI2020.Util.XML.Vehicle
{
	public abstract class XMLVehicleWriter : IXMLVehicleWriter
	{
		#region XML

		protected XElement _Xelement;

		protected XNamespace _defaultNamespace;

		#endregion

		//Template Methods
		protected abstract void Initialize();
		protected abstract void CreateElements();

		protected readonly IVehicleDeclarationInputData _inputData;
		protected IXMLWriterFactory _xmlWriterFactory;

		public XMLVehicleWriter(IVehicleDeclarationInputData inputData, IXMLWriterFactory xmlWriterFactory)
		{
			Debug.Assert(inputData != null);
			_inputData = inputData;
			_xmlWriterFactory = xmlWriterFactory;
		}


		public XElement GetElement()
		{
			if (_Xelement == null) {
				Initialize();
				CreateElements();
			}

			return _Xelement;
		}
	}

	public abstract class XMLCompletedBusVehicleWriter : XMLVehicleWriter
	{
		protected readonly IComponentWriterFactory _componentWriterFactory;
		protected readonly IGroupWriterFactory _groupWriterFactory;
		public abstract string VehicleType { get; }


		protected XMLCompletedBusVehicleWriter(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory, IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory)
		{
			_groupWriterFactory = groupWriterFactory;
			_componentWriterFactory = componentWriterFactory;
		}

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V27;

			var type = new XmlTypeAttribute();
			type.Namespace = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
			type.TypeName = VehicleType;

			XNamespace v27 = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
			_Xelement = new XElement(v27 + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr,
					_inputData.Identifier ?? "VEH-" + Guid.NewGuid().ToString("n").Substring(0, 20)),
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, VehicleType),
				new XAttribute("xmlns", _defaultNamespace));
		}
	}

	public class XMLCompletedBusVehicleWriterExempted : XMLCompletedBusVehicleWriter
	{
		public static readonly (XNamespace version, string type)[] SUPPORTEDVERSIONS = {
			(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
				XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE)
		};

		#region Overrides of XMLCompletedBusVehicleWriter

		public override string VehicleType => XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE;

		#endregion


		#region Overrides of XMLVehicleWriter

		public XMLCompletedBusVehicleWriterExempted(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory, IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory, groupWriterFactory,
			componentWriterFactory) { }

		protected override void CreateElements()
		{
			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory
				.GetVehicleDeclarationGroupWriter(GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup,
					_defaultNamespace).GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_LegislativeCategory,
				_inputData.LegislativeClass.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.CorrectedActualMass,
				_inputData.CurbMassChassis?.ToXMLFormat(0)));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_TPMLM,
				_inputData.GrossVehicleMassRating?.ToXMLFormat(0)));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_RegisteredClass,
				_inputData.RegisteredClass.ToXMLFormat()));

			_Xelement.Add(_groupWriterFactory
				.GetVehicleDeclarationGroupWriter(GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup,
					_defaultNamespace).GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_HeightIntegratedBody,
				_inputData.Height?.ConvertToMilliMeter().ToXMLFormat(0)));
		}

		#endregion
	}

	public class XMLCompletedBusVehicleWriterConventional : XMLCompletedBusVehicleWriter
	{
		public static readonly (XNamespace version, string type)[] SUPPORTEDVERSIONS = {
			(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
				XMLDeclaration_Conventional_CompletedBus_DataProviderV27.XSD_TYPE)
		};

		private new readonly IGroupWriterFactory _groupWriterFactory;
		private new readonly IComponentWriterFactory _componentWriterFactory;

		public override string VehicleType => XMLDeclaration_Conventional_CompletedBus_DataProviderV27.XSD_TYPE;

		public XMLCompletedBusVehicleWriterConventional(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory,
			IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory, groupWriterFactory,
			componentWriterFactory)
		{
			_groupWriterFactory = groupWriterFactory;
			_componentWriterFactory = componentWriterFactory;
		}

		#region Overrides of XMLVehicleWriter

		protected override void CreateElements()
		{
			CreateConventionalElements();
		}

		private void CreateConventionalElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem,
				_inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(
				_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
				_inputData.DoorDriveTechnology != null
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType,
				_inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber,
				_inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null) {
				_Xelement.Add(_componentWriterFactory
					.getDeclarationAdasWriter(GroupNames.ADAS_Conventional_Type, _defaultNamespace)
					.GetComponent(_inputData.ADAS));
			}

			if (_inputData.Components != null) {
				_Xelement.Add(_xmlWriterFactory.CreateComponentsWriterWithVersion(
					XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
					XMLTypes.Components_Conventional_CompletedBusType,
					_inputData.Components).GetComponents());
            }
		}

		#endregion
	}

	public class XMLCompletedBusVehicleWriterHEV : XMLCompletedBusVehicleWriter
	{
		public static (XNamespace version, string type) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
			XMLDeclaration_HEV_CompletedBus_DataProviderV27.XSD_TYPE);

		#region Overrides of XMLCompletedBusVehicleWriter

		public override string VehicleType => XMLDeclaration_HEV_CompletedBus_DataProviderV27.XSD_TYPE;

		#endregion

		public XMLCompletedBusVehicleWriterHEV(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory, IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory, groupWriterFactory,
			componentWriterFactory) { }

		#region Overrides of XMLVehicleWriter

		protected override void CreateElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem,
				_inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(
				_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
				_inputData.DoorDriveTechnology != null
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType,
				_inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber,
				_inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null) {

				_Xelement.Add(_componentWriterFactory
					.getDeclarationAdasWriter(GroupNames.ADAS_HEV_Type, _defaultNamespace)
					.GetComponent(_inputData.ADAS));
			}

			if (_inputData.Components != null) {
				_Xelement.Add(_xmlWriterFactory.CreateComponentsWriterWithVersion(
					XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
					XMLTypes.Components_xEV_CompletedBusType,
					_inputData.Components).GetComponents());
            }
		}

		#endregion
	}

	public class XMLCompletedBusVehicleWriterPEV : XMLCompletedBusVehicleWriter
	{
		public static (XNamespace version, string type) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
			XMLDeclaration_PEV_CompletedBus_DataProviderV27.XSD_TYPE);

		#region Overrides of XMLCompletedBusVehicleWriter

		public override string VehicleType => XMLDeclaration_PEV_CompletedBus_DataProviderV27.XSD_TYPE;

		#endregion

		public XMLCompletedBusVehicleWriterPEV(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory, IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory, groupWriterFactory,
			componentWriterFactory) { }

		#region Overrides of XMLVehicleWriter

		protected override void CreateElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem,
				_inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(
				_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
				_inputData.DoorDriveTechnology != null
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType,
				_inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber,
				_inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null) {
				_Xelement.Add(_componentWriterFactory
					.getDeclarationAdasWriter(GroupNames.ADAS_PEV_Type, _defaultNamespace)
					.GetComponent(_inputData.ADAS));
			}

			if (_inputData.Components != null) {
				_Xelement.Add(_xmlWriterFactory.CreateComponentsWriterWithVersion(
					XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27, 
					XMLTypes.Components_xEV_CompletedBusType,
					_inputData.Components).GetComponents());
            }

		}
		#endregion
	}

	// todo amogoda: delete
	public class XMLCompletedBusVehicleWriterIEPC : XMLCompletedBusVehicleWriter
	{
		public static (XNamespace version, string type) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24,
			XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE);

		#region Overrides of XMLCompletedBusVehicleWriter

		public override string VehicleType => XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE;

		#endregion

		public XMLCompletedBusVehicleWriterIEPC(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory,
			IGroupWriterFactory groupWriterFactory, IComponentWriterFactory componentWriterFactory) : base(inputData,
			xmlWriterFactory, groupWriterFactory, componentWriterFactory) { }

		#region Overrides of XMLVehicleWriter

		protected override void CreateElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem,
				_inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(
				_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
				_inputData.DoorDriveTechnology != null
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType,
				_inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber,
				_inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null) {
				_Xelement.Add(_componentWriterFactory
					.getDeclarationAdasWriter(GroupNames.ADAS_IEPC_Type, _defaultNamespace)
					.GetComponent(_inputData.ADAS));
            }

			if (_inputData.Components != null) {
				_Xelement.Add(_xmlWriterFactory.CreateComponentsWriterWithVersion(
					XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
					XMLTypes.Components_xEV_CompletedBusType,
					_inputData.Components).GetComponents());
			}
		}
	}


	public class XMLCompletedBusVehicleWriterFCHV : XMLCompletedBusVehicleWriter
	{
		public static (XNamespace version, string type) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
			XMLDeclaration_FCHV_CompletedBus_DataProviderV27.XSD_TYPE);

		#region Overrides of XMLCompletedBusVehicleWriter

		public override string VehicleType => XMLDeclaration_FCHV_CompletedBus_DataProviderV27.XSD_TYPE;

		#endregion

		public XMLCompletedBusVehicleWriterFCHV(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory, IGroupWriterFactory groupWriterFactory,
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory, groupWriterFactory,
			componentWriterFactory)
		{ }

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V27;

			var type = new XmlTypeAttribute();
			type.Namespace = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
			type.TypeName = VehicleType;

			XNamespace v27 = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
			_Xelement = new XElement(v27 + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr,
					_inputData.Identifier ?? "VEH-" + Guid.NewGuid().ToString("n").Substring(0, 20)),
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, VehicleType),
				new XAttribute("xmlns", _defaultNamespace));
		}

		protected override void CreateElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
						GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace)
					.GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem,
				_inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode,
				_inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(
				_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
				_inputData.DoorDriveTechnology != null
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType,
				_inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber,
				_inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null)
			{
				_Xelement.Add(_componentWriterFactory
					.getDeclarationAdasWriter(GroupNames.ADAS_PEV_Type, _defaultNamespace)
					.GetComponent(_inputData.ADAS));
			}

			if (_inputData.Components != null)
			{
				_Xelement.Add(_xmlWriterFactory.CreateComponentsWriterWithVersion(
					XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
					XMLTypes.Components_xEV_CompletedBusType,
					_inputData.Components).GetComponents());
			}

		}
	}
}

#endregion