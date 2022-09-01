namespace Microsoft.MixedReality.CloudLfs.Models
{
    public sealed class TransferProgressGitMessage : GitLfsMessage
    {
        public string ObjectId { get; }

        public long BytesSoFar { get; }

        public long BytesSinceLast { get; }

        public TransferProgressGitMessage(string objectId, long bytesSoFar, long bytesSinceLast)
        {
            ObjectId = objectId;
            BytesSoFar = bytesSoFar;
            BytesSinceLast = bytesSinceLast;
        }
    }
}