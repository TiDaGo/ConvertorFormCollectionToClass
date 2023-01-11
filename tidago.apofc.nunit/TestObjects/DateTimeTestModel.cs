
using System;

using tidago.apofc.Attributes;

namespace tidago.apofc.nunit.TestObjects
{
	public class DateTimeTestModel
	{
		[PropertyField]
		public DateTime MyDateTime { get; set; }
		[PropertyField]
		public DateTime? NullableDateTime { get; set; }
	}
}
