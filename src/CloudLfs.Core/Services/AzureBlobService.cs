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
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            var response = await _blobBroker.DownloadAsync(blobName, progress, contentStream, cancellationToken);
            if (response.IsError)
            {
                throw new InvalidOperationException($"The remote service returned an error, {response.Status}");
            }
        }

        public async Task DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}