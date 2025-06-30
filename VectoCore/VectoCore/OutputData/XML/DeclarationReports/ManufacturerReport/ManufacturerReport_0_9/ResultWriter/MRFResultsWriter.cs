using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
    public abstract class AbstractMRFResultsWriter : AbstractResultsWriter
	{
		protected readonly IMRFResultsWriterFactory _mrfFactory;

		protected AbstractMRFResultsWriter(IMRFResultsWriterFactory mrfFactory)
		{
			_mrfFactory = mrfFactory;
		}
		protected override XNamespace TNS => AbstractManufacturerReport.Namespace;


    }

	public class MRFResultsWriter
	{

		public class ConventionalLorry : AbstractMRFResultsWriter
		{
			public ConventionalLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			#region Overrides of AbstractResultsWriter

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryConvSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);

            public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryConvSummaryWriter(_mrfFactory, TNS);

			#endregion
		}

		public class HEVNonOVCLorry : AbstractMRFResultsWriter
		{
			public HEVNonOVCLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryHEVNonOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);

            public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryHEVNonOVCSummaryWriter(_mrfFactory, TNS);

		}

		public class HEVOVCLorry : AbstractMRFResultsWriter
		{
			public HEVOVCLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			#region Overrides of AbstractResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore));
				return new XElement(TNS + XMLNames.Report_Results,
					new XElement(TNS + XMLNames.Report_Result_Status,
						allSuccess
							? XMLNames.Report_Results_Status_Success_Val
							: XMLNames.Report_Results_Status_Error_Val),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}

			#endregion

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryHEVOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryHEVOVCSummaryWriter(_mrfFactory, TNS);

		}

		public class FCHVNonOVCLorry : AbstractMRFResultsWriter
        {
			public FCHVNonOVCLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryFCHVNonOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryFCHVNonOVCSummaryWriter(_mrfFactory, TNS);

		}

		public class FCHVOVCLorry : AbstractMRFResultsWriter
        {
			public FCHVOVCLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			#region Overrides of AbstractResultsWriter
			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVCFCHV(results);
				var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore));
				return new XElement(
					TNS + XMLNames.Report_Results,
					new XElement(
						TNS + XMLNames.Report_Result_Status,
						allSuccess ? XMLNames.Report_Results_Status_Success_Val : XMLNames.Report_Results_Status_Error_Val),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}
			#endregion

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryFCHVOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryFCHVOVCSummaryWriter(_mrfFactory, TNS);

		}

        public class PEVLorry : AbstractMRFResultsWriter
		{
			public PEVLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryPEVSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryPEVSummaryWriter(_mrfFactory, TNS);

		}
		
		// todo amogoda: 13. should PEV be treated as non-OVC? check correct content for PEV non-OVC
		public class PEVNonOVCLorry : AbstractMRFResultsWriter
		{
			public PEVNonOVCLorry(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetLorryPEVNonOVCSuccessResultWriter(_mrfFactory, TNS);

			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);

			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetLorryPEVNonOVCSummaryWriter(_mrfFactory, TNS);
		}

		public class ConventionalBus : AbstractMRFResultsWriter
		{
			public ConventionalBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusConvSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetBusErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusConvSummaryWriter(_mrfFactory, TNS);

		}

		public class HEVNonOVCBus : AbstractMRFResultsWriter
		{
			public HEVNonOVCBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusHEVNonOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetLorryErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusHEVNonOVCSummaryWriter(_mrfFactory, TNS);

		}

		public class HEVOVCBus : AbstractMRFResultsWriter
		{
			public HEVOVCBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore));
				return new XElement(TNS + XMLNames.Report_Results,
					new XElement(TNS + XMLNames.Report_Result_Status,
						allSuccess
							? XMLNames.Report_Results_Status_Success_Val
							: XMLNames.Report_Results_Status_Error_Val),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore) &&
						x.ChargeSustainingResult.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore)
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusHEVOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetBusErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusHEVOVCSummaryWriter(_mrfFactory, TNS);

		}

		public class FCHVNonOVCBus : AbstractMRFResultsWriter
        {
			public FCHVNonOVCBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusFCHVNonOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetBusErrorResultWriter(_mrfFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusFCHVNonOVCSummaryWriter(_mrfFactory, TNS);
		}

		public class FCHVOVCBus : AbstractMRFResultsWriter
        {

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore));
				return new XElement(TNS + XMLNames.Report_Results,
					new XElement(TNS + XMLNames.Report_Result_Status,
						allSuccess ? XMLNames.Report_Results_Status_Success_Val : XMLNames.Report_Results_Status_Error_Val),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusFCHVOVCSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetBusErrorResultWriter(_mrfFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusFCHVOVCSummaryWriter(_mrfFactory, TNS);

			public FCHVOVCBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }
		}

        public class PEVBus : AbstractMRFResultsWriter
		{
			public PEVBus(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _mrfFactory.GetBusPEVSuccessResultWriter(_mrfFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _mrfFactory.GetBusErrorResultWriter(_mrfFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _mrfFactory.GetBusPEVSummaryWriter(_mrfFactory, TNS);

		}

		public class ExemptedVehicle : AbstractMRFResultsWriter
		{
			public ExemptedVehicle(IMRFResultsWriterFactory mrfFactory) : base(mrfFactory) { }

			#region Implementation of IResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				return new XElement(TNS + XMLNames.Report_Results,
					new XElement(TNS + XMLNames.Report_Result_Status, XMLNames.Report_Results_Status_Success_Val),
					new XElement(TNS + XMLNames.Report_ExemptedVehicle));
			}

			[ExcludeFromCodeCoverage] // not used
			protected override IResultGroupWriter ResultSuccessWriter => null;

			[ExcludeFromCodeCoverage] // not used
			protected override IResultGroupWriter ResultErrorWriter => null;

			[ExcludeFromCodeCoverage] // not used
			public override IReportResultsSummaryWriter SummaryWriter => null;


			#endregion


		}
	}
}