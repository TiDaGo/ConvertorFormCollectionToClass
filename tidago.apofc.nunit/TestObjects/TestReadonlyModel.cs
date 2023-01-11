using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using tidago.apofc.Attributes;
using tidago.apofc.Helpers;
using tidago.apofc.Interfaces;

namespace tidago.apofc.nunit.TestObjects
{
	[DataContract]
	public class TestReadonlyModel : IDynamicFillModeController
	{

		/// <summary>
		/// Test member collection
		/// </summary>
		[JsonProperty(nameof(Phones)), DataMember(Name = nameof(Phones)), PropertyField(nameof(Phones))]
		protected DynamicArray<PhonesType, Phone> phones;

		/// <summary>
		/// Test member field
		/// </summary>
		[JsonProperty(nameof(SecondaryName)), DataMember(Name = nameof(SecondaryName)), PropertyField(nameof(SecondaryName))]
		protected string secondaryName;

		/// <summary>
		/// Test property collection
		/// </summary>
		[JsonProperty, DataMember, PropertyField]
		public DynamicArray<LocationType, Address> Address { get; set; }

		/// <summary>
		/// Test property field
		/// </summary>
		[JsonProperty, DataMember, PropertyField]
		public string FirstName { get; set; }

		public IReadOnlyCollection<Phone> Phones => phones?.Elements ?? Array.Empty<Phone>();

		[JsonProperty, DataMember, PropertyField]
		public Salary Salary { get; set; }

		public string SecondaryName => secondaryName;

		[JsonProperty, DataMember, PropertyField]
		public WorkInfo WorkInfo { get; set; }

		public void OnBeforeSetPropertyValue(IFormTreeNode node, string propertyName, object value)
		{
			Console.WriteLine("Set value into property");
		}

		public void OnFinishFillModel(string[] affectedProperties)
		{
			Console.WriteLine("Filling model finished");
		}

		public void OnStartFillModel(IEnumerable<IFormTreeNode> nodes)
		{
			Console.WriteLine("Filling model start");
		}
	}
}
