using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    internal interface IBlobService
    {
        /// <summary>
        /// Downloads the content from the storage provider into a file.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<long> DownloadAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Uploads the content from the storage provider.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task UploadAsync(string id);
    }
}
