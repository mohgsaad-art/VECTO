using System.Diagnostics;
using System.Xml.Linq;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Ninject.XMLReports
{

    internal class CIFResultsNinjectModule : NinjectModule
	{
		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		public override void Load()
		{

			Bind<ICIFResultsWriterFactory>().ToFactory()
				.WhenAnyAncestorNamed(ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM)
				.Named(ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM);

			Bind<ICIFResultsWriterFactory>().ToFactory()
				.WhenAnyAncestorNamed(ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM)
				.Named(ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM);

            var cif = XmlDocumentType.CustomerReport;
			Bind<IResultsWriter>().To<CIFResultsWriter.ConventionalLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVNonOVCLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVOVCLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.FCHVNonOVCLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.FuelCell, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.FCHVOVCLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.FuelCell, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.PEVLorry>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, VectoSimulationJobTypeHelper.PureElectric, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.Lorry, true));

			Bind<IResultsWriter>().To<CIFResultsWriter.ConventionalBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Conventional,
					false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.FCHVNonOVCBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.FuelCell, false));
			Bind<IResultsWriter>().To<CIFResultsWriter.FCHVOVCBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.FuelCell, true));
			Bind<IResultsWriter>().To<CIFResultsWriter.PEVBus>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, VectoSimulationJobTypeHelper.PureElectric,
					true));
			Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(cif, VehicleCategoryHelper.CompletedBus, true));


			Bind<IElectricRangeWriter>().To<ElectricRangeWriter>().When(request =>
					AccessedViaCIFResultsWriterFactory(request) && AnchestorNamedLike(request, ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM))
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetElectricRangeWriter(null, XNamespace.None));
			Bind<IElectricRangeWriter>().To<ElectricRangeWriter_BOL>().When(request =>
					AccessedViaCIFResultsWriterFactory(request) && AnchestorNamedLike(request, ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM))
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetElectricRangeWriter(null, XNamespace.None));

			Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) =>
					c.GetHydrogenRangeWriter(null, XNamespace.None));
			

            // -- Lorry

            Bind<IResultGroupWriter>().To<LorryConvResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryConvSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVNonOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVNonOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryPEVResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryPEVSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryErrorResultWriter(null, XNamespace.None));

			Bind<IResultSequenceWriter>().To<CIFResultMissionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetSuccessMissionWriter(null, XNamespace.None));
			Bind<IResultSequenceWriter>().To<ResultErrorMissionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetErrorMissionWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<ResultSimulationParameterLorryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorrySimulationParameterWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<ResultSimulationParameterLorryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetErrorSimulationParameterWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<LorryConvTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryConvTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVNonOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVNonOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVOVCResultWriterChargeSustaining(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryHEVOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryFCHVOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVOVCResultWriterChargeSustaining(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<NoTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVNonOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<LorryPEVTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryPEVTotalWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformanceLorry(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformancePEVLorry(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformanceFCHVLorry(null, XNamespace.None));


            Bind<IFuelConsumptionWriter>().To<LorryFuelConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetFuelConsumptionLorry(null, XNamespace.None));
			Bind<IElectricEnergyConsumptionWriter>().To<LorryElectricEnergyConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetElectricEnergyConsumptionLorry(null, XNamespace.None));
			Bind<ICO2Writer>().To<LorryCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetCO2ResultLorry(null, XNamespace.None));
			Bind<ICO2Writer>().To<LorrySummaryCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetCO2SummaryResultLorry(null, XNamespace.None));

            Bind<IReportResultsSummaryWriter>().To<LorryConvSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryConvSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryHEVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryHEVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryHEVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryFCHVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryFCHVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryFCHVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryPEVSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryPEVSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<LorryPEVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetLorryPEVNonOVCSummaryWriter(null, XNamespace.None));

			// -- Bus

			Bind<IResultGroupWriter>().To<BusConvCIFResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusConvSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusHEVNonOVCCIFResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusHEVOVCCIFResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVNonOVC_CIF_ResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVOVC_CIF_ResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusPEVCIFResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusPEVSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<ErrorResultWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusErrorResultWriter(null, XNamespace.None));
			
            Bind<IResultGroupWriter>().To<ResultSimulationParameterCIFBusWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusSimulationParameterWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<BusConvTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusConvTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusHEVNonOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVNonOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeSustaining(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<BusFCHVOVCChargeDepletingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusFCHVOVCChargeSustainingWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVOVCResultWriterChargeSustaining(null, XNamespace.None));

            Bind<IResultGroupWriter>().To<BusOVCTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVOVCTotalWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<BusPEVTotalWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusPEVTotalWriter(null, XNamespace.None));

			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformanceBus(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformancePEVBus(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VehiclePerformanceCIFWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetVehiclePerformanceFCHVBus(null, XNamespace.None));


            Bind<IFuelConsumptionWriter>().To<BusFuelConsumptionWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetFuelConsumptionBus(null, XNamespace.None));
			Bind<IElectricEnergyConsumptionWriter>().To<BusElectricEnergyConsumptionWriter>()
				.When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetElectricEnergyConsumptionBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetCO2ResultBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusPEVCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetCO2ResultPEVBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusSummaryCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetCO2SummaryResultBus(null, XNamespace.None));
			Bind<ICO2Writer>().To<BusPEVSummaryCO2Writer>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetCO2SummaryResultPEVBus(null, XNamespace.None));

            Bind<IReportResultsSummaryWriter>().To<BusConvSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusConvSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<BusHEVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<BusHEVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusHEVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<BusFCHVNonOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<BusFCHVOVCSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusFCHVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<BusPEVSummaryWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetBusPEVSummaryWriter(null, XNamespace.None));

			// -- common

			Bind<IResultSequenceWriter>().To<CIFErrorDetailsWriter>().When(AccessedViaCIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetErrorDetailsWriter(null, XNamespace.None));

		}

		private bool AnchestorNamedLike(IRequest request, string name)
		{
			return request.ParentContext?.Binding.Metadata.Name == name;

		}

		[DebuggerStepThrough]
		private bool AccessedViaCIFResultsWriterFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}

			return typeof(ICIFResultsWriterFactory).IsAssignableFrom(request.ParentRequest.Service);
		}
	}
}