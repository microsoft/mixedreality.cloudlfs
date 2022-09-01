using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class TransferProgressGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "progress";

        [JsonProperty("oid")]
        public string ObjectId { get; set; } = null!;

        public long BytesSoFar { get; set; }

        public long BytesSinceLast { get; set; }
    }
}