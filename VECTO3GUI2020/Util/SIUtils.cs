using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TUGraz.VectoCommon.Utils;


//Copied from VECTO3GUI franz kober
namespace VECTO3GUI2020.Util
{
	public static class SIUtils
	{
		static readonly Dictionary<Type, Func<double, object>> Constructors = new Dictionary<Type, Func<double, object>>();

		public static SI CreateSIValue(Type targetType, double value)
		{
			if (!typeof(SI).IsAssignableFrom(targetType)) {
				return null;
			}

			if (!Constructors.ContainsKey(targetType)) {
				const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
				var constructorInfo = targetType.GetConstructor(bindingFlags, null, new[] { typeof(double) }, null);
				var param = Expression.Parameter(typeof(double));
				var expr = Expression.Lambda<Func<double, object>>(Expression.New(constructorInfo, param), param);
				var constr = expr.Compile();
				Constructors[targetType] = constr;
			}

			return (SI)Constructors[targetType](value);
		}
	}
}
