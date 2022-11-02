using System;
using System.Windows;
using System.Windows.Media;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.Helper
{
	public static class VisualTreeHelperExtensions
	{

		public static T GetChild<T>(FrameworkElement rootElement) where T : FrameworkElement
		{
			T child = null;
			for(var i = 0; i < VisualTreeHelper.GetChildrenCount(rootElement); i++) {
				var childElement = VisualTreeHelper.GetChild(rootElement, i);
				if (childElement.GetType() == typeof(T)) {
					child = childElement as T;
					return child;
				} else {
					return GetChild<T>(childElement as FrameworkElement);
				}
			}


			return child;
		}
	}
}