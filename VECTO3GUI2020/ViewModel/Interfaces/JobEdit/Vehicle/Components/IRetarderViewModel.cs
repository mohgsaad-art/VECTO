using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IRetarderViewModel : IRetarderInputData, IComponentViewModel
    {
		new RetarderType Type { get; set; }

		new double Ratio { get; set; }

    }
}
