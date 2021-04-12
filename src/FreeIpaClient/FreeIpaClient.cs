using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FreeIpaClient.Exceptions;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FreeIpaClient
{
    public class FreeIpaApiClient : IFreeIpaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly FreeIpaConfig _config;
        private bool _authenticated;

        public FreeIpaApiClient(
            HttpClient httpClient, 
            FreeIpaConfig config)
        {
            _config = config;
            _httpClient = httpClient;
            httpClient.BaseAddress = _config.Host;
        }        

        public async Task Ping()
        {
            await Post<object, string>("ping", new FreeIpaRequestOptions());
        }
        
        public async Task<TResult> Post<TResult, TValue>(
            string method, 
            FreeIpaRequestOptions options,
            bool sendNulls = false, 
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null)
        {
            if (!_authenticated)
            {
                await Login();
            }

            options.Version = _config.ApiVersion;
            options.All = all;
            options.Raw = raw;

            var request = args == null ? new FreeIpaRequest(method, options) 
                : new FreeIpaRequest(method, options, args);
            

            var requestString = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Formatting = Formatting.None,
                NullValueHandling = sendNulls ? NullValueHandling.Include : NullValueHandling.Ignore
            });

            var httpContent = new StringContent(requestString, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;
            var responseMessage = await _httpClient.PostAsync("session/json", httpContent);
            responseMessage.EnsureSuccessStatusCode();

            var content = await responseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<FreeIpaResponse<TResult, TValue>>(content);

            if (response.Error != null)
            {
                throw new FreeIpaException(response.Error.Message, responseMessage.StatusCode, response.Error);
            }
            
            return response.Result.Result;
        }
        
        private async Task Login()
        {
            var httpContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user", _config.User),
                new KeyValuePair<string, string>("password", _config.Password)
            });

            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;
            var response = await _httpClient.PostAsync("session/login_password", httpContent);

            if (!response.IsSuccessStatusCode && response.Headers.TryGetValues("X-IPA-Rejection-Reason", out var rejectionReasons))
            {
                throw new FreeIpaException($"FreeIPA login error: {rejectionReasons.FirstOrDefault()}", response.StatusCode);
            }

            response.EnsureSuccessStatusCode();

            if (!response.Headers.TryGetValues("Set-Cookie", out var cookies) || !cookies.Any(c => c.Contains("ipa_session")))
            {
                throw new FreeIpaException("FreeIPA login error: Response doesn't contain ipa_session cookie.", response.StatusCode);
            }

            _authenticated = true;

            await Task.CompletedTask;
        }
    }
}