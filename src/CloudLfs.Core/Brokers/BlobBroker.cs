using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    internal class BlobBroker : IBlobBroker
    {
        public Task<bool> DownloadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream)
        {
            throw new NotImplementedException();
        }
    }
}
