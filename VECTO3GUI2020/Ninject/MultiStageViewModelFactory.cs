using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
	public class MultiStageViewModelFactory : IMultiStageViewModelFactory	
	{
		private IMultiStageViewModelFactoryDefaultInstanceProvider _multiStageVmFactoryDefaultInstanceProvider;
		private IMultiStageViewModelFactoryTypeAsNameInstanceProvider _multiStageViewModelFactoryTypeAsNameInstanceProvider;

		private IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
			_multistageViewModelFactoryFirstParameterAsNameInstanceProvider;


		public MultiStageViewModelFactory(IMultiStageViewModelFactoryDefaultInstanceProvider multiStageVmFactoryDefaultInstanceProvider, 
			IMultiStageViewModelFactoryTypeAsNameInstanceProvider multiStageViewModelFactoryTypeAsNameInstanceProvider, IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider multistageViewModelFactoryFirstParameterAsNameInstanceProvider)
		{
			_multiStageVmFactoryDefaultInstanceProvider = multiStageVmFactoryDefaultInstanceProvider;
			_multiStageViewModelFactoryTypeAsNameInstanceProvider = multiStageViewModelFactoryTypeAsNameInstanceProvider;
			_multistageViewModelFactoryFirstParameterAsNameInstanceProvider = multistageViewModelFactoryFirstParameterAsNameInstanceProvider;
		}

		#region Implementation of IMultiStageViewModelFactoryDefaultInstanceProvider

		public IDocumentViewModel GetStageInputViewModel(bool exemptedVehicle)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetStageInputViewModel(exemptedVehicle);
		}

		public IViewModelBase GetNewMultistageJobViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetNewMultistageJobViewModel();
		}

		public IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistageBusInputDataProvider inputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultiStageJobViewModel(inputData);
		}

		public IVehicleViewModel GetInterimStageVehicleViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetInterimStageVehicleViewModel();
		}

		public IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, bool exempted)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetInterimStageVehicleViewModel(consolidatedVehicleData, exempted);
		}

		public IManufacturingStageViewModel GetManufacturingStageViewModel(
			IManufacturingStageInputData consolidatedManufacturingStageInputData, bool exempted)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetManufacturingStageViewModel(consolidatedManufacturingStageInputData, exempted);
		}

		public IMultistageAirdragViewModel GetMultistageAirdragViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultistageAirdragViewModel();
		}

		public IMultistageAirdragViewModel GetMultistageAirdragViewModel(IAirdragDeclarationInputData consolidatedAirdragInputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetMultistageAirdragViewModel(consolidatedAirdragInputData);
		}

		public IMultistageAuxiliariesViewModel GetAuxiliariesViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetAuxiliariesViewModel();
		}

		public IMultistageAuxiliariesViewModel
			GetAuxiliariesViewModel(IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetAuxiliariesViewModel(consolidatedAuxiliariesInputData);
		}

		public ICreateVifViewModel GetCreateNewVifViewModel(bool completed)
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetCreateNewVifViewModel(completed);
		}

		public ICreateVifViewModel GetCreateNewVifViewModel()
		{
			return _multiStageVmFactoryDefaultInstanceProvider.GetCreateNewVifViewModel();
		}

		#endregion

		#region Implementation of IMultiStageViewModelFactoryTypeAsNameInstanceProvider

		public IDocumentViewModel CreateDocumentViewModel(IDeclarationInputDataProvider inputData)
		{
			return _multiStageViewModelFactoryTypeAsNameInstanceProvider.CreateDocumentViewModel(inputData);
		}

		public IDocumentViewModel CreateDocumentViewModel(IInputDataProvider inputData)
		{
			return _multiStageViewModelFactoryTypeAsNameInstanceProvider.CreateDocumentViewModel(inputData);
		}

		public IVehicleViewModel CreateStageInputVehicleViewModel(IVehicleDeclarationInputData inputData)
		{
			return _multiStageViewModelFactoryTypeAsNameInstanceProvider.CreateStageInputVehicleViewModel(inputData);
		}

		#endregion


		#region Implementation of IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider

		public IVehicleViewModel CreateStageInputVehicleViewModel(string inputProviderType)
		{
			return _multistageViewModelFactoryFirstParameterAsNameInstanceProvider.CreateStageInputVehicleViewModel(inputProviderType);
		}

		#endregion
	}
}