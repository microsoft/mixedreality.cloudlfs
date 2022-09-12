using System.Collections.Generic;

namespace Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts
{
    public class GitLfsBatchResponseContractV1
    {
        public List<GitLfsObjectMetadataContractV1> Objects { get; set; } = new List<GitLfsObjectMetadataContractV1>();
    }
}