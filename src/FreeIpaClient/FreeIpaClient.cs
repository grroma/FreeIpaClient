using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FreeIpaClient.Constants;
using FreeIpaClient.Exceptions;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
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
            await Post<object, string>(FreeIpaApiMethods.Ping, new FreeIpaRequestOptions());
        }
        
        public async Task<FreeIpaUser> UserAdd(FreeIpaUserRequestOptions options, bool stage = false)
        {
            options.Sn ??= options.Uid;
            options.Givenname ??= options.Uid;

            var user = await Post<FreeIpaUser, string>(
               stage ? FreeIpaApiMethods.StageUserAdd : FreeIpaApiMethods.UserAdd, options, false, true, true);
            user.Stage = stage;
            return user;
        }
        
        public async Task<FreeIpaUser> UserMod(FreeIpaUserAddModRequestOptions options, bool stage = false)
        {
            var user = await Post<FreeIpaUser, string>(
                stage ? FreeIpaApiMethods.StageUserMod : FreeIpaApiMethods.UserMod, options, true, true, true);
            user.Stage = stage;
            return user;
        }

        public Task<bool> Passwd(FreeIpaPasswdRequestOptions options)
        {
            return Post<bool, string>(FreeIpaApiMethods.Passwd, options);
        }

        public async Task<FreeIpaUser[]> UserFind(FreeIpaUserFindRequestOptions options)
        {
            var users = await Post<FreeIpaUser[], string>(FreeIpaApiMethods.UserFind, options, false, true, true);
            return users;
        }
        
        public async Task<FreeIpaUser[]> StageUserFind(FreeIpaStageUserFindRequestOptions options)
        {
            var users = 
                await Post<FreeIpaUser[], string>(FreeIpaApiMethods.StageUserFind, options, false, true, true);
            return users;
        }
        
        public async Task<FreeIpaUser[]> UserShow(FreeIpaUserShowRequestOptions options)
        {
            var users = await Post<FreeIpaUser[], string>(FreeIpaApiMethods.UserShow, options, false, true, true);
            return users;
        }

        public Task<bool> UserDisable(FreeIpaUserDisableRequestOptions options)
        {
            return Post<bool, string>(FreeIpaApiMethods.UserDisable, options);
        }

        public Task<bool> UserEnable(FreeIpaUserEnableRequestOptions options)
        {
            return Post<bool, string>(FreeIpaApiMethods.UserEnable, options);
        }

        public async Task<string[]> UserDel(FreeIpaUserDelRequestOptions options, bool stage = false)
        {
            var result = await Post<FreeIpaUserDelResult, string[]>(
                stage ? FreeIpaApiMethods.StageUserDel : FreeIpaApiMethods.UserDel, options);

            return result?.Failed;
        }

        public async Task<string[]> UserUndel(FreeIpaUserUndelRequestOptions options)
        {
            var result = await Post<FreeIpaUserUndelResult, string[]>(FreeIpaApiMethods.UserUndel, options);
            return result?.Error;
        }

        public async Task<FreeIpaUser> StageUserActivate(FreeIpaStageUserActivateRequestOptions options)
        {
            var user = await Post<FreeIpaUser, string>(FreeIpaApiMethods.StageUserActivate, options);
            user.Stage = false;
            return user;
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

            var httpContent = new StringContent(requestString, Encoding.UTF8, MediaTypeNames.Application.Json);

            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;
            var responseMessage = await _httpClient.PostAsync(FreeIpaConstants.Api, httpContent);
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
                new KeyValuePair<string, string>(FreeIpaConstants.User, _config.User),
                new KeyValuePair<string, string>(FreeIpaConstants.Password, _config.Password)
            });

            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;
            var response = await _httpClient.PostAsync(FreeIpaConstants.Login, httpContent);

            if (!response.IsSuccessStatusCode && response.Headers.TryGetValues("X-IPA-Rejection-Reason", out var rejectionReasons))
            {
                throw new FreeIpaException($"FreeIPA login error: {rejectionReasons.FirstOrDefault()}", response.StatusCode);
            }

            response.EnsureSuccessStatusCode();

            if (!response.Headers.TryGetValues("Set-Cookie", out var cookies) || !cookies.Any(c => c.Contains(FreeIpaConstants.IpaSession)))
            {
                throw new FreeIpaException("FreeIPA login error: Response doesn't contain ipa_session cookie.", response.StatusCode);
            }

            _authenticated = true;

            await Task.CompletedTask;
        }
    }
}