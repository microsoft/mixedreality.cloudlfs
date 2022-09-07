using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class BlobBroker : IBlobBroker
    {
        private Uri _storageUri;
        private BlobServiceClient _blobServiceClient;

        public BlobBroker(Uri storageUri)
        {
            _storageUri = storageUri;
            _blobServiceClient = new BlobServiceClient(_storageUri, new DefaultAzureCredential());
        }

        public async Task<Response> DownloadAsync(string blobName, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("objects");
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.DownloadToAsync(contentStream, cancellationToken);
        }

        public async Task<Response<BlobContentInfo>> UploadAsync(string blobName, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("objects");
            var blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.UploadAsync(contentStream, true, cancellationToken);
        }
    }
}