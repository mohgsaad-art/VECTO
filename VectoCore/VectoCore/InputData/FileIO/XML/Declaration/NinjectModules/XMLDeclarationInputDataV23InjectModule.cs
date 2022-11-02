using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV23InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV23>()
												.Named(XMLDeclarationEngineDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV23>().Named(
				XMLDeclarationTyreDataProviderV23.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}