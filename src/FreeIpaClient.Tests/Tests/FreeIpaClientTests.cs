using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FreeIpaClient.Tests.Tests
{
    [Trait("Category", "Integration")]
    public partial class FreeIpaClientTests : IDisposable
    {
        private const string TestUserPrefix = "fitest";

        private readonly FreeIpaConfig _config;
        private readonly IFreeIpaApiClient _client;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly HashSet<string> _usersToCleanup = new HashSet<string>();

        public FreeIpaClientTests()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build()
                .GetSection("FreeIPA")
                .Get<FreeIpaConfig>();

            _httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(_httpClientHandler);
            _client = new FreeIpaApiClient(_httpClient, _config);
            
        }

        public void Dispose()
        {
            CleanupUsers();
            
            _httpClient.Dispose();
            _httpClientHandler.Dispose();
        }
        
        private static FreeIpaUserRequestOptions NewUserRequestOptionsFixture()
        {
            var uid = $"{TestUserPrefix}{Guid.NewGuid():N}"[..24];
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
        
        private static void AssertUser(FreeIpaUserRequestOptions options, FreeIpaUser user)
        {
            Assert.Equal(options.Uid, user.Uid.Single());
            Assert.Equal(options.Givenname, user.Givenname.Single());
            Assert.Equal(options.Sn, user.Sn.Single());
            Assert.Equal(options.Cn, user.Cn.Single());
            Assert.Equal(options.Mail, user.Mail.Single());
            Assert.Equal(options.Mobile, user.Mobile.Single());
            Assert.Equal(options.Ou, user.Ou.Single());
            Assert.Equal(options.Title, user.Title.Single());
        }

        private Task<FreeIpaUser> UserAddForTest(
            FreeIpaUserRequestOptions options,
            bool stage = false)
        {
            MarkForCleanup(options.Uid);
            return _client.UserAdd(options, stage);
        }
        
        private void MarkForCleanup(FreeIpaUser user)
        {
            if (user is {Uid: { }} && user.Uid.Length != 0)
            {
                MarkForCleanup(user.Uid[0]);
            }
        }

        private void MarkForCleanup(string userId)
        {
            if (userId != null)
            {
                _usersToCleanup.Add(userId);
            }
        }

        private void CleanupUsers()
        {
            if (_usersToCleanup.Count == 0)
            {
                return;
            }

            var userIds = _usersToCleanup.ToArray();
            CleanupUsers(userIds, stage: false);
            CleanupUsers(userIds, stage: true);
        }

        private void CleanupUsers(string[] userIds, bool stage)
        {
            try
            {
                _client.UserDel(new FreeIpaUserDelRequestOptions
                {
                    Uid = userIds,
                    Continue = true
                }, stage).GetAwaiter().GetResult();
            }
            catch
            {
                // Skip cleanup errors to keep the original test failure visible.
            }
        }
    }
}
