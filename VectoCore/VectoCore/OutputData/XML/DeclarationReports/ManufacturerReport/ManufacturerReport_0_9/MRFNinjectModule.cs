using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Lorry;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{
	internal class MRFNinjectModule : AbstractNinjectModule
    {

		//IXMLManufacturerReport GetManufacturerReport(string vehicleType, VectoSimulationJobType jobType,
		//	ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		private static readonly VehicleTypeAndArchitectureStringHelperReport _vehicleTypeAndArchitectureStringHelper = new VehicleTypeAndArchitectureStringHelperReport();

		public override void Load()
		{
			LoadModule<ContextPreservationModule>();

			Bind<IManufacturerReportFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = VehicleTypeAndArchitectureStringHelper.CreateName,
					skipArguments = 6,
					takeArguments = 6,
					methods = new[] {
						typeof(IManufacturerReportFactory).GetMethod(
							nameof(IManufacturerReportFactory.GetManufacturerReport))
					}
				})).InSingletonScope();

			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryManufacturerReport());

			#region LorryMRF
			Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_LorryManufacturerReport>()
				.Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					true)));

			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_LorryManufacturerReport>()
				.Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.IHPC,
					ArchitectureID.P2,
                    false,
					false,
					true)));

			Bind<IXMLManufacturerReport>().To<HEV_S2_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
                    false,
					false,
					false)));


			Bind<IXMLManufacturerReport>().To<HEV_S3_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S4_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_IEPC_S_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
                    false,
					true,
					false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.Multiple_FCHV,
					ArchitectureID.F2,
                    false,
					false,
					false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.Lorry,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<FCHV_F2_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F2,
                    false,
					false,
					false)));


			Bind<IXMLManufacturerReport>().To<FCHV_F3_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<FCHV_F4_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<FCHV_IEPC_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.FCHV_IEPC,
					ArchitectureID.F_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E2_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E3_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E4_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_IEPC_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<Exempted_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.Lorry,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    true,
					false,
					false)));

			//Bind<IXMLManufacturerReport>().To<ConventionalLorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.ConventionalVehicle,
			//		ArchitectureID.UNKNOWN,
			//		false,
			//		false,
			//		false)));
			//Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_LorryManufacturerReport>()
			//	.Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.ParallelHybridVehicle,
			//		ArchitectureID.UNKNOWN,
			//		false,
			//		false,
			//		true)));

			//Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_LorryManufacturerReport>()
			//	.Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.IHPC,
			//		ArchitectureID.P2,
			//		false,
			//		false,
			//		true)));

			//Bind<IXMLManufacturerReport>().To<HEV_S2_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.SerialHybridVehicle,
			//		ArchitectureID.S2,
			//		false,
			//		false,
			//		false)));


			//Bind<IXMLManufacturerReport>().To<HEV_S3_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.SerialHybridVehicle,
			//		ArchitectureID.S3,
			//		false,
			//		false,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<HEV_S4_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.SerialHybridVehicle,
			//		ArchitectureID.S4,
			//		false,
			//		false,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<HEV_IEPC_S_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.IEPC_S,
			//		ArchitectureID.S_IEPC,
			//		false,
			//		true,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<PEV_E2_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.BatteryElectricVehicle,
			//		ArchitectureID.E2,
			//		false,
			//		false,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<PEV_E3_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.BatteryElectricVehicle,
			//		ArchitectureID.E3,
			//		false,
			//		false,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<PEV_E4_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.BatteryElectricVehicle,
			//		ArchitectureID.E4,
			//		false,
			//		false,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<PEV_IEPC_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.IEPC_E,
			//		ArchitectureID.E_IEPC,
			//		false,
			//		true,
			//		false)));

			//Bind<IXMLManufacturerReport>().To<Exempted_LorryManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
			//		VehicleCategoryHelper.Van,
			//		VectoSimulationJobType.ConventionalVehicle,
			//		ArchitectureID.UNKNOWN,
			//		true,
			//		false,
			//		false)));

			#endregion
			#region PrimaryBUSMRF
			Bind<IXMLManufacturerReport>().To<Conventional_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					true)));

			Bind<IXMLManufacturerReport>().To<HEV_Px_IHPC_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
				VehicleCategoryHelper.PrimaryBus,
				VectoSimulationJobType.IHPC,
				ArchitectureID.P2,
                false,
				false,
				true)));

			Bind<IXMLManufacturerReport>().To<HEV_S2_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S3_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_S4_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_IEPC_S_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
					false, 
					false, 
					false)));

			Bind<IXMLManufacturerReport>().To<FCHV_F2_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F2,
                    false,
					false,
					false)));


			Bind<IXMLManufacturerReport>().To<FCHV_F3_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<FCHV_F4_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV,
					ArchitectureID.F4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<FCHV_IEPC_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.FCHV_IEPC,
					ArchitectureID.F_IEPC,
                    false,
					true,
					false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F2,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_FCHV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_PEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<Multiple_SHEV_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
                    VehicleCategoryHelper.PrimaryBus,
                    VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC,
                    false,
                    false,
                    false)));

            Bind<IXMLManufacturerReport>().To<PEV_E2_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E3_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_E4_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_IEPC_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<Exempted_PrimaryBus_ManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.PrimaryBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.UNKNOWN,
                    true,
					false,
					false)));


			#endregion
			#region CompletedBus MRF

			Bind<IXMLManufacturerReport>().To<Conventional_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.UNKNOWN,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
				VehicleCategoryHelper.CompletedBus,
				VectoSimulationJobType.IHPC,
				ArchitectureID.P2,
                false,
				false,
				false)));

			// todo amogoda: 2.3. - completed bus
            Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<HEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4,
                    false,
					false,
					false)));

			Bind<IXMLManufacturerReport>().To<PEV_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC,
                    false,
					true,
					false)));

			Bind<IXMLManufacturerReport>().To<Exempted_CompletedBusManufacturerReport>().Named(VehicleTypeAndArchitectureStringHelper.CreateName.Invoke(VehicleTypeAndArchitectureStringHelper.ToParams(
					VehicleCategoryHelper.CompletedBus,
					VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN,
                    true,
					false,
					false)));



			#endregion

			#region Vehicle

			Bind<IXmlTypeWriter>().To<ConventionalLorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleType());
			Bind<IXmlTypeWriter>().To<HevPxIhpcLorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<HevS2LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<HevS3LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<HevS4LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<HevIepcSLorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<PevE2LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<PevE3LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<PevE4LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<PevIEPCLorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_IEPC_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<ExemptedLorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetExempted_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<Multiple_FCHV_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_FCHV_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<Multiple_PEV_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_PEV_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<Multiple_SHEV_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_SHEV_LorryVehicleType());

            Bind<IXmlTypeWriter>().To<FCHV_F2_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F2_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_F3_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F3_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_F4_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F4_LorryVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_IEPC_LorryVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_IEPC_LorryVehicleType());

			Bind<IXmlTypeWriter>().To<ConventionalPrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevPxIhpcPrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHPC_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevS2PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevS3PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevS4PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevIepcSPrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_PrimaryBusVehicleType());

            Bind<IXmlTypeWriter>().To<Multiple_FCHV_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_FCHV_PrimaryBusVehicleType());
            Bind<IXmlTypeWriter>().To<Multiple_PEV_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_PEV_PrimaryBusVehicleType());
            Bind<IXmlTypeWriter>().To<Multiple_SHEV_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_SHEV_PrimaryBusVehicleType());

            Bind<IXmlTypeWriter>().To<FCHV_F2_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F2_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_F3_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F3_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_F4_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F4_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<FCHV_IEPC_PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_IEPC_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<PevE2PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<PevE3PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<PevE4PrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<PevIepcPrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_IEPC_PrimaryBusVehicleType());
			Bind<IXmlTypeWriter>().To<ExemptedPrimaryBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetExempted_PrimaryBusVehicleType());

			Bind<IXmlTypeWriter>().To<ConventionalCompletedBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_CompletedBusVehicleType());
			Bind<IXmlTypeWriter>().To<HevCompletedBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_CompletedBusVehicleType());
			Bind<IXmlTypeWriter>().To<PevCompletedBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_CompletedBusVehicleType());
			Bind<IXmlTypeWriter>().To<ExemptedCompletedBusVehicleTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetExempted_CompletedBusVehicleType());
			#endregion
			#region Components
			Bind<IXmlTypeWriter>().To<ConventionalLorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevPxIhpcLorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHCP_LorryComponentsType());

			Bind<IXmlTypeWriter>().To<MrfhevS2LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevS3LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevS4LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevIepcSLorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_LorryComponentsType());

			Bind<IXmlTypeWriter>().To<MRF_Multiple_FCHV_LorryComponentsTypeWriter>()
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_FCHV_LorryComponentsType());

            Bind<IXmlTypeWriter>().To<MRF_Multiple_PEV_LorryComponentsTypeWriter>()
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_PEV_LorryComponentsType());

            Bind<IXmlTypeWriter>().To<MRF_Multiple_SHEV_LorryComponentsTypeWriter>()
               .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_SHEV_LorryComponentsType());

            Bind<IXmlTypeWriter>().To<MRF_FCHV_F2_LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F2_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_F3_LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F3_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_F4_LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F4_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_IEPC_LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_IEPC_LorryComponentsType());

			Bind<IXmlTypeWriter>().To<MrfpevE2LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_LorryComponentsType());

			Bind<IXmlTypeWriter>().To<MrfpevE3LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevE4LorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_LorryComponentsType());
			Bind<IXmlTypeWriter>().To<MrfPevIEPCLorryComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_IEPC_S_LorryComponentsType());

			Bind<IXmlTypeWriter>().To<ConventionalPrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventional_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevPxIhpcPrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_Px_IHPC_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevS2PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S2_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevS3PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S3_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevS4PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_S4_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevIepcSPrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_IEPC_S_PrimaryBusComponentsType());
			
			Bind<IXmlTypeWriter>().To<MRF_Multiple_FCHV_PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_FCHV_PrimaryBusComponentsType());
            Bind<IXmlTypeWriter>().To<MRF_Multiple_PEV_PrimaryBusComponentsTypeWriter>()
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_PEV_PrimaryBusComponentsType());
            Bind<IXmlTypeWriter>().To<MRF_Multiple_SHEV_PrimaryBusComponentsTypeWriter>()
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetMultiple_SHEV_PrimaryBusComponentsType());

            Bind<IXmlTypeWriter>().To<MRF_FCHV_F2_PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F2_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_F3_PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F3_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_F4_PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_F4_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MRF_FCHV_IEPC_PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_IEPC_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevE2PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E2_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevE3PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E3_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevE4PrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_E4_PrimaryBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevIepcPrimaryBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_IEPC_PrimaryBusComponentsType());


			Bind<IXmlTypeWriter>().To<ConventionalCompletedBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod(
					(IManufacturerReportFactory f) => f.GetConventional_CompletedBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfhevCompletedBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod(
					(IManufacturerReportFactory f) => f.GetHEV_CompletedBusComponentsType());
			Bind<IXmlTypeWriter>().To<MrfpevCompletedBusComponentsTypeWriter>()
				.NamedLikeFactoryMethod(
					(IManufacturerReportFactory f) => f.GetPEV_CompletedBusComponentsType());


			#endregion


			Bind<IReportOutputGroup>().To<HEV_VehicleSequenceGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_VehicleSequenceGroup());

			Bind<IReportOutputGroup>().To<FCHV_VehicleSequenceGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_VehicleSequenceGroup());

			Bind<IReportOutputGroup>().To<PEV_VehicleSequenceGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_VehicleSequenceGroup());
			#region ADAS
			Bind<IMRFAdasType>().To<MRFConventionalAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalADASType());
			Bind<IMRFAdasType>().To<MRFHevAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVADASType());
			Bind<IMRFAdasType>().To<MRFPevAdasType>()
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVADASType());

			#endregion ADAS
			Bind<IXmlTypeWriter>().To<TorqueLimitationsTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineTorqueLimitationsType());

			Bind<IXmlTypeWriter>().To<EngineTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetEngineType());



			Bind<IXmlTypeWriter>().To<TransmissionTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetTransmissionType());

			Bind<IXmlAxlePowertrainTypeWriter>().To<TransmissionTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainTransmissionType());

            Bind<IXmlTypeWriter>().To<RetarderTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetRetarderType());

            Bind<IXmlAxlePowertrainTypeWriter>().To<RetarderTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainRetarderType());

            Bind<IXmlTypeWriter>().To<TorqueConverterTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetTorqueConverterType());

            Bind<IXmlAxlePowertrainTypeWriter>().To<TorqueConverterTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainTorqueConverterType());

            Bind<IXmlTypeWriter>().To<AngleDriveTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAngleDriveType());

            Bind<IXmlAxlePowertrainTypeWriter>().To<AngleDriveTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainAngleDriveType());

            Bind<IMrfAirdragType>().To<MRFAirdragType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAirdragType());

			Bind<IXmlTypeWriter>().To<AxleWheelsTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxleWheelsType());

			Bind<IXmlTypeWriter>().To<AxleGearTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxleGearType());

            Bind<IXmlAxlePowertrainTypeWriter>().To<AxleGearTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainAxleGearType());

            Bind<IXmlTypeWriter>().To<ElectricMachineTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetElectricMachineType());

			Bind<IXmlAxlePowertrainTypeWriter>().To<ElectricMachineTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainElectricMachineType());

            Bind<IXmlTypeWriter>().To<MRFElectricMachineGenTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetElectricMachineGenType());

            Bind<IXmlTypeWriter>().To<MRFGeneratorTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneratorType());

            Bind<IXmlTypeWriter>().To<MRF_IEPCSpecificationsTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetIEPCSpecifications());

            Bind<IXmlAxlePowertrainTypeWriter>().To<MRF_IEPCSpecificationsTypeWriter>().When(AccessedViaMRFFactory)
                .NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetAxlePowertrainIEPCSpecifications());

            Bind<IXmlTypeWriter>().To<MrfreessSpecificationsTypeWriter>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetREESSSpecificationsType());

			Bind<IMrfVehicleType>().To<MRFBoostingLimitationsType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetBoostingLimitationsType());

			Bind<IXmlTypeWriter>().To<MRFFuelCellSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFuelCellSystemType());

			#region Auxiliaries
			Bind<IMRFLorryAuxiliariesType>().To<MRFConventionalLorryAuxiliariesType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryAuxType());
			Bind<IMRFLorryAuxiliariesType>().To<MRFHEV_LorryAuxiliariesType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_LorryAuxiliariesType());
			Bind<IMRFLorryAuxiliariesType>().To<MRFPEV_LorryAuxiliariesType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_LorryAuxiliariesType());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_Conventional>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_Conventional());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_HEV_P>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_HEV_P());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_HEV_S>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_HEV_S());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_HEV_F>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_HEV_F());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusAuxType_PEV>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusAuxType_PEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_HEV_S>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_HEV_S());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_PEV_IEPC>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_PEV_IEPC());
			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusPneumaticSystemType_HEV_F>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusPneumaticSystemType_HEV_F());



			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusElectricSystemType_Conventional_HEV>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusElectricSystemType_Conventional_HEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusElectricSystemType_PEV>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusElectricSystemType_PEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusHVACSystemType_Conventional_HEV>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusHVACSystemType_Conventional_HEV());

			Bind<IMRFBusAuxiliariesType>().To<MRFPrimaryBusElectricSystemType_FCHV>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusHVACSystemType_FCHV());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBusAuxType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBusAuxType());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBus_HVACSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBus_HVACSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFConventionalCompletedBusElectricSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBusElectricSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFHEVCompletedBusAuxType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVCompletedBusAuxType());

			Bind<IMRFBusAuxiliariesType>().To<MRFHEVCompletedBus_HVACSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVCompletedBus_HVACSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFHEVCompletedBusElectricSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVCompletedBusElectricSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFPEVCompletedBusAuxType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVCompletedBusAuxType());

			Bind<IMRFBusAuxiliariesType>().To<MRFPEVCompletedBus_HVACSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVCompletedBus_HVACSystemType());

			Bind<IMRFBusAuxiliariesType>().To<MRFPEVCompletedBusElectricSystemType>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVCompletedBusElectricSystemType());

			#region Groups

			Bind<IReportVehicleOutputGroup>().To<GeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<LorryGeneralVehicleOutputXmlGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetGeneralLorryVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<ConventionalLorryVehicleXmlGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalLorryVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<PrimaryBusGeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPrimaryBusGeneralVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<ExemptedPrimaryBusGeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetExemptedPrimaryBusGeneralVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<HEVPrimaryBusVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_PrimaryBusVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<FCHVPrimaryBusVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_PrimaryBusVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<PEVPrimaryBusVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_PrimaryBusVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<HEV_LorryVehicleOutputTypeGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<FCHV_LorryVehicleOutputTypeGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetFCHV_lorryVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<PEV_LorryVehicleOutputTypeGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEV_lorryVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<HEV_LorryVehicleOutputTypeSequenceGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEV_lorryVehicleOutputSequenceGroup());

			Bind<IReportVehicleOutputGroup>().To<CompletedBusSequenceOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusSequenceGroup());

			Bind<IReportVehicleOutputGroup>().To<CompletedBusDimensionsSequenceOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBusDimensionSequenceGroup());

			Bind<IReportOutputGroup>().To<ConventionalCompletedBusGeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetConventionalCompletedBusGeneralVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<HEVCompletedBusGeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetHEVCompletedBusGeneralVehicleOutputGroup());

			Bind<IReportOutputGroup>().To<PEVCompletedBusGeneralVehicleOutputGroup>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetPEVCompletedBusGeneralVehicleOutputGroup());


			Bind<IMrfBusAuxGroup>().To<CompletedBus_HVACSystem_Group>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBus_HVACSystemGroup());

			Bind<IMrfBusAuxGroup>().To<CompletedBus_xEVHVACSystem_Group>().When(AccessedViaMRFFactory)
				.NamedLikeFactoryMethod((IManufacturerReportFactory f) => f.GetCompletedBus_xEVHVACSystemGroup());

			#endregion



		}

		private bool AccessedViaMRFFactory(IRequest request)
		{
			if (request.ParentRequest == null)
			{
				return false;
			}
			return typeof(IManufacturerReportFactory).IsAssignableFrom(request.ParentRequest.Service);
		}

		#endregion

		public static VehicleTypeAndArchitectureStringHelperReport VehicleTypeAndArchitectureStringHelper
		{
			get { return _vehicleTypeAndArchitectureStringHelper; }
		}
	}
}
