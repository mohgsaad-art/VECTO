using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Annotations;

namespace VECTO3GUI2020.Views.CustomControls
{
    public static class CustomControlExtensionMethods
    {

		private static string unresolved = "unresolved";
		private static string _suffix = "_"; //used to mark properties;

		/// <summary>
		/// Looks up the Label by the name the Property that is used for the binding
		/// </summary>
		/// <param name="dependencyProperty"></param>
		/// <param name="resourceManagers">The resourceManagers where the name of the property will be looked up if</param>
		/// <returns></returns>
		public static string GetLabelByPropertyName(this UserControl userControl, DependencyProperty dependencyProperty, params ResourceManager[] resourceManagers)
		{
			
			string name = null;
			var binding = userControl.GetBindingExpression(dependencyProperty);
			var propertyName = binding?.ResolvedSourcePropertyName;
			

			if (propertyName == null || binding == null) {
				var status = binding?.Status;
				throw new VectoException("Could not resolve binding");
				return name;
			}

			foreach (var resourceManager in resourceManagers) {
				var resolvedName = resourceManager?.GetString(propertyName);
				if (resolvedName != null) {
					name = resolvedName;
					return name;
				}
			}


			name = propertyName + _suffix;

			//var extendedPropertyName = binding?.ResolvedSource.GetType().Name + "_" + propertyName;
			//name = resourceManager?.GetString(extendedPropertyName) ?? resourceManager?.GetString(propertyName) ?? (propertyName + "_"); //_Postfix to label Property Names that are not in strings.resx

			return name;
		}

		public static string GetLabelByPropertyName(this UserControl userControl, DependencyProperty dependencyProperty,
			[NotNull] ResourceManager resourceManager)
		{
			return GetLabelByPropertyName(userControl, dependencyProperty, resourceManagers:resourceManager);
		}

		public static Type GetPropertyType(this UserControl userControl, DependencyProperty dependencyProperty)
		{
			var Binding = userControl.GetBindingExpression(dependencyProperty);
			var PropertyName = Binding?.ResolvedSourcePropertyName;

			if (PropertyName == null || Binding == null) {
				return typeof(object);
			}
			var PropertyType = Binding?.ResolvedSource?.GetType().GetProperty(PropertyName).PropertyType;
			return PropertyType;
		}

		public static object CreateDummyContent(this UserControl userControl, DependencyPropertyChangedEventArgs e, bool createEnum = false)
		{
			var type = userControl.GetPropertyType(e.Property);
			if (type == null)
			{
				return null;
			}
			try
			{
				dynamic dynType = type;
				var baseType = dynType.BaseType;
				//Create SI Dummy

				if (baseType?.BaseType != null && baseType.BaseType == typeof(SI))
				{
					var createMethod = baseType.GetMethod("Create");
					var dummyContent = createMethod?.Invoke(null, new object[] { (new double()) });
					return dummyContent;
				}
				else if(createEnum)
				{
					var bindingProperty = userControl.GetBindingExpression(e.Property);
					var dataItemType = bindingProperty?.DataItem.GetType();
					var sourcePropertyType =
						dataItemType?.GetProperty(bindingProperty?.ResolvedSourcePropertyName)?.PropertyType;

					var underlyingType = Nullable.GetUnderlyingType(dynType);
					Enum dummyEnum;
					if (underlyingType != null)
					{
						dummyEnum = Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
					}
					else
					{
						dummyEnum = Enum.Parse(dynType, dynType.GetEnumNames()[0]);
					}

					return dummyEnum;
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}

			return null;
		}

	}
}
