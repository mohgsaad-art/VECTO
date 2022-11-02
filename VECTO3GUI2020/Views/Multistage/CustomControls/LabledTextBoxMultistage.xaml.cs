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
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Views.CustomControls;

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{




    /// <summary>
    /// Interaction logic for LabledTextBoxMultistage.xaml
    /// </summary>
    public partial class LabledTextBoxMultistage : UserControl
    {
		#region Dependency Properties

		public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register(
			"ReadOnly", typeof(bool), typeof(LabledTextBoxMultistage), new PropertyMetadata(default(bool)));

		public bool ReadOnly
		{
			get { return (bool)GetValue(ReadOnlyProperty); }
			set { SetValue(ReadOnlyProperty, value); }
		}


		public static readonly DependencyProperty DummyContentProperty = DependencyProperty.Register(
			"DummyContent", typeof(object), typeof(LabledTextBoxMultistage), new PropertyMetadata(default(object)));

		public object DummyContent
		{
			get { return (object)GetValue(DummyContentProperty); }
			set { SetValue(DummyContentProperty, value); }
		}

		public new static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
				typeof(object),
				typeof(LabledTextBoxMultistage),
				new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ContentChanged))); //TODO Default value null breaks it for SIs default value "" for strings

		public new object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set
			{
				SetCurrentValue(ContentProperty, value);
			}
		}


		public static readonly DependencyProperty GeneratedLabelTextProperty = DependencyProperty.Register(
			"GeneratedLabelText", typeof(string), typeof(LabledTextBoxMultistage), new PropertyMetadata(null));

		public string GeneratedLabelText
		{
			get { return (string)GetValue(GeneratedLabelTextProperty); }
			set { SetValue(GeneratedLabelTextProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
			"Mode", typeof(MultistageParameterViewMode), typeof(LabledTextBoxMultistage), new PropertyMetadata(default(MultistageParameterViewMode)));

		public MultistageParameterViewMode Mode
		{
			get { return (MultistageParameterViewMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		#endregion

		private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var labledTextBoxMultistage = (CustomControls.LabledTextBoxMultistage)d;


			labledTextBoxMultistage.DummyContent = labledTextBoxMultistage.CreateDummyContent(e, 
				createEnum:labledTextBoxMultistage.Mode == MultistageParameterViewMode.COMBOBOX);

			if (labledTextBoxMultistage.GeneratedLabelText != null)
			{
				return;
			}
			labledTextBoxMultistage.GeneratedLabelText = labledTextBoxMultistage.GetLabelByPropertyName(
				LabledTextBoxMultistage.ContentProperty,
				Strings.ResourceManager);
		}

		public LabledTextBoxMultistage()
        {
            InitializeComponent();
        }
    }
}
