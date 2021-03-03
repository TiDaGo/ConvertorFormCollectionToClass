using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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

            CheckTestReadonlyModel(resultObject);
        }

        [Test]
        public void TestJsonSerialize()
        {
            var resultObject = new TestReadonlyModel();
            new ObjectPopulator().Populate(collection, resultObject);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(resultObject);

            var afterJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<TestReadonlyModel>(json);

            CheckTestReadonlyModel(afterJsonObject);
        }

        [Test]
        public void TestXmlSerialize()
        {
            var resultObject = new TestReadonlyModel();
            new ObjectPopulator().Populate(collection, resultObject);

            string xml;
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader stream = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(resultObject.GetType());
                serializer.WriteObject(memoryStream, resultObject);
                memoryStream.Position = 0;
                xml = stream.ReadToEnd();
            }

            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(TestReadonlyModel));
                var afterXmlObject = deserializer.ReadObject(stream) as TestReadonlyModel;

                CheckTestReadonlyModel(afterXmlObject);
            }
        }

        private void CheckTestReadonlyModel(TestReadonlyModel model)
        {
            // check simple property in main model
            Assert.AreEqual(model.FirstName, "Petrov");
            Assert.AreEqual(model.SecondaryName, "Petr");

            // check included class to mail model
            Assert.IsNotNull(model.Salary);
            // check simple property in included model
            Assert.AreEqual(model.Salary.Rate, "Eur");
            // check field of different type for property
            Assert.AreEqual(model.Salary.Amount, 10000m);

            // check dynamic array
            Assert.NotNull(model.Phones);
            var mobilePhone = model.Phones.FirstOrDefault(x => x.Type == PhonesType.Mobile);
            // check element at dynamic array
            Assert.NotNull(mobilePhone);
            // check modified property from field
            Assert.AreEqual(mobilePhone.Number, "5555555");

            // repeat test with other item at phones dynamic array
            var homePhone = model.Phones.FirstOrDefault(x => x.Type == PhonesType.Home);
            Assert.NotNull(homePhone);
            Assert.AreEqual(homePhone.Number, "6666666");

            // repeat test with other model
            var homeAddress = model.Address.FirstOrDefault(x => x.Type == LocationType.Home);
            Assert.NotNull(homeAddress);
            Assert.AreEqual(homeAddress.City, "Moscow");
            Assert.AreEqual(homeAddress.Street, "Petrova");

            // repeat test with other item
            Assert.NotNull(model.WorkInfo);
            Assert.NotNull(model.WorkInfo.Address);
            Assert.AreEqual(model.WorkInfo.Address.City, "Kazan");
            Assert.AreEqual(model.WorkInfo.Address.Street, "Lenina");

            // Test with many attachments
            Assert.NotNull(model.WorkInfo.Staff);
            var staffBoss = model.WorkInfo.Staff.FirstOrDefault(x => x.Type == StaffType.Boss);
            Assert.NotNull(staffBoss);
            Assert.AreEqual(staffBoss.FirstName, "Ivanov");
            Assert.AreEqual(staffBoss.SecondaryName, "Ivan");
            Assert.NotNull(staffBoss.Phones);
            var staffBossMobile = staffBoss.Phones.FirstOrDefault(x => x.Type == PhonesType.Mobile);
            Assert.NotNull(staffBossMobile);
            Assert.AreEqual(staffBossMobile.Number, "2222222");

            var secStaffBoss = model.WorkInfo.Staff.FirstOrDefault(x => x.Type == StaffType.SecondaryBoss);
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

            Assert.NotNull(model.WorkInfo.Address);
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