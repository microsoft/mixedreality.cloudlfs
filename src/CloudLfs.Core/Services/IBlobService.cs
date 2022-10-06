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
        /// Calls the Blob Broker's DownloadAsync function, which downloads a blob into the <paramref name="contentStream"/> parameter. 
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to write to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="AzureBlobServiceResultCode"/> that represents the operation result</returns>
        public Task<AzureBlobServiceResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken);

        /// <summary>
        /// Calls the Blob Broker's DownloadAsync function, which downloads a subsection of a blob into the <paramref name="contentStream"/> parameter. 
        /// </summary>
        /// <param name="blobName">The blob name.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to download to.</param>
        /// <param name="startBytes">The position in the blob to start at, must be less than blob size.</param>
        /// <param name="endBytes">The position in the blob to end at, must be less than blob size and greater than <paramref name="endBytes"/></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="AzureBlobServiceResultCode"/> that represents the operation result</returns>
        public Task<AzureBlobServiceResultCode> DownloadAsync(string blobName, IProgress<long> progress, Stream contentStream, long startBytes, long endBytes, CancellationToken cancellationToken);

        /// <summary>
        /// Calls the Blob Brokers UploadAsync function to upload the content of <paramref name="contentStream"/> into a blob storage.
        /// </summary>
        /// <param name="blobName">The id of the content.</param>
        /// <param name="progress">The progress handler, reports how many bytes have been transferred.</param>
        /// <param name="contentStream">The stream to read from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="AzureBlobServiceResultCode"/> that represents the operation result</returns>
        public Task<AzureBlobServiceResultCode> UploadAsync(string blobName, IProgress<long> progress, Stream contentStream, CancellationToken cancellationToken);
    }
}
