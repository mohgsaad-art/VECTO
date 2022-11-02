using System;
using System.Windows;
using System.Windows.Controls;
using TUGraz.VectoCommon.Exceptions;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.Views.Multistage;
using VECTO3GUI2020.Views.Multistage.CustomControls;

namespace VECTO3GUI2020.Helper.TemplateSelector
{
	public class MultistageVehicleDataTemplateSelector : DataTemplateSelector
	{

		/// <summary>
		/// Selects a vehicle viewmodel based on the ExemptedVehicleProperty
		/// </summary>
		/// <param name="item"></param>
		/// <param name="container"></param>
		/// <returns></returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element = container as FrameworkElement;
			IVehicleViewModel vm = item as IVehicleViewModel;

			if (element != null && item != null && vm != null)
			{
				FrameworkElementFactory factory = null;
				Type type = null;

				switch (vm) {
					case InterimStageBusVehicleViewModel_v2_8 declvm:
						if (declvm.ExemptedVehicle) {
							type = typeof(VehicleView_v2_8_exempted);
						} else {
							type = typeof(VehicleView_v2_8);
						}

						break;
					default:
						throw new NotImplementedException($"no template defined for {vm.GetType()}");
				}

				factory = new FrameworkElementFactory(type);
				DataTemplate dt = new DataTemplate();
				dt.VisualTree = factory;
				return dt;
			}

			return base.SelectTemplate(item, container);
		}
	}
}