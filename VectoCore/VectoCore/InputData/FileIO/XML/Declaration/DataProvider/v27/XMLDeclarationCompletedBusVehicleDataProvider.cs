using System;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
    public abstract class AbstractXMLDeclarationCompletedBusDataProviderV27 : AbstractXMLDeclarationCompletedBusDataProviderV24
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public AbstractXMLDeclarationCompletedBusDataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
            : base(jobData, xmlNode, sourceFile, true) 
        {}

        public override string SimulationToolLicenseNumber => ElementExists("SimulationToolLicenseNumber") 
            ? GetString("SimulationToolLicenseNumber")
            : null;

        public override XmlElement MonitoringNode => _monitoringNode ?? (_monitoringNode = GetNode("MonitoringData", required: false) as XmlElement);

        public override string VehicleMonitoringData => MonitoringReader?.Data;

        public override DynamicChargingTechnology DynamicChargingTechnology => ElementExists("DynamicChargingTechnology")
            ? DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"))
            : DynamicChargingTechnology.None;

        public override Kilogram H2StorageUsableCapacity => ElementExists(XMLNames.Vehicle_H2StorageUsableCapacity)
            ? Convert.ToDouble(GetString(XMLNames.Vehicle_H2StorageUsableCapacity)).SI<Kilogram>()
            : null;

        public override HydrogenStorageTechnology? HydrogenStorageTechnology => ElementExists(XMLNames.Vehicle_H2StorageTechnology)
            ? EnumHelper.ParseEnum<HydrogenStorageTechnology>(GetString(XMLNames.Vehicle_H2StorageTechnology))
            : (HydrogenStorageTechnology?)null;
		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	public class XMLDeclaration_Conventional_CompletedBus_DataProviderV27 : AbstractXMLDeclarationCompletedBusDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_Conventional_CompletedBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_Conventional_CompletedBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : 
            base(jobData, xmlNode, sourceFile) { }
        
        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;
    }

    public class XMLDeclaration_HEV_CompletedBus_DataProviderV27 : AbstractXMLDeclarationCompletedBusDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_HEV_CompletedBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        private VectoSimulationJobType? _vehicleType;

        public XMLDeclaration_HEV_CompletedBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
            base(jobData, xmlNode, sourceFile)
        {}

        public override VectoSimulationJobType VehicleType => (_vehicleType != null) 
            ? _vehicleType.Value 
            : (_vehicleType = GetVehicleType()).Value;

        public override bool HybridElectricHDV => true;

        private VectoSimulationJobType GetVehicleType()
        {
            if (ArchitectureID.IsParallelHybridVehicle())
            {
                return VectoSimulationJobType.ParallelHybridVehicle;
            }
            if (ArchitectureID == ArchitectureID.P_IHPC)
            {
                return VectoSimulationJobType.IHPC;
            }
            if (ArchitectureID.IsSerialHybridVehicle())
            {
                return (ArchitectureID == ArchitectureID.S_IEPC) ? VectoSimulationJobType.IEPC_S : VectoSimulationJobType.SerialHybridVehicle;
            }
            else
            {
                throw new VectoException($"Invalid architecture ({ArchitectureID}) for HEV Completed Bus", ErrorCodes.XML_BAD_DATA);
            }
        }
    }

    public class XMLDeclaration_PEV_CompletedBus_DataProviderV27 : AbstractXMLDeclarationCompletedBusDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_PEV_CompletedBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        private VectoSimulationJobType? _vehicleType;

        public XMLDeclaration_PEV_CompletedBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
            base(jobData, xmlNode, sourceFile)
        {}

        public override VectoSimulationJobType VehicleType => (_vehicleType != null)
            ? _vehicleType.Value
            : (_vehicleType = GetVehicleType()).Value;

        private VectoSimulationJobType GetVehicleType()
        {
            if (ArchitectureID.IsBatteryElectricVehicle())
            {
                return (ArchitectureID == ArchitectureID.E_IEPC) ? VectoSimulationJobType.IEPC_E : VectoSimulationJobType.BatteryElectricVehicle;
            }
            else
            {
                throw new VectoException($"Invalid architecture ({ArchitectureID}) for PEV Completed Bus", ErrorCodes.XML_BAD_DATA);
            }
        }
    }

    public class XMLDeclaration_FCHV_CompletedBus_DataProviderV27 : AbstractXMLDeclarationCompletedBusDataProviderV27
    {
        public new const string XSD_TYPE = "Vehicle_FCHV_CompletedBusDeclarationType";
        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclaration_FCHV_CompletedBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
            base(jobData, xmlNode, sourceFile)
        {}

        public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;

        public override bool HybridElectricHDV => true;
    }

}
