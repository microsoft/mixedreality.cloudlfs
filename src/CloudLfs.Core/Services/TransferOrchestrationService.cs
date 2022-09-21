using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class TransferOrchestrationService : ITransferOrchestrationService
    {
        private readonly IGitLfsMessageService _messageService;

        private readonly IBlobBroker _blobBroker;

        private readonly IGitLfsBroker _lfsBroker;
        private readonly string _tempPath;

        public TransferOrchestrationService(IGitLfsMessageService messageService, IBlobBroker blobBroker, IGitLfsBroker lfsBroker, string tempPath = default)
        {
            _messageService = messageService;
            _blobBroker = blobBroker;
            _lfsBroker = lfsBroker;
            _tempPath = tempPath ?? Path.GetTempPath();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10);

                var message = _messageService.ReadMessage();
                if (message is TerminateTransferGitLfsMessage)
                {
                    return;
                }
                else if (message is InitializeTransferGitLfsMessage)
                {
                    _messageService.WriteMessage(new AcknowledgeGitLfsMessage());
                }
                else if (message is UploadObjectLfsGitMessage uploadMessage)
                {
                    // load file stream
                    var contentStream = File.OpenRead(uploadMessage.Path);

                    // stage download progress
                    _messageService.WriteMessage(new TransferProgressGitMessage(uploadMessage.ObjectId, 0, 0));

                    // upload to source always
                    await UploadToSourceAsync(uploadMessage.ObjectId, uploadMessage.Size, contentStream);

                    // download complete
                    contentStream.Close();
                    contentStream.Dispose();
                    _messageService.WriteMessage(new TransferCompleteGitLfsMessage(uploadMessage.ObjectId, contentStream.Name));
                }
                else if (message is DownloadObjectLfsGitMessage downloadMessage)
                {
                    // create place for file
                    var tempFolderPath = Path.Combine(_tempPath, "cloudlfs");
                    Directory.CreateDirectory(tempFolderPath);
                    var tempFilePath = Path.Combine(tempFolderPath, downloadMessage.ObjectId);
                    var contentStream = File.Open(tempFilePath, FileMode.Create);

                    // stage download progress
                    _messageService.WriteMessage(new TransferProgressGitMessage(downloadMessage.ObjectId, 0, 0));

                    // try download from cache...
                    if (!await DownloadFromCacheAsync(downloadMessage.ObjectId, contentStream))
                    {
                        // try download from source...
                        if (await DownloadFromSourceAsync(downloadMessage.ObjectId, downloadMessage.Size, contentStream))
                        {
                            // close write stream
                            contentStream.Close();
                            contentStream.Dispose();

                            // open read stream
                            contentStream = File.OpenRead(tempFilePath);

                            // upload to cache for next consumer
                            await UploadToCacheAsync(downloadMessage.ObjectId, contentStream);
                        }
                    }

                    // download complete
                    contentStream.Close();
                    contentStream.Dispose();
                    _messageService.WriteMessage(new TransferCompleteGitLfsMessage(downloadMessage.ObjectId, contentStream.Name));
                }
            }
        }

        private async Task<bool> UploadToSourceAsync(string objectId, long size, FileStream contentStream)
        {
            var progress = new Progress<TransferStatus>();
            progress.ProgressChanged += (sender, args) =>
            {
                _messageService.WriteMessage(new TransferProgressGitMessage(objectId, args.BytesSoFar, args.BytesSinceLast));
            };

            if (await _lfsBroker.UploadAsync(objectId, size, progress, contentStream))
            {
                return true;
            }

            return false;
        }

        private async Task UploadToCacheAsync(string objectId, FileStream contentStream)
        {
            var progress = new Progress<long>();
            await _blobBroker.UploadAsync(objectId, progress, contentStream, CancellationToken.None);
        }

        private async Task<bool> DownloadFromSourceAsync(string objectId, long size, FileStream contentStream)
        {
            var progress = new Progress<TransferStatus>();
            progress.ProgressChanged += (sender, args) =>
            {
                _messageService.WriteMessage(new TransferProgressGitMessage(objectId, args.BytesSoFar, args.BytesSinceLast));
            };

            if (await _lfsBroker.DownloadAsync(objectId, size, progress, contentStream))
            {
                return true;
            }

            return false;
        }

        private async Task<bool> DownloadFromCacheAsync(string objectId, FileStream contentStream)
        {
            var progress = new Progress<long>();
            var response = await _blobBroker.DownloadAsync(objectId, progress, contentStream, CancellationToken.None);
            if (response != null)
            {
                return true;
            }

            return false;
        }
    }
}