using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class StructureMapTests
    {
        [Test]
        public void Test()
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = false;

                x.ForConcreteType<SomeDbRepository>().Configure.
                    WithCtorArg("connectionString").EqualTo("some connection string");

                //x.ForConcreteType<SomeWebPage>().Configure.
                //  SetterDependency<SomeDbRepository>().Is<SomeDbRepository>();

                x.SetAllProperties(o => o.OfType<SomeDbRepository>());
            });

            var dbRepository =
                ObjectFactory.GetInstance<SomeDbRepository>();

            var webPage = new SomeWebPage();

            ObjectFactory.BuildUp(webPage);

            webPage.DbRepository.ConnectionString.ShouldEqual("some connection string");
        }
    }

    public class SomeDbRepository
    {
        public SomeDbRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        // ...
    }

    public class SomeWebPage
    {
        public SomeDbRepository DbRepository { get; set; }

        // ...
    }
}