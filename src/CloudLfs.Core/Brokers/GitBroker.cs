using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.CloudLfs.Brokers
{
    public class GitBroker : IGitBroker
    {
        private string _repositoryPath;

        private string? _gitPath;

        private Regex _endpointRegex = new Regex(@"Endpoint=(.*)\n");

        private Regex _usernameRegex = new Regex(@"username=(.*)\n");

        private Regex _passwordRegex = new Regex(@"password=(.*)\n");

        private Regex _trimRegex = new Regex(@"\(.*\)$");

        public GitBroker(string repositoryPath, string? gitPath = default)
        {
            _repositoryPath = repositoryPath;
            _gitPath = gitPath;
        }

        public async Task<NetworkCredential> GetCredentials(string hostname, string protocol)
        {
            using var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _gitPath ?? "git",
                    WorkingDirectory = _repositoryPath,
                    Arguments = "credential fill",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };

            process.Start();
            process.StandardInput.WriteLine($"protocol={protocol}");
            process.StandardInput.WriteLine($"host={hostname}");
            process.StandardInput.WriteLine(string.Empty);
            process.WaitForExit();

            var output = await process.StandardOutput.ReadToEndAsync();

            if (TryExtractMatch(output, _passwordRegex, out string password))
            {
                TryExtractMatch(output, _usernameRegex, out string username);

                return new NetworkCredential(username, password);
            }

            throw new KeyNotFoundException($"The Git credentials for host {hostname} and protocol {protocol} could not be found");
        }

        public async Task<Uri> GetLfsEndpoint()
        {
            using var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _gitPath ?? "git",
                    WorkingDirectory = _repositoryPath,
                    Arguments = "lfs env",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };

            process.Start();
            process.WaitForExit();

            var output = await process.StandardOutput.ReadToEndAsync();

            if (TryExtractMatch(output, _endpointRegex, out string endpoint))
            {
                return new Uri(_trimRegex.Replace(endpoint.Trim(), string.Empty).Trim());
            }

            throw new KeyNotFoundException("The Endpoint key was not found, check if repository is configured for LFS");
        }

        private bool TryExtractMatch(string input, Regex regex, out string value)
        {
            foreach (Match m in regex.Matches(input))
            {
                value = m.Groups[1].Value;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}