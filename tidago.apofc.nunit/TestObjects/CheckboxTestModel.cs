using tidago.apofc.Attributes;

namespace tidago.apofc.nunit.TestObjects
{
	public class CheckboxTestModel
	{
		[PropertyField]
		public bool TestBool1 { get; set; }

		[PropertyField]
		public bool TestBool2 { get; set; }

		[PropertyField]
		public bool TestBool3 { get; set; }

		[PropertyField]
		public bool TestBool4 { get; set; }

		[PropertyField]
		public bool TestBool5 { get; set; }

		[PropertyField(nameof(TestBool6))]
		protected int _testBool6;

		public bool TestBool6
		{
			get => _testBool6 == 1;
			set => _testBool6 = value ? 1 : 0;
		}

		[PropertyField]
		public bool? TestBool7 { get; set; }
	}
}
