using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationJobInputData : IDeclarationJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLDeclarationInputData InputData { get; }
	}

	public interface IXMLDeclarationTrailerJobInputData : IDeclarationTrailerJobInputData, IXMLResource
	{
		IXMLJobTrailerDataReader Reader { set; }

		IXMLDeclarationTrailerInputData InputData { get; }
	}

	public interface IXMLPrimaryVehicleBusJobInputData : IDeclarationJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLPrimaryVehicleBusInputData InputData { get; }
	}
}
