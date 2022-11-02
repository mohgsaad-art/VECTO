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

namespace VECTO3GUI.Views.CustomControls
{
	/// <summary>
	/// Interaction logic for CheckboxParameter.xaml
	/// </summary>
	public partial class CheckboxParameter : UserControl
	{
		public CheckboxParameter()
		{
			InitializeComponent();
			Validation.SetErrorTemplate(this, null);
			(Content as FrameworkElement).DataContext = this;
		}



		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(CheckboxParameter), new PropertyMetadata(""));




		public bool Value
		{
			get { return (bool)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(bool), typeof(CheckboxParameter), new FrameworkPropertyMetadata() {BindsTwoWayByDefault = true});



		public string Unit
		{
			get { return (string)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitProperty =
			DependencyProperty.Register("Unit", typeof(string), typeof(CheckboxParameter), new PropertyMetadata(""));




		public string CaptionWidthGroup
		{
			get { return (string)GetValue(CaptionWidthGroupProperty); }
			set { SetValue(CaptionWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CaptionWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionWidthGroupProperty =
			DependencyProperty.Register("CaptionWidthGroup", typeof(string), typeof(CheckboxParameter), new PropertyMetadata(""));




		public string UnitWidthGroup
		{
			get { return (string)GetValue(UnitWidthGroupProperty); }
			set { SetValue(UnitWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UnitWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitWidthGroupProperty =
			DependencyProperty.Register("UnitWidthGroup", typeof(string), typeof(CheckboxParameter), new PropertyMetadata(""));



	}
}
