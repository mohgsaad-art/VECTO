using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV221InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedVehicleDataProviderV221>()
				.Named(XMLDeclarationExemptedVehicleDataProviderV221.QUALIFIED_XSD_TYPE);

		}

		#endregion
	}
}