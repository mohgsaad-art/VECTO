using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VECTO3GUI2020.ViewModel.Implementation;


namespace VECTO3GUI2020.Views
{
    /// <summary>
    /// Interaction logic for JobListView.xaml
    /// </summary>
    public partial class JobListView : UserControl
    {

		public JobListView()
		{
			InitializeComponent();
		
		}


		private async void JobDataGrid_OnDrop(object sender, DragEventArgs e)
		{
			var success = true;
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				

				var fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
				if (fileNames != null) {
					foreach (var fileName in fileNames) {
						await ((JobListViewModel)this.DataContext).AddJobAsync(fileName);
					}
				}

			} else {
				success = false;
			}

			if (!success) {
				e.Effects = DragDropEffects.None; //DO NOT ACCEPT THE DROP
			}
		}

		private void JobDataGrid_OnPreviewDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effects = DragDropEffects.None;
			}
		}
	}
}
