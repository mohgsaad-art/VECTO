using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24
{
	public abstract class AbtractXMLDeclarationExemptedVehicleDataProviderV24 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public AbtractXMLDeclarationExemptedVehicleDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;

		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion

		public override VehicleCategory VehicleCategory => VehicleCategory.HeavyBusPrimaryVehicle;

		public override bool ExemptedVehicle => true;

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override LegislativeClass? LegislativeClass => GetString(XMLNames.Vehicle_LegislativeCategory)?.ParseEnum<LegislativeClass>();


		public override IList<ITorqueLimitInputData> TorqueLimits => new List<ITorqueLimitInputData>();

		public override PerSecond EngineIdleSpeed => null;

		public override bool VocationalVehicle => false;

		public override bool? SleeperCab => false;

		public override TankSystem? TankSystem => null;

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS => null;

		public override bool ZeroEmissionVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle));

		public override bool HybridElectricHDV => false;

		public override bool DualFuelVehicle => false;

		public override Watt MaxNetPower1 => GetDouble("SumNetPower").SI<Watt>();

		public override Watt MaxNetPower2 => null;

		public override string ExemptedTechnology => GetString("Technology");

		public override IVehicleComponentsDeclaration Components => null;

		public override XmlElement ComponentNode => null;

		public override XmlElement PTONode => null;

		public override XmlElement ADASNode => null;

		public override AngledriveType AngledriveType => AngledriveType.None;

		public override RetarderType RetarderType => RetarderType.None;

		public override double RetarderRatio => 0;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;
	}

	// ---------------------------------------------------------------------------------------
	public class XMLDeclarationExemptedHeavyLorryDataProviderV24 : AbtractXMLDeclarationExemptedVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Vehicle_Exempted_HeavyLorryDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedHeavyLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedMediumLorryDataProviderV24 : AbtractXMLDeclarationExemptedVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Vehicle_Exempted_MediumLorryDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedMediumLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedPrimaryBusDataProviderV24 : AbtractXMLDeclarationExemptedVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Vehicle_Exempted_PrimaryBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override Kilogram CurbMassChassis => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedCompletedBusDataProviderV24 : AbtractXMLDeclarationExemptedVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_Exempted_CompletedBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedCompletedBusDataProviderV24(IXMLDeclarationJobInputData jobData,
			XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		public override string Model =>
			ElementExists(XMLNames.Component_Model)
				? GetString(XMLNames.Component_Model) : null;

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

		public override RegistrationClass? RegisteredClass =>
			ElementExists(XMLNames.Vehicle_RegisteredClass)
				? RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First()
				: null;

		public override VehicleCategory VehicleCategory => VehicleCategory.HeavyBusCompletedVehicle;

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

		public override XmlElement PTONode => null;

		public override XmlElement ComponentNode => null;

		public override IVehicleComponentsDeclaration Components => null;


		public override bool ExemptedVehicle => true;

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

}