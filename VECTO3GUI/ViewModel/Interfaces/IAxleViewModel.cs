using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IAxleViewModel : IComponentViewModel
	{
		IAxleDeclarationInputData ModelData { get; }

		int AxleIndex { get; }
		AxleType AxleType { get; set; }
		bool TwinTyres { get; set; }
		bool Steered { get; set; }
		ITyreViewModel Tyre { get; }
		AllowedEntry<AxleType>[] AllowedAxleTypes { get; }
		
	}
}