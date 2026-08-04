using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.Impl;

namespace TUGraz.VectoCore.Ninject
{
	public class EngineeringDataAdapterNinjectModule : AbstractNinjectModule
	{
		public override void Load()
		{
			Bind<IEngineeringDataAdapter>().To<EngineeringDataAdapter>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.Conventional>()
				.WhenInjectedExactlyInto<EngineeringVTPModeVectoRunDataFactoryLorries>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Conventional>()
				.WhenInjectedExactlyInto<EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary>();
        }
	}
}