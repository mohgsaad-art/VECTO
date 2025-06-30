using System.Diagnostics;
using System.Xml.Linq;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{

    internal class MRFResultsNinjectModule : NinjectModule
	{
		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		public override void Load()
		{
			Bind<IMRFResultsWriterFactory>().ToFactory()
				.WhenAnyAncestorNamed(ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM)
				.Named(ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM);
			Bind<IMRFResultsWriterFactory>().ToFactory()
				.WhenAnyAncestorNamed(ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM)
				.Named(ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM);

            var mrf = XmlDocumentType.ManufacturerReport;
			Bind<IResultsWriter>().To<MRFResultsWriter.ConventionalLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVNonOVCLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVOVCLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<MRFResultsWriter.FCHVNonOVCLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.FuelCell, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.FCHVOVCLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.FuelCell, true));

            Bind<IResultsWriter>().To<MRFResultsWriter.PEVLorry>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.PureElectric, true));

			Bind<IResultsWriter>().To<MRFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.Lorry, true));

			Bind<IResultsWriter>().To<MRFResultsWriter.ConventionalBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<MRFResultsWriter.FCHVNonOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.FuelCell, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.FCHVOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.FuelCell, true));
            Bind<IResultsWriter>().To<MRFResultsWriter.PEVBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.PureElectric, true));
			Bind<IResultsWriter>().To<MRFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.PrimaryBus, true));

            Bind<IResultsWriter>().To<MRFResultsWriter.ConventionalBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<MRFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<MRFResultsWriter.PEVBus>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.PureElectric,true));
			Bind<IResultsWriter>().To<MRFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(mrf, VehicleCategoryHelper.CompletedBus, true));

			Bind<IElectricRangeWriter>().To<ElectricRangeWriter>().When(request =>
					AccessedViaMRFResultsWriterFactory(request) && request.ParentContext?.Binding.Metadata.Name ==
					ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetElectricRangeWriter(null, XNamespace.None));
			Bind<IElectricRangeWriter>().To<ElectricRangeWriter_BOL>().When(request =>
					AccessedViaMRFResultsWriterFactory(request) && request.ParentContext?.Binding.Metadata.Name ==
					ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetElectricRangeWriter(null, XNamespace.None));

			Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetHydrogenRangeWriter(null, XNamespace.None));

            // -- Lorry

            Bind<IResultGroupWriter>().To<LorryConvResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryConvSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVNonOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVNonOVCSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVNonOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVOVCSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryPEVResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryPEVSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryErrorResultWriter(null, XNamespace.None));

            Bind<IResultSequenceWriter>().To<MRFResultSuccessMissionWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetSuccessMissionWriter(null, XNamespace.None));
			Bind<IResultSequenceWriter>().To<ResultErrorMissionWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetErrorMissionWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<ResultSimulationParameterLorryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorrySimulationParameterWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<ResultSimulationParameterErrorWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetErrorSimulationParameterWriter(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<LorryConvTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryConvTotalWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVNonOVCTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVNonOVCTotalWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVOVCChargeDepletingWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeDepleting(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVOVCChargeSustainingWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeSustaining(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryHEVOVCTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCChargeDepletingWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCChargeSustainingWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVOVCResultWriterChargeSustaining(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<NoTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVNonOVCTotalWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<LorryPEVTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryPEVTotalWriter(null, XNamespace.None));


            Bind<IFuelConsumptionWriter>().To<LorryFuelConsumptionWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetFuelConsumptionLorry(null, XNamespace.None));
            Bind<IElectricEnergyConsumptionWriter>().To<LorryElectricEnergyConsumptionWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetElectricEnergyConsumptionLorry(null, XNamespace.None));
            Bind<ICO2Writer>().To<LorryCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2ResultLorry(null, XNamespace.None));
			Bind<ICO2Writer>().To<LorrySummaryCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2SummaryResultLorry(null, XNamespace.None));

            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryConvSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVNonOVCSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryHEVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryFCHVOVCSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetLorryPEVSummaryWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<VehiclePerformanceMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformanceLorry(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformancePEVMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformancePEVLorry(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceFCHVMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformanceFCHVLorry(null, XNamespace.None));

            // -- Bus

            Bind<IResultGroupWriter>().To<BusConvMRFResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusConvSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusHEVNonOVCMRFResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVNonOVCSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusHEVOVCMRFResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVNonOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVOVCResultWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVOVCSuccessResultWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusPEVMRFResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusPEVSuccessResultWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusErrorResultWriter(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<ResultSimulationParameterMRFBusWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusSimulationParameterWriter(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<BusConvTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusConvTotalWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusHEVNonOVCTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVNonOVCTotalWriter(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusOVCChargeDepletingWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeDepleting(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusOVCChargeSustainingWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeSustaining(null, XNamespace.None));
            Bind<IResultGroupWriter>().To<BusOVCTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVOVCChargeDepletingWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVOVCChargeSustainingWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVOVCResultWriterChargeSustaining(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<BusPEVTotalWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusPEVTotalWriter(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<VehiclePerformanceMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformanceBus(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformancePEVMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformancePEVBus(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceFCHVMRFWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetVehiclePerformanceFCHVBus(null, XNamespace.None));


            Bind<IFuelConsumptionWriter>().To<BusFuelConsumptionWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetFuelConsumptionBus(null, XNamespace.None));


			Bind<IElectricEnergyConsumptionWriter>().To<BusElectricEnergyConsumptionWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetElectricEnergyConsumptionBus(null, XNamespace.None));
            Bind<ICO2Writer>().To<BusCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2ResultBus(null, XNamespace.None));
            Bind<ICO2Writer>().To<BusPEVCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2ResultPEVBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusSummaryCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2SummaryResultBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusPEVSummaryCO2Writer>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2SummaryResultPEVBus(null, XNamespace.None));

            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusConvSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVNonOVCSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusHEVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusFCHVOVCSummaryWriter(null, XNamespace.None));
            Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaMRFResultsWriterFactory)
                .NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetBusPEVSummaryWriter(null, XNamespace.None));

			// -- common

			Bind<IResultSequenceWriter>().To<MRFErrorDetailsWriter>().When(AccessedViaMRFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetErrorDetailsWriter(null, XNamespace.None));
        }

        [DebuggerStepThrough]
		private bool AccessedViaMRFResultsWriterFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}

			return typeof(IMRFResultsWriterFactory).IsAssignableFrom(request.ParentRequest.Service);
		}
	}

}