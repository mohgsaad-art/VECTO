using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.InterimComponents;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	internal class VIFNinjectModule : MRFNinjectModule
	{
		#region Overrides of NinjectModule
		

		public override void Load()
		{
			LoadModule<ContextPreservationModule>();

			Bind<IVIFReportInterimFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings()
				{
					combineToNameDelegate = VehicleTypeAndArchitectureStringHelper.CreateName,
					skipArguments = 6,
					takeArguments = 6,
					methods = new[] {
						typeof(IVIFReportInterimFactory).GetMethod(
							nameof(IVIFReportInterimFactory.GetInterimVIFReport))
					}
				})).InSingletonScope();


			Bind<IVIFReportFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings()
				{
					combineToNameDelegate = VehicleTypeAndArchitectureStringHelper.CreateName,
					skipArguments = 7,
					takeArguments = 7,
					methods = new[] {
						typeof(IVIFReportFactory).GetMethod(
							nameof(IVIFReportFactory.GetVIFReport))
					}
				})).InSingletonScope();

			
			
			#region Primary Vehicle Information File Reports

			Bind<IXMLVehicleInformationFile>().To<Conventional_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_Px_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));


			Bind<IXMLVehicleInformationFile>().To<HEV_Px_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2,
					false,
					false,
					false)));

            Bind<IXMLVehicleInformationFile>().To<HEV_S2_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_S3_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
                    false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_S4_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
                    false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_IEPC_S_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
                    false,
					true,
					false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_FCHV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F2,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_FCHV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_FCHV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_FCHV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_PEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_PEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_PEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_PEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_SHEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_SHEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_SHEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<Multiple_SHEV_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLVehicleInformationFile>().To<HEV_F2_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F2,
                    false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_F3_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F3,
                    false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_F4_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F4,
                    false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<HEV_IEPC_F_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV_IEPC,
					ArchitectureID.F_IEPC,
					false,
					true,
					false)));

			Bind<IXMLVehicleInformationFile>().To<PEV_E2_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<PEV_E3_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<PEV_E4_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLVehicleInformationFile>().To<PEV_IEPC_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLVehicleInformationFile>().To<Exempted_PrimaryBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));

			#endregion

			#region Complete(d) Primary Vehicle Information File Reports

			// -- Reports

			Bind<IXMLMultistepIntermediateReport>().To<Conventional_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2,
					false,
					false,
					false)));

            Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
					false,
					true,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<HEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<PEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<PEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<PEV_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<IEPC_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLMultistepIntermediateReport>().To<Exempted_CompletedBus_VIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));

			// -- Vehicle type writers

			Bind<IXmlMultistepTypeWriter>().To<ConventionalInterimVehicleType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetConventionalVehicleType());

			Bind<IXmlMultistepTypeWriter>().To<HEVInterimVehicleType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetHEVVehicleType());

			Bind<IXmlMultistepTypeWriter>().To<PEVInterimVehicleType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetPEVVehicleType());

			Bind<IXmlMultistepTypeWriter>().To<IEPCInterimVehicleType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetIEPCVehicleType());


			Bind<IXmlMultistepTypeWriter>().To<ExemptedInterimVehicleType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetExemptedVehicleType());

			// -- Components type writers

			Bind<IReportMultistepCompletedBusTypeWriter>().To<ConventionalComponentsInterimVIFType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetConventionalInterimComponentsType());

			Bind<IReportMultistepCompletedBusTypeWriter>().To<XEVComponentsInterimVIFType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetxEVInterimComponentsType());

			// -- ADAS type writers

			Bind<IVIFFAdasType>().To<VIFCompletedConventionalAdasType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetConventionalInterimADASType());

			Bind<IVIFFAdasType>().To<VIFCompletedHEVAdasType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetHEVInterimADASType());

			Bind<IVIFFAdasType>().To<VIFCompletedPEVAdasType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetPEVInterimADASType());

			Bind<IVIFFAdasType>().To<VIFCompletedIEPCAdasType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetIEPCInterimADASType());


			// -- Airdrag type writers

			Bind<IReportMultistepCompletedBusTypeWriter>().To<AirdragInterimType>().When(AccessedViaInterimVIFFactory)
                .NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetInterimAirdragType());

            // -- Aux type writers
			
			Bind<IReportMultistepCompletedBusTypeWriter>().To<ConventionalInterimAuxiliaryType>().When(AccessedViaInterimVIFFactory)
                .NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetInterimConventionalAuxiliariesType());

			Bind<IReportMultistepCompletedBusTypeWriter>().To<XEVInterimAuxiliaryType>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetInterimxEVAuxiliariesType());

			// -- Parameter Groups

			Bind<IReportMultistepCompletedBusOutputGroup>().To<CompletedBusGeneralParametersGroup>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetCompletedBusGeneralParametersGroup());

			Bind<IReportMultistepCompletedBusOutputGroup>().To<GetCompletedBusParametersGroup>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetCompletedBusParametersGroup());

			Bind<IReportMultistepCompletedBusOutputGroup>().To<CompletedBusPassengerCountGroup>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetCompletedBusPassengerCountGroup());

			Bind<IReportMultistepCompletedBusOutputGroup>().To<CompletedBusDimensionsGroup>().When(AccessedViaInterimVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportInterimFactory f) => f.GetCompletedBusDimensionsGroup());

			#endregion

			#region VehicleTypes

			Bind<IXmlTypeWriter>().To<ConventionalVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalVehicleType());

			Bind<IXmlTypeWriter>().To<HevIepcSVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSVehicleType());

			Bind<IXmlTypeWriter>().To<HevPxVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxVehicleType());

			Bind<IXmlTypeWriter>().To<HevS2VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS2VehicleType());

			Bind<IXmlTypeWriter>().To<HevS3VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS3VehicleType());

			Bind<IXmlTypeWriter>().To<HevS4VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS4VehicleType());

			Bind<IXmlTypeWriter>().To<MultipleFCHVVehicleType>().When(AccessedViaVIFFactory)
                .NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetMultipleFCHVVehicleType());

            Bind<IXmlTypeWriter>().To<MultiplePEVVehicleType>().When(AccessedViaVIFFactory)
                .NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetMultiplePEVVehicleType());

            Bind<IXmlTypeWriter>().To<MultipleSHEVVehicleType>().When(AccessedViaVIFFactory)
                .NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetMultipleSHEVVehicleType());

            Bind<IXmlTypeWriter>().To<HevF2VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF2VehicleType());

			Bind<IXmlTypeWriter>().To<HevF3VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF3VehicleType());

			// todo amogoda: vif - is implementation correct?
			Bind<IXmlTypeWriter>().To<HevF4VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF4VehicleType());

			Bind<IXmlTypeWriter>().To<IepcFVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcFVehicleType());

			Bind<IXmlTypeWriter>().To<PevE2VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE2VehicleType());

			Bind<IXmlTypeWriter>().To<PevE3VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE3VehicleType());

			Bind<IXmlTypeWriter>().To<PevE4VehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE4VehicleType());

			Bind<IXmlTypeWriter>().To<PevIEPCVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevIEPCVehicleType());

			Bind<IXmlTypeWriter>().To<ExemptedVehicleType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetExemptedVehicleType());

			#endregion


			#region Component Group

			Bind<IXmlTypeWriter>().To<ConventionalComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalComponentType());

			Bind<IXmlTypeWriter>().To<HevIepcSComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevPxComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS2ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS2ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS3ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS3ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevS4ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevS4ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevF2ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF2ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevF3ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF3ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevF4ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevF4ComponentVIFType());

			Bind<IXmlTypeWriter>().To<HevIepcFComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcFComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE2ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE2ComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE3ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE3ComponentVIFType());

			Bind<IXmlTypeWriter>().To<PevE4ComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevE4ComponentVIFType());

			Bind<IXmlTypeWriter>().To<IepcComponentVIFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevIEPCComponentVIFType());

			#endregion

			#region Vehicle Group

			Bind<IReportOutputGroup>().To<ConventionalVIFVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalVehicleGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusGeneralParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusGeneralParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusChassisParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusChassisParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusRetarderParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusRetarderParameterGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusXeVParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPrimaryBusXevParameterGroup());

			Bind<IReportOutputGroup>().To<HevIepcSVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevIepcSVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<HevSxVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevSxVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<PevExVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevExVehicleParmeterGroup());
			
			Bind<IReportOutputGroup>().To<PevIEPCVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPevIEPCVehicleParmeterGroup());
			
			Bind<IReportOutputGroup>().To<HevPxVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHevPxVehicleParameterGroup());

			Bind<IReportOutputGroup>().To<ExemptedVehicleParameterGroup>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetExemptedVehicleParameterGroup());

			#endregion


			#region Components

			Bind<IVIFFAdasType>().To<VIFConventionalAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetConventionalADASType());

			Bind<IVIFFAdasType>().To<VIFHEVAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetHEVADASType());

			Bind<IVIFFAdasType>().To<VIFPEVAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetPEVADASType());

			Bind<IVIFFAdasType>().To<VIFIEPCAdasType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIEPCADASType());

			Bind<IXmlTypeWriter>().To<VIFAngleDriveType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAngelDriveType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryHevSType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryHevSType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryHevFType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryHevFType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryIEPC_SType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryIEPC_SType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryHevPType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryHevPType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryIEPCType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryIEPCType());

			Bind<IXmlTypeWriter>().To<VIFAuxiliaryPEVType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAuxiliaryPEVType());

			Bind<IXmlTypeWriter>().To<VIFAxlegearType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxlegearType());

			Bind<IXmlTypeWriter>().To<VIFRetarderType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetRetarderType());

			Bind<IXmlTypeWriter>().To<VIFAxleWheelsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetAxleWheelsType());

			Bind<IXmlTypeWriter>().To<VIFBoostingLimitationsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetBoostingLimitationsType());

			Bind<IXmlTypeWriter>().To<VIFElectricEnergyStorageType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricEnergyStorageType());

			Bind<IXmlTypeWriter>().To<VIFElectricMachineGENType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineGENType());

			Bind<IXmlElectricMachineSystemType>().To<XmlElectricMachineSystemMeasuredType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineSystemType());

			Bind<IXmlTypeWriter>().To<VIFElectricMachineType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMachineType());
			
			Bind<IXmlTypeWriter>().To<VIFElectricMotorTorqueLimitsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetElectricMotorTorqueLimitsType());

			Bind<IXmlTypeWriter>().To<VIFEngineType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetEngineType());

			Bind<IXmlTypeWriter>().To<VIFTorqueConverterType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTorqueConvertType());

			Bind<IXmlTypeWriter>().To<VIFIepcType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetIepcType());

			Bind<IXmlTypeWriter>().To<VIFTorqueLimitsType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTorqueLimitsType());

			Bind<IXmlTypeWriter>().To<VIFTransmissionType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetTransmissionType());

			Bind<IXmlTypeWriter>().To<VIFFuelCellType>().When(AccessedViaVIFFactory)
				.NamedLikeFactoryMethod((IVIFReportFactory f) => f.GetFuelCellType());

			#endregion
		}


		private bool AccessedViaVIFFactory(IRequest request)
		{
			if (request.ParentRequest == null)
			{
				return false;
			}
			return typeof(IVIFReportFactory).IsAssignableFrom(request.ParentRequest.Service);
		}


		private bool AccessedViaInterimVIFFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}
			return typeof(IVIFReportInterimFactory).IsAssignableFrom(request.ParentRequest.Service);
		}

		#endregion
	}
}
