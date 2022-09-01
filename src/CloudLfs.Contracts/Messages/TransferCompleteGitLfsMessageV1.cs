using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class TransferCompleteGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "complete";

        [JsonProperty("oid")]
        public string ObjectId { get; set; } = null!;
    }
}