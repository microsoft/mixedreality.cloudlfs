using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class AzureTransferOrchestrationServiceTests : TransferOrchestrationServiceBaseTests
    {
        private TelemetryClient _mockTelemetryClient;

        public AzureTransferOrchestrationServiceTests()
        {
            _mockTelemetryClient = new TelemetryClient(new TelemetryConfiguration());
        }

        public override ITransferOrchestrationService CreateTransferOrchestrationService(IConsoleBroker consoleBroker)
        {
            var messageService = new GitLfsMessageService(consoleBroker);
            var blobBroker = new AzureBlobBroker(_mockTelemetryClient, new Uri("https://cloudlfswus2premiumint.blob.core.windows.net/"));
            var lfsBroker = (IGitLfsBroker)null;

            return new TransferOrchestrationService(messageService, blobBroker, lfsBroker);
        }
    }
}