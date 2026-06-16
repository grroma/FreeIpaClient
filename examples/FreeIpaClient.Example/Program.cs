using FreeIpaClient;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
using Newtonsoft.Json.Linq;

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

var uid = $"example{Guid.NewGuid():N}"[..19];

try
{
    await client.Ping();
    var detectedApiVersion = await client.GetApiVersion();
    Console.WriteLine($"Connected to FreeIPA at {host}");
    Console.WriteLine($"API version: {detectedApiVersion ?? "server default"}");

    var addOptions = new FreeIpaUserRequestOptions
    {
        Uid = uid,
        Givenname = "Example",
        Sn = "User",
        Cn = "Example User",
        Mail = $"{uid}@example.test",
        Mobile = "+70000000000",
        Title = "Integration example"
    };

    var createdUser = await client.UserAdd(addOptions);
    Console.WriteLine($"Created user: {createdUser.Uid.FirstOrDefault()}");

    var shownUser = await client.UserShow(new FreeIpaUserShowRequestOptions { Uid = uid });
    Console.WriteLine($"User DN: {shownUser.Dn}");
    Console.WriteLine($"Home directory: {shownUser.Homedirectory?.FirstOrDefault()}");

    var findResult = await client.UserFindResult(new FreeIpaUserFindRequestOptions { Mail = addOptions.Mail });
    Console.WriteLine($"Search count: {findResult.Count}");
    Console.WriteLine($"Search truncated: {findResult.Truncated.GetValueOrDefault()}");

    var groupFindResult = await client.PostResult<JObject[], string>(
        "group_find",
        new FreeIpaDynamicRequestOptions()
            .Add("cn", "admins")
            .Add("pkey_only", false),
        all: true,
        raw: true);
    var adminsGroup = groupFindResult.Result.FirstOrDefault(group =>
        group["cn"]?.Values<string>().Contains("admins") == true);

    Console.WriteLine($"Dynamic group_find count: {groupFindResult.Count}");
    Console.WriteLine($"Dynamic group_find first CN: {adminsGroup?["cn"]?.FirstOrDefault()}");
}
finally
{
    try
    {
        await client.UserDel(new FreeIpaUserDelRequestOptions
        {
            Uid = new[] { uid },
            Continue = true
        });
        Console.WriteLine($"Deleted user: {uid}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Cleanup skipped: {ex.Message}");
    }

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
