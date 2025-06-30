using TUGraz.VectoCore.InputData.Reader.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class EngineeringDataAdapterNinjectModule : AbstractNinjectModule
	{
		public override void Load()
		{
			Bind<IEngineeringDataAdapter>().To<EngineeringDataAdapter>();
		}
	}
}