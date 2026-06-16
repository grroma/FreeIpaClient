using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FreeIpaClient.CodeGen;

public sealed class FreeIpaCodeGenerationOptions
{
    public string Namespace { get; set; } = "FreeIpaClient.Generated";
    public IReadOnlyCollection<string> CommandPrefixes { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Commands { get; set; } = Array.Empty<string>();
}

public sealed class FreeIpaGeneratedSources
{
    public FreeIpaGeneratedSources(string optionsSource, string clientExtensionsSource, int commandCount)
    {
        OptionsSource = optionsSource;
        ClientExtensionsSource = clientExtensionsSource;
        CommandCount = commandCount;
    }

    public string OptionsSource { get; }
    public string ClientExtensionsSource { get; }
    public int CommandCount { get; }
}

public static class FreeIpaMetadataCodeGenerator
{
    private static readonly string[] BaseOptionNames =
    {
        "all",
        "raw",
        "version",
        "no_members",
        "setattr",
        "addattr",
        "delattr"
    };

    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
        "checked", "class", "const", "continue", "decimal", "default", "delegate",
        "do", "double", "else", "enum", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
        "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
        "new", "null", "object", "operator", "out", "override", "params", "private",
        "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    public static FreeIpaGeneratedSources Generate(
        string metadataJson,
        FreeIpaCodeGenerationOptions? options = null)
    {
        options ??= new FreeIpaCodeGenerationOptions();

        using var document = JsonDocument.Parse(metadataJson);
        var commands = ReadCommands(GetMetadataRoot(document.RootElement))
            .Where(command => ShouldGenerate(command, options))
            .OrderBy(command => command.Name, StringComparer.Ordinal)
            .Select(ToGeneratedCommand)
            .ToArray();
        var model = new GeneratedModel(options.Namespace, commands, commands.SelectMany(command => command.Methods).ToArray());

        return new FreeIpaGeneratedSources(
            CodeTemplate.Render("Options.cs.mustache", model),
            CodeTemplate.Render("ClientExtensions.cs.mustache", model),
            commands.Length);
    }

    private static GeneratedCommand ToGeneratedCommand(CommandSpec command)
    {
        var usedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var options = command.Options
            .Where(option => !BaseOptionNames.Contains(option.Name, StringComparer.OrdinalIgnoreCase))
            .GroupBy(option => option.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Select(option => new GeneratedProperty(
                GetSummary(option.Doc, 2),
                EscapeStringLiteral(option.Name),
                GetCSharpType(option, optional: true),
                MakeUnique(ToPascalCase(option.Name), usedPropertyNames)))
            .ToArray();
        var optionsClassName = GetOptionsClassName(command.Name);

        return new GeneratedCommand(
            GetSummary(command.Doc, 1),
            optionsClassName,
            HasAttributeOptions(command) ? "FreeIpaRequestOptionsAttr" : "FreeIpaRequestOptions",
            options,
            new[]
            {
                ToGeneratedMethod(command, optionsClassName, resultEnvelope: false),
                ToGeneratedMethod(command, optionsClassName, resultEnvelope: true)
            });
    }

    private static GeneratedMethod ToGeneratedMethod(
        CommandSpec command,
        string optionsClassName,
        bool resultEnvelope)
    {
        var usedParameterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "client",
            "options",
            "sendNulls",
            "cancellationToken"
        };
        var parameters = command.Args
            .Select(argument =>
            {
                var name = MakeUnique(ToCamelCase(argument.Name), usedParameterNames);
                return new GeneratedParameter(
                    EscapeIdentifier(name),
                    GetCSharpType(argument, optional: !argument.Required),
                    argument.Required ? string.Empty : " = null",
                    argument.Multivalue.ToString().ToLowerInvariant());
            })
            .ToArray();

        return new GeneratedMethod(
            GetSummary(command.Doc, 2),
            resultEnvelope ? "Task<FreeIpaResult<JsonElement, JsonElement>>" : "Task<JsonElement>",
            ToPascalCase(command.Name) + (resultEnvelope ? "Result" : string.Empty),
            EscapeStringLiteral(command.Name),
            optionsClassName,
            resultEnvelope ? "PostResult" : "Post",
            parameters.Length > 0,
            parameters);
    }

    private static JsonElement GetMetadataRoot(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Object &&
            TryGetProperty(root, "result", out var result) &&
            result.ValueKind == JsonValueKind.Object &&
            TryGetProperty(result, "commands", out _))
        {
            return result;
        }

