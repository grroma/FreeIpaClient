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

There is also a code generation example in `examples/FreeIpaClient.CodeGenExample`.
It shows how generated extension methods are added to an application project.

```bash
dotnet run --project examples/FreeIpaClient.CodeGenExample
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

## Built-in typed methods

[FreeIPA API navigator](https://ipa.demo1.freeipa.org/ipa/ui/#/p/apibrowser/type=command)
(maybe you need authorization : admin / Secret123)

The NuGet package contains typed methods for these FreeIPA commands:

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

`stageuser_add`, `stageuser_mod`, and `stageuser_del` are exposed through the user
methods with `stage: true`.

For commands where FreeIPA returns response metadata, use `PostResult`, `PostResponse`,
`UserFindResult`, or `StageUserFindResult` to access `count`, `truncated`, `failed`,
`completed`, and `messages`.

For commands that are not built in, use `Post`, `PostResult`, `PostResponse`, or generate
typed wrappers from `json_metadata`.

## Code generation from FreeIPA metadata

The repository contains a code generation tool that can generate typed request options
and extension methods from a FreeIPA `json_metadata` response.

```bash
dotnet run --project tools/FreeIpaClient.CodeGen -- \
  --metadata ./freeipa-json-metadata.json \
  --output-dir ./generated/freeipa \
  --namespace MyProject.FreeIpa.Generated
```

You can limit generation to selected API areas:

```bash
dotnet run --project tools/FreeIpaClient.CodeGen -- \
  --metadata ./freeipa-json-metadata.json \
  --output-dir ./generated/freeipa \
  --namespace MyProject.FreeIpa.Generated \
  --prefix user \
  --prefix group
```

Use `--command user_add` for a single command. Generated methods return `JsonElement`;
the matching `*Result` methods return the full `FreeIpaResult<JsonElement, JsonElement>`
envelope with response metadata.

The generator is intentionally a separate non-packable tool. Generated files are included
only in the project where you place them; they are not added to the `FreeIpaClient` NuGet
package automatically.

## Tests
You can run tests to verify that your application is working correctly. To do this, edit the file `testsettings.json`
