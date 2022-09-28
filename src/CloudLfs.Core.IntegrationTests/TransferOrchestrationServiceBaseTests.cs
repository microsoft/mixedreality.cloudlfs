using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    public abstract class TransferOrchestrationServiceBaseTests
    {
        public abstract ITransferOrchestrationService CreateTransferOrchestrationService(IConsoleBroker consoleBroker);

        [TestMethod]
        public async Task TransferOrchestrationService_CanInitDownloadAndTerminate()
        {
            // arrange
            var console = new TestConsoleBroker();
            var service = CreateTransferOrchestrationService(console);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(100));

            // act
            var runTask = service.RunAsync(cts.Token);
            console.SendLine(@"{ ""event"": ""init"", ""operation"": ""download"", ""remote"": ""origin"", ""concurrent"": true, ""concurrenttransfers"": 3 }");
            console.SendLine(@"{ ""event"": ""download"", ""oid"": ""002BB6E25CAA50BB2A444CC77F7E920E88D2E7EB95EB61F6E2178CBD922E0628"", ""size"": 21245 }");
            console.SendLine(@"{ ""event"": ""terminate"" }");

            await runTask;

            // assert
            Assert.AreEqual(3, console.Output.Count);
            Assert.AreEqual("{}", console.Output[0]);
        }
    }
}