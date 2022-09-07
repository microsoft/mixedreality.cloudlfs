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
    public class BlobServiceTests
    {
        [TestMethod]
        public async Task BlobService_CanUploadFile()
        {
            // arrange
            var broker = new BlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var service = new BlobService(broker);
            var id = Guid.NewGuid().ToString("n");
            var buffer = new byte[100000000];
            var rng = new Random();
            rng.NextBytes(buffer);
            using var memoryStream = new MemoryStream(buffer);

            // act
            await service.UploadAsync(id, new Progress<TransferStatus>(), memoryStream, CancellationToken.None);
        }
    }
}