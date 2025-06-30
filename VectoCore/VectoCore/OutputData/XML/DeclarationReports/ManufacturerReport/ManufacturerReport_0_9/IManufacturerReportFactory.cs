using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IManufacturerReportFactory
	{
		IXMLManufacturerReport GetConventionalLorryManufacturerReport();

		IXMLManufacturerReport GetManufacturerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IXmlTypeWriter GetConventionalLorryVehicleType();
		IXmlTypeWriter GetHEV_Px_IHCP_LorryVehicleType();
		IXmlTypeWriter GetHEV_S2_LorryVehicleType();
		IXmlTypeWriter GetHEV_S3_LorryVehicleType();
		IXmlTypeWriter GetHEV_S4_LorryVehicleType();
		IXmlTypeWriter GetHEV_IEPC_S_LorryVehicleType();
		IXmlTypeWriter GetPEV_E2_LorryVehicleType();
		IXmlTypeWriter GetPEV_E3_LorryVehicleType();
		IXmlTypeWriter GetPEV_E4_LorryVehicleType();
		IXmlTypeWriter GetPEV_IEPC_LorryVehicleType();

		IXmlTypeWriter GetExempted_LorryVehicleType();

        IXmlTypeWriter GetMultiple_FCHV_LorryVehicleType();
		IXmlTypeWriter GetMultiple_PEV_LorryVehicleType();
        IXmlTypeWriter GetMultiple_SHEV_LorryVehicleType();

        IXmlTypeWriter GetFCHV_F2_LorryVehicleType();
		IXmlTypeWriter GetFCHV_F3_LorryVehicleType();
		IXmlTypeWriter GetFCHV_F4_LorryVehicleType();
		IXmlTypeWriter GetFCHV_IEPC_LorryVehicleType();

		IXmlTypeWriter GetConventional_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S2_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S3_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S4_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusVehicleType();
		IXmlTypeWriter GetMultiple_FCHV_PrimaryBusVehicleType();
        IXmlTypeWriter GetMultiple_PEV_PrimaryBusVehicleType();
        IXmlTypeWriter GetMultiple_SHEV_PrimaryBusVehicleType();
        IXmlTypeWriter GetFCHV_F2_PrimaryBusVehicleType();
		IXmlTypeWriter GetFCHV_F3_PrimaryBusVehicleType();
		IXmlTypeWriter GetFCHV_F4_PrimaryBusVehicleType();
		IXmlTypeWriter GetFCHV_IEPC_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E2_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E3_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E4_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_IEPC_PrimaryBusVehicleType();

		IXmlTypeWriter GetExempted_PrimaryBusVehicleType();

		IXmlTypeWriter GetConventional_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_CompletedBusVehicleType();

		IXmlTypeWriter GetExempted_CompletedBusVehicleType();


		IXmlTypeWriter GetConventionalLorryComponentsType();
		IXmlTypeWriter GetHEV_Px_IHCP_LorryComponentsType();
		IXmlTypeWriter GetHEV_S2_LorryComponentsType();
		IXmlTypeWriter GetHEV_S3_LorryComponentsType();
		IXmlTypeWriter GetHEV_S4_LorryComponentsType();
		IXmlTypeWriter GetHEV_IEPC_S_LorryComponentsType();
		IXmlTypeWriter GetMultiple_FCHV_LorryComponentsType();
        IXmlTypeWriter GetMultiple_PEV_LorryComponentsType();
        IXmlTypeWriter GetMultiple_SHEV_LorryComponentsType();
        IXmlTypeWriter GetFCHV_F2_LorryComponentsType();
		IXmlTypeWriter GetFCHV_F3_LorryComponentsType();
		IXmlTypeWriter GetFCHV_F4_LorryComponentsType();
		IXmlTypeWriter GetFCHV_IEPC_LorryComponentsType();
		IXmlTypeWriter GetPEV_E2_LorryComponentsType();
		IXmlTypeWriter GetPEV_E3_LorryComponentsType();
		IXmlTypeWriter GetPEV_E4_LorryComponentsType();
		IXmlTypeWriter GetPEV_IEPC_S_LorryComponentsType();
		IXmlTypeWriter GetConventional_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S2_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S3_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S4_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusComponentsType();
        IXmlTypeWriter GetMultiple_FCHV_PrimaryBusComponentsType();
        IXmlTypeWriter GetMultiple_PEV_PrimaryBusComponentsType();
        IXmlTypeWriter GetMultiple_SHEV_PrimaryBusComponentsType();
        IXmlTypeWriter GetFCHV_F2_PrimaryBusComponentsType();
		IXmlTypeWriter GetFCHV_F3_PrimaryBusComponentsType();
		IXmlTypeWriter GetFCHV_F4_PrimaryBusComponentsType();
		IXmlTypeWriter GetFCHV_IEPC_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E2_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E3_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E4_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_IEPC_PrimaryBusComponentsType();
		IXmlTypeWriter GetConventional_CompletedBusComponentsType();
		IXmlTypeWriter GetHEV_CompletedBusComponentsType();
		IXmlTypeWriter GetPEV_CompletedBusComponentsType();


		IReportVehicleOutputGroup GetGeneralVehicleOutputGroup();


		IReportOutputGroup GetGeneralLorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_VehicleSequenceGroup();
		IReportOutputGroup GetFCHV_VehicleSequenceGroup();
		IReportOutputGroup GetPEV_VehicleSequenceGroup();
		IReportOutputGroup GetConventionalLorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_lorryVehicleOutputGroup();
		IReportOutputGroup GetFCHV_lorryVehicleOutputGroup();
        IReportOutputGroup GetPEV_lorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_lorryVehicleOutputSequenceGroup();
		IReportOutputGroup GetPrimaryBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetExemptedPrimaryBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetHEV_PrimaryBusVehicleOutputGroup();
		IReportOutputGroup GetFCHV_PrimaryBusVehicleOutputGroup();


		IXmlTypeWriter GetEngineTorqueLimitationsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetRetarderType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainRetarderType();
        IXmlTypeWriter GetTorqueConverterType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainTorqueConverterType();
        IXmlTypeWriter GetAngleDriveType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainAngleDriveType();
        IXmlTypeWriter GetTransmissionType();
		IXmlAxlePowertrainTypeWriter GetAxlePowertrainTransmissionType();
        IXmlTypeWriter GetElectricMachineType();
		IXmlAxlePowertrainTypeWriter GetAxlePowertrainElectricMachineType();
        IXmlTypeWriter GetElectricMachineGenType();
        IXmlTypeWriter GetGeneratorType();
        IXmlTypeWriter GetAxleGearType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainAxleGearType();
        IXmlTypeWriter GetAxleWheelsType();
		IMRFAdasType GetConventionalADASType();
		IMRFAdasType GetHEVADASType();
		IMRFAdasType GetPEVADASType();
		IXmlTypeWriter GetIEPCSpecifications();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainIEPCSpecifications();
        IXmlTypeWriter GetREESSSpecificationsType();
		IMrfAirdragType GetAirdragType();
		IXmlTypeWriter GetFuelCellSystemType();


		IMRFLorryAuxiliariesType GetConventionalLorryAuxType();
		IMRFLorryAuxiliariesType GetHEV_LorryAuxiliariesType();
		IMRFLorryAuxiliariesType GetFCHV_LorryAuxiliariesType();

		IMRFLorryAuxiliariesType GetPEV_LorryAuxiliariesType();


		IMRFBusAuxiliariesType GetPrimaryBusAuxType_Conventional();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_P();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_S();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_F();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_PEV();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_HEV_S();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_PEV_IEPC();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_HEV_F();

		IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_Conventional_HEV();

		IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_PEV();
		IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_Conventional_HEV();
		IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_FCHV();

		IMRFBusAuxiliariesType GetConventionalCompletedBusAuxType();
		IMRFBusAuxiliariesType GetConventionalCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetConventionalCompletedBusElectricSystemType();

		IMRFBusAuxiliariesType GetHEVCompletedBusAuxType();
		IMRFBusAuxiliariesType GetHEVCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetHEVCompletedBusElectricSystemType();

		IMRFBusAuxiliariesType GetPEVCompletedBusAuxType();
		IMRFBusAuxiliariesType GetPEVCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetPEVCompletedBusElectricSystemType();


		IReportOutputGroup GetPEV_PrimaryBusVehicleOutputGroup();
		IReportOutputGroup GetConventionalCompletedBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetHEVCompletedBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetPEVCompletedBusGeneralVehicleOutputGroup();
		IReportVehicleOutputGroup GetCompletedBusSequenceGroup();
		IReportVehicleOutputGroup GetCompletedBusDimensionSequenceGroup();
		IMrfBusAuxGroup GetCompletedBus_HVACSystemGroup();
		IMrfBusAuxGroup GetCompletedBus_xEVHVACSystemGroup();
		IMrfVehicleType GetBoostingLimitationsType();

	}


}
