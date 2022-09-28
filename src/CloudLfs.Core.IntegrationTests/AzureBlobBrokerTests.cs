using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class AzureBlobBrokerTests
    {
        [TestMethod]
        public async Task BlobBroker_CanUploadAndDownloadFile()
        {
            // arrange
            var broker = new AzureBlobBroker(new Uri("https://cloudlfswus2premiumint.blob.core.windows.net/"));
            for (var i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid().ToString("n");
                var buffer = new byte[100 * 1000 * 1024];
                var rng = new Random();
                rng.NextBytes(buffer);
                using var uploadStream = new MemoryStream(buffer);
                using var downloadStream = new MemoryStream();

                // act -- upload
                var stopwatch = Stopwatch.StartNew();
                var uploadResponse = await broker.UploadAsync(id, new Progress<long>(), uploadStream, CancellationToken.None);
                Console.WriteLine($"upload {i}: {(buffer.Length / 1000000f) / stopwatch.Elapsed.TotalSeconds}");

                // act -- download
                stopwatch.Restart();
                var downloadResponse = await broker.DownloadAsync(id, new Progress<long>(), downloadStream, CancellationToken.None);
                Console.WriteLine($"download {i}: {(buffer.Length / 1000000f) / stopwatch.Elapsed.TotalSeconds}");

                // assert
                var uploadResponseStatus = uploadResponse.GetRawResponse();
                Assert.IsFalse(uploadResponseStatus.IsError);
                var downloadResponseStatus = downloadResponse.IsError;
                Assert.IsFalse(downloadResponseStatus);
            }
        }

        [TestMethod]
        public async Task BlobBroker_CanUploadFile()
        {
            // arrange
            var broker = new AzureBlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var id = Guid.NewGuid().ToString("n");
            var buffer = new byte[1000];
            var rng = new Random();
            rng.NextBytes(buffer);
            using var memoryStream = new MemoryStream(buffer);

            // act
            var response = await broker.UploadAsync(id, new Progress<long>(), memoryStream, CancellationToken.None);

            // assert
            var responseStatus = response.GetRawResponse();
            Assert.IsFalse(responseStatus.IsError);
            // we can also check status code
        }

        [TestMethod]
        public async Task BlobBroker_CanDownloadFile()
        {
            // arrange
            var broker = new AzureBlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var id = Guid.NewGuid().ToString("n");
            var buffer = new byte[1000];
            var rng = new Random();
            rng.NextBytes(buffer);
            using var uploadStream = new MemoryStream(buffer);
            using var downloadStream = new MemoryStream();

            // act
            var uploadSucessful = await broker.UploadAsync(id, new Progress<long>(), uploadStream, CancellationToken.None);
            var downloadSucessful = await broker.DownloadAsync(id, new Progress<long>(), downloadStream, CancellationToken.None);

            // assert
            uploadStream.Position = 0;
            downloadStream.Position = 0;
            for (var i = 0; i < downloadStream.Length; i++)
            {
                Assert.AreEqual(uploadStream.ReadByte(), downloadStream.ReadByte());
            }
        }

        [TestMethod]
        public async Task BlobBroker_CanDownloadFile2()
        {
            // arrange
            var broker = new AzureBlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            var downloadStream = new MemoryStream();
            var response = await broker.DownloadAsync("100MB.bin", new Progress<long>(), downloadStream, CancellationToken.None);
            Assert.IsTrue(response.Status <= 300);
            Assert.IsTrue(downloadStream.Length > 90000000);

            var watch = Stopwatch.StartNew();
            var response2 = await broker.DownloadAsync("100MB.bin", new Progress<long>(), downloadStream, CancellationToken.None);
            var elapsed = watch.Elapsed.TotalSeconds;
            Console.WriteLine(elapsed);
        }
    }
}