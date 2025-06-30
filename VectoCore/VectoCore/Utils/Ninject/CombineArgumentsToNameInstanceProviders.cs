using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.Utils.Ninject
{
    public class CombineArgumentsToNameInstanceProvider : StandardInstanceProvider
	{
		public class MethodSettings
		{
			/// <summary>
			/// This delegate is used to create a name out of the parameters
			/// </summary>
			public CombineToName combineToNameDelegate;
			/// <summary>
			/// Specifies the number of arguments that are NOT passed to the constructor
			/// </summary>
			public int skipArguments;
			/// <summary>
			/// Specifies the number of arguments that are passed to the <see cref="combineToNameDelegate"/>
			/// </summary>
			public int takeArguments;

            //TODO: use expressions instead of method info 
			// Expression<Func<int, string>> expression = i => i.ToString();
			// MethodInfo method = ((MethodCallExpression)expression.Body).Method;

            /// <summary>
            /// Sets the methods for which these settings apply, leave empty for default settings
            /// </summary>
            public MethodInfo[] methods;
		}

		public delegate string CombineToName(params object[] arguments);

		private Dictionary<MethodInfo, MethodSettings> _methodSettings = new Dictionary<MethodInfo, MethodSettings>();

		private MethodSettings _fallbackMethodSetting;


        /// <summary>
        /// Constructor for CombineArgumentsToNameInstanceProvider
        /// </summary>

        public CombineArgumentsToNameInstanceProvider(params MethodSettings[] settings) : this(false, settings)
		{
			
		}

		public CombineArgumentsToNameInstanceProvider(bool fallback, params MethodSettings[] settings)
		{
			Fallback = fallback;
			if (settings != null) {
				var settingsWithoutMethods = settings.Where(s => s.methods == null).ToArray();
				if (settingsWithoutMethods.Length > 1) {
					throw new ArgumentException("only one fallback for method settings allowed (i.e. MethodInfo[] is null)");
				}

				if (settingsWithoutMethods.Length > 0) {
					_fallbackMethodSetting = settingsWithoutMethods.First();
				}
				
				foreach (var setting in settings)
				{
					if (setting.methods == null) {
						continue;
					}
                    foreach (var method in setting.methods)
					{
						
						_methodSettings.Add(method, setting);
					}
				}

			}
        }


		#region Overrides of StandardInstanceProvider

		public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
		{
			try
			{
				return base.GetInstance(instanceResolver, methodInfo, arguments);
			}
			catch (Exception e) {
				var name = GetName(methodInfo, arguments);
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}' name '{3}'", e, methodInfo, methodInfo.ReturnType.Name, arguments[0].ToString(), name);
				
				//throw e;
			}
		}

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			if (!GetMethodSettings(methodInfo, arguments, out var methodSettings)) {
				if (_fallbackMethodSetting != null) {
					return _fallbackMethodSetting.combineToNameDelegate.Invoke(arguments
						.Take(_fallbackMethodSetting.takeArguments).ToArray());
				}
				return base.GetName(methodInfo, arguments);
			}

			return methodSettings.combineToNameDelegate.Invoke(arguments.Take(methodSettings.takeArguments).ToArray());

		}

		bool GetMethodSettings(MethodInfo methodInfo, object[] arguments, out MethodSettings methodSettings)
		{
			if (!_methodSettings.TryGetValue(methodInfo, out methodSettings)) {
				//Fall back to name
				var methodInfos = _methodSettings.Keys.Where(method => method.Name == methodInfo.Name);
				var methodInfoByName = methodInfos.FirstOrDefault();
				if (methodInfoByName != null) {
					methodSettings = _methodSettings[methodInfoByName];
				} else {
					return false;
				}
			}

			return true;
		}

		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			
			if (!GetMethodSettings(methodInfo, arguments, out var methodSettings)) {
				if (_fallbackMethodSetting != null) {
					return base.GetConstructorArguments(methodInfo, arguments).Skip(_fallbackMethodSetting.skipArguments).ToArray();
                }

				return base.GetConstructorArguments(methodInfo, arguments);
			}

			return base.GetConstructorArguments(methodInfo, arguments).Skip(methodSettings.skipArguments).ToArray();
		}

		#endregion
	}
}
