using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IEngineViewModel : IEngineDeclarationInputData, IComponentViewModel
    {
        PerSecond IdlingSpeed { get; set; }
    }
}
