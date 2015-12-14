using System;
using System.Xml;
using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class AlternativeConfigurationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.BackupStructureMapConfig();

            DataMother.WriteDocument("Config1.xml");
            DataMother.WriteDocument("Config2.xml");
            DataMother.WriteDocument("FullTesting.XML");
        }

        [TearDown]
        public void TearDown()
        {
            DataMother.RestoreStructureMapConfig();
        }

        #endregion

        public void assertTheDefault(string color, Action<ConfigurationExpression> action)
        {
            var container = new Container(action);

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual(color);
        }

        [Test]
        public void AddNodeDirectly()
        {
            string xml = "<StructureMap><Assembly Name=\"StructureMap.Testing.GenericWidgets\"/></StructureMap>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);


            var container = new Container(x =>
            {
                x.AddConfigurationFromXmlFile("StructureMap.config");
                x.AddConfigurationFromNode(doc.DocumentElement);
            });

            container.GetInstance<IPlug<string>>().ShouldNotBeNull();
        }

        [Test]
        public void Load_configuration_file_after_the_container_has_already_been_initialized()
        {
            var container = new Container(x => x.AddConfigurationFromXmlFile("Config1.xml"));

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Orange");
            container.Configure(x => x.AddConfigurationFromXmlFile("Config2.xml"));
            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Green");
        }

        [Test]
        public void NotTheDefault()
        {
            assertTheDefault("Orange", x => { x.AddConfigurationFromXmlFile("Config1.xml"); });
        }

        [Test]
        public void WithTheDefault()
        {
            // This code enforces the existence of the StructureMap.config file
            // Initialize() will throw an exception if the StructureMap.config file
            // cannot be found
            ObjectFactory.Initialize(x => { x.UseDefaultStructureMapConfigFile = true; });

            ObjectFactory.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }
    }
}