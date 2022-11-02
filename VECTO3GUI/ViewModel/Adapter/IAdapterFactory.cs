using TUGraz.VectoCommon.InputData;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter
{
	public interface IAdapterFactory
	{
		IAirdragDeclarationInputData AirdragDeclarationAdapter(AirdragViewModel viewModel);

		IAngledriveInputData AngledriveDeclarationAdapter(IAngledriveViewModel angledriveViewModel);

		IAuxiliariesDeclarationInputData AuxiliariesDeclarationAdapter(IAuxiliariesViewModel auxiliariesViewModel);

		IAxleDeclarationInputData AxleDeclarationAdapter(IAxleViewModel axleViewModel);

		IAxleGearInputData AxlegearDeclarationAdapter(IAxlegearViewModel axlegearViewModel);

		IEngineDeclarationInputData EngineDeclarationAdapter(EngineViewModel viewModel);

		IGearboxDeclarationInputData GearboxDeclarationAdapter(GearboxViewModel viewModel);

		IRetarderInputData RetarderDeclarationAdapter(IRetarderViewModel retarderViewModel);

		ITorqueConverterDeclarationInputData TorqueConverterDeclarationAdapter(
			ITorqueConverterViewModel torqueConverterViewModel);

		ITyreDeclarationInputData TyreDeclarationAdapter(ITyreViewModel tyreViewModel);

		IVehicleDeclarationInputData VehicleDeclarationAdapter(IVehicleViewModel vehicleViewModel);
	}
}
