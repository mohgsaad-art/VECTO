using TUGraz.VectoCommon.InputData;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter
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
