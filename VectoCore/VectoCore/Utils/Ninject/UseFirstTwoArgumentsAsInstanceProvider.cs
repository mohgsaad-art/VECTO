using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.Utils.Ninject
{
	public class UseFirstTwoArgumentsAsInstanceProvider : StandardInstanceProvider
	{
		public static string GetName(string first, string second)
		{
			return first + second;
		}

		private int _skipNamingArguments;
		/// <summary>
		/// Uses the first to Arguments to resolve a named binding, the arguments are combined with the static GetName Method of this class,
		/// </summary>
		/// <param name="skipNamingArguments">Defines how many arguments are skipped and not passed to the constructor of the created object</param>
		/// <param name="fallback">Defines if a anonymous bing should be used if no binding exists for the given name</param>
		public UseFirstTwoArgumentsAsInstanceProvider(int skipNamingArguments = 0, bool fallback = false) : base()
		{
			_skipNamingArguments = skipNamingArguments;
			base.Fallback = fallback;
		}

		public UseFirstTwoArgumentsAsInstanceProvider() : base()
		{

		}

		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			if (arguments.Length < 2)
			{
				return base.GetConstructorArguments(methodInfo, arguments).ToArray();
			}
			return base.GetConstructorArguments(methodInfo, arguments).Skip(_skipNamingArguments).ToArray();
		}

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			if (arguments.Length < 2)
			{
				return "";
			}

			return GetName(arguments[0].ToString(), arguments[1].ToString());
		}

		public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
		{
			try
			{
				return base.GetInstance(instanceResolver, methodInfo, arguments);
			}
			catch (Exception e)
			{
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}'", e, methodInfo.Name, methodInfo.ReturnType.Name, arguments[0]);
				
			}
		}
    }
}