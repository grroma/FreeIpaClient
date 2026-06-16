# FreeIpaClient Example

Console example that references the local `FreeIpaClient` project and calls a FreeIPA server.
It demonstrates typed API calls and a dynamic call to an untyped FreeIPA command.

The project is marked with `IsPackable=false` and is not included in the NuGet package.

## Run

```bash
dotnet run --project examples/FreeIpaClient.Example
```

Default settings target a local FreeIPA instance:

```text
FREEIPA_HOST=https://ipa.test.local/ipa/
FREEIPA_USER=admin
FREEIPA_PASSWORD=Secret123
FREEIPA_ALLOW_INVALID_CERTIFICATE=true
```

`FREEIPA_API_VERSION` is optional. If it is not set, the client detects the API version via `env`.
