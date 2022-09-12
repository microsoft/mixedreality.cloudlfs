namespace Microsoft.MixedReality.CloudLfs.Models
{
    public class InitializeTransferGitLfsMessage : GitLfsMessage
    {
        public string Operation { get; set; } = null!;

        public string Remote { get; set; } = null!;

        public bool Concurrent { get; set; }

        public int ConcurrentTransfers { get; set; }
    }
}