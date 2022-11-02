using System;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using VECTO3GUI2020.Properties;

namespace VECTO3GUI2020.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {


        ResourceManager _resourceManager;


        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DateTimePicker), new PropertyMetadata(""));




        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetCurrentValue(DateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(ContentChanged)));


        public DateTimePicker()
        {
            InitializeComponent();
            _resourceManager = Strings.ResourceManager;
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue == e.OldValue))
            {
                return;
            }
            DateTimePicker dateTimePicker = (DateTimePicker)d;
            dateTimePicker.UpdateLabel(e);
        }


        private void UpdateLabel(DependencyPropertyChangedEventArgs e)
        {

            var Binding = this.GetBindingExpression(DateProperty);
            var PropertyName = Binding?.ResolvedSourcePropertyName;
            if (PropertyName == null || Binding == null)
            {
                //Debug.WriteLine("Binding or Property name == null");
                return;
            }
            //Debug.WriteLine("PropertyName: " + PropertyName);

            Label = _resourceManager?.GetString(PropertyName) ?? PropertyName;
        }
    }
}
