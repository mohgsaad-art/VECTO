using System;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject.Factories
{
    public class MultiStageViewModelFactory : IMultiStageViewModelFactory
    {
        private IMultiStageViewModelFactoryDefaultInstanceProvider _multiStageVmFactoryDefaultInstanceProvider;

        private IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider
            _multistageViewModelFactoryFirstParameterAsNameInstanceProvider;

        private readonly IDocumentViewModelFactory _documentViewModelFactory;
		private readonly IVehicleViewModelFactory _vehicleViewModelFactory;
        private readonly IMultistepComponentViewModelFactory _multistepComponentViewModelFactory;

        public MultiStageViewModelFactory(
            IMultiStageViewModelFactoryDefaultInstanceProvider multiStageVmFactoryDefaultInstanceProvider,
            IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider multistageViewModelFactoryFirstParameterAsNameInstanceProvider,
            IDocumentViewModelFactory documentViewModelFactory,
            IVehicleViewModelFactory vehicleViewModelFactory, IMultistepComponentViewModelFactory multistepComponentViewModelFactory)
        {
            _multiStageVmFactoryDefaultInstanceProvider = multiStageVmFactoryDefaultInstanceProvider;
            _multistageViewModelFactoryFirstParameterAsNameInstanceProvider = multistageViewModelFactoryFirstParameterAsNameInstanceProvider;

			_multistepComponentViewModelFactory = multistepComponentViewModelFactory;
            _documentViewModelFactory = documentViewModelFactory;
            _vehicleViewModelFactory = vehicleViewModelFactory;
        }

        #region Implementation of IMultiStageViewModelFactoryDefaultInstanceProvider

        public IViewModelBase GetNewMultistageJobViewModel()
        {
            return _multiStageVmFactoryDefaultInstanceProvider.GetNewMultistageJobViewModel();
        }

        public IMultiStageJobViewModel GetMultiStageJobViewModel(IMultistepBusInputDataProvider inputData)
        {
            return _documentViewModelFactory.CreateDocumentViewModel(inputData) as IMultiStageJobViewModel;
        }

        public IMultistageVehicleViewModel GetInterimStageVehicleViewModel(CompletedBusArchitecture arch)
        {

			if (_vehicleViewModelFactory.CreateNewVehicleViewModel(arch) is IMultistageVehicleViewModel veh) {
				return veh;
			};
			throw new Exception($"Could not create viewmodel for {arch} completed bus!");
		}

        public IVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, bool exempted)
		{
			return _vehicleViewModelFactory.CreateVehicleViewModel(consolidatedVehicleData, null);
		}

		public IMultistageVehicleViewModel GetInterimStageVehicleViewModel(IVehicleDeclarationInputData inputData)
		{
			return _vehicleViewModelFactory.CreateVehicleViewModel(null, vehicleInput: inputData) as IMultistageVehicleViewModel;
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

        public IMultistageAuxiliariesViewModel GetAuxiliariesViewModel(
			CompletedBusArchitecture arch)
        {
            return _multistepComponentViewModelFactory.CreateNewMultistepBusAuxViewModel(arch);
        }

        public IMultistageAuxiliariesViewModel
			GetAuxiliariesViewModel(CompletedBusArchitecture arch,
				IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
        {
            return _multistepComponentViewModelFactory.CreateNewMultistepBusAuxViewModel(arch);
        }

        #endregion





        #region Implementation of IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider

        public IVehicleViewModel CreateStageInputVehicleViewModel(string inputProviderType)
        {
            return _multistageViewModelFactoryFirstParameterAsNameInstanceProvider.CreateStageInputVehicleViewModel(inputProviderType);
        }

        #endregion

        #region Implementation of IDocumentViewModelFactory

        public IDocumentViewModel CreateDocumentViewModel(IInputDataProvider declarationInput)
        {
			return _documentViewModelFactory.CreateDocumentViewModel(declarationInput);
        }


        public IDocumentViewModel GetCreateNewStepInputViewModel(bool exemptedVehicle)
        {
            return _documentViewModelFactory.GetCreateNewStepInputViewModel(exemptedVehicle);
        }

        public IDocumentViewModel GetCreateNewVifViewModel(bool completed)
        {
            return _documentViewModelFactory.GetCreateNewVifViewModel(completed);
        }

        #endregion

    }
}