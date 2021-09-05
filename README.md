# FreeIpaClient

The client implements interaction with the API of the FreeIPA server.

## Install [NuGet package](https://www.nuget.org/packages/FreeIpaClient/1.0.0)

`dotnet add package FreeIpaClient --version 1.0.0`

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
            Password = "Secret123",
            ApiVersion = "2.240"
        }));
   // ...            
```
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
```

## Tests
You can run tests to verify that your application is working correctly. To do this, edit the file `testsettings.json`
