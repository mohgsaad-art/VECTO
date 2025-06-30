using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public abstract class AbstractXMLDeclarationMediumLorryVehicleDataProviderV27 : AbstractXMLVehicleDataProviderV27
    {
        public AbstractXMLDeclarationMediumLorryVehicleDataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) 
        {
            if ((VehicleCategory == VehicleCategory.Van) && !ElementExists(XMLNames.Vehicle_CargoVolume))
            {
                throw new VectoException("Medium lorries with type Van require the input parameter cargo volume!");
            }
        }

        public override bool? SleeperCab => false;

        public override bool VocationalVehicle => false;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;

        public override XmlElement PTONode => null;

        public override CubicMeter CargoVolume => ElementExists(XMLNames.Vehicle_CargoVolume) ? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>() : null;
    }

    public class XMLDeclaration_Conventional_MediumLorry_DataProviderV27 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Conventional_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Conventional_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : 
            base(jobData, xmlNode,sourceFile)
        { }

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            //Allow here all conventionals and disallow non-H2 fueled at job level.
        }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;
    }

    public class XMLDeclaration_PHEV_MediumLorry_DataProviderV27 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Px_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PHEV_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {
            if (!OVC && BatteryOnlyMode)
            {
                throw new VectoException("For PHEV vehicles, BatteryOnlyMode should be false if OVC is false.");
            }
        }

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            if (DynamicChargingTechnology == DynamicChargingTechnology.None)
            {
                base.CheckVehicleAllowed("Vehicle does not have in-motion charging.");
            }
        }

        public override VectoSimulationJobType VehicleType => Components.ElectricMachines.Entries.Any(em => em.ElectricMachine.IsIHPC())
            ? VectoSimulationJobType.IHPC
            : VectoSimulationJobType.ParallelHybridVehicle;

        public override string PowertrainPositionPrefix => "P";

        public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_SHEV_MediumLorry_DataProviderV27 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-Sx_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            if (DynamicChargingTechnology == DynamicChargingTechnology.None)
            {
                base.CheckVehicleAllowed("Vehicle does not have in-motion charging.");
            }
        }

        public override string PowertrainPositionPrefix => "E";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.SerialHybridVehicle;

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_SHEV_IEPC_MediumLorry_DataProviderV27 : XMLDeclaration_SHEV_MediumLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_SHEV_IEPC_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_S;
    }

    public class XMLDeclaration_PEV_MediumLorry_DataProviderV27 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_PEV_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
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

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            if (DynamicChargingTechnology == DynamicChargingTechnology.None)
            {
                base.CheckVehicleAllowed("Vehicle does not have in-motion charging.");
            }
        }

        public override string PowertrainPositionPrefix => "E";

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.BatteryElectricVehicle;

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

    public class XMLDeclaration_PEV_IEPC_MediumLorry_DataProviderV27 : XMLDeclaration_PEV_MediumLorry_DataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_IEPC_MediumLorryDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_PEV_IEPC_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) 
        {}

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.IEPC_E;
    }

    public class XMLDeclaration_FCHV_MediumLorry_DataProviderV27 : AbstractXMLDeclarationMediumLorryVehicleDataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_Fx_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_FCHV_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile) { }

        protected override void CheckVehicleAllowed(string extraMessage = "")
        {
            //Do nothing -> Vehicle Allowed
        }

        public override string PowertrainPositionPrefix => "F";

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;
		
		public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));

        public override IList<ITorqueLimitInputData> TorqueLimits => null;
    }

	public class XMLDeclaration_FCHV_IEPC_MediumLorry_DataProviderV27 : XMLDeclaration_FCHV_MediumLorry_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_IEPC_MediumLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_IEPC_MediumLorry_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV_IEPC;

        public override XmlElement PTONode => null;

        public override IPTOTransmissionInputData GetPTOTransmissionInputData(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) => null;
    }
}
