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

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{
    /// <summary>
    /// Interaction logic for FilePicker.xaml
    /// </summary>
    public partial class FilePicker : UserControl
    {
		public static readonly DependencyProperty ButtonContentTemplateProperty = DependencyProperty.Register(
			"ButtonContentTemplate", typeof(DataTemplate), typeof(FilePicker), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate ButtonContentTemplate
		{
			get { return (DataTemplate)GetValue(ButtonContentTemplateProperty); }
			set { SetValue(ButtonContentTemplateProperty, value); }
		}


		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			"Command", typeof(ICommand), typeof(FilePicker), new PropertyMetadata(default(ICommand)));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text", typeof(string), typeof(FilePicker), new PropertyMetadata(default(string)));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

        public FilePicker()
        {
            InitializeComponent();
        }
    }
}
