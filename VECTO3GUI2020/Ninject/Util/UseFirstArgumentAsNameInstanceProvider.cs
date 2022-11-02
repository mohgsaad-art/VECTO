using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace VECTO3GUI2020.Ninject.Util
{
    /// <summary>
    /// Ninject Instance Provider uses the first argument as name to resolve a named binding.
    /// </summary>
    public class UseFirstArgumentAsNameInstanceProvider : StandardInstanceProvider
    {
        private bool _skipFirstArgument = true;
        public UseFirstArgumentAsNameInstanceProvider(bool skipFirstArgument=true, bool fallback=false) : base()
        {
            _skipFirstArgument = skipFirstArgument;
			base.Fallback = fallback;
		}

        public UseFirstArgumentAsNameInstanceProvider() : base()
        {

        }

		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
			if (arguments.Length == 0) {
				return base.GetConstructorArguments(methodInfo, arguments).ToArray();
			}
            return base.GetConstructorArguments(methodInfo, arguments).Skip(_skipFirstArgument ? 1 : 0).ToArray();
        }

        protected override string GetName(MethodInfo methodInfo, object[] arguments)
        {
			if (arguments.Length == 0) {
				return "";
			}
            return arguments[0].ToString();
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
