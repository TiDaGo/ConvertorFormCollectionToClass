using System;

using tidago.apofc.Interfaces;

namespace tidago.apofc.Convertors
{
	/// <summary>
	/// Default converter for convert value to property type
	/// </summary>
	public class DefaultPropertyValueConverter : IPropertyValueConverter
	{
		public virtual object ConvertToBoolean(string dataValue, bool hasNull = false)
		{
			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				return false;
			}
			dataValue = dataValue.Split(',')[0].Trim();
			if (!bool.TryParse(dataValue, out var result))
			{
				result = "1".Equals(dataValue)
					|| "yes".Equals(dataValue, StringComparison.InvariantCultureIgnoreCase)
					|| "y".Equals(dataValue, StringComparison.InvariantCultureIgnoreCase)
					|| "true".Equals(dataValue, StringComparison.InvariantCultureIgnoreCase);
			}
			return result;
		}

		public virtual object ConvertToDateTime(string dataValue, bool hasNull = false)
		{
			if (string.IsNullOrWhiteSpace(dataValue) || !DateTime.TryParse(dataValue, out var result))
			{
				if (hasNull)
					return null;
				result = DateTime.MinValue;
			}
			return result;
		}

		public virtual object ConvertToDecimal(string dataValue, bool hasNull = false)
		{
			decimal result;

			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				result = default;
			}
			else
			{
				dataValue = dataValue.Replace(" ", string.Empty);
				if (!decimal.TryParse(dataValue, out result))
				{
					if (hasNull)
						return null;
					result = default;
				}
			}
			return result;
		}

		public virtual object ConvertToDouble(string dataValue, bool hasNull = false)
		{
			double result;

			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				result = default;
			}
			else
			{
				dataValue = dataValue.Replace(" ", string.Empty);
				if (!double.TryParse(dataValue, out result))
				{
					if (hasNull)
						return null;
					result = default;
				}
			}
			return result;
		}

		public virtual object ConvertToEnum(Type enumType, string dataValue)
		{
			if (!string.IsNullOrEmpty(dataValue))
			{
				try
				{
					return Enum.Parse(enumType, dataValue, true);
				}
				catch
				{
				}
			}
			return Enum.GetValues(enumType).GetValue(0);
		}

		public object ConvertToFieldType(Type fieldType, object value)
		{
			if (value is null)
				return null;

			var valueType = value.GetType();

			if (valueType != fieldType)
			{
				if (fieldType == typeof(string))
					return value.ToString();
				if (fieldType == typeof(DateTime?))
					return ConvertToDateTime(value.ToString(), true);
				if (fieldType == typeof(DateTime))
					return ConvertToDateTime(value.ToString());
				if (fieldType == typeof(int) && (valueType == typeof(bool) || valueType == typeof(bool?)))
					return ConvertToInt((bool?)value);
				if (fieldType == typeof(int?) && (valueType == typeof(bool) || valueType == typeof(bool?)))
					return ConvertToInt((bool?)value, true);
			}
			return value;
		}

		public virtual object ConvertToFloat(string dataValue, bool hasNull = false)
		{
			float result;

			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				result = default;
			}
			else
			{
				dataValue = dataValue.Replace(" ", string.Empty);
				if (!float.TryParse(dataValue, out result))
				{
					if (hasNull)
						return null;
					result = default;
				}
			}
			return result;
		}

		public virtual object ConvertToGuid(string dataValue, bool hasNull = false)
		{
			Guid result;

			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				result = default;
			}
			else
			{
				dataValue = dataValue.Replace(" ", string.Empty);
				if (!Guid.TryParse(dataValue, out result))
				{
					if (hasNull)
						return null;
					result = default;
				}
			}
			return result;
		}

		public virtual object ConvertToInt(bool? dataValue, bool hasNull = false)
		{
			return dataValue is null
				? hasNull
					? (int?)null
					: 0
				: dataValue.Value
					? 1
					: 0;
		}

		public virtual object ConvertToInt(string dataValue, bool hasNull = false)
		{
			int result;

			if (string.IsNullOrWhiteSpace(dataValue))
			{
				if (hasNull)
					return null;
				result = default;
			}
			else
			{
				dataValue = dataValue.Replace(" ", string.Empty);
				if (!int.TryParse(dataValue, out result))
				{
					if (hasNull)
						return null;
					result = default;
				}
			}
			return result;
		}

		public object ConvertToPropertyType(Type propertyType, string dataValue)
		{
			if (propertyType == typeof(string))
				return dataValue;
			else if (propertyType == typeof(Enum) || propertyType.BaseType == typeof(Enum))
				return ConvertToEnum(propertyType, dataValue);
			else if (propertyType == typeof(Guid?))
				return ConvertToGuid(dataValue, true);
			else if (propertyType == typeof(Guid))
				return ConvertToGuid(dataValue);
			else if (propertyType == typeof(int?))
				return ConvertToInt(dataValue, true);
			else if (propertyType == typeof(int))
				return ConvertToInt(dataValue);
			else if (propertyType == typeof(decimal?))
				return ConvertToDecimal(dataValue, true);
			else if (propertyType == typeof(decimal))
				return ConvertToDecimal(dataValue);
			else if (propertyType == typeof(double?))
				return ConvertToDouble(dataValue, true);
			else if (propertyType == typeof(double))
				return ConvertToDouble(dataValue);
			else if (propertyType == typeof(float?))
				return ConvertToFloat(dataValue, true);
			else if (propertyType == typeof(float))
				return ConvertToFloat(dataValue);
			else if (propertyType == typeof(DateTime?))
				return ConvertToDateTime(dataValue, true);
			else if (propertyType == typeof(DateTime))
				return ConvertToDateTime(dataValue);
			else if (propertyType == typeof(bool?))
				return ConvertToBoolean(dataValue, true);
			else if (propertyType == typeof(bool))
				return ConvertToBoolean(dataValue);
			else
				throw new NotSupportedException($"Not implement convertor for {propertyType.FullName} into IPropertyValueConverter");
		}
	}
}
