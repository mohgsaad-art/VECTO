using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTOTrailer.Util;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface IGearboxViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IGearboxDeclarationInputData ModelData { get; }

		CertificationMethod CertificationMethod { get; set; }
		AllowedEntry<CertificationMethod>[] AllowedCertificationMethods { get; }

		GearboxType TransmissionType { get; set; }
		AllowedEntry<GearboxType>[] AllowedTransmissionTypes { get; }
		ObservableCollection<Gear> Gears { get; }

		Gear SelectedGear { get; set; }
	}
}
