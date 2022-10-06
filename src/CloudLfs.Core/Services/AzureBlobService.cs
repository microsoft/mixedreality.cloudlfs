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

        public async Task<AzureBlobServiceResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AzureBlobServiceResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AzureBlobServiceResultCode> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
