using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Linq;
using tidago.apofc.Helpers;

namespace tidago.apofc.nunit {

    public class Tests {

        private FormCollection collection;

        [SetUp]
        public void Setup()
        {
            collection = new FormCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
            {
                {"TestSystemField", "Value"},
                {"TestSystemCollection.Field1", "Value"},
                {"TestSystemCollection.Field2", "Value"},

                {"FirstName", "Petrov"},
                {"SecondaryName", "Petr" },
                {"Salary.Amount", "10 000" },
                {"Salary.Rate", "Eur" },
                {"Phones.Mobile.Number", "555-55-55" },
                {"Phones.Home.Number", "666-66-66" },
                {"Address.Home.City", "Moscow" },
                {"Address.Home.Street", "Petrova" },
                {"Address.SecondaryHome.Street", "Petrova" },
                {"WorkInfo.Address.City", "Kazan" },
                {"WorkInfo.Address.Street", "Lenina" },
                {"WorkInfo.Staff.Boss.FirstName", "Ivanov" },
                {"WorkInfo.Staff.Boss.SecondaryName", "Ivan" },
                {"WorkInfo.Staff.Boss.Phones.Mobile.Number", "222-22-22" },
                {"WorkInfo.Staff.SecondaryBoss.FirstName", "Sidorova" },
                {"WorkInfo.Staff.SecondaryBoss.SecondaryName", "Anna" },
                {"WorkInfo.Staff.SecondaryBoss.Phones.Undefined.Number", "333-33-33" },
                {"WorkInfo.Staff.SecondaryBoss.Phones.Undefined.Type", "Work" },
                {"WorkInfo.Staff.SecondaryBoss.Phones.Home.Number", "111-11-11" },
            });
        }
        [Test]
        public void TestTreeNodesConvertor()
        {
            var treeCollection = FormTreeCollector.ConvertToTree(collection);
            //{ "TestSystemField", "Value"},
            TestElement(treeCollection, "Value", "TestSystemField");
            //{ "TestSystemCollection.Field1", "Value"},
            TestElement(treeCollection, "Value", "TestSystemCollection", "Field1");
            //{ "TestSystemCollection.Field2", "Value"},
            TestElement(treeCollection, "Value", "TestSystemCollection", "Field2");

            //{"fname", "Petrov"},
            TestElement(treeCollection, "Petrov", "FirstName");
            //{"sname", "Petr" },
            TestElement(treeCollection, "Petr", "SecondaryName");

            //{"salary.amount", "10 000"}
            TestElement(treeCollection, "10 000", "Salary", "Amount");
            //{"salary.rate", "Eur"}
            TestElement(treeCollection, "Eur", "Salary", "Rate");

            //{"Phone.mobile.number", "555-55-55" },
            TestElement(treeCollection, "555-55-55", "Phones", "Mobile", "Number");
            //{"phone.home.number", "666-66-66" },
            TestElement(treeCollection, "666-66-66", "Phones", "Home", "Number");

            //{"address.home.city", "Moscow" },
            TestElement(treeCollection, "Moscow", "Address", "Home", "City");
            //{"address.home.street", "Petrova" },
            TestElement(treeCollection, "Petrova", "Address", "Home", "Street");

            //{"Workinfo.address.city", "Kazan" },
            TestElement(treeCollection, "Kazan", "WorkInfo", "Address", "City");
            //{"Workinfo.address.street", "Lenina" },
            TestElement(treeCollection, "Lenina", "WorkInfo", "Address", "Street");
            //{"Workinfo.staff.boss.fname", "Ivanov" },
            TestElement(treeCollection, "Ivanov", "WorkInfo", "Staff", "Boss", "FirstName");
            //{"Workinfo.staff.boss.sname", "Ivan" },
            TestElement(treeCollection, "Ivan", "WorkInfo", "Staff", "Boss", "SecondaryName");
            //{"Workinfo.staff.boss.phone.mobile.number", "222-22-22" },
            TestElement(treeCollection, "222-22-22", "WorkInfo", "Staff", "Boss", "Phones", "Mobile", "Number");
            
            //{"Workinfo.staff.SecondaryBoss.fname", "Sidorova" },
            TestElement(treeCollection, "Sidorova", "WorkInfo", "Staff", "SecondaryBoss", "FirstName");
            //{"Workinfo.staff.SecondaryBoss.sname", "Anna" },
            TestElement(treeCollection, "Anna", "WorkInfo", "Staff", "SecondaryBoss", "SecondaryName");
            //{"Workinfo.staff.SecondaryBoss.phone.mobile.number", "333-33-33" },
            TestElement(treeCollection, "333-33-33", "WorkInfo", "Staff", "SecondaryBoss", "Phones", "Undefined", "Number");
            //{"WorkInfo.Staff.SecondaryBoss.Phones.Undefined.Type", "Work" },
            TestElement(treeCollection, "Work", "WorkInfo", "Staff", "SecondaryBoss", "Phones", "Undefined", "Type");
            //{"Workinfo.staff.SecondaryBoss.phone.home.number", "111-11-11" },
            TestElement(treeCollection, "111-11-11", "WorkInfo", "Staff", "SecondaryBoss", "Phones", "Home", "Number");
        }

