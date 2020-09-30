using System;
using System.Linq;
using System.Reflection;

using SIUtils = VECTOTrailer.Util.SIUtils;

namespace VECTOTrailer.Helper {
	public class SIUnit : DataContextBaseExtension
	{

		protected PropertyInfo targetProperty;

		protected readonly string Param;

		public SIUnit(string param)
		{
			Param = param;
		}

		
		#region Overrides of DataContextBaseExtension

		protected override object OnProvideValue(IServiceProvider serviceProvider)
		{
			if (TargetProperty != null) {
				targetProperty = TargetProperty as PropertyInfo;

			}
			return "Unit";
		}

		protected override void OnDataContextFound()
		{
			var properties = DataContext?.GetType().GetProperties().Where(x => x.Name == Param).ToArray();
			if (properties?.Length != 1) {
				return;
			}

			var targetType = properties.First().GetMethod.ReturnType;
			var val = SIUtils.CreateSIValue(targetType, 0);
			UpdateProperty(val.UnitString);
		}
		
		private void UpdateProperty(object value)
		{
			if (TargetObjectDependencyObject != null) {
				if (TargetObjectDependencyProperty != null) {
					Action update = () => TargetObjectDependencyObject
						.SetValue(TargetObjectDependencyProperty, value);

					if (TargetObjectDependencyObject.CheckAccess()) {
						update();
					} else {
						TargetObjectDependencyObject.Dispatcher.Invoke(update);
					}
				} else {
					if (targetProperty != null) {
						targetProperty.SetValue(TargetObject, value, null);
					}
				}
			}
		}


		#endregion
	}
}