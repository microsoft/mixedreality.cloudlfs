using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsObjectIdentifierContractV1
    {
        [JsonProperty("oid")]
        public string ObjectId { get; set; } = null!;

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}