using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    public class VIFMockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Kernel.Bind<IVIFReportFactory>().To<MockupVIFReportFactory>()
				.WhenInjectedExactlyInto<MockupReportFactory>().InSingletonScope();

			Kernel.Bind<IVIFReportInterimFactory>().To<MockupVIFReportFactory>()
				.WhenInjectedExactlyInto<MockupReportFactory>().InSingletonScope();
		}

		#endregion

		public class MockupVIFReportFactory : IVIFReportFactory, IVIFReportInterimFactory
		{
			private IVIFReportFactory _vifReportFactoryImplementation;
			private readonly IVIFReportInterimFactory _interimFactory;


			public MockupVIFReportFactory(IVIFReportFactory vifReportFactoryImplementation, IVIFReportInterimFactory interimFactory)
			{
				_vifReportFactoryImplementation = vifReportFactoryImplementation;
				_interimFactory = interimFactory;
			}


			#region Implementation of IVIFReportFactory

			public IXMLVehicleInformationFile GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType, ArchitectureID archId, 
				bool exempted, bool iepc, bool ihpc)
			{
				return new MockupPrimaryVehicleInformationFile(
					_vifReportFactoryImplementation.GetVIFReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
			}
			
			public IXMLMultistepIntermediateReport GetInterimVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
				ArchitectureID archId, bool exempted, bool iepc, bool ihpc)
			{
				return new MockupInterimVehicleInformationFile(
					_interimFactory.GetInterimVIFReport(vehicleType, jobType, archId, exempted, iepc, ihpc));
			}

			IXmlMultistepTypeWriter IVIFReportInterimFactory.GetConventionalVehicleType()
			{
				return _interimFactory.GetConventionalVehicleType();
			}

			public IXmlMultistepTypeWriter GetHEVVehicleType()
			{
				return _interimFactory.GetHEVVehicleType();
			}

			public IXmlMultistepTypeWriter GetPEVVehicleType()
			{
				return _interimFactory.GetPEVVehicleType();
			}

			public IXmlMultistepTypeWriter GetIEPCVehicleType()
			{
				return _interimFactory.GetIEPCVehicleType();
			}

			IXmlMultistepTypeWriter IVIFReportInterimFactory.GetExemptedVehicleType()
			{
				return _interimFactory.GetExemptedVehicleType();
			}

			public IReportMultistepCompletedBusOutputGroup GetCompletedBusGeneralParametersGroup()
			{
				return _interimFactory.GetCompletedBusGeneralParametersGroup();
			}

			public IReportMultistepCompletedBusOutputGroup GetCompletedBusParametersGroup()
			{
				return _interimFactory.GetCompletedBusParametersGroup();
			}

			public IReportMultistepCompletedBusOutputGroup GetCompletedBusPassengerCountGroup()
			{
				return _interimFactory.GetCompletedBusPassengerCountGroup();
			}

			public IReportMultistepCompletedBusOutputGroup GetCompletedBusDimensionsGroup()
			{
				return _interimFactory.GetCompletedBusDimensionsGroup();
			}

			public IReportMultistepCompletedBusTypeWriter GetInterimConventionalAuxiliariesType()
			{
				return _interimFactory.GetInterimConventionalAuxiliariesType();
			}

			public IReportMultistepCompletedBusTypeWriter GetInterimxEVAuxiliariesType()
			{
				return _interimFactory.GetInterimxEVAuxiliariesType();
			}

			public IVIFFAdasType GetConventionalInterimADASType()
			{
				return _interimFactory.GetConventionalInterimADASType();
			}

			public IVIFFAdasType GetHEVInterimADASType()
			{
				return _interimFactory.GetHEVInterimADASType();
			}

			public IVIFFAdasType GetPEVInterimADASType()
			{
				return _interimFactory.GetPEVInterimADASType();
			}

			public IVIFFAdasType GetIEPCInterimADASType()
			{
				return _interimFactory.GetIEPCInterimADASType();
			}

			public IReportMultistepCompletedBusTypeWriter GetConventionalInterimComponentsType()
			{
				return _interimFactory.GetConventionalInterimComponentsType();
			}

			public IReportMultistepCompletedBusTypeWriter GetxEVInterimComponentsType()
			{
				return _interimFactory.GetxEVInterimComponentsType();
			}

			public IReportMultistepCompletedBusTypeWriter GetInterimAirdragType()
			{
				return _interimFactory.GetInterimAirdragType();
			}

			public IXmlTypeWriter GetConventionalVehicleType()
			{
				return _vifReportFactoryImplementation.GetConventionalVehicleType();
			}

			public IXmlTypeWriter GetHevIepcSVehicleType()
			{
				return _vifReportFactoryImplementation.GetHevIepcSVehicleType();
			}

			public IXmlTypeWriter GetHevPxVehicleType()
			{
				return _vifReportFactoryImplementation.GetHevPxVehicleType();
			}

			public IXmlTypeWriter GetHevS2VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevS2VehicleType();
			}

			public IXmlTypeWriter GetHevS3VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevS3VehicleType();
			}

			public IXmlTypeWriter GetHevS4VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevS4VehicleType();
			}

            public IXmlTypeWriter GetMultiplePEVVehicleType()
            {
                return _vifReportFactoryImplementation.GetMultiplePEVVehicleType();
            }

            public IXmlTypeWriter GetMultipleSHEVVehicleType()
            {
                return _vifReportFactoryImplementation.GetMultipleSHEVVehicleType();
            }

            public IXmlTypeWriter GetMultipleFCHVVehicleType()
			{
				return _vifReportFactoryImplementation.GetMultipleFCHVVehicleType();
            }

            public IXmlTypeWriter GetHevF2VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevF2VehicleType();
			}

			public IXmlTypeWriter GetHevF3VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevF3VehicleType();
			}

			public IXmlTypeWriter GetHevF4VehicleType()
			{
				return _vifReportFactoryImplementation.GetHevF4VehicleType();
			}

			public IXmlTypeWriter GetHevIepcFVehicleType()
			{
				return _vifReportFactoryImplementation.GetHevIepcFVehicleType();
			}

			public IXmlTypeWriter GetPevE2VehicleType()
			{
				return _vifReportFactoryImplementation.GetPevE2VehicleType();
			}

			public IXmlTypeWriter GetPevE3VehicleType()
			{
				return _vifReportFactoryImplementation.GetPevE3VehicleType();
			}

			public IXmlTypeWriter GetPevE4VehicleType()
			{
				return _vifReportFactoryImplementation.GetPevE4VehicleType();
			}

			public IXmlTypeWriter GetPevIEPCVehicleType()
			{
				return _vifReportFactoryImplementation.GetPevIEPCVehicleType();
			}

			public IXmlTypeWriter GetExemptedVehicleType()
			{
				return _vifReportFactoryImplementation.GetExemptedVehicleType();
			}

			public IXmlTypeWriter GetConventionalComponentType()
			{
				return _vifReportFactoryImplementation.GetConventionalComponentType();
			}

			public IXmlTypeWriter GetHevIepcSComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevIepcSComponentVIFType();
			}

			public IXmlTypeWriter GetHevPxComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevPxComponentVIFType();
			}

			public IXmlTypeWriter GetHevS2ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevS2ComponentVIFType();
			}

			public IXmlTypeWriter GetHevS3ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevS3ComponentVIFType();
			}

			public IXmlTypeWriter GetHevS4ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevS4ComponentVIFType();
			}

			public IXmlTypeWriter GetHevF2ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevF2ComponentVIFType();
			}

			public IXmlTypeWriter GetHevF3ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevF3ComponentVIFType();
			}

			public IXmlTypeWriter GetHevF4ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevF4ComponentVIFType();
			}

			public IXmlTypeWriter GetHevIepcFComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetHevIepcFComponentVIFType();
			}

			public IXmlTypeWriter GetPevE2ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetPevE2ComponentVIFType();
			}

			public IXmlTypeWriter GetPevE3ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetPevE3ComponentVIFType();
			}

			public IXmlTypeWriter GetPevE4ComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetPevE4ComponentVIFType();
			}

			public IXmlTypeWriter GetPevIEPCComponentVIFType()
			{
				return _vifReportFactoryImplementation.GetPevIEPCComponentVIFType();
			}

			public IVIFFAdasType GetConventionalADASType()
			{
				return _vifReportFactoryImplementation.GetConventionalADASType();
			}

			public IVIFFAdasType GetHEVADASType()
			{
				return _vifReportFactoryImplementation.GetHEVADASType();
			}

			public IVIFFAdasType GetPEVADASType()
			{
				return _vifReportFactoryImplementation.GetPEVADASType();
			}

			public IVIFFAdasType GetIEPCADASType()
			{
				return _vifReportFactoryImplementation.GetIEPCADASType();
			}

			public IXmlTypeWriter GetTorqueConvertType()
			{
				return _vifReportFactoryImplementation.GetTorqueConvertType();
			}

			public IXmlTypeWriter GetIepcType()
			{
				return _vifReportFactoryImplementation.GetIepcType();
			}

			public IXmlTypeWriter GetTorqueLimitsType()
			{
				return _vifReportFactoryImplementation.GetTorqueLimitsType();
			}

			public IXmlTypeWriter GetTransmissionType()
			{
				return _vifReportFactoryImplementation.GetTransmissionType();
			}

			public IReportOutputGroup GetConventionalVehicleGroup()
			{
				return _vifReportFactoryImplementation.GetConventionalVehicleGroup();
			}

			public IReportOutputGroup GetPrimaryBusGeneralParameterGroup()
			{
				return _vifReportFactoryImplementation.GetPrimaryBusGeneralParameterGroup();
			}

			public IReportOutputGroup GetPrimaryBusChassisParameterGroup()
			{
				throw new NotImplementedException();
			}

			public IReportOutputGroup GetPrimaryBusRetarderParameterGroup()
			{
				return _vifReportFactoryImplementation.GetPrimaryBusRetarderParameterGroup();
			}

			public IReportOutputGroup GetPrimaryBusXevParameterGroup()
			{
				return _vifReportFactoryImplementation.GetPrimaryBusXevParameterGroup();
			}

			public IReportOutputGroup GetHevIepcSVehicleParameterGroup()
			{
				return _vifReportFactoryImplementation.GetHevIepcSVehicleParameterGroup();
			}

			public IReportOutputGroup GetHevSxVehicleParameterGroup()
			{
				return _vifReportFactoryImplementation.GetHevSxVehicleParameterGroup();
			}

			public IReportOutputGroup GetPevExVehicleParmeterGroup()
			{
				return _vifReportFactoryImplementation.GetPevExVehicleParmeterGroup();
			}

			public IReportOutputGroup GetPevIEPCVehicleParmeterGroup()
			{
				return _vifReportFactoryImplementation.GetPevIEPCVehicleParmeterGroup();
			}

			public IReportOutputGroup GetHevPxVehicleParameterGroup()
			{
				return _vifReportFactoryImplementation.GetHevPxVehicleParameterGroup();
			}

			public IReportOutputGroup GetExemptedVehicleParameterGroup()
			{
				return _vifReportFactoryImplementation.GetExemptedVehicleParameterGroup();
			}

			public IXmlTypeWriter GetAngelDriveType()
			{
				return _vifReportFactoryImplementation.GetAngelDriveType();
			}

			public IXmlTypeWriter GetRetarderType()
			{
				return _vifReportFactoryImplementation.GetRetarderType();
			}

			public IXmlTypeWriter GetAuxiliaryType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryType();
			}

			public IXmlTypeWriter GetAuxiliaryHevSType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryHevSType();
			}

			public IXmlTypeWriter GetAuxiliaryIEPC_SType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryIEPC_SType();
			}

			public IXmlTypeWriter GetAuxiliaryHevPType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryHevPType();
			}

			public IXmlTypeWriter GetAuxiliaryIEPCType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryIEPCType();
			}

			public IXmlTypeWriter GetAuxiliaryPEVType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryPEVType();
			}

			public IXmlTypeWriter GetAxlegearType()
			{
				return _vifReportFactoryImplementation.GetAxlegearType();
			}

			public IXmlTypeWriter GetAxleWheelsType()
			{
				return _vifReportFactoryImplementation.GetAxleWheelsType();
			}

			public IXmlTypeWriter GetBoostingLimitationsType()
			{
				return _vifReportFactoryImplementation.GetBoostingLimitationsType();
			}

			public IXmlTypeWriter GetElectricEnergyStorageType()
			{
				return _vifReportFactoryImplementation.GetElectricEnergyStorageType();
			}

			public IXmlTypeWriter GetElectricMachineGENType()
			{
				return _vifReportFactoryImplementation.GetElectricMachineGENType();
			}

			public IXmlElectricMachineSystemType GetElectricMachineSystemType()
			{
				return _vifReportFactoryImplementation.GetElectricMachineSystemType();
			}

			public IXmlTypeWriter GetElectricMachineType()
			{
				return _vifReportFactoryImplementation.GetElectricMachineType();
			}

			public IXmlTypeWriter GetElectricMotorTorqueLimitsType()
			{
				return _vifReportFactoryImplementation.GetElectricMotorTorqueLimitsType();
			}

			public IXmlTypeWriter GetEngineType()
			{
				return _vifReportFactoryImplementation.GetEngineType();
			}

			public IReportOutputGroup GetHevIepcFVehicleParameterGroup()
			{
				return _vifReportFactoryImplementation.GetHevIepcFVehicleParameterGroup();
			}

			public IXmlTypeWriter GetFuelCellType()
			{
				return _vifReportFactoryImplementation.GetFuelCellType();
			}

			public IXmlTypeWriter GetAuxiliaryHevFType()
			{
				return _vifReportFactoryImplementation.GetAuxiliaryHevFType();
			}

			#endregion
		}

	}
}
