namespace Microsoft.MixedReality.CloudLfs.Models
{
    public class TransferStatus
    {
        public long BytesSoFar { get; }

        public long BytesSinceLast { get; }

        public TransferStatus(long bytesSoFar, long bytesSinceLast)
        {
            BytesSoFar = bytesSoFar;
            BytesSinceLast = bytesSinceLast;
        }
    }
}