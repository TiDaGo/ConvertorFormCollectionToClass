using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using tidago.apofc.Helpers;
using tidago.apofc.Interfaces;

namespace tidago.apofc
{
	/// <summary>
	/// Dynamic array of model with key, supported for serialize/deserialize XML
	/// </summary>
	/// <typeparam name="TElement">Type of element</typeparam>
	/// <typeparam name="TKey">Type of element key</typeparam>
	public class DynamicArray<TKey, TElement> : ICollection<TElement>, IEnumerable<TElement>, IDynamicFillModel, IDynamicArray
	{
		private readonly List<TElement> _elements;

		public DynamicArray()
		{
			_elements = new List<TElement>();
		}

		public int Count => _elements?.Count ?? 0;

		/// <summary>
		/// Gets a collection containing the values in the DynamicArray
		/// </summary>
		public IReadOnlyCollection<TElement> Elements => _elements;

		bool ICollection<TElement>.IsReadOnly { get; } = false;

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <returns> The value associated with the specified key. If the specified key is not found,
		/// a get operation throws a System.Collections.Generic.KeyNotFoundException, and
		/// a set operation creates a new element with the specified key.</returns>
		public TElement this[TKey key]
		{
			get
			{
				string fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
				return _elements.First(x => Equals(x.GetValue<TKey>(fieldName), key));
			}
		}

		object IDynamicArray.this[object key]
		{
			get { return this[(TKey)key]; }
		}

		/// <summary>
		///  Adds the specified value to the collection.
		/// </summary>
		/// <param name="item">The value of the element to add. The value can be null for reference types.</param>
		public void Add(TElement item)
		{
			_elements.Add(item);
		}

		/// <summary>
		/// Adds the specified values to the collection.
		/// </summary>
		/// <param name="items">The values of the elements to add. The values can be null for reference types.</param>
		public void AddRange(params TElement[] items)
		{
			_elements.AddRange(items);
		}

		void ICollection<TElement>.Clear() => _elements?.Clear();

		bool ICollection<TElement>.Contains(TElement item) => _elements?.Contains(item) ?? false;

		public bool ContainsKey(TKey key)
		{
			string fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
			return _elements.Any(x => Equals(x.GetValue<TKey>(fieldName), key));
		}

		void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex) => _elements?.CopyTo(array, arrayIndex);

		public void DynamicFill(IEnumerable<IFormTreeNode> nodes, IPropertyValueConverter converter)
		{
			ObjectPopulator objectPopulator = new ObjectPopulator(converter);
			foreach (IFormTreeNode node in nodes)
			{
				TKey propValue = (TKey)converter.ConvertToPropertyType(typeof(TKey), node.Key);
				TElement element;
				if (_elements == null || !HasKey(propValue))
				{
					element = Activator.CreateInstance<TElement>();
					Add(element);
					string fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
					(System.Reflection.MemberInfo field, Type typeOfField) = MemberHelpers.GetPropertyField(element, fieldName);
					// Convert property to field, for support readonly model
					object fieldValue = converter.ConvertToFieldType(typeOfField, propValue);
					element.SetValue(field, fieldValue);
				}
				else
				{
					element = this[propValue];
				}
				if (node is FormTreeCollection formTreeCollection)
				{
					objectPopulator.Populate(formTreeCollection.Childs, element);
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		///  Gets a collection containing the keys
		/// </summary>
		/// <returns>Keys collection</returns>
		public IReadOnlyCollection<TKey> GetKeys()
		{
			string fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
			return _elements?.Select(x => x.GetValue<TKey>(fieldName)).ToArray() ?? Array.Empty<TKey>();
		}

		IReadOnlyCollection<object> IDynamicArray.GetKeys()
		{
			return GetKeys().Select(x => (object)x).ToArray();
		}
		/// <summary>
		/// Check present key in elements.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <returns></returns>
		public bool HasKey(TKey key)
		{
			string fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
			return HasKey(key, fieldName);
		}

		bool IDynamicArray.HasKey(object key)
		{
			return HasKey((TKey)key);
		}

		public bool Remove(TElement item) => _elements?.Remove(item) ?? false;

		/// <summary>
		/// Check present key in elements.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <param name="fieldName">Search key in this field</param>
		/// <returns></returns>
		protected bool HasKey(TKey key, string fieldName)
		{
			if (_elements == null || _elements.Count == 0)
				return false;

			return _elements.Any(x => Equals(x.GetValue<TKey>(fieldName), key));
		}
	}
}
