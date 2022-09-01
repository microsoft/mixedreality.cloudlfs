namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class GitLfsErrorV1
    {
        public long Code { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}