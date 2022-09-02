using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public interface IBlobBroker
    {
        /// <summary>
        /// Download the content from the storage provider. Return FALSE if file does not exist or cannot be downloaded.
        /// </summary>
        /// <param name="id">The id of the content.</param>
        /// <param name="contentStream">The stream to write to.</param>
        /// <returns>True if the file was successfully downloaded, false otherwise.</returns>
        public Task<bool> DownloadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken);

        /// <summary>
        /// Upload the content to the storage provider. Return FALSE if file could not be uploaded.
        /// </summary>
        /// <param name="id">The id of the content.</param>
        /// <param name="contentStream">The stream to read from.</param>
        /// <returns>True if the file was successfully uploaded, false otherwise.</returns>
        public Task<bool> UploadAsync(string id, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken);
    }
}