using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task Ping()
        {
            await _client.Ping();
            Assert.True(true);
        }
        
        [Fact]
        public async Task Ping_does_not_throw_Exception_in_case_of_second_use()
        {
            await _client.Ping();
            await _client.Ping();
            Assert.True(true);
        }

        [Fact]
        public async Task Ping_throws_exception_if_wrong_client_user_password_passed()
        {
            _config.Password = "wrong password";
            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.Ping());
            Assert.Equal("FreeIPA login error: invalid-password", ex.Message);
        }
    }
}