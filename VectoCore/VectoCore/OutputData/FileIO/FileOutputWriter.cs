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
		private readonly string _jobFile;

		internal string BasePath
		{
			get { return Path.GetDirectoryName(_jobFile); }
		}

		public string PDFReportName
		{
			get { return Path.ChangeExtension(_jobFile, Constants.FileExtensions.PDFReport); }
		}

		public string XMLFullReportName
		{
			get { return Path.ChangeExtension(_jobFile, "RSLT_MANUFACTURER.xml"); }
		}

		public string XMLCustomerReportName
		{
			get { return Path.ChangeExtension(_jobFile, "RSLT_CUSTOMER.xml"); }
		}

		public string XMLMonitoringReportName
		{
			get { return Path.ChangeExtension(_jobFile, "RSLT_MONITORING.xml"); }
		}

		public string XMLVTPReportName
		{
			get { return Path.ChangeExtension(_jobFile, "VTP_Report.xml"); }
		}

		public string SumFileName
		{
			get { return Path.ChangeExtension(_jobFile, Constants.FileExtensions.SumFile); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobFile">full path of the json job-file. jobName and basePath are extracted.</param>
		public FileOutputWriter(string jobFile)
		{
			_jobFile = jobFile;
		}

		public void WriteSumData(DataTable data)
		{
			VectoCSVFile.Write(SumFileName, data, true, true);
		}

		public string GetModDataFileName(string runName, string cycleName, string runSuffix)
		{
			string modFileName;
			if (!string.IsNullOrWhiteSpace(cycleName) || !string.IsNullOrWhiteSpace(runSuffix)) {
				modFileName = string.Format("{0}_{1}{2}{3}", runName, cycleName, runSuffix, Constants.FileExtensions.ModDataFile);
			} else {
				modFileName = string.Format("{0}{1}", runName, Constants.FileExtensions.ModDataFile);
			}

			return Path.Combine(BasePath, string.Concat(modFileName.Split(Path.GetInvalidFileNameChars())));
		}

		public void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
		{
			VectoCSVFile.Write(GetModDataFileName(runName, cycleName, runSuffix), modData, true);
		}

		public virtual void WriteReport(ReportType type, XDocument data)
		{
			string fileName = null;
			switch (type) {
				case ReportType.DeclarationReportManufacturerXML:
					fileName = XMLFullReportName;
					break;
				case ReportType.DeclarationReportCustomerXML:
					fileName = XMLCustomerReportName;
					break;
				case ReportType.DeclarationReportMonitoringXML:
					fileName = XMLMonitoringReportName;
					break;
				case ReportType.DeclarationVTPReportXML:
					fileName = XMLVTPReportName;
					break;
				default:
					throw new ArgumentOutOfRangeException("ReportType");
			}
			using (var writer = new FileStream(fileName, FileMode.Create)) {
				using (var xmlWriter = new XmlTextWriter(writer, Encoding.UTF8)) {
					xmlWriter.Formatting = Formatting.Indented;
					data.WriteTo(xmlWriter);
					xmlWriter.Flush();
					xmlWriter.Close();
				}
			}
		}

		

		public virtual void WriteReport(ReportType type, Stream data)
		{
			Stream stream = null;
			switch (type) {
				case ReportType.DeclarationReportPdf:
					stream = new FileStream(PDFReportName, FileMode.Create);
					break;
				default:

					throw new ArgumentOutOfRangeException("type");
			}
			data.CopyToAsync(stream);
			//stream.Write(data);
		}
	}
}