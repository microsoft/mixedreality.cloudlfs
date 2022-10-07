using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests.Services
{
    [TestClass]
    public class AzureBlobServiceTests
    {
        private Mock<IBlobBroker> _blobBroker;
        private IBlobService _azureBlobService;
        private TelemetryClient _telemetryClient;
        private byte[] _content;

        public AzureBlobServiceTests()
        {
            _content = new byte[16];
            _telemetryClient = new TelemetryClient(new TelemetryConfiguration());
            _blobBroker = new Mock<IBlobBroker>();
            _azureBlobService = new AzureBlobService(_telemetryClient, _blobBroker.Object);
            // init random content of bytes
            new Random().NextBytes(_content);
        }

        [TestMethod]
        public async Task BlobService_SuccessfulDownloadTest()
        {
            // arrange
            IProgress<long> progress = new Progress<long>();
            MemoryStream expectedStream = new(_content);
            MemoryStream contentStream = new();
            var response = new Mock<Response>();
            response.Setup(res => res.ContentStream).Returns(expectedStream);
            response.SetupGet(res => res.IsError).Returns(false);
            response.SetupGet(res => res.Status).Returns(200);

            _blobBroker
                .Setup(broker =>
                    broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(response.Object).AsTask())
                .Callback<string, IProgress<long>, Stream, CancellationToken>(async (blob, prog, stream, ct) =>
                    await expectedStream.CopyToAsync(contentStream, ct));

            // act
            await _azureBlobService.DownloadAsync("test-blob", progress, contentStream, CancellationToken.None);

            // assert
            Assert.AreEqual(BitConverter.ToString(expectedStream.ToArray()), BitConverter.ToString(contentStream.ToArray()));

            _blobBroker.Verify(broker =>
                broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [TestMethod]
        public async Task BlobService_SuccessfulDownloadTest_WithRange()
        {
            // arrange
            IProgress<long> progress = new Progress<long>();
            MemoryStream expectedStream = new(_content.AsSpan(2, 4).ToArray());
            MemoryStream contentStream = new();
            var response = new Mock<Response>();
            response.Setup(res => res.ContentStream).Returns(expectedStream);
            response.SetupGet(res => res.IsError).Returns(false);
            response.SetupGet(res => res.Status).Returns(200);

            _blobBroker
                .Setup(broker =>
                    broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(response.Object).AsTask())
                .Callback<string, IProgress<long>, Stream, long, long, CancellationToken>(async (blob, prog, stream, startBytes, endBytes, ct) =>
                    await expectedStream.CopyToAsync(contentStream, ct));

            // act
            await _azureBlobService.DownloadAsync("test-blob", progress, contentStream, 2, 6, CancellationToken.None);

            // assert
            Assert.AreEqual(BitConverter.ToString(expectedStream.ToArray()),BitConverter.ToString(contentStream.ToArray()));
            Assert.AreEqual(4, contentStream.Length);

            _blobBroker.Verify(broker =>
                broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [TestMethod]
        public async Task BlobService_SuccessfulUploadTest()
        {
            // arrange
            IProgress<long> progress = new Progress<long>();
            MemoryStream contentStream = new(_content);
            var response = new Mock<Response<BlobContentInfo>>();
            response.SetupGet(res => res.GetRawResponse().IsError).Returns(false);
            response.SetupGet(res => res.GetRawResponse().Status).Returns(200);

            _blobBroker
                .Setup(broker =>
                    broker.UploadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(response.Object);

            // act
            await _azureBlobService.UploadAsync("test-blob", progress, contentStream, CancellationToken.None);

            // assert

            _blobBroker.Verify(broker =>
                broker.UploadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public async Task BlobService_RequestFailureDownloadTest()
        {
            // arrange
            IProgress<long> progress = new Progress<long>();
            MemoryStream expectedStream = new(_content);
            MemoryStream contentStream = new();
            var response = new Mock<Response>();
            response.Setup(res => res.ContentStream).Returns(expectedStream);
            response.SetupGet(res => res.IsError).Returns(true);
            response.SetupGet(res => res.Status).Returns(503);

            _blobBroker
                .Setup(broker =>
                    broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(response.Object).AsTask())
                .Callback<string, IProgress<long>, Stream, CancellationToken>(async (blob, prog, stream, ct) =>
                    await expectedStream.CopyToAsync(contentStream, ct));

            // act
            await _azureBlobService.DownloadAsync("test-blob", progress, contentStream, CancellationToken.None);

            // assert
            Assert.AreEqual(BitConverter.ToString(expectedStream.ToArray()), BitConverter.ToString(contentStream.ToArray()));

            _blobBroker.Verify(broker =>
                broker.DownloadAsync(It.IsAny<string>(), It.IsAny<IProgress<long>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [TestMethod]
        public async Task BlobService_RequestFailureUploadTest()
        {
            // arrange

            // act

            // assert
        }
    }
}