using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task UserEnable_enables_user()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            await _client.UserDisable(
                new FreeIpaUserDisableRequestOptions()
                {
                    Uid = addUserOptions.Uid
                });


            var result = await _client.UserEnable(
                new FreeIpaUserEnableRequestOptions()
                {
                    Uid = addUserOptions.Uid
                });

            Assert.True(result);

            var enabledUser = await _client.UserFind(new FreeIpaUserFindRequestOptions() { Uid = addUserOptions.Uid });

            Assert.False(enabledUser[0].Nsaccountlock[0]);
        }

        [Fact]
        public async Task UserEnable_throws_exception_if_user_already_enabled()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            var options = new FreeIpaUserEnableRequestOptions()
            {
                Uid = addUserOptions.Uid
            };

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.UserEnable(options));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.True(ex.IsAlreadyActive);
        }

        [Fact]
        public async Task UserEnable_throws_exception_if_wrong_user_passed()
        {
            var options = new FreeIpaUserEnableRequestOptions()
            {
                Uid = "wrong_user"
            };

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.UserEnable(options));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.True(ex.IsNotFound);
        }
    }
}