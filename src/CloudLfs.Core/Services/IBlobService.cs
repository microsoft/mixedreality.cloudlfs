using Azure;
using Microsoft.MixedReality.CloudLfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public interface IBlobService
    {
        /// <summary>
        /// Download the content from the storage provider. Return FALSE if file does not exist or cannot be downloaded.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the file was successfully downloaded, false otherwise.</returns>
        public Task<CloudLfsResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken);

        /// <summary>
        /// Download a subsection of the blob.
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to download to.</param>
        /// <param name="startBytes">The position in the blob to start at, must be less than blob size.</param>
        /// <param name="endBytes">The position in the blob to end at, must be less than blob size and greater than <paramref name="endBytes"/></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The raw response from the blob service.</returns>
        public Task<CloudLfsResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken);

        /// <summary>
        /// Upload the content to the storage provider. Return FALSE if file could not be uploaded.
        /// </summary>
        /// <param name="blobName">The id of the content.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to read from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the file was successfully uploaded, false otherwise.</returns>
        public Task<CloudLfsResultCode> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken);

    }
}
