using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using tidago.apofc.Helpers;
using tidago.apofc.Interfaces;

namespace tidago.apofc {

    /// <summary>
    /// Dynamic array of model with key, supported for serialize/deserialize XML
    /// </summary>
    /// <typeparam name="TElement">Type of element</typeparam>
    /// <typeparam name="TKey">Type of element key</typeparam>
    public class DynamicArray<TKey, TElement> : ICollection<TElement>, IEnumerable<TElement>, IDynamicFillModel {
        private List<TElement> elements;

        public DynamicArray()
        {
            elements = new List<TElement>();
        }

        int ICollection<TElement>.Count => elements?.Count ?? 0;

        /// <summary>
        /// Gets a collection containing the values in the DynamicArray
        /// </summary>
        public IReadOnlyCollection<TElement> Elements => elements;

        bool ICollection<TElement>.IsReadOnly { get; } = false;

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns> The value associated with the specified key. If the specified key is not found,
        /// a get operation throws a System.Collections.Generic.KeyNotFoundException, and
        /// a set operation creates a new element with the specified key.</returns>
        public TElement this[TKey key] {
            get {
                var fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
                return elements.First(x => Equals(x.GetValue<TKey>(fieldName), key));
            }
        }

        /// <summary>
        ///  Adds the specified value to the collection.
        /// </summary>
        /// <param name="item">The value of the element to add. The value can be null for reference types.</param>
        public void Add(TElement item)
        {
            elements.Add(item);
        }

        /// <summary>
        /// Adds the specified values to the collection.
        /// </summary>
        /// <param name="items">The values of the elements to add. The values can be null for reference types.</param>
        public void AddRange(params TElement[] items)
        {
            elements.AddRange(items);
        }

        void ICollection<TElement>.Clear() => elements?.Clear();

        bool ICollection<TElement>.Contains(TElement item) => elements?.Contains(item) ?? false;

        void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex) => elements?.CopyTo(array, arrayIndex);

        public void DynamicFill(IEnumerable<IFormTreeNode> nodes, IPropertyValueConverter converter)
        {
            var objectPopulator = new ObjectPopulator(converter);
            foreach (var node in nodes)
            {
                var propValue = (TKey)converter.ConvertToPropertyType(typeof(TKey), node.Key);
                TElement element;
                if (elements == null || !HasKey(propValue))
                {
                    element = Activator.CreateInstance<TElement>();
                    Add(element);
                    var fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
                    var (field, typeOfField) = MemberHelpers.GetPropertyField(element, fieldName);
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

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Check present key in elements.
        /// </summary>
        /// <param name="key">Search key.</param>
        /// <returns></returns>
        public bool HasKey(TKey key)
        {
            var fieldName = MemberHelpers.GetKeyPropertyFieldName<TElement>();
            return HasKey(key, fieldName);
        }

        public bool Remove(TElement item) => elements?.Remove(item) ?? false;

        /// <summary>
        /// Check present key in elements.
        /// </summary>
        /// <param name="key">Search key.</param>
        /// <param name="fieldName">Search key in this field</param>
        /// <returns></returns>
        protected bool HasKey(TKey key, string fieldName)
        {
            if (elements == null || elements.Count == 0)
                return false;

            return elements.Any(x => Equals(x.GetValue<TKey>(fieldName), key));
        }
    }
}