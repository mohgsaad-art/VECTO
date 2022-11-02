using System.Resources;
using System.Windows;
using System.Windows.Controls;
using VECTO3GUI2020.Properties;

namespace VECTO3GUI2020.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for LabledTextBoxAutomatic.xaml
    /// </summary>
    public partial class LabledTextBoxAutomatic : UserControl
    {

        private readonly ResourceManager _resourceManager;
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label",
                                        typeof(string),
                                        typeof(LabledTextBoxAutomatic),
                                        new PropertyMetadata(""));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                                        typeof(string),
                                        typeof(LabledTextBoxAutomatic),
                                        new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public string UnitText
        {
            get { return (string)GetValue(UnitTextProperty); }
            set { SetValue(UnitTextProperty, value); }
        }



        // Using a DependencyProperty as the backing store for UnitText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.Register("UnitText",
                                        typeof(string),
                                        typeof(LabledTextBoxAutomatic),
                                        new PropertyMetadata(""));

        public string UserUnit
        {
            get { return (string)GetValue(UserUnitProperty); }
            set { SetValue(UserUnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserUnitProperty =
            DependencyProperty.Register("UserUnit",
                                        typeof(string),
                                        typeof(LabledTextBoxAutomatic),
                                        new PropertyMetadata(""));


        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set
            {
                SetCurrentValue(ContentProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyObject.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                                        typeof(object),
                                        typeof(LabledTextBoxAutomatic),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ContentChanged)));

        public LabledTextBoxAutomatic()
        {
            InitializeComponent();
			
            _resourceManager = Strings.ResourceManager;
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            LabledTextBoxAutomatic labledTextBox = (LabledTextBoxAutomatic)d;
            labledTextBox.UpdateContent(e);
        }

        private void UpdateContent(DependencyPropertyChangedEventArgs e)
        {
            var Binding = this.GetBindingExpression(ContentProperty);
            var PropertyName = Binding?.ResolvedSourcePropertyName;
            if(PropertyName == null || Binding == null)
            {
                return;
            }

            var ExtendedPropertyName = Binding?.ResolvedSource.GetType().Name + "_" + PropertyName;

            Label = _resourceManager?.GetString(ExtendedPropertyName) ??_resourceManager?.GetString(PropertyName) ?? (PropertyName + "_"); //_Postfix to label Property Names that are not in strings.resx

            var data = Binding.ResolvedSource.GetType().GetProperty(PropertyName).GetValue(Binding.DataItem);
            if (data == null) return;

            if (UserUnit != "")
            {
                //UserUnit Visibility
                this.AutoUnitLabel.Visibility = Visibility.Hidden;
                this.UserUnitLabel.Visibility = Visibility.Visible;
            }

            /*
            if (data is SI SIUnit)
            {
                UnitText = SIUnit.UnitString;
                Text = SIUnit.ToGUIFormat();
            }
            else if (data is string || data is double){
                Text = Content?.ToString();
            }else
            {
                Debug.Assert(false, "Unsopported Property Type" + data.GetType().ToString());
            }
            */

        }


    }
}
