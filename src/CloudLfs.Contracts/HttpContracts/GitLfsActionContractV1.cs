using Newtonsoft.Json;
using System;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsActionContractV1
    {
        public string Href { get; set; } = null!;

        [JsonProperty("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}