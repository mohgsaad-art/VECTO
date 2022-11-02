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
    /// Interaction logic for MultistageParameterComboBoxView.xaml
    /// </summary>
    public partial class MultistageParameterComboBoxView : UserControl
    {
		public static readonly DependencyProperty SharedSizeGroupLabelColumnProperty = DependencyProperty.Register(
			"SharedSizeGroupLabelColumn", typeof(string), typeof(MultistageParameterComboBoxView), new PropertyMetadata(default(string)));

		//public string SharedSizeGroupLabelColumn
		//{
		//	get { return (string)GetValue(SharedSizeGroupLabelColumnProperty); }
		//	set { SetValue(SharedSizeGroupLabelColumnProperty, value); }
		//}

		//public static readonly DependencyProperty SharedSizeGroupCheckboxColumnProperty = DependencyProperty.Register(
		//	"SharedSizeGroupCheckboxColumn", typeof(string), typeof(MultistageParameterComboBoxView), new PropertyMetadata(default(string)));

		//public string SharedSizeGroupCheckboxColumn
		//{
		//	get { return (string)GetValue(SharedSizeGroupCheckboxColumnProperty); }
		//	set { SetValue(SharedSizeGroupCheckboxColumnProperty, value); }
		//}

		//public static readonly DependencyProperty SharedSizeGroupConsolidatedColumnProperty = DependencyProperty.Register(
		//	"SharedSizeGroupConsolidatedColumn", typeof(string), typeof(MultistageParameterComboBoxView), new PropertyMetadata(default(string)));

		//public string SharedSizeGroupConsolidatedColumn
		//{
		//	get { return (string)GetValue(SharedSizeGroupConsolidatedColumnProperty); }
		//	set { SetValue(SharedSizeGroupConsolidatedColumnProperty, value); }
		//}

		//public static readonly DependencyProperty SharedSizeGroupContentColumnProperty = DependencyProperty.Register(
		//	"SharedSizeGroupContentColumn", typeof(string), typeof(MultistageParameterComboBoxView), new PropertyMetadata(default(string)));

		//public string SharedSizeGroupContentColumn
		//{
		//	get { return (string)GetValue(SharedSizeGroupContentColumnProperty); }
		//	set { SetValue(SharedSizeGroupContentColumnProperty, value); }
		//}

		//public static readonly DependencyProperty SharedSizeGroupUnitColumnProperty = DependencyProperty.Register(
		//	"SharedSizeGroupUnitColumn", typeof(string), typeof(MultistageParameterComboBoxView), new PropertyMetadata(default(string)));

		//public string SharedSizeGroupUnitColumn
		//{
		//	get { return (string)GetValue(SharedSizeGroupUnitColumnProperty); }
		//	set { SetValue(SharedSizeGroupUnitColumnProperty, value); }
		//}


		public MultistageParameterComboBoxView()
        {
            InitializeComponent();
        }

		private void CheckBoxCombo_OnChecked(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
