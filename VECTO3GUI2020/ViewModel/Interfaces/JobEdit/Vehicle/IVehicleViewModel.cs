using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle
{
    public interface IVehicleViewModel : IComponentViewModel, IVehicleDeclarationInputData
    {
		IPTOViewModel PTOViewModel { get; }


		//TODO: Move to IVehicleDeclarationInputData
		RetarderType RetarderType { get; }
		double RetarderRatio { get; }
		AngledriveType AngledriveType { get; }
		IPTOTransmissionInputData PTOTransmissionInputData { get; }


		ObservableCollection<IComponentViewModel> ComponentViewModels { get; set; }
		
	}
}
