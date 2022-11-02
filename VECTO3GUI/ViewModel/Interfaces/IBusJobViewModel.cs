using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IBusJobViewModel
	{
		JobFileType FirstFileType { get; }
		JobFileType SecondFileType { get; }
		string FirstFilePath { get;  }
		string SecondFilePath { get;  }
		string FirstLabelText { get;  }
		string SecondLabelText { get;  }
		JobEntry SavedJobEntry { get; }
		ICommand SelectFirstFileCommand { get; }
		ICommand SelectSecondFileCommand { get; }
		ICommand CancelCommand { get; }
		ICommand SaveCommand { get; }
	}
}
