using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTOTrailer.Util;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ITorqueConverterViewModel : IComponentViewModel, ICommonComponentParameters
	{
		ITorqueConverterDeclarationInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		ObservableCollection<TorqueConverterCharacteristics> Characteristics { get; }
	}
}
