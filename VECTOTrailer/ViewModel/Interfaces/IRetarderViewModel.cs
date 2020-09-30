using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTOTrailer.Util;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface IRetarderViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IRetarderInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		ObservableCollection<RetarderLossMapEntry> LossMap { get; }
		RetarderType Type { get; set; }
		double Ratio { get; set; }
	}
}
