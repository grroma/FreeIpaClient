using System.Linq;
using System.Threading.Tasks;
using FreeIpaClient.RequestOptions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task UserFindResult_returns_result_metadata()
        {
            var userAddOptions = NewUserRequestOptionsFixture();
            var addedUser = await _client.UserAdd(userAddOptions);
            MarkForCleanup(addedUser);

            var result = await _client.UserFindResult(new FreeIpaUserFindRequestOptions
            {
                Uid = userAddOptions.Uid
            });

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.False(result.Truncated.GetValueOrDefault());
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
        }

        [Fact]
        public async Task Env_returns_api_version_for_autodetect()
        {
            var apiVersion = await _client.GetApiVersion();
            var env = await _client.Env();

            Assert.False(string.IsNullOrWhiteSpace(apiVersion));
            Assert.Equal(apiVersion, env.ApiVersion);
            Assert.False(string.IsNullOrWhiteSpace(env.Version));
        }

        [Fact]
        public async Task CommandShow_returns_command_schema()
        {
            var command = await _client.CommandShow("user_show");

            Assert.Equal("user_show", command.Name);
            Assert.Contains("uid", command.ParamsParam);
            Assert.Contains("raw", command.ParamsParam);
            Assert.Contains("all", command.ParamsParam);
        }

        [Fact]
        public async Task JsonMetadata_returns_schema_sections()
        {
            var metadata = await _client.JsonMetadata();

            Assert.True(metadata.Objects.Properties().Any());
            Assert.True(metadata.Commands.Properties().Any());
        }

        [Fact]
        public async Task DynamicRequestOptions_can_call_untyped_group_find()
        {
            var result = await _client.PostResult<JObject[], string>(
                "group_find",
                new FreeIpaDynamicRequestOptions()
                    .Add("cn", "admins")
                    .Add("pkey_only", false),
                all: true,
                raw: true);

            Assert.NotNull(result);
            Assert.True(result.Count >= 1);
            Assert.Contains(result.Result, group =>
                group["cn"]?.Values<string>().Contains("admins") == true);
        }
    }
}
