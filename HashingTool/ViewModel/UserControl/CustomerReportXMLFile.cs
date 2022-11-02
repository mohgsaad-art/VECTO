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
using System.ComponentModel;
using System.Linq;
using System.Xml;
using TUGraz.VectoHashing;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public class CustomerReportXMLFile : ReportXMLFile
	{
		private ManufacturerReportXMLFile _manufacturerReport;
		private string[] _manufacturerReportCanonicalizationMethodRead;
		private string _manufacturerReportDigestMethodRead;
		private string _manufacturerReportDigestValueRead;
		private bool _manufacturerReportMatchesReport;
		private string _manufacturerReportDigestValueComputed;
		private bool _manufacturerReportDigestValid;

		public CustomerReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, XmlDocumentType.CustomerReport, hashValidation)
		{
			
		}


		public ManufacturerReportXMLFile ManufacturerReport
		{
			get => _manufacturerReport;
			set {
				if (_manufacturerReport == value) {
					return;
				}
				_manufacturerReport = value;
				_manufacturerReport.PropertyChanged += ManufacturerReportChanged;
			}
		}

		public string[] ManufacturerReportCanonicalizationMethodRead
		{
			get => _manufacturerReportCanonicalizationMethodRead;
			set
			{
				if (_manufacturerReportCanonicalizationMethodRead == value) {
					return;
				}
				_manufacturerReportCanonicalizationMethodRead = value;
				RaisePropertyChanged("ManufacturerReportCanonicalizationMethodRead");
			}
		}

		public string ManufacturerReportDigestMethodRead
		{
			get => _manufacturerReportDigestMethodRead;
			set
			{
				if (_manufacturerReportDigestMethodRead == value) {
					return;
				}
				_manufacturerReportDigestMethodRead = value;
				RaisePropertyChanged("ManufacturerReportDigestMethodRead");
			}
		}

		public string ManufacturerReportDigestValueRead
		{
			get => _manufacturerReportDigestValueRead;
			set
			{
				if (_manufacturerReportDigestValueRead == value) {
					return;
				}
				_manufacturerReportDigestValueRead = value;
				RaisePropertyChanged("ManufacturerReportDigestValueRead");
			}
		}

		public bool ManufacturerReportMatchesReport
		{
			get => _manufacturerReportMatchesReport;
			set
			{
				if (_manufacturerReportMatchesReport == value) {
					return;
				}
				_manufacturerReportMatchesReport = value;
				RaisePropertyChanged("ManufacturerReportMatchesReport");
			}
		}

		public string ManufacturerReportDigestValueComputed
		{
			get => _manufacturerReportDigestValueComputed;
			set
			{
				if (_manufacturerReportDigestValueComputed == value) {
					return;
				}
				_manufacturerReportDigestValueComputed = value;
				RaisePropertyChanged("ManufacturerReportDigestValueComputed");
			}
		}
		
		public bool ManufacturerReportDigestValid
		{
			get => _manufacturerReportDigestValid;
			set {
				if (_manufacturerReportDigestValid == value) {
					return;
				}
				_manufacturerReportDigestValid = value;
				RaisePropertyChanged("ManufacturerReportDigestValid");
			}
		}

		protected virtual void ManufacturerReportChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender == _manufacturerReport && e.PropertyName == GeneralUpdate) {
				VerifyManufacturerReport();
			}
		}

		protected override void ReportChanged(object sender, PropertyChangedEventArgs e)
		{
			base.ReportChanged(sender, e);
			if (sender == _xmlFile && e.PropertyName == GeneralUpdate) {
				VerifyManufacturerReport();
			}
		}


		protected virtual void VerifyManufacturerReport()
		{
			var manufacturerReportDigestValueRead = "";
			var manufacturerReportDigestMethodRead = "";
			var manufacturerReportCanonicalizationMethodRead = new string[] { };

			var manufacturerReportDigestValueComputed = "";
			var digestMatch = false;

			if (_xmlFile.Document != null && _xmlFile.Document.DocumentElement != null) {
				var digestValueNode =
					_xmlFile.Document.SelectSingleNode("//*[local-name()='ResultDataSignature']//*[local-name()='DigestValue']");
				if (digestValueNode != null) {
					manufacturerReportDigestValueRead = digestValueNode.InnerText;
				}
				var digestMethodNode =
					_xmlFile.Document.SelectSingleNode(
						"//*[local-name()='ResultDataSignature']//*[local-name()='DigestMethod']/@Algorithm");
				if (digestMethodNode != null) {
					manufacturerReportDigestMethodRead = digestMethodNode.InnerText;
				}

				var c14NtMethodNodes =
					_xmlFile.Document.SelectNodes("//*[local-name()='ResultDataSignature']//*[local-name()='Transform']/@Algorithm");
				if (c14NtMethodNodes != null) {
					manufacturerReportCanonicalizationMethodRead = (from XmlNode node in c14NtMethodNodes select node.InnerText).ToArray();
				}

				if (_manufacturerReport != null && _manufacturerReport.XMLFile != null && _manufacturerReport.XMLFile.IsValid == XmlFileStatus.ValidXML) {
					var h = VectoHash.Load(_manufacturerReport.XMLFile.Document);
					manufacturerReportDigestValueComputed = h.ComputeHash(manufacturerReportCanonicalizationMethodRead,
						manufacturerReportDigestMethodRead);
					digestMatch = manufacturerReportDigestValueRead == manufacturerReportDigestValueComputed;
				}
			}
			ManufacturerReportDigestMethodRead = manufacturerReportDigestMethodRead;
			ManufacturerReportCanonicalizationMethodRead = manufacturerReportCanonicalizationMethodRead;
			ManufacturerReportDigestValueRead = manufacturerReportDigestValueRead;
			ManufacturerReportDigestValueComputed = manufacturerReportDigestValueComputed;

			ManufacturerReportMatchesReport = FileIntegrityValid != null && FileIntegrityValid.Value && digestMatch;

			ManufacturerReportDigestValid = digestMatch;
		}

	}
}