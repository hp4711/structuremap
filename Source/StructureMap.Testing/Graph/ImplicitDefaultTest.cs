using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ImplicitDefaultTest
    {
        [Test]
        public void CanSetTheDefaultInstanceKeyImplicitlyFromObjectFactory()
        {
            var gateway = ObjectFactory.GetInstance(typeof (IGateway)) as DefaultGateway;
            Assert.IsNotNull(gateway);
        }
    }
}