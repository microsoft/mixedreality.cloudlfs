using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class AzureTransferOrchestrationServiceTests : TransferOrchestrationServiceBaseTests
    {
        public override ITransferOrchestrationService CreateTransferOrchestrationService(IConsoleBroker consoleBroker)
        {
            var messageService = new GitLfsMessageService(consoleBroker);
            var blobBroker = new AzureBlobBroker(new Uri("https://cloudlfswus2premiumint.blob.core.windows.net/"));
            var lfsBroker = (IGitLfsBroker)null;

            return new TransferOrchestrationService(messageService, blobBroker, lfsBroker);
        }
    }
}