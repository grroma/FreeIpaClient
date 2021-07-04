using Newtonsoft.Json.Converters;

namespace FreeIpaClient.Models
{
    public class FreeIpaDateTimeConverter : IsoDateTimeConverter
    {
        public const string Format = "yyyyMMddHHmmssZ";
        public FreeIpaDateTimeConverter()
        {
            DateTimeFormat = Format;
        }
    }
}