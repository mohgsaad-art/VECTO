using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
	public class BackingStorage<T> : ObservableObject 
		where T : INotifyPropertyChanged
	{

		delegate object GetterDelegate();
		private IReadOnlyCollection<string> _observedProperties;
		private T _observedObject;

		private Dictionary<string, GetterDelegate> _getterDelegatesMap = new Dictionary<string, GetterDelegate>();
		private Dictionary<string, Type> _propertyTypesMap = new Dictionary<string, Type>();
		private IDictionary<string, IEqualityComparer> _equalityComparers = new Dictionary<string, IEqualityComparer>();

		private IDictionary<string, object> _savedValues = null;
		private IDictionary<string, object> _unsavedChanges = new Dictionary<string, object>();
		private IDictionary<string, object> _currentValues = new Dictionary<string, object>();


		public BackingStorage(T observedObject, params string[] observedProperties)
		{
			_observedProperties = new HashSet<string>(observedProperties);
			_observedObject = observedObject;
			observedObject.PropertyChanged += OnObservedPropertyChanged;
		}

		public bool UnsavedChanges
		{
			get => (_savedValues == null) || (_unsavedChanges.Count != 0);
		}

		private void ResetUnsavedChanges()
		{
			_unsavedChanges.Clear();
			OnPropertyChanged(nameof(UnsavedChanges));
		}

		public void SaveChanges()
		{
			_savedValues = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(_currentValues));
			ResetUnsavedChanges();
		}

		private void OnObservedPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var propertyName = e.PropertyName;
			if (!_observedProperties.Contains(propertyName)) {
				return;
			}

			if (!_getterDelegatesMap.ContainsKey(propertyName)) {
				StorePropertyInfo(propertyName);
			}

			UpdateValue(propertyName);
		}

		private void UpdateValue(string propertyName)
		{
			
			var newValue = _getterDelegatesMap[propertyName]();
			_currentValues[propertyName] = newValue;
			if (ValueHasChanged(newValue, propertyName)) {
				_unsavedChanges[propertyName] = newValue;
			} else {
				_unsavedChanges.Remove(propertyName);
			}
			OnPropertyChanged(nameof(UnsavedChanges));
		}

		private bool ValueHasChanged(object newValue, string propertyName)
		{
			if (_savedValues == null
				|| !_savedValues.ContainsKey(propertyName)) {
				return true;
			}

			var savedValue = _savedValues[propertyName];

			if (_equalityComparers.ContainsKey(propertyName)) {
				var equalityComparer = _equalityComparers[propertyName];
				return !equalityComparer.Equals(newValue, savedValue);
			} else {
				return newValue == savedValue;
			}
		}

		private void StorePropertyInfo(string propertyName)
		{
			var propInfo = _observedObject.GetType().GetProperty(propertyName);


			var propertyType = propInfo.PropertyType;
			_propertyTypesMap[propertyName] = propInfo.PropertyType;

			try {
				_equalityComparers[propertyName] = CreateEqualityComparer(propertyType);
			} catch (Exception ex) {
				// TODO;
			}
			


			var getMethod = propInfo.GetGetMethod();
			var getterDelegate = (GetterDelegate)getMethod.CreateDelegate(typeof(GetterDelegate), _observedObject);
			_getterDelegatesMap[propertyName] = getterDelegate;


		}

		private static IEqualityComparer CreateEqualityComparer(Type type)
		{

			Type myGeneric = typeof(EqualityComparer<>);
			Type constructedClass = myGeneric.MakeGenericType(type);
			return (IEqualityComparer)constructedClass
				.GetProperty(nameof(EqualityComparer<T>.Default), BindingFlags.Static | BindingFlags.Public)
				.GetValue(null);

		}
	}
}