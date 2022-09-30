using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.CloudLfs.Models.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class AzureBlobBroker : IBlobBroker
    {
        private Uri _storageUri;

        private BlobServiceClient _blobServiceClient;

        private readonly BlobContainerClient _containerClient;
        
        private readonly TelemetryClient _telemetryClient;

        public AzureBlobBroker(TelemetryClient telmetryClient, Uri storageUri)
        {
            _telemetryClient = telmetryClient;
            _storageUri = storageUri;
            _blobServiceClient = new BlobServiceClient(_storageUri, new DefaultAzureCredential(true));
            _containerClient = _blobServiceClient.GetBlobContainerClient("objects");
        }

        public async Task<Response> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                var blobClient = _containerClient.GetBlockBlobClient(blobName);
                return await blobClient.DownloadToAsync(contentStream, new BlobDownloadToOptions { ProgressHandler = progress }, cancellationToken);
            }
            catch (RequestFailedException rfex)
            {
                _telemetryClient.TrackException(rfex);
                return null;
            }
            finally
            {
                stopwatch.Stop();
                var azureBlobEvent = new AzureBlobEvent()
                {
                    ElapsedTimeInSeconds = stopwatch.ElapsedMilliseconds,
                    BlobName = blobName,
                    BlobSize = contentStream.Length,
                    OperationType = "Download"
                };

                _telemetryClient.TrackEvent(JsonConvert.SerializeObject(azureBlobEvent));
            }
        }

        public async Task<Response> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                var blobClient = _containerClient.GetBlockBlobClient(blobName);
                var result = await blobClient.DownloadStreamingAsync(new HttpRange(startBytes, (endBytes - startBytes)), progressHandler: progress, cancellationToken: cancellationToken);
                await result.Value.Content.CopyToAsync(contentStream);
                return result.GetRawResponse();
            } 
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                return null;
            }
            finally
            {
                stopwatch.Stop();
                var operation = new AzureBlobEvent()
                {
                    ElapsedTimeInSeconds = stopwatch.ElapsedMilliseconds,
                    BlobName = blobName,
                    BlobSize = contentStream.Length,
                    OperationType = "Download"
                };
                _telemetryClient.TrackEvent(JsonConvert.SerializeObject(operation));
            }
        }

        public async Task<Response<BlobContentInfo>> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                return await blobClient.UploadAsync(contentStream, new BlobUploadOptions { ProgressHandler = progress }, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                return null;
            }
            finally
            {
                stopwatch.Stop();
                var operation = new AzureBlobEvent()
                {
                    ElapsedTimeInSeconds = stopwatch.ElapsedMilliseconds,
                    BlobName = blobName,
                    BlobSize = contentStream.Length,
                    OperationType = "Upload"
                };
                _telemetryClient.TrackEvent(JsonConvert.SerializeObject(operation));
            }
        }
    }
}