        return root;
    }

    private static IEnumerable<CommandSpec> ReadCommands(JsonElement metadata)
    {
        if (!TryGetProperty(metadata, "commands", out var commands) ||
            commands.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Metadata JSON does not contain result.commands.");
        }

        foreach (var commandProperty in commands.EnumerateObject())
        {
            if (commandProperty.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var command = commandProperty.Value;
            var name = ReadString(command, "name") ?? commandProperty.Name;
            yield return new CommandSpec(
                name,
                ReadString(command, "doc") ?? string.Empty,
                ReadParameters(command, "takes_args"),
                ReadParameters(command, "takes_options"));
        }
    }

    private static ParamSpec[] ReadParameters(JsonElement command, string propertyName)
    {
        if (!TryGetProperty(command, propertyName, out var parameters) ||
            parameters.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<ParamSpec>();
        }

        var result = new List<ParamSpec>();
        foreach (var item in parameters.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var name = ReadString(item, "name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            result.Add(new ParamSpec(
                name,
                ReadString(item, "type") ?? "str",
                ReadString(item, "doc") ?? string.Empty,
                ReadBool(item, "required"),
                ReadBool(item, "multivalue")));
        }

        return result.ToArray();
    }

    private static bool ShouldGenerate(CommandSpec command, FreeIpaCodeGenerationOptions options)
    {
        var commands = options.Commands ?? Array.Empty<string>();
        var prefixes = options.CommandPrefixes ?? Array.Empty<string>();

        if (commands.Count == 0 && prefixes.Count == 0)
        {
            return true;
        }

        if (commands.Contains(command.Name, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        return prefixes.Any(prefix =>
            command.Name.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
            command.Name.StartsWith(prefix + "_", StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasAttributeOptions(CommandSpec command)
    {
        return command.Options.Any(option =>
            option.Name.Equals("setattr", StringComparison.OrdinalIgnoreCase) ||
            option.Name.Equals("addattr", StringComparison.OrdinalIgnoreCase) ||
            option.Name.Equals("delattr", StringComparison.OrdinalIgnoreCase));
    }

    private static string GetOptionsClassName(string commandName)
    {
        return $"FreeIpa{ToPascalCase(commandName)}Options";
    }

    private static string GetCSharpType(ParamSpec parameter, bool optional)
    {
        var scalar = parameter.Type.ToLowerInvariant() switch
        {
            "bool" => "bool",
            "int" => "int",
            "decimal" => "decimal",
            "dict" => "object",
            "bytes" => "byte[]",
            _ => "string"
        };

        if (parameter.Multivalue)
        {
            return scalar == "byte[]" ? "byte[][]" : scalar + "[]";
        }

        if (scalar is "string" or "object" or "byte[]")
        {
            return scalar;
        }

        return optional ? scalar + "?" : scalar;
    }

    private static string GetSummary(string text, int indentationLevel)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var indentation = new string(' ', indentationLevel * 4);
        var builder = new StringBuilder()
            .Append(indentation)
            .AppendLine("/// <summary>");

        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0)
            {
                builder.Append(indentation)
                    .Append("/// ")
                    .AppendLine(EscapeXml(trimmed));
            }
        }

        return builder
            .Append(indentation)
            .AppendLine("/// </summary>")
            .ToString();
    }

    private static string ToPascalCase(string value)
    {
        var builder = new StringBuilder();
        var upperNext = true;

        foreach (var character in value)
        {
            if (!char.IsLetterOrDigit(character))
            {
                upperNext = true;
                continue;
            }

            if (builder.Length == 0 && char.IsDigit(character))
            {
                builder.Append("Value");
            }

            builder.Append(upperNext ? char.ToUpperInvariant(character) : character);
            upperNext = false;
        }

        return builder.Length == 0 ? "Value" : builder.ToString();
    }

    private static string ToCamelCase(string value)
    {
        var pascal = ToPascalCase(value);
        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    private static string MakeUnique(string preferredName, ISet<string> usedNames)
    {
        var name = preferredName;
        var suffix = 2;

        while (!usedNames.Add(name))
        {
            name = preferredName + suffix.ToString(CultureInfo.InvariantCulture);
            suffix++;
        }

        return name;
    }

    private static string EscapeIdentifier(string value)
    {
        return CSharpKeywords.Contains(value) ? "@" + value : value;
    }

    private static string? ReadString(JsonElement element, string propertyName)
    {
        return TryGetProperty(element, propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static bool ReadBool(JsonElement element, string propertyName)
    {
        return TryGetProperty(element, propertyName, out var property) &&
            property.ValueKind == JsonValueKind.True;
    }

    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement property)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            property = default;
            return false;
        }

        if (element.TryGetProperty(propertyName, out property))
        {
            return true;
        }

        foreach (var candidate in element.EnumerateObject())
        {
            if (candidate.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = candidate.Value;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static string EscapeStringLiteral(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private static string EscapeXml(string value)
    {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }

    private sealed record CommandSpec(
        string Name,
        string Doc,
        IReadOnlyList<ParamSpec> Args,
        IReadOnlyList<ParamSpec> Options);

    private sealed record ParamSpec(
        string Name,
        string Type,
        string Doc,
        bool Required,
        bool Multivalue);

    private sealed record GeneratedModel(
        string Namespace,
        IReadOnlyList<GeneratedCommand> Commands,
        IReadOnlyList<GeneratedMethod> Methods);

    private sealed record GeneratedCommand(
        string Summary,
        string OptionsClassName,
        string OptionsBaseClass,
        IReadOnlyList<GeneratedProperty> Properties,
        IReadOnlyList<GeneratedMethod> Methods);

    private sealed record GeneratedProperty(
        string Summary,
        string JsonName,
        string TypeName,
        string Name);

    private sealed record GeneratedMethod(
        string Summary,
        string ReturnType,
        string Name,
        string RpcName,
        string OptionsClassName,
        string PostMethod,
        bool HasArguments,
        IReadOnlyList<GeneratedParameter> Parameters);

    private sealed record GeneratedParameter(
        string Name,
        string TypeName,
        string DefaultValue,
        string MultivalueLiteral);
}
