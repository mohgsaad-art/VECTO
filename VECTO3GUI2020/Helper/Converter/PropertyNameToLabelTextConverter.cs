using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Data;
using VECTO3GUI2020.Properties;

namespace VECTO3GUI2020.Helper.Converter
{
	public class PropertyNameToLabelTextConverter : IValueConverter
	{
		#region Implementation of IValueConverter
		/// <summary>
		/// Maps the value to a string found in the given ResourceFiles (first match).
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <example>
		/// 
		/// <Binding Path="." Converter="{StaticResource PropertyNameToLabelConverter}">
		///		<Binding.ConverterParameter>
		///			<x:Array Type="resources:ResourceManager">
		///				<x:Static MemberType="properties:Strings" Member="ResourceManager"/>
		///			</x:Array>
		///		</Binding.ConverterParameter>
		///	</Binding>
		///
		/// </example>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter is IEnumerable<ResourceManager> resourceManagers) {
				return NameResolver.ResolveName(value as string, resourceManagers.ToArray());
			} else {
				return NameResolver.ResolveName(value as string, BusStrings.ResourceManager, Strings.ResourceManager) ?? Binding.DoNothing;
			}

			
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}