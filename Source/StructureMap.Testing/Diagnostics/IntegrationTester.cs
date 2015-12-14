using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class IntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Smoke_test_error_report_on_InstanceManager()
        {
            PluginGraph graph = DataMother.GetDiagnosticPluginGraph("Invalid.config");
        }
    }
}