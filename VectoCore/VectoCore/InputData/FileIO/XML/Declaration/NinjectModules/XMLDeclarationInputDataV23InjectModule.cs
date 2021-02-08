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

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV23>().Named(XMLDeclarationADASDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>()
				.To<XMLADASReaderV23>().Named(XMLADASReaderV23.QUALIFIED_XSD_TYPE);


			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV23>()
														.Named(XMLDeclarationAuxiliariesDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV23>()
														.Named(XMLAuxiliaryDeclarationDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLAuxiliaryReaderV23>().Named(XMLAuxiliaryReaderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV23>().Named(
				XMLDeclarationTyreDataProviderV23.QUALIFIED_XSD_TYPE);

		}

		#endregion
	}
}