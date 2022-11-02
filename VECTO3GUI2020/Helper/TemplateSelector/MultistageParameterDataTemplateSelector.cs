using System;
using System.Windows;
using System.Windows.Controls;
using TUGraz.VectoCommon.Exceptions;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.Views.Multistage.CustomControls;

namespace VECTO3GUI2020.Helper.TemplateSelector
{
	public class MultistageParameterDataTemplateSelector : DataTemplateSelector
	{
		#region Overrides of DataTemplateSelector

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element = container as FrameworkElement;
			MultistageParameterViewModel vm = item as MultistageParameterViewModel;

			if (element != null && item != null && vm != null) {

				FrameworkElementFactory factory = null;
				Type type = null;
				switch (vm.Mode) {
					case (ViewMode.TEXTBOX):
						type = typeof(MultistageParameterTextView);
						break;
					case (ViewMode.CHECKBOX):
						type = typeof(MultistageParameterCheckBoxView);
						break;
					case (ViewMode.COMBOBOX):
						type = typeof(MultistageParameterComboBoxView);
						break;
					default:
						throw new VectoException("Unknown MultistageParameterType");
						break;
				}
				factory = new FrameworkElementFactory(type);
				DataTemplate dt = new DataTemplate();
				dt.VisualTree = factory;
				return dt;


			}




			return base.SelectTemplate(item, container);
		}

		#endregion
	}
}