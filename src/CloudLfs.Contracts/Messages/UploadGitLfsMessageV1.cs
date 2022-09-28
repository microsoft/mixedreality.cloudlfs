using Newtonsoft.Json;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class UploadGitLfsMessageV1 : GitLfsMessageV1
    {
        public override string Event => "upload";

        [JsonProperty("oid")]
        public string ObjectId { get; set; } = null!;

        public long Size { get; set; }

        public string Path { get; set; }
    }
}