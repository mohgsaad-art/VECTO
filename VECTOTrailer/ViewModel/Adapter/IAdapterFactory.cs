using TUGraz.VectoCommon.InputData;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter
{
	public interface IAdapterFactory
	{
		ITrailerDeclarationInputData TrailerDeclarationAdapter(ICompleteTrailerViewModel trailerViewModel);
	}
}
