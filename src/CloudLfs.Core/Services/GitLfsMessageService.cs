using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Contracts.Messages;
using Microsoft.MixedReality.CloudLfs.Models;
using Newtonsoft.Json;
using System;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class GitLfsMessageService : IGitLfsMessageService
    {
        private readonly IConsoleBroker _console;

        private readonly GitMessageContractConverter _converter;

        public GitLfsMessageService(IConsoleBroker console)
        {
            _console = console;
            _converter = new GitMessageContractConverter();
        }

        public GitLfsMessage? ReadMessage()
        {
            var line = _console.ReadLine();
            if (line == null)
            {
                return null;
            }

            var contract = JsonConvert.DeserializeObject<GitLfsMessageV1>(line, _converter);
            if (contract is InitializeTransferGitLfsMessageV1 initContract)
            {
                return new InitializeTransferGitLfsMessage
                {
                    Concurrent = initContract.Concurrent,
                    ConcurrentTransfers = initContract.ConcurrentTransfers,
                    Operation = initContract.Operation,
                    Remote = initContract.Remote,
                };
            }
            else if (contract is TerminateTransferGitLfsMessageV1)
            {
                return new TerminateTransferGitLfsMessage();
            }
            else if (contract is DownloadGitLfsMessageV1 downloadContract)
            {
                return new DownloadObjectLfsGitMessage(downloadContract.ObjectId, downloadContract.Size);
            }
            else if (contract is UploadGitLfsMessageV1 uploadContract)
            {
                return new UploadObjectLfsGitMessage(uploadContract.ObjectId, uploadContract.Size, uploadContract.Path);
            }
            else
            {
                return null;
            }
        }

        public void WriteMessage(GitLfsMessage message)
        {
            string json;

            if (message is TransferCompleteGitLfsMessage contract)
            {
                json = JsonConvert.SerializeObject(new TransferCompleteGitLfsMessageV1
                {
                    ObjectId = contract.ObjectId,
                    Path = contract.Path,
                }, Formatting.None);
            }
            else if (message is AcknowledgeGitLfsMessage)
            {
                json = "{}";
            }
            else if (message is TransferProgressGitMessage progressMessage)
            {
                json = JsonConvert.SerializeObject(new TransferProgressGitLfsMessageV1
                {
                    ObjectId = progressMessage.ObjectId,
                    BytesSinceLast = progressMessage.BytesSinceLast,
                    BytesSoFar = progressMessage.BytesSoFar,
                }, Formatting.None);
            }
            else
            {
                throw new NotSupportedException($"Message of type {message.GetType().Name} is not supported.");
            }

            _console.WriteLine(json);
        }
    }
}