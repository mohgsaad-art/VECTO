using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    // ReSharper disable once InconsistentNaming
    public class MRFMockupModule : AbstractNinjectModule
    {
        #region Overrides of NinjectModule

        public override void Load()
        {
            Kernel.Bind<IManufacturerReportFactory>().To<MockupManufacturerReportFactory>()
                .WhenInjectedExactlyInto<MockupReportFactory>().InSingletonScope();
        }

        #endregion
    }


	public class MockupManufacturerReportFactory : IManufacturerReportFactory
    {
        private IManufacturerReportFactory _manufacturerReportFactoryImplementation;

        #region Implementation of IManufacturerReportFactory

        public MockupManufacturerReportFactory(IManufacturerReportFactory mrfFactory)
        {
            _manufacturerReportFactoryImplementation = mrfFactory;
        }
        public IXMLManufacturerReport GetConventionalLorryManufacturerReport()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalLorryManufacturerReport();
        }

        public IXMLManufacturerReport GetManufacturerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
            ArchitectureID archId, bool exempted, bool iepc, bool ihpc)
        {
            return new MockupManufacturerReport(_manufacturerReportFactoryImplementation.GetManufacturerReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
        }

        public IXmlTypeWriter GetConventionalLorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalLorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_Px_IHCP_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_Px_IHCP_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S2_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S2_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S3_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S3_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_S4_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S4_LorryVehicleType();
        }

        public IXmlTypeWriter GetHEV_IEPC_S_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_IEPC_S_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E2_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E2_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E3_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E3_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_E4_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E4_LorryVehicleType();
        }

        public IXmlTypeWriter GetPEV_IEPC_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_IEPC_LorryVehicleType();
        }

		public IXmlTypeWriter GetExempted_LorryVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetExempted_LorryVehicleType();
		}

		public IXmlTypeWriter GetConventional_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetConventional_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_Px_IHPC_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_S2_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S2_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_S3_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S3_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_S4_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S4_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_IEPC_S_PrimaryBusVehicleType();
		}

        public IXmlTypeWriter GetMultiple_PEV_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_PEV_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetMultiple_SHEV_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_SHEV_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetMultiple_FCHV_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_FCHV_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetFCHV_F2_PrimaryBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F2_PrimaryBusVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F3_PrimaryBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F3_PrimaryBusVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F4_PrimaryBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F4_PrimaryBusVehicleType();
		}

		public IXmlTypeWriter GetFCHV_IEPC_PrimaryBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_IEPC_PrimaryBusVehicleType();
		}

		public IXmlTypeWriter GetPEV_E2_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E2_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetPEV_E3_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E3_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetPEV_E4_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E4_PrimaryBusVehicleType();
        }

        public IXmlTypeWriter GetPEV_IEPC_PrimaryBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_IEPC_PrimaryBusVehicleType();
        }

		public IXmlTypeWriter GetExempted_PrimaryBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetExempted_PrimaryBusVehicleType();
		}

		public IXmlTypeWriter GetConventional_CompletedBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetConventional_CompletedBusVehicleType();
        }

        public IXmlTypeWriter GetHEV_CompletedBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_CompletedBusVehicleType();
        }

        public IXmlTypeWriter GetPEV_CompletedBusVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_CompletedBusVehicleType();
        }

		public IXmlTypeWriter GetExempted_CompletedBusVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetExempted_CompletedBusVehicleType();
		}

		public IXmlTypeWriter GetConventionalLorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalLorryComponentsType();
        }

        public IXmlTypeWriter GetHEV_Px_IHCP_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_Px_IHCP_LorryComponentsType();
        }

        public IXmlTypeWriter GetHEV_S2_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S2_LorryComponentsType();
        }

        public IXmlTypeWriter GetHEV_S3_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S3_LorryComponentsType();
        }

        public IXmlTypeWriter GetHEV_S4_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S4_LorryComponentsType();
        }

        public IXmlTypeWriter GetHEV_IEPC_S_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_IEPC_S_LorryComponentsType();
        }

        public IXmlTypeWriter GetMultiple_FCHV_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_FCHV_LorryComponentsType();
        }

        public IXmlTypeWriter GetMultiple_PEV_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_PEV_LorryComponentsType();
        }

        public IXmlTypeWriter GetMultiple_SHEV_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_SHEV_LorryComponentsType();
        }

        public IXmlTypeWriter GetFCHV_F2_LorryComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F2_LorryComponentsType();
		}

		public IXmlTypeWriter GetFCHV_F3_LorryComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F3_LorryComponentsType();
		}

		public IXmlTypeWriter GetFCHV_F4_LorryComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F4_LorryComponentsType();
		}

		public IXmlTypeWriter GetFCHV_IEPC_LorryComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_IEPC_LorryComponentsType();
		}

		public IXmlTypeWriter GetPEV_E2_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E2_LorryComponentsType();
        }

        public IXmlTypeWriter GetPEV_E3_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E3_LorryComponentsType();
        }

        public IXmlTypeWriter GetPEV_E4_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E4_LorryComponentsType();
        }

        public IXmlTypeWriter GetPEV_IEPC_S_LorryComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_IEPC_S_LorryComponentsType();
        }

        public IXmlTypeWriter GetConventional_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetConventional_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_Px_IHPC_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_S2_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S2_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_S3_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S3_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_S4_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_S4_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_IEPC_S_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetMultiple_FCHV_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_FCHV_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetMultiple_PEV_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_PEV_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetMultiple_SHEV_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_SHEV_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetFCHV_F2_PrimaryBusComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F2_PrimaryBusComponentsType();
		}

		public IXmlTypeWriter GetFCHV_F3_PrimaryBusComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F3_PrimaryBusComponentsType();
		}

		public IXmlTypeWriter GetFCHV_F4_PrimaryBusComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F4_PrimaryBusComponentsType();
		}

		public IXmlTypeWriter GetFCHV_IEPC_PrimaryBusComponentsType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_IEPC_PrimaryBusComponentsType();
		}

		public IXmlTypeWriter GetPEV_E2_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E2_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetPEV_E3_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E3_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetPEV_E4_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_E4_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetPEV_IEPC_PrimaryBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_IEPC_PrimaryBusComponentsType();
        }

        public IXmlTypeWriter GetConventional_CompletedBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetConventional_CompletedBusComponentsType();
        }

        public IXmlTypeWriter GetHEV_CompletedBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_CompletedBusComponentsType();
        }

        public IXmlTypeWriter GetPEV_CompletedBusComponentsType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_CompletedBusComponentsType();
        }

        public IReportVehicleOutputGroup GetGeneralVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetGeneralVehicleOutputGroup();
        }

        public IReportOutputGroup GetGeneralLorryVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetGeneralLorryVehicleOutputGroup();
        }

        public IReportOutputGroup GetHEV_VehicleSequenceGroup()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_VehicleSequenceGroup();
        }

        public IReportOutputGroup GetPEV_VehicleSequenceGroup()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_VehicleSequenceGroup();
        }

        public IReportOutputGroup GetConventionalLorryVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalLorryVehicleOutputGroup();
        }

        public IReportOutputGroup GetHEV_lorryVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_lorryVehicleOutputGroup();
        }

        public IReportOutputGroup GetPEV_lorryVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_lorryVehicleOutputGroup();
        }

        public IReportOutputGroup GetHEV_lorryVehicleOutputSequenceGroup()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_lorryVehicleOutputSequenceGroup();
        }

        public IReportOutputGroup GetPrimaryBusGeneralVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusGeneralVehicleOutputGroup();
        }

		public IReportOutputGroup GetExemptedPrimaryBusGeneralVehicleOutputGroup()
		{
			return _manufacturerReportFactoryImplementation.GetExemptedPrimaryBusGeneralVehicleOutputGroup();
		}

		public IReportOutputGroup GetHEV_PrimaryBusVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_PrimaryBusVehicleOutputGroup();
        }

        public IXmlTypeWriter GetEngineTorqueLimitationsType()
        {
            return _manufacturerReportFactoryImplementation.GetEngineTorqueLimitationsType();
        }

        public IXmlTypeWriter GetEngineType()
        {
            return _manufacturerReportFactoryImplementation.GetEngineType();
        }

        public IXmlTypeWriter GetRetarderType()
        {
            return _manufacturerReportFactoryImplementation.GetRetarderType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainRetarderType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainRetarderType();
        }

        public IXmlTypeWriter GetTorqueConverterType()
        {
            return _manufacturerReportFactoryImplementation.GetTorqueConverterType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainTorqueConverterType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainTorqueConverterType();
        }

        public IXmlTypeWriter GetAngleDriveType()
        {
            return _manufacturerReportFactoryImplementation.GetAngleDriveType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainAngleDriveType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainAngleDriveType();
        }

        public IXmlTypeWriter GetTransmissionType()
        {
            return _manufacturerReportFactoryImplementation.GetTransmissionType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainTransmissionType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainTransmissionType();
        }

        public IXmlTypeWriter GetElectricMachineType()
        {
            return _manufacturerReportFactoryImplementation.GetElectricMachineType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainElectricMachineType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainElectricMachineType();
        }

        public IXmlTypeWriter GetElectricMachineGenType()
        {
            return _manufacturerReportFactoryImplementation.GetElectricMachineGenType();
        }

        public IXmlTypeWriter GetGeneratorType()
        {
            return _manufacturerReportFactoryImplementation.GetGeneratorType();
        }

        public IXmlTypeWriter GetAxleGearType()
        {
            return _manufacturerReportFactoryImplementation.GetAxleGearType();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainAxleGearType()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainAxleGearType();
        }

        public IXmlTypeWriter GetAxleWheelsType()
        {
            return _manufacturerReportFactoryImplementation.GetAxleWheelsType();
        }

        public IMRFAdasType GetConventionalADASType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalADASType();
        }

        public IMRFAdasType GetHEVADASType()
        {
            return _manufacturerReportFactoryImplementation.GetHEVADASType();
        }

        public IMRFAdasType GetPEVADASType()
        {
            return _manufacturerReportFactoryImplementation.GetPEVADASType();
        }

        public IXmlTypeWriter GetIEPCSpecifications()
        {
            return _manufacturerReportFactoryImplementation.GetIEPCSpecifications();
        }

        public IXmlAxlePowertrainTypeWriter GetAxlePowertrainIEPCSpecifications()
        {
            return _manufacturerReportFactoryImplementation.GetAxlePowertrainIEPCSpecifications();
        }

        public IXmlTypeWriter GetREESSSpecificationsType()
        {
            return _manufacturerReportFactoryImplementation.GetREESSSpecificationsType();
        }

        public IXmlTypeWriter GetFuelCellSystemType()
        {
            return _manufacturerReportFactoryImplementation.GetFuelCellSystemType();
        }

        public IMrfAirdragType GetAirdragType()
        {
            return _manufacturerReportFactoryImplementation.GetAirdragType();
        }

        public IMRFLorryAuxiliariesType GetConventionalLorryAuxType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalLorryAuxType();
        }

        public IMRFLorryAuxiliariesType GetHEV_LorryAuxiliariesType()
        {
            return _manufacturerReportFactoryImplementation.GetHEV_LorryAuxiliariesType();
        }

        public IMRFLorryAuxiliariesType GetPEV_LorryAuxiliariesType()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_LorryAuxiliariesType();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusAuxType_Conventional()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusAuxType_Conventional();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_P()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusAuxType_HEV_P();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_S()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusAuxType_HEV_S();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusAuxType_PEV()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusAuxType_PEV();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_HEV_S()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusPneumaticSystemType_HEV_S();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_PEV_IEPC()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusPneumaticSystemType_PEV_IEPC();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_Conventional_HEV()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusElectricSystemType_Conventional_HEV();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_PEV()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusElectricSystemType_PEV();
        }

        public IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_Conventional_HEV()
        {
            return _manufacturerReportFactoryImplementation.GetPrimaryBusHVACSystemType_Conventional_HEV();
        }

		public IMRFBusAuxiliariesType GetConventionalCompletedBusAuxType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalCompletedBusAuxType();
        }

        public IMRFBusAuxiliariesType GetConventionalCompletedBus_HVACSystemType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalCompletedBus_HVACSystemType();
        }

        public IMRFBusAuxiliariesType GetConventionalCompletedBusElectricSystemType()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalCompletedBusElectricSystemType();
        }

		public IMRFBusAuxiliariesType GetHEVCompletedBusAuxType()
		{
			return _manufacturerReportFactoryImplementation.GetHEVCompletedBusAuxType();
		}

		public IMRFBusAuxiliariesType GetHEVCompletedBus_HVACSystemType()
		{
			return _manufacturerReportFactoryImplementation.GetHEVCompletedBus_HVACSystemType();
		}

		public IMRFBusAuxiliariesType GetHEVCompletedBusElectricSystemType()
		{
			return _manufacturerReportFactoryImplementation.GetHEVCompletedBusElectricSystemType();
		}

		public IMRFBusAuxiliariesType GetPEVCompletedBusAuxType()
		{
			return _manufacturerReportFactoryImplementation.GetPEVCompletedBusAuxType();
		}

		public IMRFBusAuxiliariesType GetPEVCompletedBus_HVACSystemType()
		{
			return _manufacturerReportFactoryImplementation.GetPEVCompletedBus_HVACSystemType();
		}

		public IMRFBusAuxiliariesType GetPEVCompletedBusElectricSystemType()
		{
			return _manufacturerReportFactoryImplementation.GetPEVCompletedBusElectricSystemType();
		}

		public IReportOutputGroup GetPEV_PrimaryBusVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetPEV_PrimaryBusVehicleOutputGroup();
        }

        public IReportOutputGroup GetConventionalCompletedBusGeneralVehicleOutputGroup()
        {
            return _manufacturerReportFactoryImplementation.GetConventionalCompletedBusGeneralVehicleOutputGroup();
        }

		public IReportOutputGroup GetHEVCompletedBusGeneralVehicleOutputGroup()
		{
			return _manufacturerReportFactoryImplementation.GetHEVCompletedBusGeneralVehicleOutputGroup();
		}

		public IReportOutputGroup GetPEVCompletedBusGeneralVehicleOutputGroup()
		{
			return _manufacturerReportFactoryImplementation.GetPEVCompletedBusGeneralVehicleOutputGroup();
		}

		public IReportVehicleOutputGroup GetCompletedBusSequenceGroup()
        {
            return _manufacturerReportFactoryImplementation.GetCompletedBusSequenceGroup();
        }

        public IReportVehicleOutputGroup GetCompletedBusDimensionSequenceGroup()
        {
            return _manufacturerReportFactoryImplementation.GetCompletedBusDimensionSequenceGroup();
        }

        public IMrfBusAuxGroup GetCompletedBus_HVACSystemGroup()
        {
            return _manufacturerReportFactoryImplementation.GetCompletedBus_HVACSystemGroup();
        }

		public IMrfBusAuxGroup GetCompletedBus_xEVHVACSystemGroup()
		{
			return _manufacturerReportFactoryImplementation.GetCompletedBus_xEVHVACSystemGroup();
		}

		public IMrfVehicleType GetBoostingLimitationsType()
        {
            return _manufacturerReportFactoryImplementation.GetBoostingLimitationsType();
        }

        public IXmlTypeWriter GetMultiple_FCHV_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_FCHV_LorryVehicleType();
        }

        public IXmlTypeWriter GetMultiple_PEV_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_PEV_LorryVehicleType();
        }

        public IXmlTypeWriter GetMultiple_SHEV_LorryVehicleType()
        {
            return _manufacturerReportFactoryImplementation.GetMultiple_SHEV_LorryVehicleType();
        }

        public IXmlTypeWriter GetFCHV_F2_LorryVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F2_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F3_LorryVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F3_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_F4_LorryVehicleType()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_F4_LorryVehicleType();
		}

		public IXmlTypeWriter GetFCHV_IEPC_LorryVehicleType()
		{
            return _manufacturerReportFactoryImplementation.GetFCHV_IEPC_LorryVehicleType();
		}

        public IReportOutputGroup GetFCHV_VehicleSequenceGroup()
		{
            return _manufacturerReportFactoryImplementation.GetFCHV_VehicleSequenceGroup();
		}

		public IReportOutputGroup GetFCHV_lorryVehicleOutputGroup()
		{
            return _manufacturerReportFactoryImplementation.GetFCHV_lorryVehicleOutputGroup();
		}

		public IReportOutputGroup GetFCHV_PrimaryBusVehicleOutputGroup()
		{
			return _manufacturerReportFactoryImplementation.GetFCHV_PrimaryBusVehicleOutputGroup();
		}

		public IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_F()
		{
            return _manufacturerReportFactoryImplementation.GetPrimaryBusAuxType_HEV_F();
		}

		public IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_HEV_F()
		{
			return _manufacturerReportFactoryImplementation.GetPrimaryBusPneumaticSystemType_HEV_F();
		}

		public IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_FCHV()
		{
			return _manufacturerReportFactoryImplementation.GetPrimaryBusHVACSystemType_FCHV();
		}

		public IMRFLorryAuxiliariesType GetFCHV_LorryAuxiliariesType()
		{
            return _manufacturerReportFactoryImplementation.GetFCHV_LorryAuxiliariesType();
		}

		#endregion
	}
}