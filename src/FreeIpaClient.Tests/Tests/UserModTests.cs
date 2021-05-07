using System.Threading.Tasks;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserMod(bool stage)
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            await _client.UserAdd(addUserOptions, stage);

            var userModOptions = NewUserRequestOptionsFixture();
            userModOptions.Uid = addUserOptions.Uid;

            var userModResult = await _client.UserMod(userModOptions, stage);

            Assert.NotNull(userModResult);
            Assert.Equal(stage, userModResult.Stage);
            AssertUser(userModOptions, userModResult);

        }
    }
}