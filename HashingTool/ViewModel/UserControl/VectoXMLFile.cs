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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using HashingTool.Helper;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public class VectoXMLFile : ObservableObject
	{
		protected readonly XMLFileSelector _xmlFile;

		protected string _digestValueComputed;
		protected bool? _fileIntegrityValid;
		protected string _name;
		protected string _tooltip;
		protected string _componentType;
		protected readonly Action<XmlDocument, VectoXMLFile> _validateHashes;
		private string _digestMethod;


		public VectoXMLFile(string name, bool validate, Func<XmlDocument, IErrorLogger, bool?> contentCheck, XmlDocumentType xmlDocumentType, Action<XmlDocument, VectoXMLFile> hashValidation = null)
		{
			_validateHashes = hashValidation;
			_xmlFile = new XMLFileSelector(IoService, name, xmlDocumentType, validate, contentCheck);
			_xmlFile.PropertyChanged += FileChanged;
			Name = name;
			CanonicalizationMethods = new ObservableCollection<string>();

			FileIntegrityValid = null;
			FileIntegrityTooltip = HashingHelper.ToolTipNone;
		}

		protected virtual void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != GeneralUpdate) {
				return;
			}

			if (_xmlFile.IsValid == XmlFileStatus.ValidXML && _validateHashes != null) {
				_validateHashes(_xmlFile.Document, this);
			} else {
				FileIntegrityValid = null;
			}
			RaisePropertyChanged(GeneralUpdate);
		}


		public XMLFileSelector XMLFile => _xmlFile;

		public string Name
		{
			get => _name;
			private set {
				if (_name == value) {
					return;
				}
				_name = value;
				RaisePropertyChanged("Name");
			}
		}

		public ObservableCollection<string> CanonicalizationMethods { get; private set; }

		public void SetCanonicalizationMethod(IEnumerable<string> c14NMethods)
		{
			CanonicalizationMethods.Clear();
			foreach (var c14N in c14NMethods) {
				CanonicalizationMethods.Add(c14N);
			}
			RaisePropertyChanged("CanonicalizationMethods");
		}

		public string DigestMethod
		{
			get => _digestMethod;
			set {
				if (_digestMethod == value) {
					return;
				}
				_digestMethod = value;
				RaisePropertyChanged("DigestMethod");
			}
		}


		public string DigestValueComputed
		{
			get => _digestValueComputed;
			internal set {
				if (_digestValueComputed == value) {
					return;
				}
				_digestValueComputed = value;
				RaisePropertyChanged("DigestValueComputed");
			}
		}


		public bool? FileIntegrityValid
		{
			get => _fileIntegrityValid;
			internal set {
				if (_fileIntegrityValid == value) {
					return;
				}
				_fileIntegrityValid = value;
				RaisePropertyChanged("FileIntegrityValid");
			}
		}

		public string FileIntegrityTooltip
		{
			get => _tooltip;
			set {
				if (_tooltip == value) {
					return;
				}
				_tooltip = value;
				RaisePropertyChanged("FileIntegrityTooltip");
			}
		}

		public string Component
		{
			get => _componentType;
			set {
				if (_componentType == value) {
					return;
				}
				_componentType = value;
				RaisePropertyChanged("Component");
			}
		}
	}
}
