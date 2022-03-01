using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using tidago.apofc.Attributes;

namespace tidago.apofc.Helpers {

    /// <summary>
    /// Helpers for reflection
    /// </summary>
    internal static class MemberHelpers {

        /// <summary>
        /// Get field or property name from PropertyFieldAttribute or DataMemberAttribute for MemberInfo.
        /// </summary>
        /// <param name="member">Memeber info it's based class for FieldInfo and PropertyInfo.</param>
        /// <returns>Property name.</returns>
        /// <exception cref="NullReferenceException">This exception can be thrown if the member does not have a PropertyFieldAttribute or DataMemberAttribute.</exception>
        public static string GetFieldName(MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return member.Name;
            }

            string fieldName = member.GetCustomAttribute<PropertyFieldAttribute>(false)?.PropertyName;
            if (!string.IsNullOrEmpty(fieldName))
            {
                return fieldName;
            }

            fieldName = member.GetCustomAttribute<DataMemberAttribute>(false)?.Name;
            return !string.IsNullOrEmpty(fieldName) 
                ? fieldName 
                : throw new NullReferenceException();
        }

        /// <summary>
        /// Get field or property in object with KeyPropertyFieldAttribute
        /// </summary>
        /// <param name="typeOfObject">The type of object for search field or property</param>
        /// <returns>MemberInfo, type of element, field name</returns>
        public static string GetKeyPropertyField(Type typeOfObject)
        {
            PropertyInfo property = typeOfObject
                .GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                .Where(x => HasKeyPropertyFieldAttribute(x))
                .FirstOrDefault();

            if (property != null)
            {
                return GetFieldName(property);
            }

            FieldInfo field = typeOfObject
                .GetFields(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic)
                .Where(x => HasKeyPropertyFieldAttribute(x))
                .FirstOrDefault();
            return GetFieldName(field);
        }

        /// <summary>
        /// Get field or property in object with KeyPropertyFieldAttribute
        /// </summary>
        /// <typeparam name="T">The type of object for search field or property</typeparam>
        /// <returns>MemberInfo, type of element, field name</returns>
        public static string GetKeyPropertyFieldName<T>()
        {
            return GetKeyPropertyField(typeof(T));
        }

        /// <summary>
        /// Get field or property in object by name.
        /// </summary>
        /// <param name="obj">The object for search field or property.</param>
        /// <param name="fieldName">Search field name.</param>
        /// <returns>MemberInfo and type of element</returns>
        public static (MemberInfo, Type) GetPropertyField(this object obj, string fieldName)
        {
            PropertyInfo property = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                .Where(x => CheckMemberHasLinkToProperty(x, fieldName))
                .FirstOrDefault();

            if (property != null)
                return (property, property.PropertyType);

            FieldInfo field = obj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic)
                .Where(x => CheckMemberHasLinkToProperty(x, fieldName))
                .FirstOrDefault();
            if (field != null)
                return (field, field.FieldType);

            // Support for system fields
            return (null, null);
        }
        /// <summary>
        /// Get value from untyped model
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="obj">Untyped model</param>
        /// <param name="propertyName">The name of the property for which the value will be obtained</param>
        /// <returns>Typed T value from obj for property name</returns>
        public static T GetValue<T>(this object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);
            return (T)propInfo.GetValue(obj);
        }
        /// <summary>
        /// Set a value in an object field or property.
        /// </summary>
        /// <param name="obj">The object whose field value will be set.</param>
        /// <param name="memberInfo">Property of field of object.</param>
        /// <param name="value">The value assigned to the field.</param>
        public static void SetValue(this object obj, MemberInfo memberInfo, object value)
        {
            if (memberInfo is FieldInfo field)
            {
                field.SetValue(obj, value);
            }
            else if (memberInfo is PropertyInfo property)
            {
                property.SetValue(obj, value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Checking if an memberInfo has a reference in attribute to the property name
        /// </summary>
        /// <param name="memberInfo">Checked memberInfo.</param>
        /// <param name="propertyName">Checked propertyName.</param>
        /// <returns>true if MemberInfo has an attribute with the requested propertyName</returns>
        private static bool CheckMemberHasLinkToProperty(MemberInfo memberInfo, string propertyName)
        {
            var result = 
                string.Equals(memberInfo.GetCustomAttribute<PropertyFieldAttribute>(false)?.PropertyName, propertyName)
                || string.Equals(memberInfo.GetCustomAttribute<DataMemberAttribute>(false)?.Name, propertyName);
            return result;
        }

        /// <summary>
        /// Checking the memberInfo whether it has KeyPropertyFieldAttribute
        /// </summary>
        /// <param name="memberInfo">Checked memberInfo.</param>
        /// <returns>True if memberInfo has KeyPropertyFieldAttribute</returns>
        private static bool HasKeyPropertyFieldAttribute(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<KeyPropertyFieldAttribute>(false) != null;
        }
    }
}