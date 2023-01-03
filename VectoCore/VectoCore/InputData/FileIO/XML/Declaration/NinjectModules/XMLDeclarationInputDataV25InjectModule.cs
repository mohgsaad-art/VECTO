using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV25InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV25>().Named(
				XMLDeclarationTyreDataProviderV25.QUALIFIED_XSD_TYPE);
		}
	}
}