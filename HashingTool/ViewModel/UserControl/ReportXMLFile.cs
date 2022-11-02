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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoHashing;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public class ReportXMLFile : HashedXMLFile
	{
		protected readonly ObservableCollection<string> _validationErrors = new ObservableCollection<string>();

		private string _jobDigestValueReadRead;
		private string _jobDigestMethodRead;
		private string[] _jobCanonicalizationMethodRead;
		private string _jobDigestComputed;
		private bool _jobDigestMatchesReport;
		protected VectoJobFile _jobData;
		private string _reportVin;

		public ReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck, XmlDocumentType xmlDocumentType, Action<XmlDocument, VectoXMLFile> hashValidation = null)
			: base(name, contentCheck, xmlDocumentType, hashValidation)
		{
			_xmlFile.PropertyChanged += ReportChanged;
		}

		public ObservableCollection<string> ValidationErrors => _validationErrors;

		public VectoJobFile JobData
		{
			private get { return _jobData; }
			set {
				if (_jobData == value) {
					return;
				}
				_jobData = value;
				_jobData.PropertyChanged += JobDataChanged;
			}
		}

		protected virtual void ReportChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender == _xmlFile && e.PropertyName == GeneralUpdate) {
				ReadReportData();
				VerifyJobDataMatchesReport();
			}
		}


		protected virtual void JobDataChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender == _jobData && e.PropertyName == GeneralUpdate) {
				VerifyJobDataMatchesReport();
			}
		}


		// check digest value and vin of report against given job
		protected virtual void VerifyJobDataMatchesReport()
		{
			_validationErrors.Clear();
			if (_xmlFile.IsValid != XmlFileStatus.ValidXML || _jobData == null ||
				_jobData.XMLFile.IsValid != XmlFileStatus.ValidXML) {
				JobDigestValueComputed = "";
				JobDigestMatchesReport = false;
				return;
			}
			try {
				var h = VectoHash.Load(_jobData.XMLFile.Document);
				var jobDigest = h.ComputeHash(JobCanonicalizationMethodRead,
					JobDigestMethodRead);
				JobDigestValueComputed = jobDigest;
				var digestMatch = _jobDigestComputed == JobDigestValueRead;
				var vinMatch = _jobData.VehicleIdentificationNumber == ReportVIN;

				if (!digestMatch) {
					_validationErrors.Add("Job Digest Value mismatch! " +
										$"Computed job digest: '{_jobDigestComputed}', " +
										$"digest read: '{JobDigestValueRead}'");
				}

				if (!vinMatch) {
					_validationErrors.Add("VIN mismatch! " +
										$"VIN from job data: '{_jobData.VehicleIdentificationNumber}', " +
										$"VIN from report: '{ReportVIN}'");
				}

				JobDigestMatchesReport = vinMatch
										&& digestMatch;
			} catch (Exception e) {
				_xmlFile.LogError(e.Message);
				JobDigestValueComputed = "";
				JobDigestMatchesReport = false;
			}
		}

		// readout all required fields from the report xml: c14n, digest method, digest value of job, VIN, ...
		protected virtual void ReadReportData()
		{
			var digestData = new DigestData("", new string[]{}, "", "");
			var vin = "";
			
			if (_xmlFile.Document != null && _xmlFile.Document.DocumentElement != null) {
				digestData = new DigestData(_xmlFile.Document.SelectSingleNode("//*[local-name()='InputDataSignature']"));
				var vinNode = _xmlFile.Document.SelectSingleNode("//*[local-name()='VIN']");
				if (vinNode != null) {
					vin = vinNode.InnerText;
				}
			}
			JobCanonicalizationMethodRead = digestData.CanonicalizationMethods;
			JobDigestMethodRead = digestData.DigestMethod;
			JobDigestValueRead = digestData.DigestValue;
			ReportVIN = vin;
			RaisePropertyChanged(GeneralUpdate);
		}

		public string ReportVIN
		{
			get => _reportVin;
			set {
				if (_reportVin == value) {
					return;
				}
				_reportVin = value;
				RaisePropertyChanged("ReportVIN");
			}
		}

		public string JobDigestMethodRead
		{
			get => _jobDigestMethodRead;
			set {
				if (_jobDigestMethodRead == value) {
					return;
				}
				_jobDigestMethodRead = value;
				RaisePropertyChanged("JobDigestMethodRead");
			}
		}

		public string[] JobCanonicalizationMethodRead
		{
			get => _jobCanonicalizationMethodRead;
			set {
				if (_jobCanonicalizationMethodRead == value) {
					return;
				}
				_jobCanonicalizationMethodRead = value;
				RaisePropertyChanged("JobCanonicalizationMethodRead");
			}
		}

		public string JobDigestValueRead
		{
			get => _jobDigestValueReadRead;
			internal set {
				if (_jobDigestValueReadRead == value) {
					return;
				}
				_jobDigestValueReadRead = value;
				RaisePropertyChanged("JobDigestValueRead");
			}
		}

		public string JobDigestValueComputed
		{
			get => _jobDigestComputed;
			protected set {
				if (_jobDigestComputed == value) {
					return;
				}
				_jobDigestComputed = value;
				RaisePropertyChanged("JobDigestValueComputed");
			}
		}

		public bool JobDigestMatchesReport
		{
			get => _jobDigestMatchesReport;
			protected set {
				if (_jobDigestMatchesReport == value) {
					return;
				}
				_jobDigestMatchesReport = value;
				RaisePropertyChanged("JobDigestMatchesReport");
			}
		}
	}
}
