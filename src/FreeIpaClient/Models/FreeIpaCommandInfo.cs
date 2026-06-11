using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreeIpaClient.Models
{
    public class FreeIpaCommandInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("params_param")]
        public string[] ParamsParam { get; set; }

        public string Doc { get; set; }

        [JsonProperty("topic_topic")]
        public string TopicTopic { get; set; }

        [JsonProperty("obj_class")]
        public string ObjectClass { get; set; }

        [JsonProperty("attr_name")]
        public string AttributeName { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}
