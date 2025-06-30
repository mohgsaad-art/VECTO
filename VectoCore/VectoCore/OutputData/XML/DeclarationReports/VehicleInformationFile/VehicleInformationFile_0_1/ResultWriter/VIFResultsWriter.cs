using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter
{
	public abstract class AbstractVIFFResultsWriter : AbstractResultsWriter
	{
		protected readonly IVIFResultsWriterFactory _vifFactory;

		protected AbstractVIFFResultsWriter(IVIFResultsWriterFactory vifFactory)
		{
			_vifFactory = vifFactory;
		}
		protected override XNamespace TNS => "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";

	}

	public class VIFResultsWriter
	{
		public class ConventionalBus : AbstractVIFFResultsWriter
		{
			public ConventionalBus(IVIFResultsWriterFactory vifFactory) : base(vifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _vifFactory.GetBusConvSuccessResultWriter(_vifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _vifFactory.GetBusErrorResultWriter(_vifFactory, TNS);
			
            public override IReportResultsSummaryWriter SummaryWriter => _vifFactory.GetBusConvSummaryWriter(_vifFactory, TNS);

		}

		public class HEVNonOVCBus : AbstractVIFFResultsWriter
		{
			public HEVNonOVCBus(IVIFResultsWriterFactory vifFactory) : base(vifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _vifFactory.GetBusHEVNonOVCSuccessResultWriter(_vifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _vifFactory.GetBusErrorResultWriter(_vifFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _vifFactory.GetBusHEVNonOVCSummaryWriter(_vifFactory, TNS);

		}

		public class HEVOVCBus : AbstractVIFFResultsWriter
		{
			public HEVOVCBus(IVIFResultsWriterFactory vifFactory) : base(vifFactory) { }

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

			protected override IResultGroupWriter ResultSuccessWriter => _vifFactory.GetBusHEVOVCSuccessResultWriter(_vifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _vifFactory.GetBusErrorResultWriter(_vifFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _vifFactory.GetBusHEVOVCSummaryWriter(_vifFactory, TNS);

		}

		public class PEVBus : AbstractVIFFResultsWriter
		{
			public PEVBus(IVIFResultsWriterFactory vifFactory) : base(vifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _vifFactory.GetBusPEVSuccessResultWriter(_vifFactory, TNS);
			protected override IResultGroupWriter ResultErrorWriter => _vifFactory.GetBusErrorResultWriter(_vifFactory, TNS);
			
			public override IReportResultsSummaryWriter SummaryWriter => _vifFactory.GetBusPEVSummaryWriter(_vifFactory, TNS);

		}

		public class ExemptedVehicle : AbstractVIFFResultsWriter
		{
			public ExemptedVehicle(IVIFResultsWriterFactory vifFactory) : base(vifFactory) { }

			#region Implementation of IResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				return new XElement(TNS + "Results",
					new XElement(TNS + "Status", XMLNames.Report_Results_Status_Success_Val),
					new XElement(TNS + "ExemptedVehicle"));
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