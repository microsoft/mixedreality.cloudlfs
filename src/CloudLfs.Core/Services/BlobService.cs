using Microsoft.MixedReality.CloudLfs.Brokers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class BlobService : IBlobService
    {
        private IBlobBroker _blobBroker;

        public BlobService(IBlobBroker broker)
        {
            _blobBroker = broker;
        }

        public Task RetrieveBlobAsync(string blobName)
        {
            throw new NotImplementedException();
        }

        public Task StoreBlobAsync(string blobName)
        {
            throw new NotImplementedException();
        }
    }
}
