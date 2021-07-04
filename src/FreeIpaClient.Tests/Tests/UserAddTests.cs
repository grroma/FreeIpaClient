using System.Threading.Tasks;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserAdd(bool stage)
        {
            var options = NewUserRequestOptionsFixture();
            var result = await _client.UserAdd(options, stage);

            Assert.NotNull(result);
            Assert.Equal(stage, result.Stage);
        }
    }
}