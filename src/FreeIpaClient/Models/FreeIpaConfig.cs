using System;

namespace FreeIpaClient.Models
{
    public class FreeIpaConfig
    {
        public Uri Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ApiVersion  { get; set; }
        public bool AutoDetectApiVersion { get; set; } = true;
        public bool RetryOnUnauthorized { get; set; } = true;
    }
}
