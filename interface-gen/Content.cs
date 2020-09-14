using System.Text.Json;
using System.Text.Json.Serialization;
namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class for serializing and deserializing dtdl interface content
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Type of the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("@type")]
        public string @type { get; set; }

        /// <summary>
        /// Name of the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("name")]
        public string name { get; set; }

        /// <summary>
        /// Schema of the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("schema")]
        public string schema { get; set; }

        /// <summary>
        /// Request for the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("request")]
        public Request request { get; set; }

        /// <summary>
        /// Display name for the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("displayName")]
        public string displayName { get; set; }

        /// <summary>
        /// Description for the content
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("description")]
        public string description { get; set; }
    }
}