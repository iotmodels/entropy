using Microsoft.Azure.DigitalTwins.Parser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTModels.Resolvers
{
    /// <summary>
    /// Resolver for public github registry
    /// </summary>
    public class PublicRepoResolver : IResolver
    {
        private string modelRepoUrl;
        private ILogger logger;

        /// <summary>
        /// Creates a public repo resolver
        /// </summary>
        /// <param name="log">ILogger for logging</param>
        /// <param name="repoUrl">Base url of the repo to resolve against</param>
        /// <returns></returns>
        public PublicRepoResolver(ILogger log, string repoUrl = "https://iotmodels.github.io/registry/")
        {
            logger = log;
            modelRepoUrl = repoUrl;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtmis"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> DtmiResolver(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<string> resolvedModels = new List<string>();
            foreach (var dtmi in dtmis)
            {
                logger.LogInformation($"Resolving {dtmi.AbsoluteUri}");
                var path = DtmiConvention.Dtmi2Path(dtmi.AbsoluteUri);
                string url = modelRepoUrl + path;
                logger.LogTrace("Request: " + url);
                resolvedModels.Add(await Get(url));
                logger.LogTrace("OK:" + url);
            }
            return await Task.FromResult(resolvedModels);
        }

        async Task<string> Get(string url)
        {
            logger.LogInformation("GET: " + url);
            using (var http = new HttpClient())
            {
                var data = await http.GetStringAsync(url);
                return data;
            }
        }
    }
}
