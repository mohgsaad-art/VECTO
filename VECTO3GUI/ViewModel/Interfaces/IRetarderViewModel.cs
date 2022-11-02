using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
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