        [Test]
        public void TestTreeNodesConvertToObject()
        {
            var resultObject = new TestReadonlyModel();
            new ObjectPopulator().Populate(collection, resultObject);

            Assert.AreEqual(resultObject.FirstName, "Petrov");
            Assert.AreEqual(resultObject.SecondaryName, "Petr");
            Assert.IsNotNull(resultObject.Salary);
            Assert.AreEqual(resultObject.Salary.Amount, 10000m);
            Assert.AreEqual(resultObject.Salary.Rate, "Eur");

            var mobilePhone = resultObject.Phones.FirstOrDefault(x => x.Type == PhonesType.Mobile);
            Assert.NotNull(mobilePhone);
            Assert.AreEqual(mobilePhone.Number, "5555555");

            var homePhone = resultObject.Phones.FirstOrDefault(x => x.Type == PhonesType.Home);
            Assert.NotNull(homePhone);
            Assert.AreEqual(homePhone.Number, "6666666");

            var homeAddress = resultObject.Address.FirstOrDefault(x => x.Type == LocationType.Home);
            Assert.NotNull(homeAddress);
            Assert.AreEqual(homeAddress.City, "Moscow");
            Assert.AreEqual(homeAddress.Street, "Petrova");

            Assert.NotNull(resultObject.WorkInfo);
            Assert.NotNull(resultObject.WorkInfo.Address);
            Assert.AreEqual(resultObject.WorkInfo.Address.City, "Kazan");

            Assert.NotNull(resultObject.WorkInfo.Address);
            Assert.AreEqual(resultObject.WorkInfo.Address.Street, "Lenina");

            Assert.NotNull(resultObject.WorkInfo.Staff);
            var staffBoss = resultObject.WorkInfo.Staff.FirstOrDefault(x => x.Type == StaffType.Boss);
            Assert.NotNull(staffBoss);
            Assert.AreEqual(staffBoss.FirstName, "Ivanov");
            Assert.AreEqual(staffBoss.SecondaryName, "Ivan");
            Assert.NotNull(staffBoss.Phones);
            var staffBossMobile = staffBoss.Phones.FirstOrDefault(x => x.Type == PhonesType.Mobile);
            Assert.NotNull(staffBossMobile);
            Assert.AreEqual(staffBossMobile.Number, "2222222");

            var secStaffBoss = resultObject.WorkInfo.Staff.FirstOrDefault(x => x.Type == StaffType.SecondaryBoss);
            Assert.NotNull(secStaffBoss);
            Assert.AreEqual(secStaffBoss.FirstName, "Sidorova");
            Assert.AreEqual(secStaffBoss.SecondaryName, "Anna");
            Assert.NotNull(secStaffBoss.Phones);
            var secStaffBossMobile = secStaffBoss.Phones.FirstOrDefault(x => x.Type == PhonesType.Work);
            Assert.NotNull(secStaffBossMobile);
            Assert.AreEqual(secStaffBossMobile.Number, "3333333");

            var secStaffBossHome = secStaffBoss.Phones.FirstOrDefault(x => x.Type == PhonesType.Home);
            Assert.NotNull(secStaffBossHome);
            Assert.AreEqual(secStaffBossHome.Number, "1111111");

            Assert.NotNull(resultObject.WorkInfo.Address);
        }

        private void TestElement(IFormTreeNode[] nodes, string value, params string[] path)
        {
            var collection_path = path.Reverse().Skip(1).Reverse();
            IFormTreeNode currentElement;
            foreach (var childName in collection_path)
            {
                currentElement = nodes.FirstOrDefault(x => string.Equals(x.Key, childName, System.StringComparison.InvariantCulture)) as FormTreeCollection;
                Assert.IsNotNull(currentElement);
                nodes = ((FormTreeCollection)currentElement).Childs;
            }

            var element = nodes.FirstOrDefault(x => string.Equals(x.Key, path.Last(), System.StringComparison.InvariantCulture)) as FormTreeNode;
            Assert.IsNotNull(element);
            Assert.AreEqual(element.Value, value);
        }
    }
}