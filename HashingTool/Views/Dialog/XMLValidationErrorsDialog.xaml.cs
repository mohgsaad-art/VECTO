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
using System.Collections;
using System.Linq;
using System.Windows;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for XMLValidationErrorsDialog.xaml
	/// </summary>
	public partial class XMLValidationErrorsDialog : Window
	{
		public static readonly DependencyProperty XMLErrorsProperty = DependencyProperty.Register("XMLErrors",
			typeof(ICollection), typeof(XMLValidationErrorsDialog));

		public static readonly DependencyProperty ErrorCountProperty = DependencyProperty.Register("ErrorCount",
			typeof(int), typeof(XMLValidationErrorsDialog));

		public XMLValidationErrorsDialog()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;
		}

		public ICollection XMLErrors
		{
			get => (ICollection)GetValue(XMLErrorsProperty);
			set => SetValue(XMLErrorsProperty, value);
		}

		public int ErrorCount
		{
			get {
				var value = GetValue(ErrorCountProperty);
				if (value != null) {
					return (int)value;
				}
				return 0;
			}
			set => SetValue(ErrorCountProperty, value);
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			var errors = string.Join(Environment.NewLine,(from object item in lbErrors.Items select item.ToString()).ToList());

			Clipboard.SetText( errors);
		}
	}
}
