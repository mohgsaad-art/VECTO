using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
	public abstract class AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27 : AbstractXMLVehicleDataProviderV27
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27(
			IXMLDeclarationJobInputData jobData,
			XmlNode xmlNode,
			string sourceFile)
			: base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            base.CheckVehicleAllowed("Buses not supported yet.");
        }

        #region Overrides of XMLDeclarationVehicleDataProviderV10

        public override bool? SleeperCab => false;

		public override CubicMeter CargoVolume => null;

		public override XmlElement PTONode => null;

		public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		public override Kilogram CurbMassChassis => null;

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override Meter EntranceHeight => null;

		public override bool VocationalVehicle => false;
		
		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	public class XMLDeclaration_Conventional_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
	{
        public new const string XSD_TYPE = "Vehicle_Conventional_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Conventional_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;
    }

    public class XMLDeclaration_PHEV_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Px_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PHEV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
		{}

        public override string PowertrainPositionPrefix => "P";
        
        public override VectoSimulationJobType VehicleType => Components.ElectricMachines.Entries.AsEnumerable().Any(em => em.ElectricMachine.IsIHPC())
                ? VectoSimulationJobType.IHPC
                : VectoSimulationJobType.ParallelHybridVehicle;

        public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_PEV_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_PEV_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override string PowertrainPositionPrefix => "E";

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.BatteryElectricVehicle;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_PEV_IEPC_PrimaryBus_DataProviderV27 : XMLDeclaration_PEV_PrimaryBus_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_IEPC_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_IEPC_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_E;
    }

    public class XMLDeclaration_SHEV_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Sx_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        { }

        public override string PowertrainPositionPrefix => "E";

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.SerialHybridVehicle;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override bool HybridElectricHDV => true;
    }

    public class XMLDeclaration_SHEV_IEPC_PrimaryBus_DataProviderV27 : XMLDeclaration_SHEV_PrimaryBus_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_IEPC_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_S;
    }

    public class XMLDeclaration_FCHV_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
	{
		public new const string XSD_TYPE = "Vehicle_FCHV_Fx_PrimaryBusDeclarationType"; 
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

        public override string PowertrainPositionPrefix => "F";

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;

		public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

	public class XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27 : XMLDeclaration_FCHV_PrimaryBus_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_IEPC_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV_IEPC;
	}

    public class XMLDeclaration_Multiple_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
    {
        public XMLDeclaration_Multiple_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => null;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override ArchitectureID ArchitectureIDPwt2 => ArchitectureIDHelper.Parse(GetString("ArchitectureIDPwt2"));

        public override TableData BoostingLimitations => null;

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = -1) => null;
        
        public override RetarderType GetRetarderType(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
        {
            var node = GetNodes(XMLNames.Vehicle_RetarderType).Cast<XmlNode>().FirstOrDefault(x => int.Parse(GetAttribute(x, "axleNumber")) == axleNumber);

            if (node == null)
            {
                throw new VectoException($"No {XMLNames.Vehicle_RetarderType} found for axle number: {axleNumber}");
            }

            return RetarderTypeHelper.Parse(node.InnerText);
        }

        public override double GetRetarderRatio(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
        {
            var node = GetNodes(XMLNames.Vehicle_RetarderRatio).Cast<XmlNode>().FirstOrDefault(x => int.Parse(GetAttribute(x, "axleNumber")) == axleNumber);

            return (node != null) ? double.Parse(node.InnerText) : 0;
        }

        public override AngledriveType GetAngledriveType(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
        {
            var node = GetNodes(XMLNames.Vehicle_AngledriveType).Cast<XmlNode>().FirstOrDefault(x => int.Parse(GetAttribute(x, "axleNumber")) == axleNumber);

            if (node == null)
            {
                throw new VectoException($"No {XMLNames.Vehicle_AngledriveType} found for axle number: {axleNumber}");
            }

            return node.InnerText.ParseEnum<AngledriveType>();
        }
    }

    public class XMLDeclaration_Multiple_FCHV_PrimaryBus_DataProviderV27 : XMLDeclaration_Multiple_PrimaryBus_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_FCHV_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_FCHV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => "F";

        public override bool HybridElectricHDV => true;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_FCHV;
    }

    public class XMLDeclaration_Multiple_PEV_PrimaryBus_DataProviderV27 : XMLDeclaration_Multiple_PrimaryBus_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_PEV_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_PEV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => "E";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_PEV;
    }

    public class XMLDeclaration_Multiple_SHEV_PrimaryBus_DataProviderV27 : XMLDeclaration_Multiple_PrimaryBus_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_SHEV_PrimaryBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_SHEV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => "S";

        public override bool HybridElectricHDV => true;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_SHEV;
    }
}
