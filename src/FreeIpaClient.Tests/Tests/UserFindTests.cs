using System.Threading.Tasks;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserFind_returns_single_user_by_id_with_all_mapped_fields(bool stage)
        {
            var userAddOptions = NewUserRequestOptionsFixture();
            var addedUser = await _client.UserAdd(userAddOptions, stage);
            MarkForCleanup(addedUser);

            var users = await _client
                .UserFind(new FreeIpaUserFindRequestOptions { Uid = userAddOptions.Uid });

            Assert.NotNull(users);
            Assert.Single(users);
            var foundUser = users[0];
            Assert.NotNull(foundUser);
            AssertUser(userAddOptions, foundUser);
            Assert.Equal(stage, foundUser.Stage);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserFind_returns_single_user_by_mail_with_all_mapped_fields(bool stage)
        {
            var userAddOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(userAddOptions, stage);
            MarkForCleanup(addUserResult);

            var users = await _client
                .UserFind(new FreeIpaUserFindRequestOptions { Mail = userAddOptions.Mail });

            Assert.NotNull(users);
            Assert.Single(users);
            var foundUser = users[0];
            Assert.NotNull(foundUser);
            AssertUser(userAddOptions, foundUser);
            Assert.Equal(stage, foundUser.Stage);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserFind_returns_single_user_by_mobile_with_all_mapped_fields(bool stage)
        {
            var userAddOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(userAddOptions, stage);
            MarkForCleanup(addUserResult);

            var users = await _client
                .UserFind(new FreeIpaUserFindRequestOptions() { Mobile = userAddOptions.Mobile });

            Assert.NotNull(users);
            Assert.Single(users);
            var foundUser = users[0];
            Assert.NotNull(foundUser);
            AssertUser(userAddOptions, foundUser);
            Assert.Equal(stage, foundUser.Stage);
        }

        [Fact]
        public async Task UserFind_returns_empty_users_by_wrong_id()
        {

            var users = await _client.UserFind(new FreeIpaUserFindRequestOptions()
            {
                Uid = "wrongId"
            });

            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task UserFind_returns_empty_users_by_wrong_mail()
        {

            var users = await _client.UserFind(new FreeIpaUserFindRequestOptions()
            {
                Mail = "wrongId"
            });

            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task UserFind_returns_empty_users_by_wrong_mobile()
        {

            var users = await _client.UserFind(new FreeIpaUserFindRequestOptions()
            {
                Mobile = "wrongId"
            });

            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserFind_returns_two_users_by_not_unique_mail(bool stage)
        {
            var userAddOptions1 = NewUserRequestOptionsFixture();
            var addUserResult1 = await _client.UserAdd(userAddOptions1, stage);
            MarkForCleanup(addUserResult1);

            var userAddOptions2 = NewUserRequestOptionsFixture();
            userAddOptions2.Mail = userAddOptions1.Mail;
            var addUserResult2 = await _client.UserAdd(userAddOptions2, stage);
            MarkForCleanup(addUserResult2);

            var users = await _client
                .UserFind(new FreeIpaUserFindRequestOptions { Mail = userAddOptions1.Mail });

            Assert.NotNull(users);
            Assert.Equal(2, users.Length);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserFind_returns_two_usesr_by_not_unique_mobile(bool stage)
        {
            var userAddOptions1 = NewUserRequestOptionsFixture();
            var addUserResult1 = await _client.UserAdd(userAddOptions1, stage);
            MarkForCleanup(addUserResult1);

            var userAddOptions2 = NewUserRequestOptionsFixture();
            userAddOptions2.Mobile = userAddOptions1.Mobile;
            var addUserResult2 = await _client.UserAdd(userAddOptions2, stage);
            MarkForCleanup(addUserResult2);

            var users = await _client
                .UserFind(new FreeIpaUserFindRequestOptions() { Mobile = userAddOptions1.Mobile });

            Assert.NotNull(users);
            Assert.Equal(2, users.Length);
        }
    }
}