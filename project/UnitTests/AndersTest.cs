using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests
{
    [TestFixture]
    public class AndersTest
    {
        [Test]
        public void TestThatMsTestCallHasOutput()
        {
            var mocks = new MockRepository();
            var server = mocks.DynamicMock<ICruiseServer>();
            var manager = new CruiseManager(server);

            manager.Start("ActorTotal");
        }
    }
}
