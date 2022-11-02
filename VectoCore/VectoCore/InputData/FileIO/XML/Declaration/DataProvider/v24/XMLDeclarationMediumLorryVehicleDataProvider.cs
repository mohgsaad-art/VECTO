using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24
{

	public abstract class AbstractXMLDeclarationMediumLorryVehicleDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public AbstractXMLDeclarationMediumLorryVehicleDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override VehicleCategory VehicleCategory => VehicleCategoryHelper.Parse(GetString(XMLNames.ChassisConfiguration));

		public override LegislativeClass? LegislativeClass => GetString(XMLNames.Vehicle_LegislativeCategory)?.ParseEnum<LegislativeClass>();

		public override bool? SleeperCab => false;

		public override bool VocationalVehicle => false;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override XmlElement PTONode => null;

		public override Kilogram CurbMassChassis => GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>();

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override CubicMeter CargoVolume {
			get {
				if (VehicleCategory == VehicleCategory.Van && !ElementExists(XMLNames.Vehicle_CargoVolume)) {
					throw new VectoException("Medium lorries with type Van require the input parameter cargo volume!");
				}
				return ElementExists(XMLNames.Vehicle_CargoVolume) ? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>()
					: null;
			}
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationConventionalMediumLorryVehicleDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "Vehicle_Conventional_MediumLorryDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalMediumLorryVehicleDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode,
			sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevPxMediumLorryDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Px_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationHevPxMediumLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

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


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;
		public override XmlElement PTONode => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevSxMediumLorryDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Sx_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevSxHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHevSxMediumLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;
		public override XmlElement PTONode => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPevMediumLorryExDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_PEV_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationPevMediumLorryExDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;
		public override XmlElement PTONode => null;

		#endregion

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override TableData BoostingLimitations => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIepcMediumLorryDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_IEPC_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLDeclarationIepcMediumLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;
		public override XmlElement PTONode => null;
		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override VehicleCategory VehicleCategory =>
			VehicleCategoryHelper.Parse(GetString(XMLNames.ChassisConfiguration));

		public override Kilogram CurbMassChassis => GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>();

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.TPMLM).SI<Kilogram>();

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHeviepcsMediumLorryDataProviderV24 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHeviepcsHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHeviepcsMediumLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume =>
			ElementExists(XMLNames.Vehicle_CargoVolume)
				? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>() : null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override XmlElement PTONode => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------


}