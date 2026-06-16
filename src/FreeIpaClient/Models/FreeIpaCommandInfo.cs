using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeIpaClient.Models
{
    public class FreeIpaCommandInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("params_param")]
        public string[] ParamsParam { get; set; }

        public string Doc { get; set; }

        [JsonPropertyName("topic_topic")]
        public string TopicTopic { get; set; }

        [JsonPropertyName("obj_class")]
        public string ObjectClass { get; set; }

        [JsonPropertyName("attr_name")]
        public string AttributeName { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> AdditionalData { get; set; }
    }
}
