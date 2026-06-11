# FreeIpaClient

![dotnet](https://img.shields.io/badge/dotnet-10-blue?logo=dotnet)
[![nuget](https://img.shields.io/badge/nuget-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/FreeIpaClient)
[![test](https://github.com/grroma/FreeIpaClient/actions/workflows/test.yml/badge.svg)](https://github.com/grroma/FreeIpaClient/actions/workflows/test.yml)

The client implements interaction with the API of the FreeIPA server.

## Install [NuGet package](https://www.nuget.org/packages/FreeIpaClient)

`dotnet add package FreeIpaClient --version 10.0.0`

## Example

There is a runnable console example in `examples/FreeIpaClient.Example`.

```bash
dotnet run --project examples/FreeIpaClient.Example
```

## DI registration

For the FreeIpaClient's work, input parameters are required: `HttpClient()` and `FreeIpaConfig()`

Register DI in `Startup.cs.` For example:
```c#
 public void ConfigureServices(IServiceCollection services)
{
  // ...
    services.AddScoped<IFreeIpaApiClient, FreeIpaApiClient>(options => 
        new FreeIpaApiClient(new HttpClient(), new FreeIpaConfig
        {
            Host = new Uri("https://ipa.demo1.freeipa.org/ipa/"),
            User = "admin",
            Password = "Secret123"
        }));
   // ...            
```

`ApiVersion` is detected from FreeIPA `env` by default. You can still set it explicitly in `FreeIpaConfig.ApiVersion`
if you need to pin client/server compatibility.

## Options example
```json
{
  "FreeIPA": {
    "Host": "https://ipa.demo1.freeipa.org/ipa/",
    "User": "admin",
    "Password": "Secret123"
  }
}
```

## Implements methods:

[FreeIPA API navigator](https://ipa.demo1.freeipa.org/ipa/ui/#/p/apibrowser/type=command)
(maybe you need authorization : admin / Secret123)

```
- ping
- user_find
- user_add
- user_mod
- stageuser_find
- stageuser_add
- stageuser_mod
- passwd
- user_show
- user_disable
- user_enable
- user_del
- user_undel
- stageuser_del
- stageuser_activate
- session_logout
- env
- command_show
- json_metadata
```

For commands where FreeIPA returns response metadata, use `PostResult`, `PostResponse`,
`UserFindResult`, or `StageUserFindResult` to access `count`, `truncated`, `failed`,
`completed`, and `messages`.

## Tests
You can run tests to verify that your application is working correctly. To do this, edit the file `testsettings.json`
