using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using System;
using System.CommandLine;
using System.IO;
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

                // global options
                var gitPathOption = new Option<string>("--git-path", "The path to the git executable, if not set, it is assumed that git is in the PATH environment variable.");
                var repositoryPathOption = new Option<string>("--repository-path", "The path to the repository, if not set, it is assumed that the working directory is the repository path.");

                // define azure backend
                var azureBlobCommand = new Command("azure", "Use the azure blob storage backend");
                {
                    var uriOption = new Option<string>("--uri", "The underlying URI of the storage account; the storage account must support block blobs.") { IsRequired = true };
                    azureBlobCommand.AddOption(uriOption);
                    azureBlobCommand.SetHandler(
                        (uri, gitPath, repositoryPath) => ExecuteAzureHandler(uri, gitPath, repositoryPath, cts.Token),
                        uriOption,
                        gitPathOption,
                        repositoryPathOption);
                }

                // define root command
                var rootCommand = new RootCommand
                {
                    azureBlobCommand
                };
                rootCommand.AddOption(gitPathOption);

                // run commands
                await rootCommand.InvokeAsync(args);
            }
            catch
            {
                // unhandled exception, non-graceful shutdown, TODO: log
                throw;
            }
        }

        private static async Task ExecuteAzureHandler(string uriValue, string gitPath, string repositoryPath, CancellationToken cancellationToken)
        {
            var consoleBroker = new ConsoleBroker();
            var gitBroker = new GitBroker(repositoryPath ?? Directory.GetCurrentDirectory(), gitPath);
            var gitLfsMessageService = new GitLfsMessageService(consoleBroker);
            var lfsBroker = new GitLfsBroker(await gitBroker.GetLfsEndpoint());
            var blobBroker = new AzureBlobBroker(new Uri(uriValue));
            var transferService = new TransferOrchestrationService(gitLfsMessageService, blobBroker, lfsBroker);

            await transferService.RunAsync(cancellationToken);
        }
    }
}