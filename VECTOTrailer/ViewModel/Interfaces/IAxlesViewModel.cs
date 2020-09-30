using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace VECTOTrailer.ViewModel.Interfaces {
	public interface IAxlesViewModel : IComponentViewModel
	{
		IList<IAxleDeclarationInputData> ModelData { get; }

		AxleConfiguration AxleConfiguration { get; }
		IAxleViewModel CurrentAxle { get; set; }

		ObservableCollection<IAxleViewModel> Axles { get; }

		int NumSteeredAxles { get; }
	}
}