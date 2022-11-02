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
using System.Windows;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyComponentInputDataViewModel : HashedXMLFile, IMainView, INotifyPropertyChanged
	{
		private string _certificationNumber;
		//private bool _componentDataValid;

		public VerifyComponentInputDataViewModel()
			: base("Verify Component Data", HashingHelper.IsComponentFile, XmlDocumentType.DeclarationComponentData, HashingHelper.ValidateDocumentHash)
		{
			_xmlFile.PropertyChanged += ComponentFilechanged;
		}

		public ICommand ShowHomeViewCommand => ApplicationViewModel.HomeView;

		public string CertificationNumber
		{
			get => _certificationNumber;
			private set {
				if (_certificationNumber == value)
					return;
				_certificationNumber = value;
				RaisePropertyChanged("CertificationNumber");
			}
		}

		private void ComponentFilechanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GeneralUpdate) {
				ReadComponentName();
			}
		}

		private void ReadComponentName()
		{
			if (_xmlFile.ContentValid == null || !_xmlFile.ContentValid.Value || _xmlFile.Document == null) {
				Component = "";
				return;
			}
			try {
				var h = VectoHash.Load(_xmlFile.Document);

				if (h.GetContainigComponents().Count != 1) {
					IoService.Messagebox("Selected file is not a component file!", "Error reading XML File", MessageBoxButton.OK);
					throw new InvalidDataException();
				}
				Component = h.GetContainigComponents().First().XMLElementName();
				CertificationNumber = h.GetCertificationNumber(h.GetContainigComponents().First(), 0);
			} catch (Exception e) {
				Component = "";
				_xmlFile.LogError(e.Message);
			}
		}
	}
}
