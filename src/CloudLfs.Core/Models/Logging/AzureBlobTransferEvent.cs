using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.CloudLfs.Models.Logging
{
    public class AzureBlobTransferEvent
    {
        public long ElapsedTimeMS { get; set; }

        public string BlobName { get; set; }

        public long BlobSize { get; set; }

        public string OperationType { get; set; }

        public IDictionary<string, double> ToMetrics()
        {
            return new Dictionary<string, double>()
            {
                { nameof(ElapsedTimeMS), ElapsedTimeMS },
                { nameof(BlobSize), BlobSize },
            };
        }

        public IDictionary<string, string> ToProperties()
        {
            return new Dictionary<string, string>()
            {
                {nameof(BlobName), BlobName  },
                {nameof(OperationType), OperationType  },
           };
        }
    }
}