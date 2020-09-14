using System.IO;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class for deserializing metadata used in creating DTMIs from motelMetadat.json
    /// </summary>
    public class ModelMetadata
    {
        /// <summary>
        /// Top level domains used in creating DTMIs
        /// </summary>
        /// <value>list of domains to be used</value>
        [JsonPropertyName("topLevelDomains")]
        public IList<string> topLevelDomains { get; set; }

        /// <summary>
        /// Company names used in creating DTMIs
        /// </summary>
        /// <value>list of company names to be used</value>
        [JsonPropertyName("companyNames")]
        public IList<string> companyNames { get; set; }

        /// <summary>
        /// Product names used in creating DTMIs
        /// </summary>
        /// <value>list of product names to be used</value>
        [JsonPropertyName("productNames")]
        public IList<string> productNames { get; set; }

        /// <summary>
        /// Read the data from the model metadata file
        /// </summary>
        /// <returns>ModelMetadate with deserialized data</returns>
        public static ModelMetadata DeserializeFromFile()
        {
            return JsonSerializer.Deserialize<ModelMetadata>(File.ReadAllText("modelMetadata.json"));
        }
    }
}