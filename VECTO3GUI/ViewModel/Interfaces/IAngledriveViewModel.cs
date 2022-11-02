using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAngledriveViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IAngledriveInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		double Ratio { get; set; }
		ObservableCollection<GearLossMapEntry> LossMap { get; }
		AngledriveType AngledriveType { get;  }
	}
}
