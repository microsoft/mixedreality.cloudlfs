using Microsoft.MixedReality.CloudLfs.Contracts.HttpContracts;
using Microsoft.MixedReality.CloudLfs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class GitLfsBroker : IGitLfsBroker
    {
        private Uri _lfsEndpoint;
        private Func<Uri, Task<NetworkCredential>> _getCredentials;

        public GitLfsBroker(Uri lfsEndpoint, NetworkCredential credentials)
        {
            _lfsEndpoint = lfsEndpoint;
            _getCredentials = (x) => Task.FromResult(credentials);
        }

        public GitLfsBroker(Uri lfsEndpoint, Func<Uri, Task<NetworkCredential>> getCredentials)
        {
            _lfsEndpoint = lfsEndpoint;
            _getCredentials = getCredentials;
        }

        public async Task<bool> DownloadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var client = await CreateClient();

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

            var action = responseBody.Objects.First().Actions["download"];

            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = await GetBasicAuthorizationHeader();
            foreach (var header in action.Header)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.Trim());
            }
            response = await client.GetAsync(action.Href);
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();
            await responseStream.CopyToAsync(contentStream);

            return true;
        }

        public async Task<bool> UploadAsync(string objectId, long size, IProgress<TransferStatus> progress, Stream contentStream, CancellationToken cancellationToken)
        {
            var client = await CreateClient();

            // create request body
            var requestBody = new GitLfsBatchRequestContractV1
            {
                Operation = "upload",
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

            var action = responseBody.Objects.First().Actions["upload"];

            client = new HttpClient();
            foreach (var header in action.Header)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.Trim());
            }
            response = await client.PutAsync(action.Href, new StreamContent(contentStream));
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return true;
        }

        private async Task<HttpClient> CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.git-lfs+json"));
            client.DefaultRequestHeaders.Authorization = await GetBasicAuthorizationHeader();
            return client;
        }

        private async Task<AuthenticationHeaderValue> GetBasicAuthorizationHeader()
        {
            var networkCredentials = await _getCredentials(_lfsEndpoint);
            var headerBytes = Encoding.UTF8.GetBytes(networkCredentials.UserName + ":" + networkCredentials.Password);
            var headerValue = Convert.ToBase64String(headerBytes);
            var authorizationHeader = new AuthenticationHeaderValue("Basic", headerValue); ;
            return authorizationHeader;
        }
    }
}