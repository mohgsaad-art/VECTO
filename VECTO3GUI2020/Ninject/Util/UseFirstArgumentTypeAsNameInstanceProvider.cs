using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using TUGraz.VectoCommon.Exceptions;

namespace VECTO3GUI2020.Ninject.Util
{
    /// <summary>
    /// Ninject Instance Provider
    /// uses the type of the first argument as name to resolve a named binding
    /// </summary>
    public class UseFirstArgumentTypeAsNameInstanceProvider
        : StandardInstanceProvider
    {

		public UseFirstArgumentTypeAsNameInstanceProvider() : base()
		{
				
		}
        /// <summary>
        /// Constructor for UseFirstArgumentTypeAsNameInstanceProvider
        /// </summary>
        /// <param name="fallback">set to true if you want a binding without name as fallback</param>
		public UseFirstArgumentTypeAsNameInstanceProvider(bool fallback) : base()
		{
			base.Fallback = fallback;
		}
		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
            
            return base.GetConstructorArguments(methodInfo, arguments).ToArray();
        }

        protected override string GetName(MethodInfo methodInfo, object[] arguments)
        {
			var name = arguments[0].GetType().ToString();
            return name;
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
