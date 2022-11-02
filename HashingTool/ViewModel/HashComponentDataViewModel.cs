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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : VectoXMLFile, IMainView
	{
		private string _digestValue;

		private XDocument _result;

		private readonly RelayCommand _saveCommand;

		private bool? _componentDataValid;

		private bool _busy;
		private DateTime? _date;

		public HashComponentDataViewModel()
			: base("Hash Component Data", false, HashingHelper.IsComponentFile, XmlDocumentType.DeclarationComponentData)
		{
			_xmlFile.PropertyChanged += SourceChanged;
			_saveCommand = new RelayCommand(SaveDocument,
				() => !_busy && ComponentDataValid != null && ComponentDataValid.Value && _result != null);
		}


		public ICommand ShowHomeViewCommand => ApplicationViewModel.HomeView;

		public string DigestValue
		{
			get => _digestValue;
			set {
				if (_digestValue == value) {
					return;
				}
				_digestValue = value;
				RaisePropertyChanged("DigestValue");
			}
		}

		public ICommand SaveHashedDocument => _saveCommand;

		private void SourceChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GeneralUpdate) {
				DoComputeHash();
			}
		}

		private void SaveDocument()
		{
			var stream = IoService.SaveData(null, ".xml", "VECTO XML file|*.xml", out var filename);
			if (stream == null) {
				return;
			}
			using (var writer = new XmlTextWriter(stream, Encoding.UTF8) {
				Formatting = Formatting.Indented,
				Indentation = 4
			}) {
				_result.WriteTo(writer);
				writer.Flush();
				writer.Close();
			}
		}

		public bool? ComponentDataValid
		{
			get => _componentDataValid;
			private set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				RaisePropertyChanged("ComponentDataValid");
			}
		}

		private async void DoComputeHash()
		{
			if (_busy) {
				return;
			}
			if (_xmlFile.Document == null || _xmlFile.IsValid != XmlFileStatus.ValidXML) {
				ComponentDataValid = false;
				DigestValue = "";
				DigestMethod = "";
				SetCanonicalizationMethod(new string[] { });
				return;
			}

			try {
				_busy = true;
				ComponentDataValid = false;
				DigestValue = "";
				_xmlFile.XMLValidationErrors.Clear();
				SetCanonicalizationMethod(new string[] { });

				var h = VectoHash.Load(_xmlFile.Document);

				_result = h.AddHash();
				Date = h.GetCertificationDate(h.GetContainigComponents().First(), 0);
				// validate generated component file
				using (MemoryStream ms = new MemoryStream()) {
					using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true })) {
						_result.WriteTo(xw);
						xw.Flush();
					}
					ms.Flush();
					ms.Seek(0, SeekOrigin.Begin);
					ComponentDataValid = true;
					var validator = new AsyncXMLValidator(XmlReader.Create(ms), r => { ComponentDataValid = r; },
						(s, e) => {
							Application.Current.Dispatcher.Invoke(() => _xmlFile.LogError(
								string.Format("Validation {0} Line {2}: {1}", 
									s == XmlSeverityType.Warning ? "WARNING" : "ERROR",
									e.ValidationEventArgs == null
										? e.Exception.Message + (e.Exception.InnerException != null ? Environment.NewLine + e.Exception.InnerException.Message : "")
										: e.ValidationEventArgs.Message,
									e.ValidationEventArgs?.Exception.LineNumber ?? 0)));
						});
					await validator.ValidateXML(XmlDocumentType.DeclarationComponentData);
				}
				if (ComponentDataValid != null && ComponentDataValid.Value) {
					//var c14N = XMLHashProvider.DefaultCanonicalizationMethod.ToArray();
					//var digestMethod = XMLHashProvider.DefaultDigestMethod;
					//DigestValue = h.ComputeHash(c14N, digestMethod);
					using (MemoryStream ms = new MemoryStream()) {
						using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true })) {
							_result.WriteTo(xw);
							xw.Flush();
						}
						ms.Flush();
						ms.Seek(0, SeekOrigin.Begin);
						var h2 = VectoHash.Load(ms);
						DigestMethod = h2.GetDigestMethod();
						DigestValue = h2.ReadHash();
						SetCanonicalizationMethod(h2.GetCanonicalizationMethods());
					}
				}
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValue = "";
				_xmlFile.LogError(e.Message);
				SetCanonicalizationMethod(new string[] { });
				DigestMethod = "";
				Date = null;
			} finally {
				_busy = false;
				_saveCommand.RaiseCanExecuteChanged();
			}
		}

		public DateTime? Date
		{
			get => _date;
			set
			{
				if (_date == value) {
					return;
				}
				_date = value;
				RaisePropertyChanged("Date");
			}
		}
	}
}
