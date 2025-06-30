using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    // ReSharper disable once InconsistentNaming
    public class CIFMockupModule : AbstractNinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
			Kernel.Bind<ICustomerInformationFileFactory>().To<MockupCustomerInformationFileFactory>()
                .WhenInjectedExactlyInto<MockupReportFactory>().InSingletonScope();

        }

        #endregion
    }

	public class MockupCustomerInformationFileFactory : ICustomerInformationFileFactory
    {
        private readonly ICustomerInformationFileFactory _cifFactory;

        public MockupCustomerInformationFileFactory(ICustomerInformationFileFactory cifFactory)
        {
            _cifFactory = cifFactory;
        }

        #region Implementation of ICustomerInformationFileFactory

        public IXMLCustomerReport GetCustomerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
            bool exempted, bool iepc, bool ihpc)
        {
            return new MockupCustomerReport(_cifFactory.GetCustomerReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
        }

        public IXmlTypeWriter GetConventionalLorryVehicleType()
        {
            return _cifFactory.GetConventionalLorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_PxLorryVehicleType()
        {
            return _cifFactory.GetHEV_PxLorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S2_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S2_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S3_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S3_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S4_LorryVehicleType()
        {
            return _cifFactory.GetHEV_S4_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_IEPC_LorryVehicleType()
        {
            return _cifFactory.GetHEV_IEPC_LorryVehicleType();
		}

		public IXmlTypeWriter GetMultiple_FCHV_LorryVehicleType()
		{
			return _cifFactory.GetMultiple_FCHV_LorryVehicleType();
        }

        public IXmlTypeWriter GetFCHV_F2_LorryVehicleType()
		{
			return _cifFactory.GetFCHV_F2_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F3_LorryVehicleType()
		{
			return _cifFactory.GetFCHV_F3_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F4_LorryVehicleType()
		{
			return _cifFactory.GetFCHV_F4_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_IEPC_LorryVehicleType()
		{
			return _cifFactory.GetFCHV_IEPC_LorryVehicleType();
		}

        public IXmlTypeWriter GetMultiple_PEV_LorryVehicleType()
        {
            return _cifFactory.GetMultiple_PEV_LorryVehicleType();
        }

        public IXmlTypeWriter GetMultiple_SHEV_LorryVehicleType()
        {
            return _cifFactory.GetMultiple_SHEV_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E2_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E2_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E3_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E3_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E4_LorryVehicleType()
        {
            return _cifFactory.GetPEV_E4_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_IEPC_LorryVehicleType()
        {
            return _cifFactory.GetPEV_IEPC_LorryVehicleType();
        }

		public IXmlTypeWriter GetExempted_LorryVehicleType()
		{
			return _cifFactory.GetExempted_LorryVehicleType();
		}

		public IXmlTypeWriter GetConventional_CompletedBusVehicleType()
        {
            return _cifFactory.GetConventional_CompletedBusVehicleType();
        }

		public IXmlTypeWriter GetHEV_Px_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_Px_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetHEV_IHPC_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_IHPC_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetHEV_S2_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_S2_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetHEV_S3_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_S3_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetHEV_S4_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_S4_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetHEV_IEPC_S_CompletedBusVehicleType()
		{
			return _cifFactory.GetHEV_IEPC_S_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetPEV_E2_CompletedBusVehicleType()
		{
			return _cifFactory.GetPEV_E2_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetPEV_E3_CompletedBusVehicleType()
		{
			return _cifFactory.GetPEV_E3_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetPEV_E4_CompletedBusVehicleType()
		{
			return _cifFactory.GetPEV_E4_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetPEV_IEPC_CompletedBusVehicleType()
		{
			return _cifFactory.GetPEV_IEPC_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetConventional_SingleBusVehicleType()
		{
			return _cifFactory.GetConventional_SingleBusVehicleType();
		}


		public IXmlTypeWriter GetExemptedCompletedBusVehicleType()
		{
			return _cifFactory.GetExemptedCompletedBusVehicleType();
		}

		public IReportVehicleOutputGroup GetGeneralVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetGeneralVehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetLorryGeneralVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetLorryGeneralVehicleSequenceGroupWriter();
        }

        public IReportOutputGroup GetConventionalLorryVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetConventionalLorryVehicleSequenceGroupWriter();
        }

		public IReportOutputGroup GetEngineGroup()
        {
            return _cifFactory.GetEngineGroup();
        }

        public IReportOutputGroup GetTransmissionGroup()
        {
            return _cifFactory.GetTransmissionGroup();
        }

		public IReportOutputGroup GetTransmissionGroupNoGearbox()
		{
			return _cifFactory.GetTransmissionGroupNoGearbox();
		}

		public IReportOutputGroup GetAxleWheelsGroup()
        {
            return _cifFactory.GetAxleWheelsGroup();
        }

        public IReportOutputGroup GetLorryAuxGroup()
        {
            return _cifFactory.GetLorryAuxGroup();
        }

        public IReportOutputGroup GetConventionalCompletedBusAuxGroup()
        {
            return _cifFactory.GetConventionalCompletedBusAuxGroup();
        }

		public IReportOutputGroup GetHEV_Px_IHPC_CompletedBusAuxGroup()
		{
			return _cifFactory.GetHEV_Px_IHPC_CompletedBusAuxGroup();
		}

		public IReportOutputGroup GetHEV_Sx_CompletedBusAuxGroup()
		{
			return _cifFactory.GetHEV_Sx_CompletedBusAuxGroup();
		}

		public IReportOutputGroup GetPEV_CompletedBusAuxGroup()
		{
			return _cifFactory.GetPEV_CompletedBusAuxGroup();
		}

		public IReportOutputGroup GetHEV_LorryVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetHEV_LorryVehicleSequenceGroupWriter();
        }

		public IReportOutputGroup GetHEV_CompletedBusVehicleSequenceGroupWriter()
		{
			return _cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter();
		}

		public IReportOutputGroup GetHEV_LorryVehicleTypeGroup()
        {
            return _cifFactory.GetHEV_LorryVehicleTypeGroup();
        }

        public IReportOutputGroup GetElectricMachineGroup()
        {
            return _cifFactory.GetElectricMachineGroup();
        }

        public IAxlePowertrainReportOutputGroup GetAxlePowertrainElectricMachineGroup()
		{
			return _cifFactory.GetAxlePowertrainElectricMachineGroup();
        }

        public IAxlePowertrainReportOutputGroup GetAxlePowertrainTransmissionGroup()
		{
			return _cifFactory.GetAxlePowertrainTransmissionGroup();
        }

        public IAxlePowertrainReportOutputGroup GetAxlePowertrainIEPCTransmissionGroup()
		{
			return _cifFactory.GetAxlePowertrainIEPCTransmissionGroup();
		}

        public IReportOutputGroup GetREESSGroup()
        {
            return _cifFactory.GetREESSGroup();
        }

		public IReportOutputGroup GetFuelCellGroup()
		{
            return _cifFactory.GetFuelCellGroup();
        }

        public IReportOutputGroup GetPEV_LorryVehicleTypeGroup()
        {
            return _cifFactory.GetPEV_LorryVehicleTypeGroup();
        }

        public IReportOutputGroup GetPEV_LorryVehicleSequenceGroupWriter()
        {
            return _cifFactory.GetPEV_LorryVehicleSequenceGroupWriter();
        }

		public IReportOutputGroup GetPEV_CompletedBusVehicleSequenceGroupWriter()
		{
			return _cifFactory.GetPEV_CompletedBusVehicleSequenceGroupWriter();
		}

		public IReportOutputGroup GetCompletedBusVehicleTypeGroup()
        {
            return _cifFactory.GetCompletedBusVehicleTypeGroup();
        }

		public IReportOutputGroup GetExemptedCompletedBusVehicleTypeGroup()
		{
			return _cifFactory.GetExemptedCompletedBusVehicleTypeGroup();
		}

		public IReportCompletedBusOutputGroup GetGeneralVehicleSequenceGroupWriterCompletedBus()
        {
            return _cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus();
        }

		public IReportOutputGroup GetConventionalSingleBusAuxGroup()
		{
			return _cifFactory.GetConventionalSingleBusAuxGroup();
		}

		public IReportOutputGroup GetSingleBusVehicleTypeGroup()
		{
			return _cifFactory.GetSingleBusVehicleTypeGroup();
		}

		public ICIFAdasType GetConventionalADASType()
		{
			return _cifFactory.GetConventionalADASType();
		}

		public ICIFAdasType GetHEVADASType()
		{
			return _cifFactory.GetHEVADASType();
		}

		public ICIFAdasType GetPEVADASType()
		{
			return _cifFactory.GetPEVADASType();
		}

		public IReportOutputGroup GetPEVCompletedBusVehicleTypeGroup()
		{
			return _cifFactory.GetPEVCompletedBusVehicleTypeGroup();
		}

		public IReportOutputGroup GetIEPCTransmissionGroup()
		{
			return _cifFactory.GetIEPCTransmissionGroup();
		}

        public IReportOutputGroup GetFuelCell_LorryVehicleSequenceGroupWriter()
		{
            return _cifFactory.GetFuelCell_LorryVehicleSequenceGroupWriter();
		}

		public IReportOutputGroup GetFCHV_LorryVehicleTypeGroup()
		{
            return _cifFactory.GetFCHV_LorryVehicleTypeGroup();
		}

		public IReportOutputGroup GetFCHVLorryVehicleSequenceGroupWriter()
		{
            return _cifFactory.GetFCHVLorryVehicleSequenceGroupWriter();
		}

		#endregion
	}
}