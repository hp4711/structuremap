using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Diagnostics;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ConfigurationParserBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _log = new GraphLog();
            builder = new ConfigurationParserBuilder(_log);
            DataMother.BackupStructureMapConfig();
        }

        [TearDown]
        public void TearDown()
        {
            DataMother.RestoreStructureMapConfig();
        }

        #endregion

        private ConfigurationParserBuilder builder;
        private GraphLog _log;

        private void assertErrorIsLogged(int errorCode, Action action)
        {
            action();
            builder.GetParsers();
            _log.AssertHasError(errorCode);
        }

        private void assertNoErrorIsLogged(int errorCode, Action action)
        {
            action();
            builder.GetParsers();
            _log.AssertHasNoError(errorCode);
        }

        public void assertParserIdList(params string[] expected)
        {
            Array.Sort(expected);
            ConfigurationParser[] parsers = builder.GetParsers();
            Converter<ConfigurationParser, string> converter = parser => parser.Id;

            string[] actuals = Array.ConvertAll(parsers, converter);
            Array.Sort(actuals);

            Assert.AreEqual(expected, actuals);
        }

        [Test]
        public void Always_resolve_files_to_an_absolute_path_before_giving_to_configuration_parser()
        {
            builder.UseAndEnforceExistenceOfDefaultFile = false;
            builder.IgnoreDefaultFile = true;

            DataMother.WriteDocument("GenericsTesting.xml");

            builder.IncludeFile("GenericsTesting.xml");

            ConfigurationParser[] parsers = builder.GetParsers();
            Assert.IsTrue(Path.IsPathRooted(parsers[0].Description));
        }

        [Test]
        public void Do_NOT_Log_exception_100_if_StructureMap_config_is_missing_but_not_required()
        {
            assertNoErrorIsLogged(100, delegate
            {
                DataMother.RemoveStructureMapConfig();
                builder.UseAndEnforceExistenceOfDefaultFile = false;
            });
        }

        [Test]
        public void DoNotUseDefaultAndUseADifferentFile()
        {
            DataMother.RemoveStructureMapConfig();

            builder.UseAndEnforceExistenceOfDefaultFile = false;
            builder.IgnoreDefaultFile = true;

            DataMother.WriteDocument("GenericsTesting.xml");

            builder.IncludeFile("GenericsTesting.xml");
            assertParserIdList("Generics");
        }

        [Test]
        public void GetIncludes()
        {
            DataMother.RemoveStructureMapConfig();

            DataMother.WriteDocument("Include1.xml");
            DataMother.WriteDocument("Include2.xml");
            DataMother.WriteDocument("Master.xml");

            builder.UseAndEnforceExistenceOfDefaultFile = false;
            builder.IgnoreDefaultFile = true;
            builder.IncludeFile("Master.xml");

            assertParserIdList("Include1", "Include2", "Master");
        }

        [Test]
        public void GetMultiples()
        {
            DataMother.WriteDocument("Include1.xml");
            DataMother.WriteDocument("Include2.xml");
            DataMother.WriteDocument("Master.xml");

            DataMother.WriteDocument("GenericsTesting.xml");

            builder.IncludeFile("GenericsTesting.xml");
            builder.UseAndEnforceExistenceOfDefaultFile = true;
            builder.IncludeFile("Master.xml");

            assertParserIdList("Generics", "Include1", "Include2", "Main", "Master");
        }


        [Test]
        public void Log_error_150_if_a_designated_Include_cannot_be_opened()
        {
            assertErrorIsLogged(150, delegate
            {
                builder.IncludeFile("Master.xml");

                DataMother.WriteDocument("Include1.xml");
                File.Delete("Include2.xml");
                DataMother.WriteDocument("Master.xml");

                builder.IgnoreDefaultFile = true;
            });
        }

        [Test, Explicit]
        public void Log_exception_100_if_StructureMap_config_is_required_and_missing()
        {
            assertErrorIsLogged(100, delegate
            {
                DataMother.RemoveStructureMapConfig();
                builder.UseAndEnforceExistenceOfDefaultFile = true;
            });
        }

        [Test]
        public void Log_exception_160_if_additional_file_cannot_be_opened()
        {
            assertErrorIsLogged(160, delegate { builder.IncludeFile("FileThatDoesNotExist.xml"); });
        }

        [Test]
        public void Log_exception_160_if_file_is_malformed()
        {
            assertErrorIsLogged(160, delegate
            {
                var doc = new XmlDocument();
                doc.LoadXml("<a></a>");
                doc.Save("Malformed.xml");
                builder.IncludeFile("Malformed.xml");
            });
        }

        [Test]
        public void Put_the_file_name_onto_ConfigurationParser()
        {
            builder.UseAndEnforceExistenceOfDefaultFile = false;
            builder.IgnoreDefaultFile = true;

            DataMother.WriteDocument("GenericsTesting.xml");

            builder.IncludeFile("GenericsTesting.xml");

            ConfigurationParser[] parsers = builder.GetParsers();
            Assert.AreEqual("GenericsTesting.xml", Path.GetFileName(parsers[0].Description));
        }


        [Test]
        public void SimpleDefaultConfigurationParser()
        {
            builder.UseAndEnforceExistenceOfDefaultFile = true;
            assertParserIdList("Main");
        }

        [Test]
        public void UseDefaultIsTrueUponConstruction()
        {
            Assert.IsFalse(builder.UseAndEnforceExistenceOfDefaultFile);
        }
    }
}