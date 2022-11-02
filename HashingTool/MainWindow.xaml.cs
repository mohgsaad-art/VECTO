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
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using HashingTool.ViewModel;
using HashingTool.Views;

namespace HashingTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void About_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var dialog = new AboutDialog();
			if (DataContext is ApplicationViewModel applicationViewModel) {
				dialog.Title = applicationViewModel.VersionInformation;
			}
			dialog.ShowDialog();
		}

		private void Help_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", @"User Manual\HashingToolHelp.html");
			if (File.Exists(path)) {
				Process.Start(new ProcessStartInfo(path) { UseShellExecute = true});
			} else {
				MessageBox.Show("User Manual not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
