using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public interface IXMLDeclarationInputDataReader
	{
		IDeclarationJobInputData JobData { get; }
	}

	public interface IXMLDeclarationTrailerInputDataReader
	{
		IDeclarationTrailerJobInputData JobData { get; }
	}

	public interface IXMLDeclarationPrimaryVehicleBusInputDataReader : IXMLDeclarationInputDataReader
	{
		IResultsInputData ResultsInputData { get; }

		DigestData GetDigestData(XmlNode xmlNode);

		IApplicationInformation ApplicationInformation { get; }
	}
}
