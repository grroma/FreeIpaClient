# FreeIpaClient Code Generation Example

Console example that references the local `FreeIpaClient` project and uses generated
extension methods from `Generated/FreeIpaGenerated*.cs`.

The generated files in this example were created from `freeipa-json-metadata.sample.json`.
In a real project, generate them from your own FreeIPA server metadata.

## Regenerate

```bash
dotnet run --project tools/FreeIpaClient.CodeGen -- \
  --metadata examples/FreeIpaClient.CodeGenExample/freeipa-json-metadata.sample.json \
  --output-dir examples/FreeIpaClient.CodeGenExample/Generated \
  --namespace FreeIpaClient.CodeGenExample.Generated
```

For a larger generated API, replace the sample metadata with a `json_metadata`
response from your FreeIPA server and add `--prefix user`, `--prefix group`, or
`--command user_find` filters as needed.

## Run

```bash
dotnet run --project examples/FreeIpaClient.CodeGenExample
```

Default settings target a local FreeIPA instance:

```text
FREEIPA_HOST=https://ipa.test.local/ipa/
FREEIPA_USER=admin
FREEIPA_PASSWORD=Secret123
FREEIPA_ALLOW_INVALID_CERTIFICATE=true
```

`FREEIPA_API_VERSION` is optional. If it is not set, the client detects the API version via `env`.
