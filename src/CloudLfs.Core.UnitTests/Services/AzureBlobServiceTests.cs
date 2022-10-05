using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests.Services
{
    [TestClass]
    public class AzureBlobServiceTests
    {
        private Mock<IBlobBroker> _blobBroker;
        private IBlobService _azureBlobService;

        public AzureBlobServiceTests()
        {
            _azureBlobService = new AzureBlobService();
        }

        [TestMethod]
        public async Task BlobService_DownloadTest()
        {

        }

        [TestMethod]
        public async Task BlobService_UploadTest()
        {

        }
    }
}
