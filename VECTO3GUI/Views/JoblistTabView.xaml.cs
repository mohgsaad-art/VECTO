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
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Views
{
	/// <summary>
	/// Interaction logic for JoblistTabView.xaml
	/// </summary>
	public partial class JoblistTabView : UserControl
	{
		public JoblistTabView()
		{
			InitializeComponent();
		}


		public JoblistTabView(IJobEditViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
