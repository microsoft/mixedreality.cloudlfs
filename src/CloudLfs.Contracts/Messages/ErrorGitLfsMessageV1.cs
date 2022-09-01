using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class ErrorGitLfsMessageV1 : GitLfsMessageV1
    {
        [JsonIgnore]
        public override string Event => base.Event;

        public GitLfsErrorV1 Error { get; set; } = null!;
    }
}