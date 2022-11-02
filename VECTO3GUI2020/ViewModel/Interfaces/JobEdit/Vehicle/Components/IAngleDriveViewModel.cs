using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IAngleDriveViewModel : IAngledriveInputData, IComponentViewModel
    {
		new AngledriveType Type { get; set; }
    }
}
