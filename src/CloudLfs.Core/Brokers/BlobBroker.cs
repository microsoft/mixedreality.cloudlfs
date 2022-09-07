using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IBlobService
    {
    }

    public class BlobService : IBlobService
    {
        private readonly IBlobBroker _broker;

        /// <summary>
        /// The chunk size, in bytes.
        /// </summary>
        private const int DefaultChunkSize = 1000000;

        private const int MaxParallelism = 4;

        public BlobService(IBlobBroker broker)
        {
            _broker = broker;
            ServicePointManager.DefaultConnectionLimit = Math.Max(64, ServicePointManager.DefaultConnectionLimit);
        }

        public async Task UploadAsync(string blobName, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            // shard out the incoming stream
            var chunkSize = DefaultChunkSize;
            var chunkCount = contentStream.Length / chunkSize;
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(MaxParallelism, MaxParallelism);
            for (var i = 0; i < chunkCount; i++)
            {
                tasks.Add(UploadChunkAsync(contentStream, chunkSize, i, semaphore, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        private async Task UploadChunkAsync(Stream contentStream, int chunkSize, int chunkIndex, SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            try
            {
                semaphore.Wait();
                var buffer = new byte[chunkSize];
                await contentStream.ReadAsync(buffer, chunkIndex * chunkSize, chunkSize);
                var chunkStream = new MemoryStream(buffer);
                var hashAlgo = SHA256.Create();
                var hash = hashAlgo.ComputeHash(buffer);
                var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);
                await _broker.UploadAsync(hashString, new Progress<TransferStatus>(), chunkStream, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

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