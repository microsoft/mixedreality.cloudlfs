using System.Collections.Generic;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsObjectMetadataContractV1 : GitLfsObjectIdentifierContractV1
    {
        public Dictionary<string, GitLfsActionContractV1> Actions { get; set; } = new Dictionary<string, GitLfsActionContractV1>();
    }
}