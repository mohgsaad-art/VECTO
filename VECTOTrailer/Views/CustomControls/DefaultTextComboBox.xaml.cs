using System;
using System.Collections;
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

namespace VECTOTrailer.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for DefaultTextComboBox.xaml
    /// </summary>
    public partial class DefaultTextComboBox : UserControl
    {
		/// <summary>
		/// Currently ignored always set to true
		/// </summary>
		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(DefaultTextComboBox), new PropertyMetadata(default(bool)));
		/// <summary>
		/// Currently ignored always set to true
		/// </summary>
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DefaultTextComboBox), new PropertyMetadata(default(bool)));
		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DefaultTextComboBox), new PropertyMetadata(default(IEnumerable)));
		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(DefaultTextComboBox), new PropertyMetadata(default(object)));
		public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(DefaultTextComboBox), new PropertyMetadata(default(string)));
		public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register("DefaultText", typeof(string), typeof(DefaultTextComboBox), new PropertyMetadata(default(string)));
		public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(DefaultTextComboBox), new PropertyMetadata(default(string)));
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(DefaultTextComboBox), new PropertyMetadata(default(object)));
		public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(DefaultTextComboBox), new PropertyMetadata(default(int)));

		public DefaultTextComboBox()
        {
            InitializeComponent();
            //ComboBox.SelectionChanged += ComboBox_SelectionChanged;
			ComboBox.SelectedIndex = -1;
		}

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public bool IsEditable
		{
			get => (bool)GetValue(IsEditableProperty);
			set => SetValue(IsEditableProperty, value);
		}

		public bool IsReadOnly
		{
			get => (bool)GetValue(IsReadOnlyProperty);
			set => SetValue(IsReadOnlyProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public object SelectedValue
		{
			get => (object)GetValue(SelectedValueProperty);
			set => SetValue(SelectedValueProperty, value);
		}

		public string SelectedValuePath
		{
			get => (string)GetValue(SelectedValuePathProperty);
			set => SetValue(SelectedValuePathProperty, value);
		}

		public string DefaultText
		{
			get => (string)GetValue(DefaultTextProperty);
			set => SetValue(DefaultTextProperty, value);
		}

		public string DisplayMemberPath
		{
			get => (string)GetValue(DisplayMemberPathProperty);
			set => SetValue(DisplayMemberPathProperty, value);
		}

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
	}
}
