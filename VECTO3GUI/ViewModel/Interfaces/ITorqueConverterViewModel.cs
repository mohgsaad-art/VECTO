using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface ITorqueConverterViewModel : IComponentViewModel, ICommonComponentParameters
	{
		ITorqueConverterDeclarationInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		ObservableCollection<TorqueConverterCharacteristics> Characteristics { get; }
	}
}
