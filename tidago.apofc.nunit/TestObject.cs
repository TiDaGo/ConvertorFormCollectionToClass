using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using tidago.apofc.Attributes;
using tidago.apofc.Helpers;
using tidago.apofc.Interfaces;

namespace tidago.apofc.nunit {

    public enum LocationType {
        Undefined,
        Home,
        Work,
        SecondaryHome
    }

    public enum PhonesType {
        Undefined,
        Mobile,
        Home,
        Work
    }
    public enum StaffType {
        Undefined,
        Boss,
        Manager,
        SecondaryBoss
    }

    public class Salary {
        
        [PropertyField(nameof(Amount))]
        public decimal Amount { get; set; }
        [PropertyField(nameof(Rate))]
        public string Rate { get; set; }
    }

    public class Address {

        [PropertyField(nameof(City))]
        private string _city;

        [PropertyField(nameof(Street))]
        private string _street;

        [PropertyField(nameof(Type))]
        [KeyPropertyField]
        private string _type;

        public string City => _city ?? string.Empty;
        public string Street => _street ?? string.Empty;
        public LocationType Type => string.IsNullOrWhiteSpace(_type) ? LocationType.Undefined : Enum.Parse<LocationType>(_type);
    }

    public class Phone {

        [PropertyField(nameof(Number))]
        private string _number;

        [PropertyField(nameof(Type))]
        private string _type;

        public string Number {
            get {
                string raw = _number ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    raw = Regex.Replace(raw, "\\D", "");
                }
                return raw;
            }
        }

        [KeyPropertyField]
        public PhonesType Type => string.IsNullOrWhiteSpace(_type) ? PhonesType.Undefined : Enum.Parse<PhonesType>(_type);
    }

    public class WorkInfo {
        [PropertyField(nameof(Staff))]
        private DynamicArray<StaffType, Staff> _staff;

        [PropertyField]
        public Address Address { get; set; }

        public IReadOnlyCollection<Staff> Staff => _staff?.Elements ?? Array.Empty<Staff>();
    }

    public class Staff {
        [KeyPropertyField]
        [PropertyField]
        public StaffType Type { get; set; }

        [PropertyField]
        public string FirstName { get; set; }

        [PropertyField]
        public string SecondaryName { get; set; }

        [PropertyField(nameof(Phones))]
        private DynamicArray<PhonesType, Phone> _phones;

        public IReadOnlyCollection<Phone> Phones => _phones?.Elements ?? Array.Empty<Phone>();
    }

    public class TestReadonlyModel : IDynamicFillModeController {

        /// <summary>
        /// Test member collection
        /// </summary>
        [PropertyField(nameof(Phones))]
        private DynamicArray<PhonesType, Phone> _phones;

        /// <summary>
        /// Test member field
        /// </summary>
        [PropertyField(nameof(SecondaryName))]
        private string _secondaryName;

        /// <summary>
        /// Test property collection
        /// </summary>
        [PropertyField(nameof(Address))]
        public DynamicArray<LocationType, Address> Address { get; set; }

        /// <summary>
        /// Test property field
        /// </summary>
        [PropertyField(nameof(FirstName))]
        public string FirstName { get; set; }

        public IReadOnlyCollection<Phone> Phones => _phones?.Elements ?? Array.Empty<Phone>();
        public string SecondaryName => _secondaryName;

        [PropertyField(nameof(Salary))]
        public Salary Salary { get; set; }

        [PropertyField(nameof(WorkInfo))]
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