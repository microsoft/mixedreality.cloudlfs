using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class AzureBlobBroker : IBlobBroker
    {
        private Uri _storageUri;

        private BlobServiceClient _blobServiceClient;

        private readonly BlobContainerClient _containerClient;

        public AzureBlobBroker(Uri storageUri)
        {
            _storageUri = storageUri;
            _blobServiceClient = new BlobServiceClient(_storageUri, new DefaultAzureCredential(true));
            _containerClient = _blobServiceClient.GetBlobContainerClient("objects");
        }

        public async Task<Response> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            try
            {
                var blobClient = _containerClient.GetBlockBlobClient(blobName);
                return await blobClient.DownloadToAsync(contentStream, new BlobDownloadToOptions { ProgressHandler = progress }, cancellationToken);
            }
            catch (RequestFailedException rfex)
            {
                return null;
            }
        }

        public async Task<Response> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            var blobClient = _containerClient.GetBlockBlobClient(blobName);
            var result = await blobClient.DownloadStreamingAsync(new HttpRange(startBytes, (endBytes - startBytes)), progressHandler: progress, cancellationToken: cancellationToken);
            await result.Value.Content.CopyToAsync(contentStream);
            return result.GetRawResponse();
        }

        public async Task<Response<BlobContentInfo>> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.UploadAsync(contentStream, new BlobUploadOptions { ProgressHandler = progress }, cancellationToken: cancellationToken);
        }
    }
}