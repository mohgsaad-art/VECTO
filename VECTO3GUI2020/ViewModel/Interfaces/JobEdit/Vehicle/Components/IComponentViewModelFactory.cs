using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IComponentViewModelFactory
    {
        /// <summary>
        /// Creates a ViewModel for a component.
        /// </summary>
        /// <param name="inputData">The Type of the input Data is used to resolve a Named Binding and is also passed to the constructor</param>
        /// <returns></returns>
		IComponentViewModel CreateComponentViewModel(object inputData);
		IVehicleViewModel CreateVehicleViewModel(IVehicleDeclarationInputData inputData);
		IAdasViewModel CreateAdasViewModel(IAdvancedDriverAssistantSystemDeclarationInputData inputData);
		IEngineModeViewModel CreateEngineModeViewModel(IEngineModeDeclarationInputData inputData);
		IEngineFuelViewModel CreateEngineFuelViewModel(IEngineFuelDeclarationInputData inputData);
		IComponentsViewModel CreateComponentsViewModel(IXMLVehicleComponentsDeclaration inputData);
        ICommonComponentViewModel CreateCommonComponentViewModel(IComponentInputData inputData);
	}
}
