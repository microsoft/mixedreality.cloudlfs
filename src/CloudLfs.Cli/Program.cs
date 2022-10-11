using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using System;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using System.Net;

namespace Microsoft.MixedReality.CloudLfs.Cli
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 128;
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
                    // TODO: get from config file
                    var appInsightsConnectionOption = new Option<string>(new string[] { "-c", "--app-insights-connection-string" }, "The Connection String used for logging custom telemetry to an Azure Application Insights resource.");
                    azureBlobCommand.AddOption(uriOption);
                    azureBlobCommand.AddOption(appInsightsConnectionOption);
                    azureBlobCommand.SetHandler(
                        (uri, gitPath, repositoryPath, appInsightsConnStr) => ExecuteAzureHandler(uri, gitPath, repositoryPath, appInsightsConnStr, cts.Token),
                        uriOption,
                        gitPathOption,
                        repositoryPathOption,
                        appInsightsConnectionOption);
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

        private static async Task ExecuteAzureHandler(string uriValue, string gitPath, string repositoryPath, string appInsightsConnStr, CancellationToken cancellationToken)
        {
            var serviceProvider = InitServiceProvider(appInsightsConnStr);
            ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            TelemetryClient telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();
            
            try
            {
                var consoleBroker = new ConsoleBroker();
                var gitBroker = new GitBroker(repositoryPath ?? Directory.GetCurrentDirectory(), gitPath);
                var gitLfsMessageService = new GitLfsMessageService(consoleBroker);
                var lfsBroker = new GitLfsBroker(await gitBroker.GetLfsEndpoint(), (uri) => gitBroker.GetCredentials(uri.Host, uri.Scheme));
                var blobBroker = new AzureBlobBroker(telemetryClient, new Uri(uriValue));
                var transferService = new TransferOrchestrationService(gitLfsMessageService, blobBroker, lfsBroker, await gitBroker.GetLfsTempPath());
                await transferService.RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
                throw;
            }
            finally
            {
                // Explicitly call Flush() followed by sleep is required in Console Apps.
                // This is to ensure that even if application terminates, telemetry is sent to the back-end.
                if (!string.IsNullOrEmpty(appInsightsConnStr)) {
                    logger.LogInformation("Flushing Telemetry");
                    telemetryClient.Flush();
                    await Task.Delay(5000, cancellationToken);
                    logger.LogInformation("Telemetry Flushed");
                }
            }
        }

        /// <summary>
        /// Creates a ServiceProvider that contains ILogger and TelemetryClient services.
        /// If <paramref name="connectionString"/> is present, these services will log to the ApplicationInsights resource
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static IServiceProvider InitServiceProvider(string connectionString)
        {
            IServiceCollection services = new ServiceCollection();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter(typeof(Program).Namespace, LogLevel.Debug)
                    .AddSimpleConsole(options =>
                    {
                        options.SingleLine = true;
                        options.TimestampFormat = "[hh:mm:ss] ";
                    });
                });
                services.AddSingleton(loggerFactory.CreateLogger<Program>());
                services.AddSingleton(new TelemetryClient(new TelemetryConfiguration())); // create unconfigured telemetry service
                return services.BuildServiceProvider();
            }

            // logger
            services.AddLogging(loggingBuilder => {
                loggingBuilder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter(typeof(Program).Namespace, LogLevel.Debug)
                    .AddApplicationInsights(
                        configureTelemetryConfiguration: (config) => config.ConnectionString = connectionString,
                        configureApplicationInsightsLoggerOptions: (options) => { })
                    .AddSimpleConsole(options =>
                    {
                        options.SingleLine = true;
                        options.TimestampFormat = "[hh:mm:ss] ";
                    });
            });

            // telemetry
            services.AddApplicationInsightsTelemetryWorkerService((config) => config.ConnectionString = connectionString); // TODO pull from config
            return services.BuildServiceProvider();
        }
    }
}