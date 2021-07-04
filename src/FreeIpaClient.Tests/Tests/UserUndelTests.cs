using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserDel_deletes_user(bool stage)
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions, stage);
            MarkForCleanup(addUserResult);

            var result = await _client
                .UserDel(new FreeIpaUserDelRequestOptions() { Uid = new string[] { addUserOptions.Uid } }, stage);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UserDel_does_not_throw_exception_on_adding_user_again_after_deletion(bool stage)
        {
            var addUserOptions = NewUserRequestOptionsFixture();
            var addUserResult = await _client.UserAdd(addUserOptions, stage);
            MarkForCleanup(addUserResult);

            await _client
                .UserDel(new FreeIpaUserDelRequestOptions() { Uid = new string[] { addUserOptions.Uid } }, stage);

            await _client.UserAdd(addUserOptions, stage);

            Assert.True(true);
        }


        [Fact]
        public async Task UserDel_throws_exception_if_wrong_user_passed()
        {
            var ex = await Assert.ThrowsAsync<FreeIpaException>(async () =>
                await _client.UserDel(
                    new FreeIpaUserDelRequestOptions()
                    {
                        Uid = new string[] { "wrong_user" }
                    })
            );

            Assert.NotNull(ex);
            Assert.NotNull(ex.Error);
            Assert.True(ex.IsNotFound);
        }
    }
}