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
            var buffer = new byte[1000];
            var rng = new Random();
            rng.NextBytes(buffer);
            using var memoryStream = new MemoryStream(buffer);

            // act
            var successful = await broker.UploadAsync(id, new Progress<TransferStatus>(), memoryStream, CancellationToken.None);

            // assert
            Assert.IsTrue(successful);
        }

        [TestMethod]
        public async Task BlobBroker_CanDownloadFile()
        {
            // arrange
            var broker = new BlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var id = Guid.NewGuid().ToString("n");
            var buffer = new byte[1000];
            var rng = new Random();
            rng.NextBytes(buffer);
            using var uploadStream = new MemoryStream(buffer);
            using var downloadStream = new MemoryStream();

            // act
            var uploadSucessful = await broker.UploadAsync(id, new Progress<TransferStatus>(), uploadStream, CancellationToken.None);
            var downloadSucessful = await broker.DownloadAsync(id, new Progress<TransferStatus>(), uploadStream, CancellationToken.None);

            // assert
            Assert.IsTrue(uploadSucessful);
            Assert.IsTrue(downloadSucessful);

            uploadStream.Position = 0;
            for (var i = 0; i < downloadStream.Length; i++)
            {
                Assert.AreEqual(uploadStream.ReadByte(), downloadStream.ReadByte());
            }
        }
    }
}