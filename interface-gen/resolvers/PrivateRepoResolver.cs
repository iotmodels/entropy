using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.DigitalTwins.Parser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IoTModels.Resolvers
{
    /// <summary>
    /// Private repo resolver works with storage
    /// </summary>
    public class PrivateRepoResolver : IResolver
    {
        string storageConnectionString;
        BlobContainerClient containerClient;
        ILogger logger;

        /// <summary>
        /// Creates a private repo resolver
        /// </summary>
        /// <param name="connectionString">connection string for the storage container</param>
        /// <param name="log">ILogger to use for logging</param>
        public PrivateRepoResolver(string connectionString, ILogger log)
        {
            this.logger = log;
            storageConnectionString = connectionString;
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                Console.WriteLine("ERROR: PrivateRepos require credentials via config 'StorageConnectionString'");
                Environment.Exit(-1);
            }
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient("repo");
        }

        /// <summary>
        /// Implement IResolver api
        /// </summary>
        /// <param name="dtmis">set of Dtmi to resolve</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> DtmiResolver(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<string> resolvedModels = new List<string>();
            foreach (var dtmi in dtmis)
            {

                string path = DtmiConvention.Dtmi2Path(dtmi.AbsoluteUri);
                var dlModel = await containerClient.GetBlobClient(path).DownloadAsync();
                using (var sr = new StreamReader(dlModel.Value.Content))
                {
                    resolvedModels.Add(sr.ReadToEnd());
                }
                logger.LogTrace("OK " + path);

            }
            return await Task.FromResult(resolvedModels);
        }
    }
}
