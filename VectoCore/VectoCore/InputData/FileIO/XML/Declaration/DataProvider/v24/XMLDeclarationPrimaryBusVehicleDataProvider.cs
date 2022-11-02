using System.Collections.Generic;
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

	public abstract class AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{

		public AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;

		}

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool? SleeperCab => false;

		public override CubicMeter CargoVolume => null;
		public override XmlElement PTONode => null;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override LegislativeClass? LegislativeClass => VectoCommon.Models.LegislativeClass.M3;

		public override VehicleCategory VehicleCategory => VehicleCategory.HeavyBusPrimaryVehicle;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		public override Kilogram CurbMassChassis => null;

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override Meter EntranceHeight => null;

		public override TankSystem? TankSystem => VectoCommon.InputData.TankSystem.Compressed;

		public override bool VocationalVehicle => false;

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		#endregion

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Vehicle_Conventional_PrimaryBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevPxPrimaryBusDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Px_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevPxMediumLorryDataProviderV24

		public override string PowertrainPositionPrefix => "P";

		public override TableData BoostingLimitations
			=> ElementExists(XMLNames.Vehicle_BoostingLimitation)
				? ReadTableData(XMLNames.Vehicle_BoostingLimitation, XMLNames.BoostingLimitation_Entry,
					new Dictionary<string, string> {
						{XMLNames.BoostingLimitation_RotationalSpeed, XMLNames.BoostingLimitation_RotationalSpeed},
						{XMLNames.BoostingLimitation_BoostingTorque, XMLNames.BoostingLimitation_BoostingTorque}
					})
				: null;

		#endregion

		public XMLDeclarationHevPxPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevSxPrimaryBusDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Sx_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevPxMediumLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHevSxPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override IList<ITorqueLimitInputData> TorqueLimits => null;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPevPrimaryBusDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_PEV_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationPevMediumLorryExDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationPevPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIepcPrimaryBusDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_IEPC_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIepcPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHeviepcsPrimaryBusDataProviderV24 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHeviepcsHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHeviepcsPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IList<ITorqueLimitInputData> TorqueLimits => null;


		#endregion
	}


	// ---------------------------------------------------------------------------------------

}