using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public interface ITransferOrchestrationService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}