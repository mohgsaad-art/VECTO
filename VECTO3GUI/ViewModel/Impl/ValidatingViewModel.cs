using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using VECTO3GUI.Helper;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Impl
{
	public class ValidatingViewModel : ViewModelBase //		: ObservableObject, INotifyDataErrorInfo
	{
		//private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

		//private readonly Dictionary<string, ValidationAttribute[]> _validations = new Dictionary<string, ValidationAttribute[]>();

		//public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		//private readonly object _lock = new object();

		//protected ValidatingViewModel()
		//{
		//	const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
		//	var properties = GetType().GetProperties(flags);

		//	//var classValidation = GetType().GetCustomAttributes<ValidationAttribute>().ToList();

		//	foreach (var p in properties) {
		//		if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>)) {

		//			//	var eventInfo = p.PropertyType.GetEvent("CollectionChanged");
		//			//	var methodInfo = GetType().GetMethod("ValidateHandler");
		//			//	var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this,
		//			//								methodInfo);

		//			//	eventInfo.AddEventHandler(this, handler);
		//		}

		//		var attributes = p.GetCustomAttributes<VectoParameterAttribute>().ToList();
		//		var validationAttributes = p.GetCustomAttributes<ValidationAttribute>().ToList();
		//		if (attributes.Any() || validationAttributes.Any()) {
		//			var validations = new List<ValidationAttribute>();
		//			validations.AddRange(GetVectoParameterValidation(attributes));
		//			validations.AddRange(validationAttributes);
		//			//validations.AddRange(classValidation);
		//			_validations[p.Name] = validations.ToArray();
		//		}
		//	}
		//}

		//protected void ValidateHandler(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//	//ValidateProperty(sender);
		//}

		//private ValidationAttribute[] GetVectoParameterValidation(List<VectoParameterAttribute> attributes)
		//{
		//	if (attributes.Count == 0) {
		//		return new ValidationAttribute[0];
		//	}
		//	if (attributes.Count != 1) {
		//		throw new Exception(string.Format("Exactly one VectoParameter Attribute allowed! found {0}", attributes.Count));
		//	}

		//	var a = attributes.First();
		//	var p = a.Type.GetProperties().Where(x => x.Name == a.PropertyName).ToArray();
		//	if (p.Length != 1) {
		//		throw new Exception(string.Format("number of properties found: {0}, expected only 1! {1}:{2}", p.Length, a.Type, a.PropertyName));
		//	}

		//	return p.First().GetCustomAttributes<ValidationAttribute>().ToArray();
		//}


		//public bool HasErrors
		//{
		//	get {
		//		lock (_lock) {
		//			return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
		//		}
		//	}
		//}

		//public bool IsValid
		//{
		//	get { return HasErrors; }
		//}

		//public IEnumerable GetErrors(string propertyName)
		//{
		//	if (string.IsNullOrEmpty(propertyName)) {
		//		lock (_lock) {
		//			return _errors.SelectMany(err => err.Value.ToList());
		//		}
		//	}

		//	lock (_lock) {
		//		if (_errors.ContainsKey(propertyName) && _errors[propertyName] != null && _errors[propertyName].Count > 0) {
		//			return _errors[propertyName].ToList();
		//		}
		//	}

		//	return null;
		//}

		//private void OnErrorsChanged(string propertyName)
		//{
		//	if (ErrorsChanged != null) {
		//		ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		//	}
		//}

		//protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string proptertyName = null)
		//{
		//	ValidateProperty(value, proptertyName);
		//	return base.SetProperty(ref field, value,proptertyName);
		//}

		//protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
		//{
		//	if (propertyName == null || !_validations.ContainsKey(propertyName)) {
		//		return;
		//	}
		//	lock (_lock) {
		//		var validationContext = new ValidationContext(this) {
		//			MemberName = propertyName,
		//			DisplayName = propertyName,
		//		};
		//		var validationResults = new List<ValidationResult>();
		//		Validator.TryValidateValue(value, validationContext, validationResults, _validations[propertyName]);

		//		if (_errors.ContainsKey(propertyName)) {
		//			_errors.Remove(propertyName);
		//		}
		//		OnErrorsChanged(propertyName);
		//		HandleValidationResults(validationResults);
		//	}
		//}

		//public void Validate()
		//{
		//	lock (_lock) {
		//		var validationContext = new ValidationContext(this, null, null);
		//		var validationResults = new List<ValidationResult>();
		//		Validator.TryValidateObject(this, validationContext, validationResults, true);

		//		//clear all previous _errors  
		//		var propNames = _errors.Keys.ToList();
		//		_errors.Clear();
		//		propNames.ForEach(OnErrorsChanged);
		//		HandleValidationResults(validationResults);
		//	}
		//}

		//private void HandleValidationResults(List<ValidationResult> validationResults)
		//{
		//	var resultsByPropNames = from result in validationResults
		//							from member in result.MemberNames
		//							group result by member
		//							into g
		//							select g;

		//	foreach (var prop in resultsByPropNames) {
		//		var messages = prop.Select(r => r.ErrorMessage).ToList();
		//		_errors.Add(prop.Key, messages);
		//		OnErrorsChanged(prop.Key);
		//	}
		//}
	}
}
