using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class TempFileOutputWriter : FileOutputWriter
    {
		private readonly Dictionary<ReportType, XDocument> _storedReports = new Dictionary<ReportType, XDocument>();
		private readonly HashSet<ReportType> _reportsToWrite;
		private IOutputDataWriter BaseWriter;

		#region Overrides of FileOutputWriter

		public override string XMLFullReportName => Path.ChangeExtension(BaseWriter.JobFile, "RSLT_MANUFACTURER_PRIMARY.xml");

		#endregion

		/// <summary>
		/// Stores all written Documents by default.
		/// </summary>
		/// <param name="jobFile"></param>
		/// <param name="reportsToWrite">ReportTypes specified here are written to disk</param>
		public TempFileOutputWriter(IOutputDataWriter baseWriter, params ReportType[] reportsToWrite) : base(baseWriter.JobFile)
		{
			BaseWriter = baseWriter;
			_reportsToWrite = new HashSet<ReportType>();
			if (!(reportsToWrite is null))
				foreach (var reportType in reportsToWrite) {
					_reportsToWrite.Add(reportType);
				}
		}

		#region Overrides of FileOutputWriter

		public override IDictionary<ReportType, string> GetWrittenFiles()
		{
			return BaseWriter is FileOutputWriter ? base.GetWrittenFiles() : BaseWriter.GetWrittenFiles();
		}

		//public int NumberOfManufacturingStages { get; set; }
		
		//public XDocument MultistageXmlReport { get; }

		#endregion


		#region Overrides of FileOutputWriter
		public override void WriteReport(ReportType type, XDocument data)
		{
			if (type == ReportType.DeclarationReportPdf) {
				throw new ArgumentOutOfRangeException("PDF is not supported by TempFileOutputWriter");
			}

			if (_reportsToWrite.Contains(type)) {
				if (BaseWriter is FileOutputWriter) {
					base.WriteReport(type, data);
				} else {
					BaseWriter.WriteReport(type, data);
				}
			}

			_storedReports.Add(type, data);
		}

		public override void WriteReport(ReportType type, Stream data)
		{
			throw new NotImplementedException("PDF is not supported by TempFileOutputWriter");
		}
		#endregion

		public XDocument GetDocument(ReportType type)
		{
			
			var docStored = _storedReports.TryGetValue(type, out var report);
			if (!docStored) {
				Log.Warn($"No Document with type {type} stored");
			}
			return report;
		}

		#region Implementation of IModalDataWriter

		public override void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
		{
			BaseWriter.WriteModData(jobRunId, runName, cycleName, runSuffix, modData);
		}

		#endregion

		#region Implementation of ISummaryWriter

		public override void WriteSumData(DataTable sortedAndFilteredTable)
		{
			BaseWriter.WriteSumData(sortedAndFilteredTable);
		}

		#endregion

		#region Implementation of IOutputDataWriter

		//public string JobFile => BaseWriter.JobFile;

		#endregion
	}
}