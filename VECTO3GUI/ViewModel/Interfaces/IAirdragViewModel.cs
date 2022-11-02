using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAirdragViewModel : IAirdrag, IComponentViewModel
	{
		IAirdragDeclarationInputData ModelData { get; }
		bool IsEditable { get; }
		ICommand LoadFileCommand { get; }
		ICommand AirdragConfigCommand { get; }

	}
}
