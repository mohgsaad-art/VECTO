using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML;
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

		public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;

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
		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.ConventionalVehicle;
		}
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
					AttributeMappings.BoostingLimitsMapping)
				: null;

		#endregion

		public XMLDeclarationHevPxPrimaryBusDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }



		public override VectoSimulationJobType VehicleType
		{
			get => Components.ElectricMachines.Entries.Any(em => em.ElectricMachine.IsIHPC())
				? VectoSimulationJobType.IHPC
				: VectoSimulationJobType.ParallelHybridVehicle;
		}
    
		

		public override bool HybridElectricHDV => true;

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

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.SerialHybridVehicle;

		public override bool HybridElectricHDV => true;

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

		public override bool OVC => true;


		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.BatteryElectricVehicle;
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

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_E;

		#endregion

		public override bool OVC => true;

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

		#region Overrides of AbstractXMLVehicleDataProviderV24

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_S;

		#endregion

		#endregion

		public override bool HybridElectricHDV => true;

	}


	// ---------------------------------------------------------------------------------------

}