using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeIpaClient.Models
{
    internal sealed class FreeIpaBooleanJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.String => ParseString(reader.GetString()),
                JsonTokenType.Number => reader.GetInt32() != 0,
                _ => throw new JsonException($"Cannot convert JSON token {reader.TokenType} to boolean.")
            };
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }

        private static bool ParseString(string value)
        {
            if (bool.TryParse(value, out var parsed))
            {
                return parsed;
            }

            throw new JsonException($"Cannot convert JSON string '{value}' to boolean.");
        }
    }
}
