namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class InitializeTransferGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "init";

        public string Operation { get; set; } = null!;

        public string Remote { get; set; } = null!;

        public bool Concurrent { get; set; }

        public int ConcurrentTransfers { get; set; }
    }
}