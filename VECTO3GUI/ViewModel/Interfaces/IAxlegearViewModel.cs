using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAxlegearViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IAxleGearInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		double Ratio { get; set; }
		AxleLineType LineType { get; set; }

		AllowedEntry<AxleLineType>[] AllowedLineTypes { get; }
		ObservableCollection<GearLossMapEntry> LossMap { get; }
	}
}
