using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
    public interface IXMLFuelCellDeclarationInputData : IFuelCellDeclarationInputData
    {
    }

    public interface IXMLFuelCellSystemDeclarationInputData : IFuelCellSystemDeclarationInputData, IXMLResource
    {
    }
}