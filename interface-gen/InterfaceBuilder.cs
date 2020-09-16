using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using IoTModels.Resolvers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.DigitalTwins.Parser;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class to create a DTDL V2 Interface with sample data
    /// </summary>
    public class InterfaceBuilder
    {
        private const string DTDLV2_CONTEXT = "dtmi:dtdl:context;2";
        private const string DTDLV2_INTERFACE = "Interface";
        private DtdlV2Interface documentModel = new DtdlV2Interface();
        private List<Content> contents = new List<Content>();
        private string basePath;
    
        private ILogger log;

        ILogger logger 
        {
            get {
                if(log == null)
                {
                    ILogger<LocalFSResolver> log = LoggerFactory.Create(builder =>
                        builder
                        .AddDebug()
                        .AddConsole()
                    ).CreateLogger<LocalFSResolver>();
                }
                return log;
            }
        }

        private IResolver _resolver;
        IResolver Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    var lf = new LoggerFactory();
                    _resolver = new LocalFSResolver(basePath, logger);
                }
                return _resolver;
            }
        }
        private InterfaceBuilder() { }

        /// <summary>
        /// Returns the DTMI of the model
        /// </summary>
        public string DtModelId => documentModel.id;

        /// <summary>
        /// Static method for creating a new InterfaceBuilder Class
        /// </summary>
        /// <param name="knownDtmis">Set of dtmis that can be used as componenents in the interface</param>
        /// <param name="repoBasePath">Root of the repo where the models are created and stored</param>
        /// <param name="logger">Optional ILogger to log with</param>
        /// <returns></returns>
        public static async Task<InterfaceBuilder> GetInterfaceBuilder(IEnumerable<string> knownDtmis, string repoBasePath = ".", ILogger logger = null)
        {
            InterfaceBuilder ifc = new InterfaceBuilder();
            if(logger != null)
            {
                ifc.log = logger;
            }
            
            ifc.basePath = repoBasePath;
            await ifc.SetInterfaceData();
            ifc.AddContents(knownDtmis);
            return ifc;
        }

        /// <summary>
        /// Get the interface as a Json string
        /// </summary>
        /// <returns>String with Json that represents the interface</returns>
        public override string ToString()
        {
            return documentModel.Serialize();
        }

        /// <summary>
        /// Adds the interface into the correct folder in the base path 
        /// </summary>
        /// <returns>Task</returns>
        public async Task WriteToBaseDir()
        {
            string pathExt = DtmiConvention.Dtmi2Path(documentModel.id);
            string filePath = Path.Join(basePath, pathExt);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                await documentModel.Serialize(fs);
            }
        }

        /// <summary>
        /// Use the Azure.Microsoft.DigitalTwins.Parser to validate the model
        /// including resolving local components as needed. Throws parsing and
        /// resolution exceptions if the model is not correct
        /// </summary>
        /// <returns>number of components in interface</returns>
        public async Task<int> ValidateModel()
        {
            int numComponents = 0;
            ModelParser parser = new ModelParser();
            parser.DtmiResolver = Resolver.DtmiResolver;
            parser.Options = new HashSet<ModelParsingOption>() { ModelParsingOption.StrictPartitionEnforcement };
            var parserResult = await parser.ParseAsync(new[] { documentModel.Serialize() });

            foreach (var item in parserResult.Values)
            {
                if (item.EntityKind == DTEntityKind.Component)
                {
                    numComponents++;
                }
            }
            return numComponents;
        }

        private async Task SetInterfaceData()
        {
            var mg = await ModelIdGenerator.GetModelGenerator(Resolver);
            documentModel.id = mg.AbsoluteModelId;
            documentModel.context = DTDLV2_CONTEXT;
            documentModel.type = DTDLV2_INTERFACE;
            documentModel.displayName = mg.ProductName;
            documentModel.contents = contents;
        }

        private void AddContents(IEnumerable<string> knownDtmis)
        {
            var cg = new ContentGenerator();
            contents.AddRange(cg.GenerateInterfacedata(knownDtmis));

        }
    }
}