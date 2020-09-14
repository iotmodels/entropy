using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class for serialize/deserialize DTDL V2 Interfaces
    /// </summary>
    public class DtdlV2Interface
    {
        /// <summary>
        /// @context for the interface
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("@context")]
        public string @context { get; set; }

        /// <summary>
        /// @id for the interface
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("@id")]
        public string @id { get; set; }

        /// <summary>
        /// @type for the interface
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("@type")]
        public string @type { get; set; }

        /// <summary>
        /// Display name for the interface
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("displayName")]
        public string displayName { get; set; }

        /// <summary>
        /// List of all contents for the interface
        /// </summary>
        /// <value>must follow dtdl rules </value>
        [JsonPropertyName("contents")]
        public IList<Content> contents { get; set; }

        /// <summary>
        /// Serializes the interface to Json
        /// </summary>
        /// <returns>Json string of serialized interface</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize<DtdlV2Interface>(this, serializerOptions());
        }

        /// <summary>
        /// Serializes the interface to a Stream as Json
        /// </summary>
        /// <param name="utf8json">Stream to write json to</param>
        /// <returns>Task</returns>
        public Task Serialize(Stream utf8json)
        {
            return JsonSerializer.SerializeAsync<DtdlV2Interface>(utf8json, this, serializerOptions());
        }

        private static JsonSerializerOptions serializerOptions()
        {
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.WriteIndented = true;
            jso.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            return jso;
        }
    }
}