using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;

namespace VECTO3GUI2020.Helper
{
	interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
	{

	}
	public class ObservableDictionary<TKey,TValue> : IObservableDictionary<TemplateKey, TValue>
	{
		#region Implementation of INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		private IDictionary<TemplateKey, TValue> _dict = new Dictionary<TemplateKey, TValue>();

		public ObservableDictionary()
		{
			throw new NotImplementedException();
		}

		#region Implementation of IEnumerable

		IEnumerator<KeyValuePair<TemplateKey, TValue>> IEnumerable<KeyValuePair<TemplateKey, TValue>>.GetEnumerator()
		{
			return _dict.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)_dict).GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection<KeyValuePair<TemplateKey,TValue>>

		public void Add(KeyValuePair<TemplateKey, TValue> item)
		{
			_dict.Add(item);
		}

		public void Clear()
		{
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			_dict.Clear();
		}

		public bool Contains(KeyValuePair<TemplateKey, TValue> item)
		{
			return _dict.Contains(item);
		}

		public void CopyTo(KeyValuePair<TemplateKey, TValue>[] array, int arrayIndex)
		{
			_dict.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TemplateKey, TValue> item)
		{
			return _dict.Remove(item);
		}

		public int Count => _dict.Count;

		public bool IsReadOnly => _dict.IsReadOnly;

		#endregion

		#region Implementation of IDictionary<TemplateKey,TValue>

		public bool ContainsKey(TemplateKey key)
		{
			return _dict.ContainsKey(key);
		}

		public void Add(TemplateKey key, TValue value)
		{

			_dict.Add(key, value);
		}

		public bool Remove(TemplateKey key)
		{
			TValue value;
			var itemFound = _dict.TryGetValue(key, out value);
			var result = itemFound && _dict.Remove(key);
			if (result) {
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
					new List<TValue>(){
						value
					}));
			}
			return result;
		}

		public bool TryGetValue(TemplateKey key, out TValue value)
		{
			return _dict.TryGetValue(key, out value);
		}

		public TValue this[TemplateKey key]
		{
			get => _dict[key];
			set => _dict[key] = value;
		}

		public ICollection<TemplateKey> Keys => _dict.Keys;

		public ICollection<TValue> Values => _dict.Values;

		#endregion


	}
}