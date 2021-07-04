using System.Collections.Generic;
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
        Task Ping();

        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser[]> UserFind(FreeIpaUserFindRequestOptions options);

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
        Task<FreeIpaUser> UserAdd(FreeIpaUserRequestOptions options, bool stage = false);

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
        Task<FreeIpaUser> UserMod(FreeIpaUserAddModRequestOptions options, bool stage = false);

        /// <summary>
        /// Search for staged users.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser[]> StageUserFind(FreeIpaStageUserFindRequestOptions options);

        /// <summary>
        /// Set user password.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> Passwd(FreeIpaPasswdRequestOptions options);

        /// <summary>
        /// Show user details.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser[]> UserShow(FreeIpaUserShowRequestOptions options);
        
        /// <summary>
        /// Disable user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> UserDisable(FreeIpaUserDisableRequestOptions options);
        
        /// <summary>
        /// Enable user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<bool> UserEnable(FreeIpaUserEnableRequestOptions options);
        
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
        Task<string[]> UserDel(FreeIpaUserDelRequestOptions options, bool stage = false);
        
        /// <summary>
        /// Recover deleted user account.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<string[]> UserUndel(FreeIpaUserUndelRequestOptions options);
        
        /// <summary>
        /// Activate stage user.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<FreeIpaUser> StageUserActivate(FreeIpaStageUserActivateRequestOptions options);
        
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
            IEnumerable<object> args = null);
    }
}