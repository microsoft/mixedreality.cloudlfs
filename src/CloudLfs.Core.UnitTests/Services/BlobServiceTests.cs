using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.MixedReality.CloudLfs.Models;
using System.IO;
using System.Threading;

namespace CloudLfs.Core.UnitTests.Services
{
    [TestClass]
    public class BlobServiceTests
    {

        public BlobServiceTests()
        {

        }

        [TestMethod]
        public async Task BlobService_DownloadFile()
        {
            // arrange

            // act

            // assert
        }

        [TestMethod]
        public async Task BlobService_UploadFile()
        {
            // arrange
            var blobBroker = new Mock<IBlobBroker>();
            var blobService = new BlobService(blobBroker.Object);
            blobBroker.Setup(
                broker =>
                    broker.UploadAsync(It.IsAny<string>(), It.IsAny<Progress<TransferStatus>>(), It.IsAny<Stream>(), CancellationToken.None));
            // act

            // assert
        }
    }
}
