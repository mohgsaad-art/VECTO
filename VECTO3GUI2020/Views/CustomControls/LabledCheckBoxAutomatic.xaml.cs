using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using VECTO3GUI2020.Properties;


namespace VECTO3GUI2020.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for LabledCheckBoxAutomatic.xaml
    /// </summary>
    public partial class LabledCheckBoxAutomatic : UserControl
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
                                        typeof(LabledCheckBoxAutomatic),
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
                                        typeof(LabledCheckBoxAutomatic),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ContentChanged)));


        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            LabledCheckBoxAutomatic labledCheckBox = (LabledCheckBoxAutomatic)d;
            labledCheckBox.UpdateContent(e);
        }

        private void UpdateContent(DependencyPropertyChangedEventArgs e)
        {
            var Binding = this.GetBindingExpression(ContentProperty);
            var PropertyName = Binding?.ResolvedSourcePropertyName;
            if (PropertyName == null || Binding == null)
            {
                return;
            }

            var ExtendedPropertyName = Binding?.ResolvedSource.GetType().Name + "_" + PropertyName;
            Label = _resourceManager?.GetString(ExtendedPropertyName) ?? _resourceManager?.GetString(PropertyName) ?? (PropertyName + "_"); //_Postfix to label Property Names that are not in strings.resx
            
        }

        public LabledCheckBoxAutomatic()
        {
            InitializeComponent();
            _resourceManager = Strings.ResourceManager;
        }
    }
}
