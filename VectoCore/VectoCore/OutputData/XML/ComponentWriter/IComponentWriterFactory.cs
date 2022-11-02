using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
	public interface IComponentWriterFactory
	{
		IDeclarationAdasWriter getDeclarationAdasWriter(string componentName, XNamespace writerNamespace);
	}
}