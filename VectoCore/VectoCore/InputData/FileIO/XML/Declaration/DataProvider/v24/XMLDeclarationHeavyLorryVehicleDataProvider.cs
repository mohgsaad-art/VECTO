using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24
{

	public class XMLDeclarationConventionalHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_Conventional_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalHeavyLorryDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VehicleCategory VehicleCategory =>
			VehicleCategoryHelper.Parse(GetString(XMLNames.ChassisConfiguration));

		public override Kilogram CurbMassChassis => GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>();

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.TPMLM).SI<Kilogram>();

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion


		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.ConventionalVehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevPxHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Px_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of VehicleDataProviderHelper

		public override string PowertrainPositionPrefix => "P";

		#endregion

		public XMLDeclarationHevPxHeavyLorryDataProviderV24(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override TableData BoostingLimitations
			=> ElementExists(XMLNames.Vehicle_BoostingLimitation)
				? ReadTableData(XMLNames.Vehicle_BoostingLimitation, XMLNames.BoostingLimitation_Entry,
					AttributeMappings.BoostingLimitsMapping)
				: null;

		#endregion

		public override bool HybridElectricHDV => true;

		public override VectoSimulationJobType VehicleType
		{
			get => Components.ElectricMachines.Entries.Any(em => em.ElectricMachine.IsIHPC()) ? VectoSimulationJobType.IHPC : VectoSimulationJobType.ParallelHybridVehicle;
		}

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevSxHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Sx_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHevSxHeavyLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override TableData BoostingLimitations => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		#endregion

		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.SerialHybridVehicle;
		}

		public override bool HybridElectricHDV => true;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPevHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_PEV_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationPevHeavyLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationHevPxHeavyLorryDataProviderV24

		public override TableData BoostingLimitations => null;

		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override bool OVC => true;

		#endregion

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.BatteryElectricVehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIEPCHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_IEPC_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIEPCHeavyLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override bool OVC => true;

		#endregion

		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.IEPC_E;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHevIEPCSHeavyLorryDataProviderV24 : AbstractXMLVehicleDataProviderV24
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of VehicleDataProviderHelper

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHevIEPCSHeavyLorryDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits => null;
		public override TableData BoostingLimitations => null;

		#endregion

		public override VectoSimulationJobType VehicleType
		{
			get => VectoSimulationJobType.IEPC_S;
		}

		public override bool HybridElectricHDV => true;
	}

	// ---------------------------------------------------------------------------------------

}