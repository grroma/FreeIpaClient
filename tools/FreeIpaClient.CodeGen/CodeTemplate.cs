using Stubble.Core.Builders;

namespace FreeIpaClient.CodeGen;

internal static class CodeTemplate
{
    private const string ResourcePrefix = "FreeIpaClient.CodeGen.Templates.";

    public static string Render(string name, object model)
    {
        var template = Read(name);
        return new StubbleBuilder().Build().Render(template, model);
    }

    private static string Read(string name)
    {
        var assembly = typeof(CodeTemplate).Assembly;
        var resourceName = ResourcePrefix + name;
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            var resources = string.Join(", ", assembly.GetManifestResourceNames());
            throw new InvalidOperationException($"Template resource '{resourceName}' was not found. Available: {resources}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
