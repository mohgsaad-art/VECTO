using System.IO;
using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public interface IXMLInputDataReader
	{
		IInputDataProvider Create(string filename);

		IInputDataProvider Create(Stream inputData);

		IInputDataProvider Create(XmlReader inputData);

		IEngineeringInputDataProvider CreateEngineering(string filename);

		IEngineeringInputDataProvider CreateEngineering(Stream inputData);

		IEngineeringInputDataProvider CreateEngineering(XmlReader inputData);

		IDeclarationInputDataProvider CreateDeclaration(string filename);

		IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData);

		IDeclarationTrailerInputDataProvider CreateDeclarationTrailer(string filename);

		IDeclarationTrailerInputDataProvider CreateDeclarationTrailer(XmlReader inputData);
	}
}
