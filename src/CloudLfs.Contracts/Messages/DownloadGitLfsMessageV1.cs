using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class DownloadGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "download";

        [JsonProperty("oid")]
        public string ObjectId { get; set; } = null!;

        public long Size { get; set; }
    }
}