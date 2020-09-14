using System.Text.Json;
using System.Text.Json.Serialization;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Used to serialize/deserialize command requests
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Name of the request data
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("name")]
        public string name { get; set; }

        /// <summary>
        /// Schema of the request data
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("schema")]
        public string schema { get; set; }
    }
}