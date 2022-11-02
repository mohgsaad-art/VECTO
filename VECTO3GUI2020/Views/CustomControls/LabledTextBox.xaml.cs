using System.Windows;
using System.Windows.Controls;

namespace VECTO3GUI2020.Views.CustomControls
{
    /// <summary>
    /// Interaktionslogik für LabledTextBox.xaml
    /// </summary>
    public partial class LabledTextBox : UserControl
    {

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabledTextBox), new PropertyMetadata("Label"));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabledTextBox), new PropertyMetadata("Text"));




        public LabledTextBox()
        {
            InitializeComponent();
        }
    }
}
