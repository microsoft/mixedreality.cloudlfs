using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class GitBrokerTests
    {
        [TestMethod]
        public async Task GitBroker_CanGetCredentials()
        {
            // arrange
            var dir = Directory.GetCurrentDirectory();
            var broker = new GitBroker(dir);

            // act
            var credentials = await broker.GetCredentials("github.com", "https");

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(credentials.UserName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(credentials.Password));
        }

        [TestMethod]
        public async Task GitBroker_GetLfsEndpoint()
        {
            // arrange
            var dir = Directory.GetCurrentDirectory();
            var broker = new GitBroker(dir);

            // act
            var uri = await broker.GetLfsEndpoint();

            // assert
            Assert.AreEqual("https://github.com/microsoft/mixedreality.cloudlfs.git/info/lfs", uri.ToString());
        }
    }
}