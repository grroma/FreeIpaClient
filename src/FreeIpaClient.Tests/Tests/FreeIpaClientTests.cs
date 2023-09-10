using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using FluentAssertions.Execution;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
using Microsoft.Extensions.Configuration;

namespace FreeIpaClient.Tests.Tests
{
    public partial class FreeIpaClientTests : IDisposable
    {
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
            //cleanup active users
            try
            {
                _client.UserDel(new FreeIpaUserDelRequestOptions
                {
                    Uid = _usersToCleanup.ToArray(),
                    Continue = true
                }).Wait();
            }
            catch
            {
                //skip cleanup errors
            }

            //cleanup staged users
            try
            {
                _client.UserDel(new FreeIpaUserDelRequestOptions
                {
                    Uid = _usersToCleanup.ToArray(),
                    Continue = true
                }, true).Wait();
            }
            catch
            {
                //skip cleanup errors
            }
            
            _httpClient.Dispose();
            _httpClientHandler.Dispose();
        }
        
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
        
        private static void AssertUser(FreeIpaUserRequestOptions options, FreeIpaUser user)
        {
            using (new AssertionScope())
            {
                options.Uid.Should().BeEquivalentTo(user.Uid.Single());
                options.Givenname.Should().BeEquivalentTo(user.Givenname.Single());
                options.Sn.Should().BeEquivalentTo(user.Sn.Single());
                options.Cn.Should().BeEquivalentTo(user.Cn.Single());
                options.Mail.Should().BeEquivalentTo(user.Mail.Single());
                options.Mobile.Should().BeEquivalentTo(user.Mobile.Single());
                options.Ou.Should().BeEquivalentTo(user.Ou.Single());
                options.Title.Should().BeEquivalentTo(user.Title.Single());
            }
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
    }
}