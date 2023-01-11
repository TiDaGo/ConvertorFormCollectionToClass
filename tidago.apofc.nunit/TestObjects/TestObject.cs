using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using tidago.apofc.Attributes;

namespace tidago.apofc.nunit.TestObjects
{

	public enum LocationType
	{
		Undefined,
		Home,
		Work,
		SecondaryHome
	}

	public enum PhonesType
	{
		Undefined,
		Mobile,
		Home,
		Work
	}

	public enum StaffType
	{
		Undefined,
		Boss,
		Manager,
		SecondaryBoss
	}

	[DataContract]
	public class Address
	{

		[JsonProperty(nameof(City)), DataMember(Name = nameof(City)), PropertyField(nameof(City))]
		protected string city;

		[JsonProperty(nameof(Street)), DataMember(Name = nameof(Street)), PropertyField(nameof(Street))]
		protected string street;

		[JsonProperty(nameof(Type)), DataMember(Name = nameof(Type)), PropertyField(nameof(Type)), KeyPropertyField]
		protected string type;

		public string City => city ?? string.Empty;
		public string Street => street ?? string.Empty;
		public LocationType Type => string.IsNullOrWhiteSpace(type) ? LocationType.Undefined : Enum.Parse<LocationType>(type);
	}

	[DataContract]
	public class Phone
	{

		[JsonProperty(nameof(Number)), DataMember(Name = nameof(Number)), PropertyField(nameof(Number))]
		protected string number;

		[JsonProperty(nameof(Type)), DataMember(Name = nameof(Type)), PropertyField(nameof(Type))]
		protected string type;

		public string Number
		{
			get
			{
				string raw = number ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(raw))
				{
					raw = Regex.Replace(raw, "\\D", "");
				}
				return raw;
			}
		}

		[KeyPropertyField]
		public PhonesType Type => string.IsNullOrWhiteSpace(type) ? PhonesType.Undefined : Enum.Parse<PhonesType>(type);
	}

	[DataContract]
	public class Salary
	{

		[JsonProperty, DataMember, PropertyField]
		public decimal Amount { get; set; }

		[JsonProperty, DataMember, PropertyField]
		public string Rate { get; set; }
	}

	[DataContract]
	public class Staff
	{

		[JsonProperty(nameof(Phones)), DataMember(Name = nameof(Phones)), PropertyField(nameof(Phones))]
		protected DynamicArray<PhonesType, Phone> phones;

		[JsonProperty, DataMember, PropertyField]
		public string FirstName { get; set; }

		public IReadOnlyCollection<Phone> Phones => phones?.Elements ?? Array.Empty<Phone>();

		[JsonProperty, DataMember, PropertyField]
		public string SecondaryName { get; set; }

		[JsonProperty, DataMember, PropertyField, KeyPropertyField]
		public StaffType Type { get; set; }

		[JsonProperty, DataMember, PropertyField]
		public StaffType? NullableType { get; set; }
	}

	[DataContract]
	public class WorkInfo
	{

		private DynamicArray<StaffType, Staff> _staffs;

		[JsonProperty(nameof(Staff)), DataMember(Name = nameof(Staff)), PropertyField(nameof(Staff))]
		public DynamicArray<StaffType, Staff> Staff { get => _staffs; set => _staffs = value; }

		[JsonProperty, DataMember, PropertyField]
		public Address Address { get; set; }

		[JsonProperty, DataMember, PropertyField]
		public Dictionary<string, string> DictionaryCollection { get; set; }

		private Dictionary<TestEnumKeys, string> _dictionaryEnumCollection;

		[JsonProperty, DataMember, PropertyField]
		public Dictionary<TestEnumKeys, string> DictionaryEnumCollection
		{
			get => _dictionaryEnumCollection;
			set => _dictionaryEnumCollection = value;
		}
	}

	public enum TestEnumKeys
	{
		ElementA,
		ElementB,
		ElementC,
		ElementD,
		ElementE
	}
}
