using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class AcknowledgeGitLfsMessageV1 : GitLfsMessageV1
    {
        [JsonIgnore]
        public override string Event => base.Event;
    }
}