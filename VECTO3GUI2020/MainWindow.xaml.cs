using System.Windows;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        



        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
