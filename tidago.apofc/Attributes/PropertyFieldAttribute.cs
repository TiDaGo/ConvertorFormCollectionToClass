using System;
using System.Runtime.CompilerServices;

namespace tidago.apofc.Attributes {

    /// <summary>
    /// Attribute for mapping field or property with element of FormCollection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PropertyFieldAttribute : Attribute {

        /// <summary>
        /// Initialize mapping attribute
        /// </summary>
        /// <param name="propertyName">Name of public property for mapping field value to property</param>
        public PropertyFieldAttribute([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Name of property for mapping field to property
        /// </summary>
        public string PropertyName { get; }
    }
}