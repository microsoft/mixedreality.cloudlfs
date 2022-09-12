using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IGitLfsBroker
    {
        Task<bool> DownloadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream);
    }
}