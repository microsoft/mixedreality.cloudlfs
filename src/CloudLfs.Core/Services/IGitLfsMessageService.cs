using Microsoft.MixedReality.CloudLfs.Models;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public interface IGitLfsMessageService
    {
        GitLfsMessage? ReadMessage();
        void WriteMessage(GitLfsMessage message);
    }
}