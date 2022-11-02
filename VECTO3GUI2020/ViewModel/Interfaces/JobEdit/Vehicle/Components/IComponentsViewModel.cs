using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IComponentsViewModel : IVehicleComponentsDeclaration
    {
        ObservableCollection<IComponentViewModel> Components { get; set; }

        IRetarderViewModel RetarderViewModel { get; }
        IEngineViewModel EngineViewModel { get; }

        IPTOViewModel PTOViewModel { get; }

        IAirDragViewModel AirDragViewModel { get; }

        IAngleDriveViewModel AngleDriveViewModel { get; }
    }
}
