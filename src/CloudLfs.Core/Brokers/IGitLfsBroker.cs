using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IGitLfsBroker
    {
        Task<bool> DownloadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken);
        
        Task<bool> UploadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken);
    }
}