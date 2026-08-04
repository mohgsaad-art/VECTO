#if CERTIFICATION_RELEASE || RELEASE_CANDIDATE
#define PROHIBIT_NEW_XML
#endif

//#define PROHIBIT_NEW_XML

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public class XMLDeclaration_Conventional_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Conventional_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Conventional_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {}

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;
    }

    public class XMLDeclaration_PHEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Px_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PHEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {
            if (!OVC && BatteryOnlyMode)
            {
                throw new VectoException("For PHEV vehicles, BatteryOnlyMode should be false if OVC is false.");
            }
        }

        public override string PowertrainPositionPrefix => "P";

        public override bool HybridElectricHDV => true;

        public override VectoSimulationJobType VehicleType => Components.ElectricMachines.Entries.Any(em => em.ElectricMachine.IsIHPC())
            ? VectoSimulationJobType.IHPC
            : VectoSimulationJobType.ParallelHybridVehicle;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override XmlElement PTONode => (VehicleType == VectoSimulationJobType.IHPC) ? null : base.PTONode;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => 
            (VehicleType == VectoSimulationJobType.IHPC) ? null : base.GetPTOTransmissionInputData();
    }

    public class XMLDeclaration_SHEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Sx_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override string PowertrainPositionPrefix => "E";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.SerialHybridVehicle;

        public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_SHEV_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_S;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;
    }

    public class XMLDeclaration_PEV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_PEV_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        {
			if (!OVC)
            {
                throw new VectoException("OVC must be set to true for PEVs.");
            }

            if (!BatteryOnlyMode)
            {
                throw new VectoException("BatteryOnlyMode must be set to true for PEVs.");
            }
        }

        public override string PowertrainPositionPrefix => "E";

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.BatteryElectricVehicle;
    }

    public class XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_PEV_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_IEPC_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        {}

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_E;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;
    }

    public class XMLDeclaration_FCHV_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_Fx_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public override string PowertrainPositionPrefix => "F";

		public XMLDeclaration_FCHV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile)
		{}

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override TableData BoostingLimitations => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;

        public override bool HybridElectricHDV => true;
    }

	public class XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27 : XMLDeclaration_FCHV_HeavyLorry_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_IEPC_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV_IEPC;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;
    }

    public abstract class XMLDeclaration_Multiple_HeavyLorry_DataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public XMLDeclaration_Multiple_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        {
#if PROHIBIT_NEW_XML
            if (ArchitectureID.IsOneOf(ArchitectureID.E2, ArchitectureID.F2))
            {
                throw new VectoException($"Architecture {ArchitectureID} is not supported yet!");
            }

            if (ArchitectureIDPwt2.IsOneOf(ArchitectureID.E2, ArchitectureID.F2))
            {
                throw new VectoException($"Architecture {ArchitectureIDPwt2} is not supported yet!");
            }
#endif
        }

        public override string PowertrainPositionPrefix => null;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override ArchitectureID ArchitectureIDPwt2 => ArchitectureIDHelper.Parse(GetString("ArchitectureIDPwt2"));

        public override TableData BoostingLimitations => null;

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
        {
            return PTOReader.GetPTOInputData(axleNumber);
        }

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

    public class XMLDeclaration_Multiple_FCHV_HeavyLorry_DataProviderV27 : XMLDeclaration_Multiple_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_FCHV_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_FCHV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => "F";

        public override bool HybridElectricHDV => true;

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_FCHV;
    }

    public class XMLDeclaration_Multiple_PEV_HeavyLorry_DataProviderV27 : XMLDeclaration_Multiple_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_PEV_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_PEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        { }

        public override string PowertrainPositionPrefix => "E";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_PEV;
    }

    public class XMLDeclaration_Multiple_SHEV_HeavyLorry_DataProviderV27 : XMLDeclaration_Multiple_HeavyLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Multiple_SHEV_HeavyLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Multiple_SHEV_HeavyLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile)
        {
#if PROHIBIT_NEW_XML
            throw new VectoException("XML Jobs for SHEV with multiple powertrains are not yet supported!");
#endif
        }

        public override bool HybridElectricHDV => true;

        public override string PowertrainPositionPrefix => "S";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.Multiple_SHEV;
    }
}
