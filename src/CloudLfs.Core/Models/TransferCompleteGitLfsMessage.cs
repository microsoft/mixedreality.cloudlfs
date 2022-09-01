namespace Microsoft.MixedReality.CloudLfs.Models
{
    public class TransferCompleteGitLfsMessage : GitLfsMessage
    {
        public TransferCompleteGitLfsMessage(string objectId)
        {
            ObjectId = objectId;
        }

        public string ObjectId { get; }
    }
}