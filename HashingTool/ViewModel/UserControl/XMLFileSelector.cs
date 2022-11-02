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
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public enum XmlFileStatus
	{
		Unknown, // no file selected,				outline
		Invalid, // reading failed, no xml, etc.		red
		InvalidXML, // does not validate against known schemas
		IncorrectContent, // content validation failed			
		ValidXML //									green
	}

	public interface IErrorLogger
	{
		void LogError(string message);
	}

	public class XMLFileSelector : ObservableObject, IErrorLogger
	{
		private string _source;
		private XmlFileStatus _isValid;

		private bool _busy;

		private readonly bool _validate;
		private XmlDocument _document;
		private readonly Func<XmlDocument, IErrorLogger, bool?> _contentVerification;
		private bool? _contentValid;
		private RelayCommand _browseFileCommand;
		private string _prefix;
		private XmlDocumentType _expectedDocumentType;

		public XMLFileSelector(IOService ioservice, string prefix, XmlDocumentType expectedDocumentType, bool validate = false,
			Func<XmlDocument, IErrorLogger, bool?> contentCheck = null)
		{
			IoService = ioservice;
			_validate = validate;
			_prefix = prefix;
			_expectedDocumentType = expectedDocumentType;
			_browseFileCommand = new RelayCommand(BrowseXMLFile, () => !_busy);
			XMLValidationErrors = new ObservableCollection<string>();
			HasContentValidation = contentCheck != null;
			_contentVerification = contentCheck ?? ((x, c) => null);
			Source = "";
			RaisePropertyChanged("ValidateInput");
			RaisePropertyChanged("HasContentValidation");
		}

		public XmlDocument Document
		{
			get => _document;
			private set {
				if (_document == value) {
					return;
				}
				_document = value;
				RaisePropertyChanged("Document");
			}
		}

		public string Source
		{
			get => _source;
			private set {
				if (_source == value) {
					return;
				}
				SetXMLFile(value);
				_source = value;
				RaisePropertyChanged("Source");
			}
		}

		public bool ValidateInput => _validate;

		public XmlFileStatus IsValid
		{
			get => _isValid;
			private set {
				if (_isValid == value) {
					return;
				}
				_isValid = value;
				RaisePropertyChanged("IsValid");
				SetToolTip(value);
			}
		}

		private void SetToolTip(XmlFileStatus value)
		{
			var toolTip = "";
			switch (value) {
				case XmlFileStatus.Unknown:
					toolTip = "Select XML File file";
					break;
				case XmlFileStatus.Invalid:
					toolTip = "Invalid file";
					break;
				case XmlFileStatus.InvalidXML:
					toolTip = "XML Schema validation failed";
					break;
				case XmlFileStatus.IncorrectContent:
					toolTip = "Incorrect XML content";
					break;
				case XmlFileStatus.ValidXML:
					toolTip = "OK";
					break;
			}
			if (ToolTip == toolTip) {
				return;
			}
			ToolTip = toolTip;
			RaisePropertyChanged("ToolTip");
		}

		public string ToolTip { get; internal set; }

		public ObservableCollection<string> XMLValidationErrors { get; set; }

		public ICommand BrowseFileCommand => _browseFileCommand;

		public ICommand SetXMLFileCommnd
		{
			get { return new RelayCommand<string>(SetXMLFile, f => !_busy); }
		}

		private async void SetXMLFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				Document = null;
				XMLValidationErrors.Clear();
				ContentValid = null;
				IsValid = XmlFileStatus.Unknown;
				return;
			}

			try {
				_source = fileName;
				RaisePropertyChanged("Source");
				using (var stream = File.OpenRead(fileName)) {
					await LoadXMLFile(stream);
				}
			} catch (Exception e) {
				LogError(e.Message);
			}
		}


		private async void BrowseXMLFile()
		{
			try {
				using (var stream = IoService.OpenFileDialog(null, ".xml", "VECTO XML file|*.xml", out var filename)) {
					if (stream == null) {
						return;
					}

					Source = filename;
					//await LoadXMLFile(stream);
				}
			} catch (Exception e) {
				LogError(e.Message);
			}
		}

		private async Task LoadXMLFile(Stream stream)
		{
			_busy = true;
			IsValid = XmlFileStatus.Unknown;
			ContentValid = null;
			XMLValidationErrors.Clear();
			var fileValid = XmlFileStatus.ValidXML;
			bool? contentValid = null;
			try {
				var ms = new MemoryStream();
				if (_validate) {
					// copy stream beforehand if validation is needed later on
					await stream.CopyToAsync(ms);
					ms.Seek(0, SeekOrigin.Begin);
					stream.Seek(0, SeekOrigin.Begin);
				}

				var document = new XmlDocument();
				var reader = XmlReader.Create(stream);
				document.Load(reader);
				Document = document;
				var xmlValid = true;
				if (_validate) {
					xmlValid = await Validate(XmlReader.Create(ms));
					if (!xmlValid) {
						fileValid = XmlFileStatus.InvalidXML;
					}
				}
				if (HasContentValidation) {
					contentValid = _contentVerification(document, this);
					if (xmlValid && (contentValid == null || !contentValid.Value)) {
						fileValid = XmlFileStatus.IncorrectContent;
					}
				}
			} catch (Exception e) {
				LogError(e.Message);
				fileValid = XmlFileStatus.Invalid;
			} finally {
				IsValid = fileValid;
				ContentValid = contentValid;
				_busy = false;
				_browseFileCommand.RaiseCanExecuteChanged();

				RaisePropertyChanged(GeneralUpdate);
			}
		}

		public void LogError(string message) 
		{
			XMLValidationErrors.Add($"{_prefix}: {message}");
		}

		public bool HasContentValidation { get; private set; }

		public bool? ContentValid
		{
			get => _contentValid;
			set {
				if (_contentValid == value) {
					return;
				}
				_contentValid = value;
				RaisePropertyChanged("ContentValid");
			}
		}

		private async Task<bool> Validate(XmlReader xml)
		{
			var valid = true;
			try {
				var validator = new AsyncXMLValidator(xml, r => { valid = r; },
													(s, e) => {
														Application.Current.Dispatcher.Invoke(
															() => {
																if (e.ValidationEventArgs == null) {
																	LogError(
																		$"XML file does not validate against a supported version of {_expectedDocumentType.ToString()}");
																} else {
																	LogError(
																		string.Format(
																			"Validation {0} Line {2}: {1}",
																			s == XmlSeverityType.Warning ? "WARNING" : "ERROR",
																			e.ValidationEventArgs == null
																				? e.Exception.Message +
																				(e.Exception.InnerException != null
																					? Environment.NewLine + e.Exception.InnerException.Message
																					: "")
																				: e.ValidationEventArgs.Message,
																			e.ValidationEventArgs?.Exception.LineNumber ?? 0));
																}
															}
														);
													}
				);
				valid = await validator.ValidateXML(_expectedDocumentType);
			} catch (Exception e) {
				LogError(e.Message);
			}
			return valid;
		}
	}
}
