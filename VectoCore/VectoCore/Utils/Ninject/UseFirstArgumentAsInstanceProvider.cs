/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.Utils.Ninject
{
	public class UseFirstArgumentAsInstanceProvider : StandardInstanceProvider
	{
		#region Overrides of StandardInstanceProvider

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			return arguments[0].ToString();
		}


		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
		}

		#region Overrides of StandardInstanceProvider

		public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
		{
			try {
				return base.GetInstance(instanceResolver, methodInfo, arguments);
			} catch (Exception e) {
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}'", e, methodInfo.Name, methodInfo.ReturnType.Name, arguments[0]);
				//throw e;
			}
		}

		#endregion

		#endregion
	}
}
