using FreeIpaClient.CodeGen;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public class CodeGenTests
    {
        [Fact]
        public void Generate_creates_options_and_extension_methods_from_json_metadata()
        {
            var sources = FreeIpaMetadataCodeGenerator.Generate(SampleMetadata, new FreeIpaCodeGenerationOptions
            {
                Namespace = "FreeIpaClient.Generated",
                CommandPrefixes = new[] { "group" }
            });

            Assert.Equal(2, sources.CommandCount);
            Assert.Contains("public sealed class FreeIpaGroupAddOptions", sources.OptionsSource);
            Assert.Contains("[JsonPropertyName(\"gidnumber\")]", sources.OptionsSource);
            Assert.Contains("public int? Gidnumber { get; set; }", sources.OptionsSource);
            Assert.Contains("public bool? Nonposix { get; set; }", sources.OptionsSource);
            Assert.Contains("public static Task<JsonElement> GroupAdd(", sources.ClientExtensionsSource);
            Assert.Contains("public static Task<FreeIpaResult<JsonElement, JsonElement>> GroupFindResult(", sources.ClientExtensionsSource);
            Assert.Contains("AppendArgument(args, cn, multivalue: false);", sources.ClientExtensionsSource);
            Assert.DoesNotContain("UserShow", sources.ClientExtensionsSource);
        }

        [Fact]
        public void Generate_accepts_case_insensitive_metadata_property_names()
        {
            var sources = FreeIpaMetadataCodeGenerator.Generate(PascalCaseSampleMetadata, new FreeIpaCodeGenerationOptions
            {
                Namespace = "FreeIpaClient.Generated",
                Commands = new[] { "user_find" }
            });

            Assert.Equal(1, sources.CommandCount);
            Assert.Contains("public sealed class FreeIpaUserFindOptions", sources.OptionsSource);
            Assert.Contains("public static Task<JsonElement> UserFind(", sources.ClientExtensionsSource);
        }

        private const string SampleMetadata = """
            {
              "result": {
                "commands": {
                  "group_add": {
                    "name": "group_add",
                    "doc": "Create a new group.",
                    "takes_args": [
                      {
                        "name": "cn",
                        "type": "str",
                        "doc": "Group name",
                        "required": true,
                        "multivalue": false
                      }
                    ],
                    "takes_options": [
                      {
                        "name": "description",
                        "type": "str",
                        "doc": "Group description",
                        "required": false,
                        "multivalue": false
                      },
                      {
                        "name": "gidnumber",
                        "type": "int",
                        "doc": "GID",
                        "required": false,
                        "multivalue": false
                      },
                      {
                        "name": "nonposix",
                        "type": "bool",
                        "doc": "Create as a non-POSIX group",
                        "required": true,
                        "multivalue": false
                      },
                      {
                        "name": "all",
                        "type": "bool",
                        "doc": "Retrieve all attributes",
                        "required": true,
                        "multivalue": false
                      }
                    ]
                  },
                  "group_find": {
                    "name": "group_find",
                    "doc": "Search for groups.",
                    "takes_args": [
                      {
                        "name": "criteria",
                        "type": "str",
                        "doc": "Search criteria",
                        "required": false,
                        "multivalue": false
                      }
                    ],
                    "takes_options": [
                      {
                        "name": "cn",
                        "type": "str",
                        "doc": "Group name",
                        "required": false,
                        "multivalue": false
                      }
                    ]
                  },
                  "user_show": {
                    "name": "user_show",
                    "doc": "Display user.",
                    "takes_args": [
                      {
                        "name": "uid",
                        "type": "str",
                        "doc": "User login",
                        "required": true,
                        "multivalue": false
                      }
                    ],
                    "takes_options": []
                  }
                }
              },
              "error": null
            }
            """;

        private const string PascalCaseSampleMetadata = """
            {
              "Commands": {
                "user_find": {
                  "Name": "user_find",
                  "Doc": "Search for users.",
                  "Takes_Args": [
                    {
                      "Name": "criteria",
                      "Type": "str",
                      "Doc": "Search criteria",
                      "Required": false,
                      "Multivalue": false
                    }
                  ],
                  "Takes_Options": [
                    {
                      "Name": "uid",
                      "Type": "str",
                      "Doc": "User login",
                      "Required": false,
                      "Multivalue": false
                    }
                  ]
                }
              }
            }
            """;
    }
}
