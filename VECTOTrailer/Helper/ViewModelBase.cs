using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Castle.Core.Internal;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Helper
{
	public class ViewModelBase : ObservableObject, INotifyDataErrorInfo

	{

	private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	public ViewModelBase()
	{
            ErrorsChanged += ViewModelBase_ErrorsChanged;
	}

    private void ViewModelBase_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
			
    }

    public IEnumerable GetErrors(string propertyName)
	{
		if (propertyName.IsNullOrEmpty())
			return null;

		return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
	}

	public bool HasErrors
	{
		get
		{
			return _errors.Count > 0;
		}
	}

	public bool IsValid
	{
		get
		{
			return !HasErrors;
		}
	}

	public void AddPropertyError(string propertyName, string error)
	{
		if (_errors.ContainsKey(propertyName))
			_errors[propertyName].Add(error);
		else
			_errors[propertyName] = new List<string>() { error };

		NotifyErrorsChanged(propertyName);
	}

	public void RemovePropertyError(string propertyName)
	{
		if (_errors.ContainsKey(propertyName))
			_errors.Remove(propertyName);

		NotifyErrorsChanged(propertyName);
	}

	public void NotifyErrorsChanged(string propertyName)
	{
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}
	}
}
