using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task UserDisable_disables_user()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            var result = await _client.UserDisable(
                new FreeIpaUserDisableRequestOptions()
                {
                    Uid = addUserOptions.Uid,
                });

            Assert.True(result);

            var disabledUser = await _client.UserFind(new FreeIpaUserFindRequestOptions() { Uid = addUserOptions.Uid });

            Assert.True(disabledUser[0].Nsaccountlock[0]);
        }

        [Fact]
        public async Task UserDisable_throws_exception_if_user_already_disabled()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            var options = new FreeIpaUserDisableRequestOptions()
            {
                Uid = addUserOptions.Uid,
            };

            await _client.UserDisable(options);

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.UserDisable(options));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.True(ex.IsAlreadyInactive);
        }

        [Fact]
        public async Task UserDisable_throws_exception_if_wrong_user_passed()
        {
            var options = new FreeIpaUserDisableRequestOptions()
            {
                Uid = "wrong_user"
            };

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.UserDisable(options));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.True(ex.IsNotFound);
        }
    }
}