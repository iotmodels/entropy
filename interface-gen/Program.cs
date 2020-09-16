using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Reflection.PortableExecutable;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.DigitalTwins.Parser;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DtdlInterfaceGenerator
{
    class Program
    {
        /// <summary>
        /// Program to generate a bunch of interfaces into a local file store
        /// </summary>
        /// <param name="repoRoot">sets the root of the repository to generate into</param>
        /// <param name="numCreate">sets the total number of interfaces to create</param>
        /// <param name="maxFailures">sets max invalid models that will be created </param>
        /// <returns>int for success or failure</returns>
        static async Task<int> Main(string repoRoot = @"c:\temp\registry", int numCreate = 5, int maxFailures = 25)
        {
            #if (DEBUG)
            ILogger<InterfaceBuilder> logger = LoggerFactory.Create(builder =>
                builder 
                .AddDebug()
                .AddConsole()
            ).CreateLogger<InterfaceBuilder>();
            #else
            ILogger<InterfaceBuilder> logger = LoggerFactory.Create(builder =>
                builder 
            ).CreateLogger<InterfaceBuilder>();
            #endif

            if (!parametersValid(repoRoot, logger))
            {
                return 1;
            }
            int numCreated = 0;
            int numFailures = 0;
            KnownInterfaces ki = await KnownInterfaces.GetKnownInterfaces(repoRoot);

            while (numCreated < numCreate && numFailures < maxFailures)
            {
                var ifcBuilder = await InterfaceBuilder.GetInterfaceBuilder(ki.Items, repoRoot.ToString(), logger);
                try
                {
                    int numComponents = await ifcBuilder.ValidateModel();
                    await ifcBuilder.WriteToBaseDir();

                    if (numComponents == 0)
                    {
                        await ki.Add(ifcBuilder.DtModelId);
                    }
                    numCreated++;
                    logger.LogInformation($"{numCreated}: {ifcBuilder.DtModelId}");
                    logger.LogInformation($"\tcount components: {numComponents}");

                }
                catch (ResolutionException rex)
                {
                    handleException(rex, ifcBuilder);
                    numFailures++;
                }
                catch (ParsingException pex)
                {
                    handleException(pex, ifcBuilder);
                    numFailures++;
                }
            }

            logger.LogInformation($"Results:");
            logger.LogInformation($"\tModels created: {numCreated}");
            logger.LogInformation($"\tNumber failed:  {numFailures}");
            await ki.SaveItems();
            return 0;
        }

        private static void handleException(Exception ex, InterfaceBuilder ifcBuilder)
        {
            string filename = $"err-{ifcBuilder.DtModelId.Replace(":", "-").Replace(";", "-")}.json";
            using (var writer = File.CreateText(filename))
            {
                writer.Write(ex.ToString());
                writer.WriteLine();
                writer.Write(ifcBuilder.ToString());
            }
        }

        private static bool parametersValid(string repoRoot, ILogger logger)
        {
            var rootDi = new DirectoryInfo(repoRoot);
            if (!rootDi.Exists)
            {
                logger.LogError("The argument given for --repo-root is not a valid folder.");
                logger.LogError($"argument: {repoRoot}");
                return false;
            }
            return true;
        }
    }
}
