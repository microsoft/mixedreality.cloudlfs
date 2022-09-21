using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class GitLfsMessageV1
    {
        [JsonProperty("event")]
        public virtual string Event { get; } = "undefined";
    }
}