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
        public async Task GitLfsBroker_CanGetLfsObject()
        {
            // arrange
            var lfsBroker = new GitLfsBroker(new Uri("https://github.com/microsoft/mixedreality.cloudlfs.git/info/lfs"));
            using var contentStream = new MemoryStream();
            const string oid = "c27599cee7e719d9712b958ad29cd8925741688ac4f993a48fc5b73248688195";

            // act
            await lfsBroker.DownloadAsync(oid, 1000, new Progress<TransferStatus>(), contentStream);

            // assert - by convention, object id === sha256 hash
            using var hashAlgo = SHA256.Create();
            contentStream.Position = 0;
            var hash = hashAlgo.ComputeHash(contentStream);
            var strHash = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            Assert.AreEqual(oid, strHash);
        }
    }
}