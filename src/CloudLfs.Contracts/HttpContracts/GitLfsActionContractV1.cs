using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsActionContractV1
    {
        public string Href { get; set; } = null!;

        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();

        [JsonProperty("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}