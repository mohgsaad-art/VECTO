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
using System.Xml;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace HashingTool.ViewModel.UserControl
{
	public class HashedXMLFile : VectoXMLFile
	{
		protected string _digestValueRead;
		private DateTime? _date;

		public HashedXMLFile(
			string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck, XmlDocumentType xmlDocumentType,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(
			name, true, contentCheck, xmlDocumentType, hashValidation) { }

		public string DigestValueRead
		{
			get => _digestValueRead;
			internal set {
				if (_digestValueRead == value) {
					return;
				}
				_digestValueRead = value;
				RaisePropertyChanged("DigestValueRead");
			}
		}

		protected override void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FileChanged(sender, e);
			if (e.PropertyName != GeneralUpdate) {
				return;
			}

			if (_xmlFile.IsValid == XmlFileStatus.ValidXML && _validateHashes != null) {} else {
				DigestValueRead = "";
				DigestValueComputed = "";
				Date = null;
				SetCanonicalizationMethod(new string[] { });
				DigestMethod = "";
				Component = "";
			}
			RaisePropertyChanged(GeneralUpdate);
		}

		public DateTime? Date
		{
			get => _date;
			internal set {
				if (_date == value) {
					return;
				}
				_date = value;
				RaisePropertyChanged("Date");
			}
		}
	}
}
