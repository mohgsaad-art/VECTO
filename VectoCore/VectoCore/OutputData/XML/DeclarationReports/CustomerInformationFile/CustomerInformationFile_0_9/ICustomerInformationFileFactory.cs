using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    public interface ICustomerInformationFileFactory
    {
		IXMLCustomerReport GetCustomerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IXmlTypeWriter GetConventionalLorryVehicleType();
		IXmlTypeWriter GetHEV_PxLorryVehicleType();
		IXmlTypeWriter GetHEV_S2_LorryVehicleType();
		IXmlTypeWriter GetHEV_S3_LorryVehicleType();
		IXmlTypeWriter GetHEV_S4_LorryVehicleType();
		IXmlTypeWriter GetHEV_IEPC_LorryVehicleType();
		IXmlTypeWriter GetMultiple_FCHV_LorryVehicleType();
        IXmlTypeWriter GetFCHV_F2_LorryVehicleType();
		IXmlTypeWriter GetFCHV_F3_LorryVehicleType();
		IXmlTypeWriter GetFCHV_F4_LorryVehicleType();
		IXmlTypeWriter GetFCHV_IEPC_LorryVehicleType();
        IXmlTypeWriter GetMultiple_PEV_LorryVehicleType();
        IXmlTypeWriter GetMultiple_SHEV_LorryVehicleType();
        IXmlTypeWriter GetPEV_E2_LorryVehicleType();
		IXmlTypeWriter GetPEV_E3_LorryVehicleType();
		IXmlTypeWriter GetPEV_E4_LorryVehicleType();
		IXmlTypeWriter GetPEV_IEPC_LorryVehicleType();

		IXmlTypeWriter GetExempted_LorryVehicleType();

		IXmlTypeWriter GetConventional_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_Px_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_IHPC_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_S2_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_S3_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_S4_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_IEPC_S_CompletedBusVehicleType();

		IXmlTypeWriter GetPEV_E2_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_E3_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_E4_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_IEPC_CompletedBusVehicleType();


		IXmlTypeWriter GetConventional_SingleBusVehicleType();



		//IXmlTypeWriter GetPEV_CompletedBusVehicleType();

		IXmlTypeWriter GetExemptedCompletedBusVehicleType();


		IReportVehicleOutputGroup GetGeneralVehicleSequenceGroupWriter();
		IReportOutputGroup GetLorryGeneralVehicleSequenceGroupWriter();
		IReportOutputGroup GetFCHVLorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetConventionalLorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetEngineGroup();
		IReportOutputGroup GetTransmissionGroup();
		IReportOutputGroup GetTransmissionGroupNoGearbox();
		IReportOutputGroup GetAxleWheelsGroup();
		IReportOutputGroup GetLorryAuxGroup();
		IReportOutputGroup GetConventionalCompletedBusAuxGroup();
		IReportOutputGroup GetHEV_Px_IHPC_CompletedBusAuxGroup();
		IReportOutputGroup GetHEV_Sx_CompletedBusAuxGroup();
		IReportOutputGroup GetPEV_CompletedBusAuxGroup();

		IReportOutputGroup GetHEV_LorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetFuelCell_LorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetHEV_CompletedBusVehicleSequenceGroupWriter();
		IReportOutputGroup GetHEV_LorryVehicleTypeGroup();
		IReportOutputGroup GetFCHV_LorryVehicleTypeGroup();
		IReportOutputGroup GetElectricMachineGroup();
		IAxlePowertrainReportOutputGroup GetAxlePowertrainElectricMachineGroup();
        IAxlePowertrainReportOutputGroup GetAxlePowertrainTransmissionGroup();
		IAxlePowertrainReportOutputGroup GetAxlePowertrainIEPCTransmissionGroup();
        IReportOutputGroup GetREESSGroup();
        IReportOutputGroup GetFuelCellGroup();
        IReportOutputGroup GetPEV_LorryVehicleTypeGroup();

		IReportOutputGroup GetPEV_LorryVehicleSequenceGroupWriter();
		IReportOutputGroup GetPEV_CompletedBusVehicleSequenceGroupWriter();
		IReportOutputGroup GetCompletedBusVehicleTypeGroup();
		IReportOutputGroup GetExemptedCompletedBusVehicleTypeGroup();
		
		IReportCompletedBusOutputGroup GetGeneralVehicleSequenceGroupWriterCompletedBus();

		IReportOutputGroup GetConventionalSingleBusAuxGroup();


		IReportOutputGroup GetSingleBusVehicleTypeGroup();


		ICIFAdasType GetConventionalADASType();
		ICIFAdasType GetHEVADASType();
		ICIFAdasType GetPEVADASType();

		IReportOutputGroup GetPEVCompletedBusVehicleTypeGroup();
		IReportOutputGroup GetIEPCTransmissionGroup();
    }
}
