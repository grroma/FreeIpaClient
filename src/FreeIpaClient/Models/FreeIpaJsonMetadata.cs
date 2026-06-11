using Newtonsoft.Json.Linq;

namespace FreeIpaClient.Models
{
    public class FreeIpaJsonMetadata
    {
        public JObject Objects { get; set; }
        public JObject Methods { get; set; }
        public JObject Commands { get; set; }
    }
}
