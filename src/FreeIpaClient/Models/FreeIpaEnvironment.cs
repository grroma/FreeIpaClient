using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreeIpaClient.Models
{
    public class FreeIpaEnvironment
    {
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        public string Version { get; set; }
        public string Realm { get; set; }
        public string Domain { get; set; }
        public string Host { get; set; }
        public string Server { get; set; }
        public string Basedn { get; set; }
        public string Context { get; set; }

        [JsonProperty("jsonrpc_uri")]
        public string JsonRpcUri { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}
