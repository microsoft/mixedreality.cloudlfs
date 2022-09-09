using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using System.Threading;
using Microsoft.MixedReality.CloudLfs.Services;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CloudLfsFunctionApp
{
    public static class Function1
    {
        [FunctionName("cloudlfs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var files = new string[] { "file1", "file1", "file1", "file1", "file1" };
            var response = new OkObjectResult("Download successful");
            BlobService service = new(log);
            var stopwatch = Stopwatch.StartNew();
            ConcurrentQueue<long> fileSizes = new();
            try
            {
                await Parallel.ForEachAsync(files, async (file, ct) => 
                    fileSizes.Enqueue( 
                        await service.DownloadAsync(file, ct)));
            }
            catch (Exception e)
            {
                response = new OkObjectResult("DownloadFailed");
                log.LogError(e, response.Value.ToString());
            }
            stopwatch.Stop();
            log.LogInformation(GetMessageString(files.Length, fileSizes.Sum(), stopwatch.Elapsed.TotalSeconds));
            return response;
        }

        private static string GetMessageString(int numberOfFiles, long totalSize, double elapsedTimeInSeconds)
        {
            var props = new Dictionary<string, object>()
            {
                {"totalTime", elapsedTimeInSeconds},
                {"totalSize",totalSize},
                {"numberOfFiles", numberOfFiles},
            };

            return JsonConvert.SerializeObject(props);
        }
    }
}
