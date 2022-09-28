using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class GitLfsBrokerTests
    {
        [TestMethod]
        public async Task GitLfsBroker_CanGetLfsObject222()
        {
            // authenticate
            var authBroker = new GitBroker("D:\\Repos\\mixedreality.platform.v3");
            var credentials = await authBroker.GetCredentials("microsoft.visualstudio.com", "https");

            // arrange
            var lfsBroker = new GitLfsBroker(new Uri("https://microsoft.visualstudio.com/DefaultCollection/Analog/_git/mixedreality.platform.v3.git/info/lfs"), credentials);
            using var contentStream = new MemoryStream();
            const string oid = "c27599cee7e719d9712b958ad29cd8925741688ac4f993a48fc5b73248688195";

            // act
            await lfsBroker.DownloadAsync(oid, 1000, new Progress<TransferStatus>(), contentStream);

            // assert - by convention, object id === sha256 hash
            var strHash = ComputeHash(contentStream);
            Assert.AreEqual(oid, strHash);
        }

        [TestMethod]
        public async Task GitLfsBroker_CanGetLfsObject()
        {
            // authenticate
            var authBroker = new GitBroker(Directory.GetCurrentDirectory());
            var credentials = await authBroker.GetCredentials("github.com", "https");

            // arrange
            var lfsBroker = new GitLfsBroker(new Uri("https://github.com/microsoft/mixedreality.cloudlfs.git/info/lfs"), credentials);
            using var contentStream = new MemoryStream();
            const string oid = "c27599cee7e719d9712b958ad29cd8925741688ac4f993a48fc5b73248688195";

            // act
            await lfsBroker.DownloadAsync(oid, 1000, new Progress<TransferStatus>(), contentStream);

            // assert - by convention, object id === sha256 hash
            var strHash = ComputeHash(contentStream);
            Assert.AreEqual(oid, strHash);
        }

        [TestMethod]
        public async Task GitLfsBroker_CanStoreLfsObject()
        {
            // authenticate
            var authBroker = new GitBroker(Directory.GetCurrentDirectory());
            var credentials = await authBroker.GetCredentials("github.com", "https");

            // arrange
            var rng = new Random();
            var lfsBroker = new GitLfsBroker(new Uri("https://github.com/microsoft/mixedreality.cloudlfs.git/info/lfs"), credentials);
            var bytes = new byte[1000];
            rng.NextBytes(bytes);
            using var contentStream = new MemoryStream(bytes);
            var oid = ComputeHash(contentStream);
            contentStream.Position = 0;

            // act
            await lfsBroker.UploadAsync(oid, 1000, new Progress<TransferStatus>(), contentStream);

            // assert - by convention, object id === sha256 hash
            using var hashAlgo = SHA256.Create();
            contentStream.Position = 0;
            var hash = hashAlgo.ComputeHash(contentStream);
            var strHash = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            Assert.AreEqual(oid, strHash);
        }

        private string ComputeHash(Stream contentStream)
        {
            var hashAlgo = SHA256.Create();
            contentStream.Position = 0;
            var hash = hashAlgo.ComputeHash(contentStream);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}