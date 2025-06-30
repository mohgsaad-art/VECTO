using System.Xml.Linq;
using Ninject.Activation;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public class VIFResultsNinjectModule : NinjectModule
	{
		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IVIFResultsWriterFactory>().ToFactory().InSingletonScope();

			var vif = XmlDocumentType.MultistepOutputData;
			Bind<IResultsWriter>().To<VIFResultsWriter.ConventionalBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Conventional, false));
			Bind<IResultsWriter>().To<VIFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Hybrid, false));
			Bind<IResultsWriter>().To<VIFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.Hybrid, true));
			Bind<IResultsWriter>().To<VIFResultsWriter.HEVNonOVCBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.FuelCell, false));
			Bind<IResultsWriter>().To<VIFResultsWriter.HEVOVCBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.FuelCell, true));
			Bind<IResultsWriter>().To<VIFResultsWriter.PEVBus>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, VectoSimulationJobTypeHelper.PureElectric, true));
			Bind<IResultsWriter>().To<VIFResultsWriter.ExemptedVehicle>().Named(
				_namingHelper.GetName(vif, VehicleCategoryHelper.PrimaryBus, true));

			// --- Primary Bus

			Bind<IResultGroupWriter>().To<VIFConvResultWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusConvSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFHEVNonOVCResultWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVNonOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFHEVOVCResultWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVOVCSuccessResultWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFOVCChargeDepletingWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeDepleting(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFOVCChargeSustainingWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVOVCResultWriterChargeSustaining(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFPEVResultWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusPEVSuccessResultWriter(null, XNamespace.None));

			Bind<IResultSequenceWriter>().To<VIFResultSuccessMissionWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetSuccessMissionWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<ResultSimulationParameterVIFBusWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusSimulationParameterWriter(null, XNamespace.None));

			Bind<IFuelConsumptionWriter>().To<VIFFuelConsumptionWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetFuelConsumptionBus(null, XNamespace.None));
			Bind<IElectricEnergyConsumptionWriter>().To<VIFElectricEnergyConsumptionWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IMRFResultsWriterFactory c) => c.GetElectricEnergyConsumptionBus(null, XNamespace.None));
			
			Bind<IResultGroupWriter>().To<VIFErrorResultWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusErrorResultWriter(null, XNamespace.None));
			Bind<IResultSequenceWriter>().To<VIFResultErrorMissionWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetErrorMissionWriter(null, XNamespace.None));
			Bind<IResultGroupWriter>().To<VIFResultSimulationParameterErrorWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetErrorSimulationParameterWriter(null, XNamespace.None));
			Bind<IResultSequenceWriter>().To<VIFErrorDetailsWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((ICIFResultsWriterFactory c) => c.GetErrorDetailsWriter(null, XNamespace.None));



			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusConvSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVNonOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusHEVOVCSummaryWriter(null, XNamespace.None));
			Bind<IReportResultsSummaryWriter>().To<NoSummaryWriter>().When(AccessedViaVIFResultsWriterFactory)
				.NamedLikeFactoryMethod((IVIFResultsWriterFactory c) => c.GetBusPEVSummaryWriter(null, XNamespace.None));

		}

		#endregion

		private bool AccessedViaVIFResultsWriterFactory(IRequest request)
		{
			if (request.ParentRequest == null) {
				return false;
			}

			return typeof(IVIFResultsWriterFactory).IsAssignableFrom(request.ParentRequest.Service);
		}
	}
}