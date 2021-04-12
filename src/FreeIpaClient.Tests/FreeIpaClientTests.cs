using System.Net.Http;
using System.Threading.Tasks;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
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
    }
}