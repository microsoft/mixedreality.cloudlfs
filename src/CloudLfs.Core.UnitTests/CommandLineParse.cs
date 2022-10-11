using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.MixedReality.CloudLfs.Brokers;
using Microsoft.MixedReality.CloudLfs.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudLfs.Core.UnitTests
{
    [TestClass]
    public class CommandLineParse
    {
        Uri uri;
        TelemetryClient telemetry;
        IBlobBroker blobBroker;

        public CommandLineParse()
        {
            ServicePointManager.DefaultConnectionLimit = 128;
            uri = new Uri("https://cloudlfscachewusint.blob.core.windows.net/");
            telemetry = new TelemetryClient(new TelemetryConfiguration() { ConnectionString = "InstrumentationKey=91722768-2734-4f0b-b8be-1a408c93d598;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/" });
            blobBroker = new AzureBlobBroker(telemetry, uri);
        }

        [TestMethod]
        public async Task RunCommand1()
        {
            string fileString = GetFiles();
            var fileList = fileString.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var fileMap = GetFileMap(fileList);
            Assert.IsTrue(fileMap.Count > 0);
            var stopwatch = Stopwatch.StartNew();
            await Parallel.ForEachAsync(fileMap.Keys, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, async (key, ct) =>
            {
                await DownloadFile(key, ct);
            });
            stopwatch.Stop();
            //foreach (var kvp in fileMap)
            //{
            //    await DownloadFile(kvp.Key, kvp.Value);
            //}
            Console.WriteLine($"Total Time: {stopwatch.Elapsed.TotalSeconds}");
        }

        private async Task DownloadFile(string fileGuid, CancellationToken ct)
        {
            var tempFolderPath = Path.Combine(Path.GetTempPath(), "cloudlfs");
            Directory.CreateDirectory(tempFolderPath);
            var tempFilePath = Path.Combine(tempFolderPath, fileGuid);
            var contentStream = File.Open(tempFilePath, FileMode.Create);


            // try download from cache...
            await blobBroker.DownloadAsync(fileGuid, new Progress<long>(), contentStream, ct);
            // download complete
            contentStream.Close();
            contentStream.Dispose();
        }

        private Dictionary<string, List<string>> GetFileMap(string[] fileList)
        {
            Dictionary<string, List<string>> fileMap = new();

            foreach (var file in fileList)
            {
                var mapping = file.Split(@"*", StringSplitOptions.TrimEntries);
                if (fileMap.TryGetValue(mapping[0], out var list))
                {
                    list.Add(mapping[1]);
                } 
                else
                {
                    fileMap.Add(mapping[0], new List<string>() { mapping[1] });
                }
            }

            return fileMap;
        }

        private string GetFiles()
        {
            ProcessStartInfo processStartInfo = new("CMD.exe", "/C git lfs ls-files -l")
            {
                WorkingDirectory = @"D:\\mixedreality.platform2\",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var process = Process.Start(processStartInfo);
            if (process == null)
            {
                Console.WriteLine("process null");
                throw new Exception($"Could not execute {processStartInfo.FileName} with argumenst {processStartInfo.Arguments} in {processStartInfo.WorkingDirectory}");
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

    }
}
