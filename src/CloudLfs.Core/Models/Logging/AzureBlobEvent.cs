using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.CloudLfs.Models.Logging
{
    public class AzureBlobEvent
    {
        public long ElapsedTimeInSeconds { get; set; }
        public string BlobName { get; set; }
        public long BlobSize { get; set; }
        public string OperationType { get; set; }
    }
}
