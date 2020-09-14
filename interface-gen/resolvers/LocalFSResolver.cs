using Microsoft.Azure.DigitalTwins.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IoTModels.Resolvers
{
    /// <summary>
    /// Resolves local files using file path
    /// </summary>
    public class LocalFSResolver : IResolver
    {
        ILogger logger;
        string baseFolder;
        /// <summary>
        /// Creates an instance of the local folder resolver
        /// </summary>
        /// <param name="localFolderRoot">the root of the dtmi models to be resolved</param>
        /// <param name="log">logger for output</param>
        public LocalFSResolver(string localFolderRoot, ILogger<LocalFSResolver> log)
        {
            logger = log;
            baseFolder = localFolderRoot;
        }

        /// <summary>
        /// Implement the IResolver interface 
        /// </summary>
        /// <param name="dtmis">collection of dtmis to resolve</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> DtmiResolver(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<string> resolvedModels = new List<string>();
            foreach (var dtmi in dtmis)
            {
                logger.LogInformation($"Resolving {dtmi.AbsoluteUri}");
                var path = DtmiConvention.Dtmi2Path(dtmi.AbsoluteUri);
                DirectoryInfo di = new DirectoryInfo(baseFolder);
                string uri = di.FullName;
                foreach (var f in path.Split('/'))
                {
                    uri = Path.Combine(uri, f);
                }
                if (File.Exists(uri))
                {
                    logger.LogTrace("Reading: " + uri);
                    resolvedModels.Add(File.ReadAllText(uri));
                    logger.LogTrace("OK:" + uri);
                }
            }
            return await Task.FromResult(resolvedModels);
        }
    }
}
