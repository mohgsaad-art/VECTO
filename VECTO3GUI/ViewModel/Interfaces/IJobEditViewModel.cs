using System.Collections.ObjectModel;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IJobEditViewModel 
	{

		string JobFile { get; }

		IInputDataProvider InputDataProvider { get; set; }

		bool DeclarationMode { get; }
		ObservableCollection<Component> Components { get; }
		ICommand EditComponent { get; }
		Component SelectedComponent { get; }
		IComponentViewModel CurrentComponent { get; }

		ICommand SaveJob { get; }
		ICommand SaveAsJob { get; }
		ICommand CloseJob { get; }
		ICommand ValidateInput { get; }
		ICommand ShowValidationErrors { get; }
		ICommand RemoveValidationErrors { get; }

	}
}
