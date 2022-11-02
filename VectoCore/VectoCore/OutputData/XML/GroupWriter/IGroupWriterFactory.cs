using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter
{
	public interface IGroupWriterFactory
	{

		IVehicleDeclarationGroupWriter GetVehicleDeclarationGroupWriter(string groupName, XNamespace writerNamespace);
		IBusAuxiliariesDeclarationGroupWriter GetBusAuxiliariesDeclarationGroupWriter(string groupName, XNamespace writerNamespace);


	}
}