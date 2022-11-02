using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace VECTO3GUI2020.Model.Interfaces
{
    
    public interface IAuxiliaryModelFactory
    {
        /// <summary>
        /// Creates a new AuxiliaryModel
        /// </summary>
        /// <param name="auxiliaryType">Is used to determine wich AuxiliaryModel is created</param>
        IDeclarationAuxiliaryTable CreateAuxiliaryModel(AuxiliaryType auxiliaryType);
    }
}
