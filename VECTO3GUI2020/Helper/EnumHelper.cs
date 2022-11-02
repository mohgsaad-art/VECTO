using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Models;
using VECTO3GUI2020.Annotations;

namespace VECTO3GUI2020.Helper
{
	using System.Runtime.CompilerServices;

	

	internal static class EnumHelper
	{
		private static ObservableCollection<T> GetValuesAsObservableCollection<T, TInput>(bool exclude,
			params TInput[] items)
			where TInput : System.Enum
			where T : System.Enum

		{
			var values = Enum.GetValues(typeof(TInput)).Cast<TInput>().ToList().Where(e => {
				var contains = items.Contains(e);
				var result = contains;
				if (exclude) {
					result = !contains;
				}

				return result;
			});
			return new ObservableCollection<T>(values.Cast<T>());
		}

		public static ObservableCollection<T> GetValuesAsObservableCollectionIncluding<T, TInput>(params TInput[] items)
			where TInput : System.Enum
			where T : System.Enum

		{
			return GetValuesAsObservableCollection<T, TInput>(false, items);
		}

		public static ObservableCollection<T> GetValuesAsObservableCollectionExcluding<T, TInput>(params TInput[] items)
			where TInput : System.Enum
			where T : System.Enum

		{
			return GetValuesAsObservableCollection<T, TInput>(true, items);
		}

		public static ObservableCollection<T> GetValuesAsObservableCollection<T, TInput>()
			where TInput : System.Enum
			where T : System.Enum

		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(TInput)).Cast<TInput>().ToList().Cast<T>());
		}



	}
}