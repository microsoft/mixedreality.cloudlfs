namespace Microsoft.MixedReality.CloudLfs.Models
{
    public sealed class UploadObjectLfsGitMessage : GitLfsMessage
    {
        public string ObjectId { get; private set; }

        public long Size { get; private set; }

        public string Path { get; private set; }

        public UploadObjectLfsGitMessage(string objectId, long size, string path)
        {
            ObjectId = objectId;
            Size = size;
            Path = path;
        }
    }
}