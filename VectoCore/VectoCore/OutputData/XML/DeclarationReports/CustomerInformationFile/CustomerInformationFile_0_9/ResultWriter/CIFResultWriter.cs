using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
    public abstract class AbstractCIFResultsWriter : AbstractResultsWriter
	{

		protected readonly ICIFResultsWriterFactory _cifFactory;

		protected AbstractCIFResultsWriter(ICIFResultsWriterFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Overrides of AbstractResultsWriter

		protected override XNamespace TNS => AbstractCustomerReport.Namespace;
		
		#endregion
	}

	public class CIFResultsWriter
	{

		public class ConventionalLorry : AbstractCIFResultsWriter
		{
			public ConventionalLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Overrides of AbstractResultsWriter

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryConvSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);
			
            public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryConvSummaryWriter(_cifFactory, TNS);

			#endregion
		}

		public class HEVNonOVCLorry : AbstractCIFResultsWriter
		{
			public HEVNonOVCLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryHEVNonOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);
			
            public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryHEVNonOVCSummaryWriter(_cifFactory, TNS);

		}

		public class HEVOVCLorry : AbstractCIFResultsWriter
		{
			public HEVOVCLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Overrides of AbstractResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore) );
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

			#endregion

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryHEVOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);
			
            public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryHEVOVCSummaryWriter(_cifFactory, TNS);

		}

		public class FCHVNonOVCLorry : AbstractCIFResultsWriter
		{
			public FCHVNonOVCLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryFCHVNonOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryFCHVNonOVCSummaryWriter(_cifFactory, TNS);

		}

		public class FCHVOVCLorry : AbstractCIFResultsWriter
		{
			public FCHVOVCLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Overrides of AbstractResultsWriter
			
			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVCFCHV(results);
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
			
			#endregion

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryFCHVOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryFCHVOVCSummaryWriter(_cifFactory, TNS);

		}

		public class PEVLorry : AbstractCIFResultsWriter
		{
			public PEVLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryPEVSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);
			
            public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryPEVSummaryWriter(_cifFactory, TNS);

		}

		public class PEVNonOVCLorry : AbstractCIFResultsWriter
		{
			public PEVNonOVCLorry(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryPEVNonOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetLorryPEVNonOVCSummaryWriter(_cifFactory, TNS);
		}

		public class ConventionalBus : AbstractCIFResultsWriter
		{
			public ConventionalBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusConvSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter(_cifFactory, TNS);
			
			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusConvSummaryWriter(_cifFactory, TNS);

		}

		public class HEVNonOVCBus : AbstractCIFResultsWriter
		{
			public HEVNonOVCBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusHEVNonOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter(_cifFactory, TNS);
			
			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusHEVNonOVCSummaryWriter(_cifFactory, TNS);

		}

		public class HEVOVCBus : AbstractCIFResultsWriter
		{
			public HEVOVCBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

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

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusHEVOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter(_cifFactory, TNS);
			
			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusHEVOVCSummaryWriter(_cifFactory, TNS);

		}

		public class FCHVNonOVCBus : AbstractCIFResultsWriter
		{
			public FCHVNonOVCBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusFCHVNonOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter(_cifFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusFCHVNonOVCSummaryWriter(_cifFactory, TNS);
		}

        public class FCHVOVCBus : AbstractCIFResultsWriter
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

            protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusFCHVOVCSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter(_cifFactory, TNS);

			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusFCHVOVCSummaryWriter(_cifFactory, TNS);

			public FCHVOVCBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

        public class PEVBus : AbstractCIFResultsWriter
		{
			public PEVBus(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusPEVSuccessResultWriter(_cifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter(_cifFactory, TNS);
			
			public override Common.IReportResultsSummaryWriter SummaryWriter => _cifFactory.GetBusPEVSummaryWriter(_cifFactory, TNS);

		}

		public class ExemptedVehicle : AbstractCIFResultsWriter
		{
			public ExemptedVehicle(ICIFResultsWriterFactory cifFactory) : base(cifFactory) { }

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
			public override Common.IReportResultsSummaryWriter SummaryWriter => null;


			#endregion


		}
	}
}