using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
	public class ComponentWriterNinjectModule : AbstractNinjectModule{

		private string GetName(string groupName, XNamespace xNamespace)
		{
			return UseFirstTwoArgumentsAsInstanceProvider.GetName(groupName, xNamespace.ToString());
		}

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IComponentWriterFactory>().ToFactory(() => new UseFirstTwoArgumentsAsInstanceProvider(1, false));


			//ADASTypes
			var v2102 = XMLDeclarationNamespaces.v2_10_2;
			Bind<IDeclarationAdasWriter>().To<AdasConventionalWriter>().
				Named(GetName(GroupNames.ADAS_Conventional_Type, v2102));



		}

		#endregion
	}
}