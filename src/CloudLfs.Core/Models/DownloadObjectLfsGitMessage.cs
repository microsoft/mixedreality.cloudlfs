namespace Microsoft.MixedReality.CloudLfs.Models
{
    public sealed class DownloadObjectLfsGitMessage : GitLfsMessage
    {
        public string ObjectId { get; private set; }

        public long Size { get; private set; }

        public DownloadObjectLfsGitMessage(string objectId, long size)
        {
            ObjectId = objectId;
            Size = size;
        }
    }
}