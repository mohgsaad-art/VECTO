using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationReaderInjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IDeclarationInjectFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Kernel?.Load(new INinjectModule[] {
				new XMLDeclarationInputDataV10InjectModule(),
				new XMLDeclarationInputDataV20InjectModule(),
				new XMLDeclarationInputDataV21InjectModule(),
				new XMLDeclarationInputDataV22InjectModule(),
				new XMLDeclarationInputDataV23InjectModule(),
				new XMLDeclarationInputDataV26InjectModule(),
				new XMLDeclarationInputDataV27InjectModule(),
				new XMLDeclarationPrimaryVehicleBusV01InjectModule(), 
			});

			#endregion
		}
	}
}
