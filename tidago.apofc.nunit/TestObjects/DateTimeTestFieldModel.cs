
using System;

using tidago.apofc.Attributes;

namespace tidago.apofc.nunit.TestObjects
{
	public class DateTimeTestFieldModel
	{
		[PropertyField(nameof(MyDateTime))]
		protected DateTime _myDateTime;

		[PropertyField(nameof(NullableDateTime))]
		protected DateTime? _nullableDateTime;

		public string MyDateTime
		{
			get => _myDateTime.ToString("yyyy-MM-dd");
			set => _myDateTime = DateTime.Parse(value);
		}

		public string NullableDateTime
		{
			get => _nullableDateTime?.ToString("yyyy-MM-dd");
			set => _nullableDateTime = string.IsNullOrEmpty(value)
				? (DateTime?)null
				: DateTime.Parse(value);
		}
	}
}
