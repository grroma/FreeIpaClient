using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FreeIpaClient.Tests
{
    public class FreeIpaClientTests
    {
        private readonly IFreeIpaApiClient _client;

        public FreeIpaClientTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build()
                .GetSection("FreeIPA")
                .Get<FreeIpaConfig>();

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(httpClientHandler);
            _client = new FreeIpaApiClient(httpClient, config);
        }
        
        [Fact]
        public async Task Ping()
        {
            await _client.Ping();
            Assert.True(true);
        }
        
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

        #region Helpers
        private static FreeIpaUserRequestOptions NewUserRequestOptionsFixture()
        {
            var uid = Guid.NewGuid().ToString("N");
            var id8 = uid[..8];

            return new FreeIpaUserRequestOptions
            {
                Uid = uid,
                Givenname = $"first{id8}",
                Sn = $"last{id8}",
                Cn = $"fullname{id8}",
                Mail = $"{id8}@example.com",
                Mobile = $"+7{new Random().Next(1000000000, int.MaxValue)}",
                Ou = $"title{id8}",
                Title = $"department{id8}",
                Telephonenumber = $"+7{new Random().Next(1000000000, int.MaxValue)}"
            };
        }
        
        private void AssertUser(FreeIpaUserRequestOptions options, FreeIpaUser user)
        {
            using (new AssertionScope())
            {
                options.Uid.Should().Equals(user.Uid.Single());
                options.Givenname.Should().Equals(user.Givenname.Single());
                options.Sn.Should().Equals(user.Sn.Single());
                options.Cn.Should().Equals(user.Cn.Single());
                options.Mail.Should().Equals(user.Mail.Single());
                options.Mobile.Should().Equals(user.Mobile.Single());
                options.Ou.Should().Equals(user.Ou.Single());
                options.Title.Should().Equals(user.Title.Single());
            }
        }
        #endregion
    }
}