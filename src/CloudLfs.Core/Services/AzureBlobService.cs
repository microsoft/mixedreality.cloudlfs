using Azure;
using Microsoft.ApplicationInsights;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class AzureBlobService : IBlobService
    {
        private TelemetryClient _telemetryClient;
        private IBlobBroker _blobBroker;

        public AzureBlobService(TelemetryClient telemetryClient, IBlobBroker blobBroker)
        {
            _telemetryClient = telemetryClient;
            _blobBroker = blobBroker;
        }

        public async Task DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            ValidateBlobName(blobName);

            var response = await _blobBroker.DownloadAsync(blobName, progress, contentStream, cancellationToken);
            
            ValidateResponse(response);
        }

        public async Task DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            ValidateBlobName(blobName);
            
            if (startBytes > endBytes || startBytes < 0 || endBytes < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(startBytes)} should be greater than zero and less than {nameof(endBytes)}. {nameof(endBytes)} should be greater than zero");
            }

            var response = await _blobBroker.DownloadAsync(blobName, progress, contentStream, startBytes, endBytes, cancellationToken);
            ValidateResponse(response);
        }

        public async Task UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            ValidateBlobName(blobName);

            var response = await this._blobBroker.UploadAsync(
                id: blobName,
                progress: progress,
                contentStream: contentStream,
                cancellationToken: cancellationToken);

            ValidateResponse(response.GetRawResponse());
        }

        public void ValidateBlobName(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }
        }

        public void ValidateResponse(Response response)
        {
            if (response.IsError)
            {
                throw new InvalidOperationException($"The remote service returned an error, {response.Status}");
            }
        }
    }
}