using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.MultiStage.Interfaces
{

	public interface IMultiStageViewModelFactory : 
		IMultiStageViewModelFactoryDefaultInstanceProvider, 
		IMultiStageViewModelFactoryTypeAsNameInstanceProvider, 
		IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
	{
		
	}


	public interface IMultiStageViewModelFactoryTypeAsNameInstanceProvider
	{
		IDocumentViewModel CreateDocumentViewModel(IDeclarationInputDataProvider inputData);
		IDocumentViewModel CreateDocumentViewModel(IInputDataProvider inputData);
		IVehicleViewModel CreateStageInputVehicleViewModel(IVehicleDeclarationInputData inputData);
	}

	public interface IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
	{
		IVehicleViewModel CreateStageInputVehicleViewModel(string inputProviderType);
	}



    public interface IMultiStageViewModelFactoryDefaultInstanceProvider
	{
		IDocumentViewModel GetStageInputViewModel(bool exemptedVehicle);
		IViewModelBase GetNewMultistageJobViewModel();

		IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistageBusInputDataProvider inputData);

		IVehicleViewModel GetInterimStageVehicleViewModel();

		IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, bool exempted);

		IManufacturingStageViewModel GetManufacturingStageViewModel(IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted);
		IMultistageAirdragViewModel GetMultistageAirdragViewModel();

		IMultistageAirdragViewModel GetMultistageAirdragViewModel(
			IAirdragDeclarationInputData consolidatedAirdragInputData);

		IMultistageAuxiliariesViewModel GetAuxiliariesViewModel();
		IMultistageAuxiliariesViewModel GetAuxiliariesViewModel(
			IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData);

		ICreateVifViewModel GetCreateNewVifViewModel(bool completed);
		ICreateVifViewModel GetCreateNewVifViewModel();

		//IViewModelBase CreateNewMultiStageJobViewModel();

		//IMultiStageJobViewModel CreateMultiStageJobViewModel(string inputProviderType, IMultistageBusInputDataProvider inputData);

		//IVehicleViewModel CreateInterimStageVehicleViewModel(string inputProviderType);

		//IVehicleViewModel CreateInterimStageVehicleViewModel(string inputProviderType, IVehicleDeclarationInputData prevStageInputData);

		//IManufacturingStageViewModel CreateManufacturingStageViewModel(string inputProviderType, IManufacturingStageInputData consolidatedManufacturingStageInputData);

		//IMultistageAirdragViewModel CreateMultistageAirdragViewModel();
	}
}
