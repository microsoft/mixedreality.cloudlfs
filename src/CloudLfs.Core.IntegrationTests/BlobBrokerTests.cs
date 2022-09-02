using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class BlobBrokerTests
    {
        [TestMethod]
        public async Task BlobBroker_CanUploadFile()
        {
            // arrange
            var broker = new BlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var id = Guid.NewGuid().ToString("n");
            var buffer = new byte[1000000];
            var rng = new Random();
            rng.NextBytes(buffer);
            var memoryStream = new MemoryStream(buffer);

            // act
            var successful = await broker.UploadAsync(id, new Progress<TransferStatus>(), memoryStream, CancellationToken.None);

            // assert
            Assert.IsTrue(successful);
        }
    }
}