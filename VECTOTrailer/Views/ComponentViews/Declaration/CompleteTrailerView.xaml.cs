using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace VECTOTrailer.Views.ComponentViews.Declaration
{
	/// <summary>
	/// Interaction logic for CompleteTrailerView.xaml
	/// </summary>
	public partial class CompleteTrailerView : UserControl
	{
		public CompleteTrailerView()
		{
			InitializeComponent();
		}

		private void ScrollToEnd(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox) {
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
				{
					textBox.Focus();
					var caretIndex = textBox.Text.Length;
					caretIndex = caretIndex < 0 ? 0 : caretIndex;
					textBox.CaretIndex = caretIndex;
					Keyboard.ClearFocus();
				}));

			}
		}
	}
}
