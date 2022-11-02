using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace TUGraz.VectoCore.InputData.Reader
{
    class VectoRunDataFactoryNinjectModule : NinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IVectoRunDataFactoryFactory>().To<VectoRunDataFactoryFactory>().InSingletonScope();
		}

		#endregion
	}
}
