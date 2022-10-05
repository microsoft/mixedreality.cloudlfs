using Azure;
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
        public async Task<CloudLfsResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CloudLfsResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CloudLfsResultCode> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
