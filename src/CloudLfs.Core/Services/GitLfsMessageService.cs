using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Contracts.Messages;
using Microsoft.MixedReality.CloudLfs.Models;
using Newtonsoft.Json;

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
                };
            }
            else
            {
                return null;
            }
        }

        public void WriteMessage(GitLfsMessage message)
        {
            var json = JsonConvert.SerializeObject(message, Formatting.None);
            _console.WriteLine(json);
        }
    }
}