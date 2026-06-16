using System.Text.Json;

namespace FreeIpaClient.Models
{
    public class FreeIpaJsonMetadata
    {
        public JsonElement Objects { get; set; }
        public JsonElement Methods { get; set; }
        public JsonElement Commands { get; set; }
    }
}
