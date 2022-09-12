using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsBatchRequestContractV1
    {
        [JsonProperty("operation")]
        public string Operation { get; set; } = null!;

        [JsonProperty("transfers")]
        public List<string> Transfers { get; set; } = new List<string>();

        [JsonProperty("objects")]
        public List<GitLfsObjectIdentifierContractV1> Objects { get; set; } = new List<GitLfsObjectIdentifierContractV1>();
    }
}