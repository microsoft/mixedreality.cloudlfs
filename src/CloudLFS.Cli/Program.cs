using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Cli
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                // bind SIGINT and SIGTERM
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (o, e) => cts.Cancel();
                AppDomain.CurrentDomain.ProcessExit += (o, e) => cts.Cancel();

                // set up services
                var consoleBroker = new ConsoleBroker();
                var gitLfsMessageService = new GitLfsMessageService(consoleBroker);
                var blobBroker = (IBlobBroker)null;
                var lfsBroker = (IGitLfsBroker)null;
                var transferService = new TransferOrchestrationService(gitLfsMessageService, blobBroker, lfsBroker);

                // start message loop
                await transferService.RunAsync(cts.Token);
            }
            catch
            {
                // unhandled exception, non-graceful shutdown, TODO: log
                throw;
            }
        }
    }
}