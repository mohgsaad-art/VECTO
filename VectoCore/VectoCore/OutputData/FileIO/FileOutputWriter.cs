/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class FileOutputWriter : LoggingObject, IOutputDataWriter
	{
		protected readonly string _jobFile;

		public string JobFile => _jobFile;

		protected ConcurrentDictionary<ReportType, string> _writtenReports = new ConcurrentDictionary<ReportType, string>();
		public virtual IDictionary<ReportType, string> GetWrittenFiles()
		{
			return _writtenReports;
		}

		private volatile bool _sumFileWritten = false;
		public bool SumFileWritten => _sumFileWritten;

		public const string REPORT_ENDING_PREFIX = "VIF_Report_";

		private int? _numberOfManufacturingStages = null;

		public int NumberOfManufacturingStages
		{
			set => _numberOfManufacturingStages = value;
		}

		public XDocument MultistageXmlReport => XDocument.Load(XMLMultistageReportFileName);

		public string XMLMultistageReportFileName
		{
			get
			{
				return Path.ChangeExtension(
					RemoveExistingVIFEndingPrefix(_jobFile),
					$"{REPORT_ENDING_PREFIX}{(_numberOfManufacturingStages ?? 0) + 2}.xml");
			}
		}

		public string BasePath => Path.GetDirectoryName(_jobFile);

		public string PDFReportName => Path.ChangeExtension(_jobFile, Constants.FileExtensions.PDFReport);

		public virtual string XMLFullReportName => Path.ChangeExtension(_jobFile, "RSLT_MANUFACTURER.xml");


		public string XMLCustomerReportName => Path.ChangeExtension(_jobFile, "RSLT_CUSTOMER.xml");

		public string XMLPrimaryVehicleReportName => Path.ChangeExtension(_jobFile, "RSLT_VIF.xml");

		public virtual string XMLMonitoringReportName => Path.ChangeExtension(_jobFile, "RSLT_MONITORING.xml");

		public string XMLVTPReportName => Path.ChangeExtension(_jobFile, "VTP_Report.xml");

		public string SumFileName => Path.ChangeExtension(_jobFile, Constants.FileExtensions.SumFile);


		public List<string> ModDataFiles { get; protected set; } = new List<string>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobFile">full path of the json job-file. jobName and basePath are extracted.</param>
		public FileOutputWriter(string jobFile)
		{
			_jobFile = jobFile;
		}

		protected FileOutputWriter(string jobFile, int numberOfManufacturingStages) : this(jobFile)
		{
			_numberOfManufacturingStages = numberOfManufacturingStages;
		}



		protected string RemoveExistingVIFEndingPrefix(string jobFile)
		{
			var vifReportIndex = jobFile.IndexOf(REPORT_ENDING_PREFIX, StringComparison.Ordinal);
			if (vifReportIndex == -1)
				return jobFile;

			if (!jobFile.Contains(REPORT_ENDING_PREFIX))
				return jobFile;

			return $"{jobFile.Substring(0, vifReportIndex - 1)}.xml";
		}

		public virtual void WriteSumData(DataTable data)
		{
			VectoCSVFile.Write(SumFileName, data, true, true);
			_sumFileWritten = true;
		}

		public virtual string GetModDataFileName(string runName, string cycleName, string runSuffix)
		{
			string modFileName;
			if (!string.IsNullOrWhiteSpace(cycleName) || !string.IsNullOrWhiteSpace(runSuffix)) {
				modFileName = $"{runName}_{cycleName}{runSuffix}{Constants.FileExtensions.ModDataFile}";
			} else {
				modFileName = $"{runName}{Constants.FileExtensions.ModDataFile}";
			}

			return Path.Combine(BasePath, string.Concat(modFileName.Split(Path.GetInvalidFileNameChars())));
		}

		public virtual void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
		{
			var modDataFileName = GetModDataFileName(runName, cycleName, runSuffix);
			
			VectoCSVFile.Write(modDataFileName, modData, true);
			
			ModDataFiles.Add(modDataFileName);
		}

		public virtual void WriteReport(ReportType type, XDocument data)
		{
			var fileName = GetReportFilename(type);
			
			if (File.Exists(fileName)) {
				Log.Warn($"Overwriting file ({fileName})");
			}
			using (var writer = new FileStream(fileName, FileMode.Create)) {
				using (var xmlWriter = new XmlTextWriter(writer, Encoding.UTF8)) {
					xmlWriter.Formatting = Formatting.Indented;
					data.WriteTo(xmlWriter);
					
					xmlWriter.Flush();
					xmlWriter.Close();
					
				}
			}

			var added = _writtenReports.TryAdd(type, fileName);
		}

		protected virtual string GetReportFilename(ReportType type)
		{
			string fileName = null;
			switch (type) {
				case ReportType.DeclarationReportManufacturerXML:
					fileName = XMLFullReportName;
					break;
				case ReportType.DeclarationReportCustomerXML:
					fileName = XMLCustomerReportName;
					break;
				case ReportType.DeclarationReportPrimaryVehicleXML:
					fileName = XMLPrimaryVehicleReportName;
					break;
				case ReportType.DeclarationReportMonitoringXML:
					fileName = XMLMonitoringReportName;
					break;
				case ReportType.DeclarationVTPReportXML:
					fileName = XMLVTPReportName;
					break;
				case ReportType.DeclarationReportMultistageVehicleXML:
					fileName = XMLMultistageReportFileName;
					break;
				default:
					throw new ArgumentOutOfRangeException("ReportType");
			}

			return fileName;
		}


		public virtual void WriteReport(ReportType type, Stream data)
		{
			if (type != ReportType.DeclarationReportPdf)
			{
                throw new ArgumentOutOfRangeException($"ReportType is {type}, but {ReportType.DeclarationReportPdf} is expected.");
            }

			using (Stream stream = new FileStream(PDFReportName, FileMode.Create))
			{
                data.CopyToAsync(stream);
            }
		}
	}
}