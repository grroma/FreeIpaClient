using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeIpaClient.Models
{
    public class FreeIpaEnvironment
    {
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; }

        public string Version { get; set; }
        public string Realm { get; set; }
        public string Domain { get; set; }
        public string Host { get; set; }
        public string Server { get; set; }
        public string Basedn { get; set; }
        public string Context { get; set; }

        [JsonPropertyName("jsonrpc_uri")]
        public string JsonRpcUri { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> AdditionalData { get; set; }
    }
}
