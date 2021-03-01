using System;

namespace tidago.apofc.Interfaces {

    /// <summary>
    /// Interface for convert value to property type
    /// </summary>
    public interface IPropertyValueConverter {

        /// <summary>
        /// Convert from String value. to DateTime
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToDateTime(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to decimal
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToDecimal(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to double
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToDouble(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to Enum
        /// </summary>
        /// <param name="enumType">type of enum collection</param>
        /// <param name="dataValue">String value.</param>
        /// <returns>Converted value.</returns>
        object ConvertToEnum(Type enumType, string dataValue);

        /// <summary>
        /// Convert from property type to saved field type
        /// </summary>
        /// <param name="value">Value for convert</param>
        /// <param name="fieldType">Convert to this type</param>
        /// <returns>Converted value</returns>
        object ConvertToFieldType(Type fieldType, object value);

        /// <summary>
        /// Convert from String value. to float
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToFloat(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to GUID
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToGuid(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to Int32
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="hasNull">Result value can be null.</param>
        /// <returns>Converted value.</returns>
        object ConvertToInt(string dataValue, bool hasNull = false);

        /// <summary>
        /// Convert from String value. to type
        /// </summary>
        /// <param name="dataValue">String value.</param>
        /// <param name="propertyType">Convert to this type</param>
        /// <returns>Converted value</returns>
        object ConvertToPropertyType(Type propertyType, string dataValue);
    }
}