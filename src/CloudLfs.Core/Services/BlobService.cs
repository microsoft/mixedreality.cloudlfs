using Azure;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Services
{
    public class BlobService : IBlobService
    {
        private IBlobBroker _blobBroker;
        private ILogger _logger;
        public BlobService(ILogger logger)
        {
            _blobBroker = new BlobBroker(new Uri("https://cloudlfscachewusint.blob.core.windows.net/"));
            _logger = logger;
        }

        public async Task<long> DownloadAsync(string blobName, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            long contentLength = stream.Length;
            bool isException = false;
            string stackTrace = "";
            bool success = false;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await _blobBroker.DownloadAsync(blobName, new Progress<long>(), stream, cancellationToken);
                stopwatch.Stop();
                contentLength = stream.Length;
                stream.Dispose();
                success = true;
            }
            catch (Exception e)
            {
                isException = true;
                stackTrace = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
                stopwatch.Stop();
            }
            finally
            {
                var totalTime = stopwatch.Elapsed.TotalSeconds;
                var customEvent = GenerateCustomEvent(totalTime, 
                    blobName, contentLength, success, isException, stackTrace);
                _logger.LogInformation(customEvent);
            }

            return contentLength;
        }

        public async Task UploadAsync(string id)
        {
            throw new NotImplementedException();
        }

        private string GenerateCustomEvent(double totalTimeInSeconds, 
            string blobName, long fileSize, bool successful, bool isException,
            string stackTrace)
        {
            var sizeInMB = fileSize / 1024 / 1024; // in MB
            var speed = sizeInMB / totalTimeInSeconds;
            var props = new Dictionary<string, object>
            {
                {"TotalTime", totalTimeInSeconds},
                {"file", blobName},
                {"fileSize", fileSize},
                {"avgSpeedMBpS",  speed},
                {"success", successful},
                {"isException", isException},
                {"stackTrace", stackTrace},
                {"description", "parallel" }
            };

            return JsonConvert.SerializeObject(props);
        }
    }
}
