using System.Text.Json;
using FreeIpaClient;
using FreeIpaClient.CodeGenExample.Generated;
using FreeIpaClient.Models;

var host = GetStringEnvironmentVariable("FREEIPA_HOST", "https://ipa.test.local/ipa/");
var user = GetStringEnvironmentVariable("FREEIPA_USER", "admin");
var password = GetStringEnvironmentVariable("FREEIPA_PASSWORD", "Secret123");
var apiVersion = Environment.GetEnvironmentVariable("FREEIPA_API_VERSION");
var allowInvalidCertificate = GetBooleanEnvironmentVariable("FREEIPA_ALLOW_INVALID_CERTIFICATE", true);

using var httpClientHandler = new HttpClientHandler();
if (allowInvalidCertificate)
{
    httpClientHandler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
}

using var httpClient = new HttpClient(httpClientHandler);

var client = new FreeIpaApiClient(httpClient, new FreeIpaConfig
{
    Host = new Uri(host),
    User = user,
    Password = password,
    ApiVersion = apiVersion
});

try
{
    await client.Ping();

    Console.WriteLine($"Connected to FreeIPA at {host}");
    Console.WriteLine($"API version: {await client.GetApiVersion()}");

    var userFind = await client.UserFindResult(
        options: new FreeIpaUserFindOptions
        {
            Uid = user,
            PkeyOnly = true
        });

    Console.WriteLine($"Generated user_find count: {userFind.Count}");
    Console.WriteLine($"Generated user_find contains '{user}': {ContainsValue(userFind.Result, "uid", user)}");

    var groupFind = await client.GroupFindResult(
        options: new FreeIpaGroupFindOptions
        {
            Cn = "admins",
            PkeyOnly = true
        });

    Console.WriteLine($"Generated group_find count: {groupFind.Count}");
    Console.WriteLine($"Generated group_find contains 'admins': {ContainsValue(groupFind.Result, "cn", "admins")}");
}
finally
{
    try
    {
        await client.SessionLogout();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Logout skipped: {ex.Message}");
    }
}

static string GetStringEnvironmentVariable(string name, string defaultValue)
{
    return Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
        ? value
        : defaultValue;
}

static bool GetBooleanEnvironmentVariable(string name, bool defaultValue)
{
    var value = Environment.GetEnvironmentVariable(name);

    return value == null
        ? defaultValue
        : bool.TryParse(value, out var parsed) && parsed;
}

static bool ContainsValue(JsonElement result, string propertyName, string expectedValue)
{
    if (result.ValueKind != JsonValueKind.Array)
    {
        return false;
    }

    foreach (var item in result.EnumerateArray())
    {
        if (GetFirstString(item, propertyName) == expectedValue)
        {
            return true;
        }
    }

    return false;
}

static string? GetFirstString(JsonElement item, string propertyName)
{
    if (item.ValueKind != JsonValueKind.Object ||
        !item.TryGetProperty(propertyName, out var property) ||
        property.ValueKind != JsonValueKind.Array)
    {
        return null;
    }

    foreach (var value in property.EnumerateArray())
    {
        return value.GetString();
    }

    return null;
}
