using System.Threading.Tasks;
using FreeIpaClient.RequestOptions;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests
    {
        [Fact]
        public async Task UserShow_returns_single_user_by_id_with_all_mapped_fields()
        {
            var userAddOptions = NewUserRequestOptionsFixture();
            var addedUser = await UserAddForTest(userAddOptions);
            MarkForCleanup(addedUser);

            var user = await _client.UserShow(new FreeIpaUserShowRequestOptions { Uid = userAddOptions.Uid });

            Assert.NotNull(user);
            Assert.False(user.Stage);
            AssertUser(userAddOptions, user);
            Assert.False(string.IsNullOrWhiteSpace(user.Dn));
            Assert.NotEmpty(user.Homedirectory);
            Assert.NotEmpty(user.Loginshell);
            Assert.NotEmpty(user.Uidnumber);
            Assert.NotEmpty(user.Gidnumber);
        }
    }
}
