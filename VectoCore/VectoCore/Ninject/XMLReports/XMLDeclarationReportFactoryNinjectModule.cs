using TUGraz.VectoCore.OutputData.XML;
namespace TUGraz.VectoCore.Ninject.XMLReports
{
	public class XMLDeclarationReportFactoryNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationReportFactory>().To<XMLDeclarationReportFactory>().InSingletonScope();
		}

		#endregion
	}
}