using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class TransferOrchestrationService
    {
        private readonly IGitLfsMessageService _messageService;

        private readonly IBlobBroker _blobBroker;

        private readonly IGitLfsBroker _lfsBroker;

        public TransferOrchestrationService(IGitLfsMessageService messageService, IBlobBroker blobBroker, IGitLfsBroker lfsBroker)
        {
            _messageService = messageService;
            _blobBroker = blobBroker;
            _lfsBroker = lfsBroker;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _messageService.ReadMessage();
                if (message is TerminateTransferGitLfsMessage)
                {
                    break;
                }
                else if (message is InitializeTransferGitLfsMessage)
                {
                    _messageService.WriteMessage(new AcknowledgeGitLfsMessage());
                }
                else if (message is DownloadObjectLfsGitMessage downloadMessage)
                {
                    // create place for file
                    var tempFolderPath = Path.Combine(Path.GetTempPath(), "cloudlfs");
                    var tempFilePath = Path.Combine(tempFolderPath, downloadMessage.ObjectId);
                    using var contentStream = File.Open(tempFilePath, FileMode.Create);

                    // try download from cache...
                    if (!await DownloadFromCacheAsync(downloadMessage.ObjectId, contentStream))
                    {
                        // try download from source...
                        if (await DownloadFromSourceAsync(downloadMessage.ObjectId, contentStream))
                        {
                            // upload to cache for next consumer
                            await UploadToCacheAsync(downloadMessage.ObjectId, contentStream);
                        }
                    }
                }

                await Task.Delay(10);
            }
        }

        private async Task UploadToCacheAsync(string objectId, FileStream contentStream)
        {
            var progress = new Progress<TransferStatus>();
            await _blobBroker.UploadAsync(objectId, progress, contentStream, CancellationToken.None);
        }

        private async Task<bool> DownloadFromSourceAsync(string objectId, FileStream contentStream)
        {
            var progress = new Progress<TransferStatus>();
            progress.ProgressChanged += (sender, args) =>
            {
                _messageService.WriteMessage(new TransferProgressGitMessage(objectId, args.BytesSoFar, args.BytesSinceLast));
            };

            if (await _lfsBroker.DownloadAsync(objectId, progress, contentStream))
            {
                // download complete
                _messageService.WriteMessage(new TransferCompleteGitLfsMessage(objectId));
                return true;
            }

            return false;
        }

        private async Task<bool> DownloadFromCacheAsync(string objectId, Stream contentStream)
        {
            var progress = new Progress<TransferStatus>();
            progress.ProgressChanged += (sender, args) =>
            {
                _messageService.WriteMessage(new TransferProgressGitMessage(objectId, args.BytesSoFar, args.BytesSinceLast));
            };

            if (await _blobBroker.DownloadAsync(objectId, progress, contentStream, CancellationToken.None))
            {
                // download complete
                _messageService.WriteMessage(new TransferCompleteGitLfsMessage(objectId));
                return true;
            }

            return false;
        }
    }
}