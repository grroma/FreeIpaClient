using FreeIpaClient.CodeGen;

var arguments = CodeGenArguments.Parse(args);
if (arguments.ShowHelp)
{
    Console.WriteLine(CodeGenArguments.HelpText);
    return 0;
}

if (!arguments.IsValid(out var error))
{
    Console.Error.WriteLine(error);
    Console.Error.WriteLine();
    Console.Error.WriteLine(CodeGenArguments.HelpText);
    return 1;
}

var metadataJson = await File.ReadAllTextAsync(arguments.MetadataPath);
var sources = FreeIpaMetadataCodeGenerator.Generate(metadataJson, new FreeIpaCodeGenerationOptions
{
    Namespace = arguments.Namespace,
    CommandPrefixes = arguments.CommandPrefixes,
    Commands = arguments.Commands
});

Directory.CreateDirectory(arguments.OutputDirectory);

var optionsPath = Path.Combine(arguments.OutputDirectory, "FreeIpaGeneratedOptions.cs");
var extensionsPath = Path.Combine(arguments.OutputDirectory, "FreeIpaGeneratedClientExtensions.cs");

await File.WriteAllTextAsync(optionsPath, sources.OptionsSource);
await File.WriteAllTextAsync(extensionsPath, sources.ClientExtensionsSource);

Console.WriteLine($"Generated {sources.CommandCount} FreeIPA command wrappers.");
Console.WriteLine(optionsPath);
Console.WriteLine(extensionsPath);
return 0;

internal sealed class CodeGenArguments
{
    public const string HelpText = """
        FreeIpaClient.CodeGen

        Required:
          --metadata <path>       Path to FreeIPA json_metadata response JSON.
          --output-dir <path>     Directory for generated .cs files.

        Optional:
          --namespace <name>      Generated namespace. Default: FreeIpaClient.Generated
          --prefix <name>         Generate commands with this prefix, e.g. group or host. Can be repeated or comma-separated.
          --command <name>        Generate a single command, e.g. group_add. Can be repeated or comma-separated.
          --help                  Show this help.

        If neither --prefix nor --command is specified, all commands from metadata are generated.
        """;

    public string MetadataPath { get; private set; } = string.Empty;
    public string OutputDirectory { get; private set; } = string.Empty;
    public string Namespace { get; private set; } = "FreeIpaClient.Generated";
    public string[] CommandPrefixes { get; private set; } = Array.Empty<string>();
    public string[] Commands { get; private set; } = Array.Empty<string>();
    public bool ShowHelp { get; private set; }

    public static CodeGenArguments Parse(string[] args)
    {
        var parsed = new CodeGenArguments();
        var prefixes = new List<string>();
        var commands = new List<string>();

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            switch (arg)
            {
                case "--help":
                case "-h":
                    parsed.ShowHelp = true;
                    break;
                case "--metadata":
                    parsed.MetadataPath = ReadValue(args, ref index, arg);
                    break;
                case "--output-dir":
                    parsed.OutputDirectory = ReadValue(args, ref index, arg);
                    break;
                case "--namespace":
                    parsed.Namespace = ReadValue(args, ref index, arg);
                    break;
                case "--prefix":
                    prefixes.AddRange(SplitValues(ReadValue(args, ref index, arg)));
                    break;
                case "--command":
                    commands.AddRange(SplitValues(ReadValue(args, ref index, arg)));
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{arg}'.");
            }
        }

        parsed.CommandPrefixes = prefixes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        parsed.Commands = commands.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        return parsed;
    }

    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(MetadataPath))
        {
            error = "--metadata is required.";
            return false;
        }

        if (!File.Exists(MetadataPath))
        {
            error = $"Metadata file '{MetadataPath}' does not exist.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(OutputDirectory))
        {
            error = "--output-dir is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Namespace))
        {
            error = "--namespace cannot be empty.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static string ReadValue(string[] args, ref int index, string name)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"{name} requires a value.");
        }

        index++;
        return args[index];
    }

    private static IEnumerable<string> SplitValues(string value)
    {
        return value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }
}
