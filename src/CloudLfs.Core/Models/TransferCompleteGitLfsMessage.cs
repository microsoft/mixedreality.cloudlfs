namespace Microsoft.MixedReality.CloudLfs.Models
{
    public class TransferCompleteGitLfsMessage : GitLfsMessage
    {
        public TransferCompleteGitLfsMessage(string objectId, string path)
        {
            ObjectId = objectId;
            Path = path;
        }

        public string ObjectId { get; }

        public string Path { get; }
    }
}