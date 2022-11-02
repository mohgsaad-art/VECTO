using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using VECTO3GUI.ViewModel;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Views
{
	/// <summary>
	/// Interaction logic for HomeView.xaml
	/// </summary>
	public partial class JoblistView : UserControl
	{
		public JoblistView()
		{
			InitializeComponent();
			((INotifyCollectionChanged)MessageList.Items).CollectionChanged += AutoScroll;
		}

		private void AutoScroll(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (MessageList.Items.Count <= 0) {
				return;
			}

			var border = VisualTreeHelper.GetChild(MessageList, 0) as Decorator;
			var scroll = border?.Child as ScrollViewer;
			scroll?.ScrollToEnd();
		}

		private void JobList_OnDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files == null) {
				return;
			}
			var vm = (IJoblistViewModel)DataContext;
			foreach (var file in files) {
				vm.HandleFileOpen(file);
			}
		}
	}
}
