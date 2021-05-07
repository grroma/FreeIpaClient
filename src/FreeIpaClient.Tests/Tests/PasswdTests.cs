using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task Passwd_sets_initial_password()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);
            var passwdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = addUserOptions.Uid,
                Password = "Value123"
            };

            var passwdResponse = await _client.Passwd(passwdOptions);

            Assert.True(passwdResponse);
        }


        [Fact]
        public async Task Passwd_updates_password()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            var initialPasswdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = addUserOptions.Uid,
                Password = "Value123"
            };

            await _client.Passwd(initialPasswdOptions);

            var updatePasswdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = addUserOptions.Uid,
                Password = "Update123",
                Current_password = "Value123"
            };

            var updatePasswdResponse = await _client.Passwd(updatePasswdOptions);
            Assert.True(updatePasswdResponse);
        }

        [Fact]
        public async Task Passwd_throws_exception_on_update_password_if_passed_wrong_current_password()
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions);
            MarkForCleanup(addUserResult);

            var initialPasswdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = addUserOptions.Uid,
                Password = "Value123"
            };

            await _client.Passwd(initialPasswdOptions);

            var updatePasswdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = addUserOptions.Uid,
                Password = "Update123",
                Current_password = "Wrong password"
            };

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.Passwd(updatePasswdOptions));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.Equal(FreeIpaException.InvalidCredential, ex.Error.Code);
        }

        [Fact]
        public async Task Passwd_throws_exception_on_update_password_if_passed_wrong_user()
        {
            var updatePasswdOptions = new FreeIpaPasswdRequestOptions()
            {
                Principal = "wrong user",
                Password = "Update123"
            };

            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () => await _client.Passwd(updatePasswdOptions));

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.Equal(FreeIpaException.NoMatchingEntryFound, ex.Error.Code);
        }
    }
}