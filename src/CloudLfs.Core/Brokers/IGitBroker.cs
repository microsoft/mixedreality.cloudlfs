using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IGitBroker
    {
        Task<NetworkCredential> GetCredentials(string hostname, string protocol);

        Task<Uri> GetLfsEndpoint();
    }
}