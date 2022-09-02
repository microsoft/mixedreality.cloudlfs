using Azure.Identity;
using Azure.Storage.Blobs;
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

        public async Task<bool> DownloadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("objects");
            BlobClient blobClient = containerClient.GetBlobClient(id);
            await blobClient.DownloadToAsync(contentStream, cancellationToken);
            return true;
        }

        public async Task<bool> UploadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("objects");
            var blobClient = containerClient.GetBlobClient(id);
            await blobClient.UploadAsync(contentStream, true, cancellationToken);
            return true;
        }
    }
}