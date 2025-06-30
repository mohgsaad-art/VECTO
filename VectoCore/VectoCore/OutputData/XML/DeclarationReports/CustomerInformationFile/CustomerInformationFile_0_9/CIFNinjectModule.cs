using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    internal class CIFNinjectModule : MRFNinjectModule
    {
		#region Overrides of VectoNinjectModule

		public override void Load()
		{
			LoadModule<ContextPreservationModule>();

			Bind<ICustomerInformationFileFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings()
				{
					combineToNameDelegate = VehicleTypeAndArchitectureStringHelper.CreateName,
					skipArguments = 6,
					takeArguments = 6,
					methods = new[] {
						typeof(ICustomerInformationFileFactory).GetMethod(
							nameof(ICustomerInformationFileFactory.GetCustomerReport))
					}
				})).InSingletonScope();

			#region Lorry CIF
			Bind<IXMLCustomerReport>().To<ConventionalLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2,
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
                    false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3, 
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4, 
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry, 
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
                    false,
					true, 
					false)));

            Bind<IXMLCustomerReport>().To<Multiple_FCHV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F2,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_FCHV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_FCHV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_FCHV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_PEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_PEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_PEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_PEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_SHEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_SHEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_SHEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<Multiple_SHEV_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
                MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLCustomerReport>().To<FCHV_F2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<FCHV_F3_Lorry_CIF>()
				.Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper
				.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<FCHV_F4_Lorry_CIF>()
				.Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper
				.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<FCHV_IEPC_Lorry_CIF>()
				.Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper
				.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV_IEPC,
					ArchitectureID.F_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
					true,
					false,
					false)));

			#region MediumLorryCIF

			Bind<IXMLCustomerReport>().To<ConventionalLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN, 
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_PxLorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2, 
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2, 
					false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
                    false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
                    false, 
					false, 
					false)));

			Bind<IXMLCustomerReport>().To<HEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van, 
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC,
                    false, 
					true, 
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Van,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_Lorry_CIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(VehicleCategoryHelper.Van,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    true,
					false,
					false)));

			#endregion

			#endregion

			#region CompletedBUsCIF

			Bind<IXMLCustomerReport>().To<Conventional_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_Px_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_IHPC_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2,
					false,
					false,
					false)));

            Bind<IXMLCustomerReport>().To<HEV_S2_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S3_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<HEV_S4_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
					false,
					false,
					false)));


			Bind<IXMLCustomerReport>().To<HEV_IEPC_S_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
					false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E2_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
					false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E3_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
                    false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_E4_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
                    false,
					false,
					false)));

			Bind<IXMLCustomerReport>().To<PEV_IEPC_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLCustomerReport>().To<Exempted_CompletedBusCIF>().Named(MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(
				MRFNinjectModule.VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    true,
					false,
					false)));
			#endregion

			#region VehicleTypes
			Bind<IXmlTypeWriter>().To<CIFConventionalLorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEVPx_LorryVehicleWriter>().When(AccessedViaCIFFactory);

			Bind<IXmlTypeWriter>().To<CIF_HEVPx_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_PxLorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S2_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S2_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S3_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S3_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S4_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S4_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IEPC_S_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IEPC_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_Multiple_FCHV_LorryVehicleWriter>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetMultiple_FCHV_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<CIF_FCHV_F2_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHV_F2_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_FCHV_F3_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHV_F3_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_FCHV_F4_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHV_F4_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_FCHV_IEPC_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHV_IEPC_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<CIF_Multiple_PEV_LorryVehicleWriter>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetMultiple_PEV_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<CIF_Multiple_SHEV_LorryVehicleWriter>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetMultiple_SHEV_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<CIF_PEV_E2_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E2_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E3_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E3_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E4_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E4_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_IEPC_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_IEPC_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_Exempted_LorryVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExempted_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_Conventional_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventional_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_Px_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Px_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IHPC_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IHPC_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S2_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S2_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S3_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S3_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_S4_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_S4_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_HEV_IEPC_S_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_IEPC_S_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E2_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E2_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E3_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E3_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_E4_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_E4_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_PEV_IEPC_CompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_IEPC_CompletedBusVehicleType());

			Bind<IXmlTypeWriter>().To<CIF_ExemptedCompletedBusVehicleWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExemptedCompletedBusVehicleType());
			#endregion

			#region VehicleGroups
			Bind<IReportVehicleOutputGroup>().To<GeneralVehicleOutputSequenceGroupCif>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetGeneralVehicleSequenceGroupWriter());
			Bind<IReportCompletedBusOutputGroup>().To<GeneralVehicleOutputSequenceGroupCif>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetGeneralVehicleSequenceGroupWriterCompletedBus());

			Bind<IReportOutputGroup>().To<LorryGeneralVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryGeneralVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<ConventionalLorryVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalLorryVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<FuelCellLorryVehicleSequenceGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHVLorryVehicleSequenceGroupWriter());

			Bind<IReportOutputGroup>().To<HEV_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<HEV_LorryVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_LorryVehicleSequenceGroupWriter());


			Bind<IReportOutputGroup>().To<FuelCell_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFCHV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<FCHV_LorryVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFuelCell_LorryVehicleSequenceGroupWriter());

			Bind<IReportOutputGroup>().To<PEV_LorryVehicleTypeGroupCIF>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_LorryVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<PEV_LorryVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_LorryVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<CompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetCompletedBusVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<PEVCompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEVCompletedBusVehicleTypeGroup());
			Bind<IReportOutputGroup>().To<ExemptedCompletedBusVehicleTypeGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetExemptedCompletedBusVehicleTypeGroup());

			Bind<IReportOutputGroup>().To<HEV_CompletedBusVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_CompletedBusVehicleSequenceGroupWriter());
			Bind<IReportOutputGroup>().To<PEV_CompletedBusVehicleSequenceGroupWriter>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_CompletedBusVehicleSequenceGroupWriter());
			
			#endregion
			#region ComponentGroups
			Bind<IReportOutputGroup>().To<EngineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetEngineGroup());
			Bind<IReportOutputGroup>().To<TransmissionGroupWithGearbox>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroup());
			Bind<IReportOutputGroup>().To<TransmissionGroupWithoutGearbox>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetTransmissionGroupNoGearbox());
			Bind<IReportOutputGroup>().To<IEPCTransmissionGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetIEPCTransmissionGroup());
			Bind<IReportOutputGroup>().To<AxleWheelsGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxleWheelsGroup());
			Bind<IReportOutputGroup>().To<ElectricMachineGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetElectricMachineGroup());

            Bind<IAxlePowertrainReportOutputGroup>().To<ElectricMachineGroup>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxlePowertrainElectricMachineGroup());

			Bind<IAxlePowertrainReportOutputGroup>().To<TransmissionGroupWithGearbox>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxlePowertrainTransmissionGroup());

            Bind<IAxlePowertrainReportOutputGroup>().To<IEPCTransmissionGroup>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetAxlePowertrainIEPCTransmissionGroup());

            Bind<IReportOutputGroup>().To<REESSGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetREESSGroup());
            Bind<IReportOutputGroup>().To<FuelCellGroup>().When(AccessedViaCIFFactory)
                .NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetFuelCellGroup());
            Bind<IReportOutputGroup>().To<LorryAuxGroup>().When(AccessedViaCIFFactory).
				NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetLorryAuxGroup());
			Bind<IReportOutputGroup>().To<ConventionalCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalCompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<HEV_Px_IHPCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Px_IHPC_CompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<HEV_Sx_CompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEV_Sx_CompletedBusAuxGroup());
			Bind<IReportOutputGroup>().To<PEVCompletedBusAuxGroup>().When(AccessedViaCIFFactory)
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEV_CompletedBusAuxGroup());


			#endregion

			#region ADAS
			Bind<ICIFAdasType>().To<CIFConventionalAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetConventionalADASType());
			Bind<ICIFAdasType>().To<CIFHevAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetHEVADASType());
			Bind<ICIFAdasType>().To<CIFPevAdasType>()
				.NamedLikeFactoryMethod((ICustomerInformationFileFactory f) => f.GetPEVADASType());

			#endregion ADAS


		}

		private bool AccessedViaCIFFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}
			return typeof(ICustomerInformationFileFactory).IsAssignableFrom(request.ParentRequest.Service);
		}

		#endregion
	}
}
