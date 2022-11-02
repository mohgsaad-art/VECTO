using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24
{
	
	public abstract class AbstractXMLDeclarationCompletedBusDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		
		public AbstractXMLDeclarationCompletedBusDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override string Model {
			get {
				if (BaseNode.LocalName == XMLNames.Component_Vehicle) {
					return BaseNode.SelectSingleNode($"./*[local-name()='{XMLNames.Component_Model}']")?.InnerText;
				}
				return ElementExists(new[] { XMLNames.Component_Vehicle, XMLNames.Component_Model })
					? GetString(new[] { XMLNames.Component_Vehicle, XMLNames.Component_Model }) : null;
			}
		}

		public override LegislativeClass? LegislativeClass =>
			ElementExists(XMLNames.Vehicle_LegislativeCategory)
				? GetString(XMLNames.Vehicle_LegislativeCategory).ParseEnum<LegislativeClass>()
				: (LegislativeClass?)null;

		public override Kilogram CurbMassChassis =>
			ElementExists(XMLNames.CorrectedActualMass)
				? GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>()
				: null;

		public override Kilogram GrossVehicleMassRating =>
			ElementExists(XMLNames.Vehicle_TPMLM)
				? GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>()
				: null;

		public override bool? AirdragModifiedMultistep =>
			ElementExists(XMLNames.Bus_AirdragModifiedMultistep)
				? GetBool(XMLNames.Bus_AirdragModifiedMultistep)
				: (bool?)null;

		public override RegistrationClass? RegisteredClass =>
			ElementExists(XMLNames.Vehicle_RegisteredClass)
				? RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First()
				: null;

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;


		public override int? NumberPassengerSeatsLowerDeck {
			get {
				if (!ElementExists(XMLNames.Bus_NumberPassengerSeatsLowerDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengerSeatsLowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberPassengerSeatsUpperDeck {
			get {
				if (!ElementExists(XMLNames.Bus_NumberPassengerSeatsUpperDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengerSeatsUpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberPassengersStandingLowerDeck {
			get {
				if (!ElementExists(XMLNames.Bus_NumberPassengersStandingLowerDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersStandingLowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberPassengersStandingUpperDeck {
			get {
				if (!ElementExists(XMLNames.Bus_NumberPassengersStandingUpperDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersStandingUpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}



		public override VehicleCode? VehicleCode =>
			ElementExists(XMLNames.Vehicle_BodyworkCode)
				? GetString(XMLNames.Vehicle_BodyworkCode).ParseEnum<VehicleCode>()
				: (VehicleCode?)null;

		public override bool? LowEntry =>
			ElementExists(XMLNames.Bus_LowEntry)
				? GetBool(XMLNames.Bus_LowEntry)
				: (bool?)null;

		public override Meter Height =>
			ElementExists(XMLNames.Bus_HeightIntegratedBody)
				? GetDouble(XMLNames.Bus_HeightIntegratedBody).SI(Unit.SI.Milli.Meter).Cast<Meter>()
				: null;

		public override Meter Length =>
			ElementExists(XMLNames.Bus_VehicleLength)
				? GetDouble(XMLNames.Bus_VehicleLength).SI(Unit.SI.Milli.Meter).Cast<Meter>()
				: null;

		public override Meter Width =>
			ElementExists(XMLNames.Bus_VehicleWidth)
				? GetDouble(XMLNames.Bus_VehicleWidth).SI(Unit.SI.Milli.Meter).Cast<Meter>()
				: null;

		public override Meter EntranceHeight =>
			ElementExists(XMLNames.Bus_EntranceHeight)
				? GetDouble(XMLNames.Bus_EntranceHeight).SI(Unit.SI.Milli.Meter).Cast<Meter>()
				: null;

		public override ConsumerTechnology? DoorDriveTechnology =>
			ElementExists(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology)
				? ConsumerTechnologyHelper.Parse(GetString(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology))
				: (ConsumerTechnology?)null;

		public override VehicleDeclarationType VehicleDeclarationType => VehicleDeclarationTypeHelper.Parse(GetString(XMLNames.Bus_VehicleDeclarationType));


		public override XmlElement ADASNode => _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement);


		public override XmlElement ComponentNode {
			get {
				if (ExemptedVehicle)
					return null;

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components, required: false) as XmlElement);
			}
		}


		public override IVehicleComponentsDeclaration Components {
			get {
				if (ComponentNode == null)
					return null;

				if (_components == null)
					_components = ComponentReader.ComponentInputData;

				if (_components.BusAuxiliaries == null && _components.AirdragInputData == null)
					return null;

				return _components;
			}
		}

		public override XmlElement PTONode => null;


		#region Overrides of AbstractXMLResource

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#endregion

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationConventionalCompletedBusDataProviderV24 : AbstractXMLDeclarationCompletedBusDataProviderV24
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
        public new const string XSD_TYPE = "Vehicle_Conventional_CompletedBusDeclarationType";


		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalCompletedBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevCompletedBusDataProviderV24 : AbstractXMLDeclarationCompletedBusDataProviderV24
	{
		public new const string XSD_TYPE = "Vehicle_HEV_CompletedBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationHevCompletedBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPeVompletedBusDataProviderV24 : AbstractXMLDeclarationCompletedBusDataProviderV24
	{
		public new const string XSD_TYPE = "Vehicle_PEV_CompletedBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPeVompletedBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIepcCompletedBusDataProviderV24 : AbstractXMLDeclarationCompletedBusDataProviderV24
	{
		public new const string XSD_TYPE = "Vehicle_IEPC_CompletedBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIepcCompletedBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

}