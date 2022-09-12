using Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts;
using Microsoft.MixedReality.CloudLfs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class GitLfsBroker : IGitLfsBroker
    {
        private Uri _lfsEndpoint;

        public GitLfsBroker(Uri lfsEndpoint)
        {
            _lfsEndpoint = lfsEndpoint;
        }

        public async Task<bool> DownloadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.git-lfs+json"));

            // create request body
            var requestBody = new GitLfsBatchRequestContractV1
            {
                Operation = "download",
                Objects = new List<GitLfsObjectIdentifierContractV1>
                {
                    new GitLfsObjectIdentifierContractV1
                    {
                        ObjectId = objectId,
                        Size = size
                    }
                }
            };
            var requestBodyJson = JsonConvert.SerializeObject(requestBody);
            var requestBodyContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/vnd.git-lfs+json");

            // get response
            var uri = new Uri(_lfsEndpoint.ToString().TrimEnd('/') + "/objects/batch");
            var response = await client.PostAsync(uri, requestBodyContent);
            var responseBodyJson = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<GitLfsBatchResponseContractV1>(responseBodyJson);

            var stream = await client.GetStreamAsync(responseBody.Objects.First().Actions["download"].Href);
            await stream.CopyToAsync(contentStream);

            return true;
        }
    }
}