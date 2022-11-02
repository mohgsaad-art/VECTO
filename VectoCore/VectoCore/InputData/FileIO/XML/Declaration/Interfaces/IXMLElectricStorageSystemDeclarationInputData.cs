using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
    public interface IXMLElectricStorageSystemDeclarationInputData : IElectricStorageSystemDeclarationInputData, IXMLResource
	{
		IXMLREESSReader StorageTypeReader { set; }
	}
}
