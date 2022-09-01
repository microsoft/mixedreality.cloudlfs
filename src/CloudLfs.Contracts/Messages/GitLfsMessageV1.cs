using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.CloudLfs.Contracts.Messages
{
    public class GitLfsMessageV1
    {
        public virtual string Event { get; } = "undefined";
    }
}