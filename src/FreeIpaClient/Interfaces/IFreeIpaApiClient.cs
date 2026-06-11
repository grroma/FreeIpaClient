using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;

namespace FreeIpaClient.Interfaces
{
    /// <summary>
    /// Client for working with API FreeIpa.
    /// </summary>
    public interface IFreeIpaApiClient
    {
        /// <summary>
        /// Check connection with remote FreeIpa server.
        /// <exception cref="System.Net.Http.HttpRequestException">
        /// Thrown when Response status code does not indicate success: 404 (Not Found).
        /// </exception>
        /// <exception cref="FreeIpaClient.Exceptions.FreeIpaException">
        /// Thrown when FreeIpa server return some error.
        /// </exception>
        /// </summary>
        /// <returns></returns>
        Task Ping(CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser[]> UserFind(FreeIpaUserFindRequestOptions options, CancellationToken cancellationToken = default);

        Task<FreeIpaResult<FreeIpaUser[], string>> UserFindResult(
            FreeIpaUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new user.
        /// </summary>
        /// <remarks>
        /// If you want add stage user, set flag
        /// <code>stage = true</code>
        /// </remarks>
        /// <param name="options"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        Task<FreeIpaUser> UserAdd(
            FreeIpaUserRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Change user.
        /// </summary>
        /// <remarks>
        /// If you want change stage user, set flag
        /// <code>stage = true</code>
        /// </remarks>
        /// <param name="options"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        Task<FreeIpaUser> UserMod(
            FreeIpaUserAddModRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for staged users.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser[]> StageUserFind(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        Task<FreeIpaResult<FreeIpaUser[], string>> StageUserFindResult(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Set user password.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> Passwd(FreeIpaPasswdRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Show user details.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser> UserShow(FreeIpaUserShowRequestOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Disable user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> UserDisable(FreeIpaUserDisableRequestOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Enable user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> UserEnable(FreeIpaUserEnableRequestOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Delete user.
        /// </summary>
        /// <remarks>
        /// If you want delete stage user, set flag
        /// <code>stage = true</code>
        /// </remarks>
        /// <param name="options"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        Task<string[]> UserDel(
            FreeIpaUserDelRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recover deleted user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<string[]> UserUndel(FreeIpaUserUndelRequestOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Activate stage user.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser> StageUserActivate(
            FreeIpaStageUserActivateRequestOptions options,
            CancellationToken cancellationToken = default);

        Task SessionLogout(CancellationToken cancellationToken = default);

        Task<FreeIpaEnvironment> Env(CancellationToken cancellationToken = default);

        Task<string> GetApiVersion(CancellationToken cancellationToken = default);

        Task<FreeIpaCommandInfo> CommandShow(string commandName, CancellationToken cancellationToken = default);

        Task<FreeIpaJsonMetadata> JsonMetadata(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Send custom request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="options"></param>
        /// <param name="sendNulls"></param>
        /// <param name="all"></param>
        /// <param name="raw"></param>
        /// <param name="args"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        Task<TResult> Post<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);

        Task<FreeIpaResult<TResult, TValue>> PostResult<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);

        Task<FreeIpaResponse<TResult, TValue>> PostResponse<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);
    }
}
