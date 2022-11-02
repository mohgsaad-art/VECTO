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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public class ManufacturerReportXMLFile : ReportXMLFile
	{
		private bool _manufacturerReportValid;

		public ManufacturerReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, XmlDocumentType.ManufacturerReport, hashValidation)
		{
			_xmlFile.PropertyChanged += UpdateComponents;
		}

		protected override void VerifyJobDataMatchesReport()
		{
			base.VerifyJobDataMatchesReport();

			DoUpdateComponentData();
		}

		private void UpdateComponents(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != GeneralUpdate) {
				return;
			}
			DoUpdateComponentData();
			RaisePropertyChanged(GeneralUpdate);
		}

		private void DoUpdateComponentData()
		{
			if (_xmlFile.Document == null || _xmlFile.Document.DocumentElement == null ||
				_xmlFile.IsValid != XmlFileStatus.ValidXML) {
				Components = new ComponentEntry[] { };
				RaisePropertyChanged("Components");
				return;
			}
			var components = XMLManufacturerReportReader.GetContainingComponents(_xmlFile.Document).GroupBy(s => s)
				.Select(g => new { Entry = g.Key, Count = g.Count() });
			var jobComponents = _jobData?.JobDataValid == null || !_jobData.JobDataValid.Value ? new ViewModel.ComponentEntry[] { } : _jobData.Components.ToArray();
			_validationErrors.Clear();

			if (_jobData != null && _jobData.JobDataValid != null && !_jobData.JobDataValid.Value) {
				_validationErrors.Add("Verifying Manufacturer Report: Job data is not valid - no validation checks against job data.");
			}

			var hasComponentsFromJob = _jobData != null && _jobData.JobDataValid != null && _jobData.JobDataValid.Value &&
										jobComponents.Any();

			// iterate over components in manufacturer report, read out c14n, digest method, digest;
			// collect c14n, digest method, digest value read, certification nr., digest value from job (re-computed)
			var componentData = new List<ComponentEntry>();
			foreach (var component in components) {
				if (component.Entry == VectoComponents.Vehicle) {
					continue;
				}
				for (var i = 0; i < component.Count; i++) {
					var node = XMLManufacturerReportReader.GetNodes(_xmlFile.Document, component.Entry, i);
					var entry = new ComponentEntry {
						Component = component.Count == 1
							? component.Entry.XMLElementName()
							: $"{component.Entry.XMLElementName()} ({i + 1})",
						DigestValue = ReadElementValue(node, XMLNames.DI_Signature_Reference_DigestValue),
						CertificationMethod = ReadElementValue(node, XMLNames.Report_Component_CertificationMethod),
					};
					if (entry.Component.StartsWith("Tyre ")) {
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Tyre_TyreCertificationNumber);
					} else {
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Component_CertificationNumber) ??
													ReadElementValue(node, XMLNames.Report_Component_CertificationMethod);
					}
					componentData.Add(entry);
					if (!hasComponentsFromJob) {
						continue;
					}
					var jobComponent = jobComponents.Where(x => x.Component == entry.Component).ToArray();
					if (!jobComponent.Any()) {
						continue;
					}
					entry.DigestValueMatchesJobComponent = jobComponent.First().DigestValueComputed == entry.DigestValue;
					entry.DigestValueExpected = jobComponent.First().DigestValueComputed;

					if (entry.CertificationMethod == CertificationMethod.StandardValues.ToXMLFormat()) {
						continue;
					}
					entry.CertificationNumberMatchesJobComponent = jobComponent.First().CertificationNumber ==
																	entry.CertificationNumber;
					entry.CertificationNumberExpected = jobComponent.First().CertificationNumber;
				}
			}
			Components = componentData.ToArray();
			RaisePropertyChanged("Components");
			var certificationNumberMismatch =
				componentData.Where(
					x => x.CertificationNumberMatchesJobComponent != null && !x.CertificationNumberMatchesJobComponent.Value).ToArray();
			var digestMismatch = componentData.Where(x => x.DigestValueMatchesJobComponent == null || !x.DigestValueMatchesJobComponent.Value).ToArray();
			if (jobComponents.Any()) {
				foreach (var entry in certificationNumberMismatch) {
					_validationErrors.Add("Verifying Manufacturer Report: " +
										$"Certification number for component '{entry.Component}' does not match! " +
										$"Job-file: '{entry.CertificationNumberExpected}', " +
										$"Report: '{entry.CertificationNumber}'");
				}
				foreach (var entry in digestMismatch) {
					_validationErrors.Add("Verifying Manufacturer Report: " +
										$"Digest Value for component '{entry.Component}' does not match! " +
										$"Job-file: '{entry.DigestValueExpected}', " +
										$"Report: '{entry.DigestValue}'");
				}
			}

			ManufacturerReportValid = FileIntegrityValid != null && FileIntegrityValid.Value && hasComponentsFromJob && !certificationNumberMismatch.Any() && !digestMismatch.Any();
		}

		public bool ManufacturerReportValid
		{
			get => _manufacturerReportValid;
			set {
				if (_manufacturerReportValid == value) {
					return;
				}
				_manufacturerReportValid = value;
				RaisePropertyChanged("ManufacturerReportValid");
			}
		}

		private string ReadElementValue(XmlNode xmlNode, string elementName)
		{
			var node = xmlNode.SelectSingleNode($"./*[local-name()='{elementName}']");
			if (node == null) {
				return null;
			}
			return node.InnerText;
		}

		

		

		public ComponentEntry[] Components { get; private set; }

		public class ComponentEntry
		{
			public string Component { get; set; }

			public string CertificationNumber { get; set; }

			public string DigestValue { get; set; }

			public string CertificationMethod { get; set; }

			public bool? DigestValueMatchesJobComponent { get; set; }

			public bool? CertificationNumberMatchesJobComponent { get; set; }

			public string DigestValueExpected { get; set; }

			public string CertificationNumberExpected { get; set; }
		}
	}
}
