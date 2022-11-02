using System.Collections.Generic;
using System.Collections.ObjectModel;
using VECTO3GUI.Util;


namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IComponentViewModel
	{
		IJobEditViewModel JobViewModel { set; }

		bool DeclarationMode { get; }
		

		ObservableCollection<Component> Components { get; }
		IComponentViewModel ParentViewModel { get; set; }

		IComponentViewModel GetComponentViewModel(Component component);

		bool IsComponentDataChanged();

		void ResetComponentData();
		object CommitComponentData();

		void ShowValidationErrors(Dictionary<string, string> errors);
		void RemoveValidationErrors(Dictionary<string, string> errors);


	}
}