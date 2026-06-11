using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeIpaClient.Constants;
using FreeIpaClient.Exceptions;
using FreeIpaClient.Interfaces;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FreeIpaClient
{
    public class FreeIpaApiClient : IFreeIpaApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly FreeIpaConfig _config;
        private bool _authenticated;
        private string _apiVersion;

        public FreeIpaApiClient(
            HttpClient httpClient, 
            FreeIpaConfig config)
        {
            _config = config;
            _httpClient = httpClient;
            httpClient.BaseAddress = _config.Host;
        }        

        public async Task Ping(CancellationToken cancellationToken = default)
        {
            await Post<object, string>(FreeIpaApiMethods.Ping, new FreeIpaRequestOptions(), cancellationToken: cancellationToken);
        }
        
        public async Task<FreeIpaUser> UserAdd(
            FreeIpaUserRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default)
        {
            options.Sn ??= options.Uid;
            options.Givenname ??= options.Uid;

            var user = await Post<FreeIpaUser, string>(
               stage ? FreeIpaApiMethods.StageUserAdd : FreeIpaApiMethods.UserAdd,
               options,
               false,
               true,
               true,
               cancellationToken: cancellationToken);
            user.Stage = stage;
            return user;
        }
        
        public async Task<FreeIpaUser> UserMod(
            FreeIpaUserAddModRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default)
        {
            var user = await Post<FreeIpaUser, string>(
                stage ? FreeIpaApiMethods.StageUserMod : FreeIpaApiMethods.UserMod,
                options,
                false,
                true,
                true,
                cancellationToken: cancellationToken);
            user.Stage = stage;
            return user;
        }

        public Task<bool> Passwd(FreeIpaPasswdRequestOptions options, CancellationToken cancellationToken = default)
        {
            return Post<bool, string>(FreeIpaApiMethods.Passwd, options, cancellationToken: cancellationToken);
        }

        public async Task<FreeIpaUser[]> UserFind(
            FreeIpaUserFindRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = await UserFindResult(options, cancellationToken);
            return result.Result;
        }
        
        public async Task<FreeIpaResult<FreeIpaUser[], string>> UserFindResult(
            FreeIpaUserFindRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = await PostResult<FreeIpaUser[], string>(
                FreeIpaApiMethods.UserFind,
                options,
                false,
                true,
                true,
                cancellationToken: cancellationToken);
            MarkStage(result.Result, false);
            return result;
        }

        public async Task<FreeIpaUser[]> StageUserFind(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = await StageUserFindResult(options, cancellationToken);
            return result.Result;
        }

        public async Task<FreeIpaResult<FreeIpaUser[], string>> StageUserFindResult(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = await PostResult<FreeIpaUser[], string>(
                FreeIpaApiMethods.StageUserFind,
                options,
                false,
                true,
                true,
                cancellationToken: cancellationToken);
            MarkStage(result.Result, true);
            return result;
        }
        
        public Task<FreeIpaUser> UserShow(
            FreeIpaUserShowRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            return Post<FreeIpaUser, string>(
                FreeIpaApiMethods.UserShow,
                options,
                false,
                true,
                true,
                cancellationToken: cancellationToken);
        }

        public Task<bool> UserDisable(
            FreeIpaUserDisableRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            return Post<bool, string>(FreeIpaApiMethods.UserDisable, options, cancellationToken: cancellationToken);
        }

        public Task<bool> UserEnable(
            FreeIpaUserEnableRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            return Post<bool, string>(FreeIpaApiMethods.UserEnable, options, cancellationToken: cancellationToken);
        }

        public async Task<string[]> UserDel(
            FreeIpaUserDelRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default)
        {
            var result = await Post<FreeIpaUserDelResult, string[]>(
                stage ? FreeIpaApiMethods.StageUserDel : FreeIpaApiMethods.UserDel,
                options,
                cancellationToken: cancellationToken);

            return result?.Failed;
        }

        public async Task<string[]> UserUndel(
            FreeIpaUserUndelRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var result = await Post<FreeIpaUserUndelResult, string[]>(
                FreeIpaApiMethods.UserUndel,
                options,
                cancellationToken: cancellationToken);
            return result?.Error;
        }

        public async Task<FreeIpaUser> StageUserActivate(
            FreeIpaStageUserActivateRequestOptions options,
            CancellationToken cancellationToken = default)
        {
            var user = await Post<FreeIpaUser, string>(
                FreeIpaApiMethods.StageUserActivate,
                options,
                cancellationToken: cancellationToken);
            user.Stage = false;
            return user;
        }

        public async Task SessionLogout(CancellationToken cancellationToken = default)
        {
            await PostResponse<object, string>(
                FreeIpaApiMethods.SessionLogout,
                new FreeIpaRequestOptions(),
                cancellationToken: cancellationToken);

            _authenticated = false;
        }

        public async Task<FreeIpaEnvironment> Env(CancellationToken cancellationToken = default)
        {
            var result = await PostResultInternal<FreeIpaEnvironment, string>(
                FreeIpaApiMethods.Env,
                new FreeIpaEnvRequestOptions { Server = true },
                skipApiVersion: true,
                cancellationToken: cancellationToken);

            return result.Result;
        }

        public async Task<string> GetApiVersion(CancellationToken cancellationToken = default)
        {
            return await ResolveApiVersion(cancellationToken);
        }

        public Task<FreeIpaCommandInfo> CommandShow(
            string commandName,
            CancellationToken cancellationToken = default)
        {
            return Post<FreeIpaCommandInfo, string>(
                FreeIpaApiMethods.CommandShow,
                new FreeIpaRequestOptions(),
                all: true,
                args: new object[] { commandName },
                cancellationToken: cancellationToken);
        }

        public async Task<FreeIpaJsonMetadata> JsonMetadata(CancellationToken cancellationToken = default)
        {
            var result = await PostResultInternal<JObject, string>(
                FreeIpaApiMethods.JsonMetadata,
                new FreeIpaRequestOptions(),
                skipApiVersion: false,
                cancellationToken: cancellationToken);

            return new FreeIpaJsonMetadata
            {
                Objects = result.AdditionalData?["objects"] as JObject,
                Methods = result.AdditionalData?["methods"] as JObject,
                Commands = result.AdditionalData?["commands"] as JObject
            };
        }
        
        public async Task<TResult> Post<TResult, TValue>(
            string method, 
            FreeIpaRequestOptions options,
            bool sendNulls = false, 
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default)
        {
            var result = await PostResult<TResult, TValue>(
                method,
                options,
                sendNulls,
                all,
                raw,
                args,
                cancellationToken);

            return result == null ? default : result.Result;
        }

        public Task<FreeIpaResult<TResult, TValue>> PostResult<TResult, TValue>(
            string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default)
        {
            return PostResultInternal<TResult, TValue>(
                method,
                options,
                sendNulls,
                all,
                raw,
                args,
                false,
                cancellationToken);
        }

        public Task<FreeIpaResponse<TResult, TValue>> PostResponse<TResult, TValue>(
            string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default)
        {
            return PostResponseInternal<TResult, TValue>(
                method,
                options,
                sendNulls,
                all,
                raw,
                args,
                false,
                cancellationToken);
        }

        private async Task<FreeIpaResult<TResult, TValue>> PostResultInternal<TResult, TValue>(
            string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            bool skipApiVersion = false,
            CancellationToken cancellationToken = default)
        {
            var response = await PostResponseInternal<TResult, TValue>(
                method,
                options,
                sendNulls,
                all,
                raw,
                args,
                skipApiVersion,
                cancellationToken);

            return response.Result;
        }

        private async Task<FreeIpaResponse<TResult, TValue>> PostResponseInternal<TResult, TValue>(
            string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            bool skipApiVersion = false,
            CancellationToken cancellationToken = default)
        {
            if (!_authenticated)
            {
                await Login(cancellationToken);
            }

            if (!skipApiVersion)
            {
                options.Version = await ResolveApiVersion(cancellationToken);
            }

            if (all.HasValue)
            {
                options.All = all;
            }

            if (raw.HasValue)
            {
                options.Raw = raw;
            }

            var requestString = SerializeRequest(method, options, args, sendNulls);
            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;

            var responseMessage = await SendRequest(requestString, cancellationToken);

            if (_config.RetryOnUnauthorized && ShouldRelogin(responseMessage))
            {
                responseMessage.Dispose();
                _authenticated = false;
                await Login(cancellationToken);
                responseMessage = await SendRequest(requestString, cancellationToken);
            }

            responseMessage.EnsureSuccessStatusCode();

            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            var response = JsonConvert.DeserializeObject<FreeIpaResponse<TResult, TValue>>(content);

            if (response == null)
            {
                throw new FreeIpaException("FreeIPA returned an empty response.", responseMessage.StatusCode);
            }

            if (response.Error != null)
            {
                throw new FreeIpaException(response.Error.Message, responseMessage.StatusCode, response.Error);
            }
            
            return response;
        }
        
        private async Task Login(CancellationToken cancellationToken)
        {
            var httpContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(FreeIpaConstants.User, _config.User),
                new KeyValuePair<string, string>(FreeIpaConstants.Password, _config.Password)
            });

            _httpClient.DefaultRequestHeaders.Referrer = _config.Host;
            var response = await _httpClient.PostAsync(FreeIpaConstants.Login, httpContent, cancellationToken);

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

        private async Task<string> ResolveApiVersion(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_config.ApiVersion))
            {
                return _config.ApiVersion;
            }

            if (!_config.AutoDetectApiVersion)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(_apiVersion))
            {
                return _apiVersion;
            }

            var env = await Env(cancellationToken);
            _apiVersion = env?.ApiVersion;
            return _apiVersion;
        }

        private static string SerializeRequest(
            string method,
            FreeIpaRequestOptions options,
            IEnumerable<object> args,
            bool sendNulls)
        {
            var request = args == null ? new FreeIpaRequest(method, options)
                : new FreeIpaRequest(method, options, args);

            return JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Formatting = Formatting.None,
                NullValueHandling = sendNulls ? NullValueHandling.Include : NullValueHandling.Ignore
            });
        }

        private Task<HttpResponseMessage> SendRequest(
            string requestString,
            CancellationToken cancellationToken)
        {
            var httpContent = new StringContent(requestString, Encoding.UTF8, MediaTypeNames.Application.Json);
            return _httpClient.PostAsync(FreeIpaConstants.Api, httpContent, cancellationToken);
        }

        private static bool ShouldRelogin(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized ||
                responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return true;
            }

            return responseMessage.Headers.TryGetValues("X-IPA-Rejection-Reason", out var reasons) &&
                reasons.Any(reason => reason.Contains("session", System.StringComparison.OrdinalIgnoreCase));
        }

        private static void MarkStage(IEnumerable<FreeIpaUser> users, bool stage)
        {
            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                user.Stage = stage;
            }
        }
    }
}
