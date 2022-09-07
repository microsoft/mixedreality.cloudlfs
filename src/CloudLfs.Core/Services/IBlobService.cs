using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    internal interface IBlobService
    {
        public Task RetrieveBlobAsync(string blobName);
        public Task StoreBlobAsync(string blobName);
    }
}
