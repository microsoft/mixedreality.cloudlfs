namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class TerminateTransferGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "terminate";
    }
}