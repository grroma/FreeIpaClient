using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;

namespace FreeIpaClient.Interfaces
{
    /// <summary>
    /// Client for working with the FreeIPA JSON RPC API.
    /// </summary>
    public interface IFreeIpaApiClient
    {
        /// <summary>
        /// Checks that the remote FreeIPA server is reachable and accepts API calls.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <exception cref="System.Net.Http.HttpRequestException">
        /// Thrown when the HTTP response status code does not indicate success.
        /// </exception>
        /// <exception cref="FreeIpaClient.Exceptions.FreeIpaException">
        /// Thrown when FreeIPA returns a JSON RPC error.
        /// </exception>
        Task Ping(CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches active users and returns only the command result payload.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_find</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Users matching the search options.</returns>
        Task<FreeIpaUser[]> UserFind(FreeIpaUserFindRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches active users and returns the full FreeIPA response result envelope.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_find</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Search result with metadata such as count and truncated state.</returns>
        Task<FreeIpaResult<FreeIpaUser[], string>> UserFindResult(
            FreeIpaUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an active user, or a staged user when <paramref name="stage"/> is <c>true</c>.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_add</c> or <c>stageuser_add</c> options.</param>
        /// <param name="stage">Use <c>stageuser_add</c> instead of <c>user_add</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The created user.</returns>
        Task<FreeIpaUser> UserAdd(
            FreeIpaUserRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modifies an active user, or a staged user when <paramref name="stage"/> is <c>true</c>.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_mod</c> or <c>stageuser_mod</c> options.</param>
        /// <param name="stage">Use <c>stageuser_mod</c> instead of <c>user_mod</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The modified user.</returns>
        Task<FreeIpaUser> UserMod(
            FreeIpaUserAddModRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches staged users and returns only the command result payload.
        /// </summary>
        /// <param name="options">FreeIPA <c>stageuser_find</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Staged users matching the search options.</returns>
        Task<FreeIpaUser[]> StageUserFind(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches staged users and returns the full FreeIPA response result envelope.
        /// </summary>
        /// <param name="options">FreeIPA <c>stageuser_find</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Search result with metadata such as count and truncated state.</returns>
        Task<FreeIpaResult<FreeIpaUser[], string>> StageUserFindResult(
            FreeIpaStageUserFindRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes a user password using the FreeIPA <c>passwd</c> command.
        /// </summary>
        /// <param name="options">Password change options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns><c>true</c> when FreeIPA accepts the password change.</returns>
        Task<bool> Passwd(FreeIpaPasswdRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns details for one active user.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_show</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The requested user.</returns>
        Task<FreeIpaUser> UserShow(FreeIpaUserShowRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables an active user account.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_disable</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns><c>true</c> when the user was disabled.</returns>
        Task<bool> UserDisable(FreeIpaUserDisableRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables an active user account.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_enable</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns><c>true</c> when the user was enabled.</returns>
        Task<bool> UserEnable(FreeIpaUserEnableRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes active users, or staged users when <paramref name="stage"/> is <c>true</c>.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_del</c> or <c>stageuser_del</c> options.</param>
        /// <param name="stage">Use <c>stageuser_del</c> instead of <c>user_del</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>FreeIPA deletion failures, if any.</returns>
        Task<string[]> UserDel(
            FreeIpaUserDelRequestOptions options,
            bool stage = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Restores a preserved user account using the FreeIPA <c>user_undel</c> command.
        /// </summary>
        /// <param name="options">FreeIPA <c>user_undel</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>FreeIPA restore errors, if any.</returns>
        Task<string[]> UserUndel(FreeIpaUserUndelRequestOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Activates a staged user using the FreeIPA <c>stageuser_activate</c> command.
        /// </summary>
        /// <param name="options">FreeIPA <c>stageuser_activate</c> options.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The activated active user.</returns>
        Task<FreeIpaUser> StageUserActivate(
            FreeIpaStageUserActivateRequestOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out the current FreeIPA session and clears the client authentication state.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        Task SessionLogout(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns FreeIPA server environment information.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Environment information returned by the FreeIPA <c>env</c> command.</returns>
        Task<FreeIpaEnvironment> Env(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the API version detected from the FreeIPA server.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The detected API version, or the configured version when it is pinned.</returns>
        Task<string> GetApiVersion(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns metadata for one FreeIPA command.
        /// </summary>
        /// <param name="commandName">FreeIPA command name, for example <c>user_show</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Command metadata returned by <c>command_show</c>.</returns>
        Task<FreeIpaCommandInfo> CommandShow(string commandName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns FreeIPA metadata sections used for API exploration and code generation.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>Objects, methods, and commands returned by <c>json_metadata</c>.</returns>
        Task<FreeIpaJsonMetadata> JsonMetadata(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a custom FreeIPA JSON RPC request and returns only the command result payload.
        /// </summary>
        /// <typeparam name="TResult">Expected type of the FreeIPA <c>result.result</c> payload.</typeparam>
        /// <typeparam name="TValue">Expected type of the FreeIPA <c>value</c> field.</typeparam>
        /// <param name="method">FreeIPA command name.</param>
        /// <param name="options">Command options serialized to JSON RPC <c>params[1]</c>.</param>
        /// <param name="sendNulls">Send null option values instead of omitting them.</param>
        /// <param name="all">Override the FreeIPA <c>all</c> option.</param>
        /// <param name="raw">Override the FreeIPA <c>raw</c> option.</param>
        /// <param name="args">Positional command arguments serialized to JSON RPC <c>params[0]</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The deserialized command result payload.</returns>
        Task<TResult> Post<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a custom FreeIPA JSON RPC request and returns the FreeIPA result envelope.
        /// </summary>
        /// <typeparam name="TResult">Expected type of the FreeIPA <c>result.result</c> payload.</typeparam>
        /// <typeparam name="TValue">Expected type of the FreeIPA <c>value</c> field.</typeparam>
        /// <param name="method">FreeIPA command name.</param>
        /// <param name="options">Command options serialized to JSON RPC <c>params[1]</c>.</param>
        /// <param name="sendNulls">Send null option values instead of omitting them.</param>
        /// <param name="all">Override the FreeIPA <c>all</c> option.</param>
        /// <param name="raw">Override the FreeIPA <c>raw</c> option.</param>
        /// <param name="args">Positional command arguments serialized to JSON RPC <c>params[0]</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The deserialized result envelope including response metadata.</returns>
        Task<FreeIpaResult<TResult, TValue>> PostResult<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a custom FreeIPA JSON RPC request and returns the full JSON RPC response.
        /// </summary>
        /// <typeparam name="TResult">Expected type of the FreeIPA <c>result.result</c> payload.</typeparam>
        /// <typeparam name="TValue">Expected type of the FreeIPA <c>value</c> field.</typeparam>
        /// <param name="method">FreeIPA command name.</param>
        /// <param name="options">Command options serialized to JSON RPC <c>params[1]</c>.</param>
        /// <param name="sendNulls">Send null option values instead of omitting them.</param>
        /// <param name="all">Override the FreeIPA <c>all</c> option.</param>
        /// <param name="raw">Override the FreeIPA <c>raw</c> option.</param>
        /// <param name="args">Positional command arguments serialized to JSON RPC <c>params[0]</c>.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>The full FreeIPA JSON RPC response.</returns>
        Task<FreeIpaResponse<TResult, TValue>> PostResponse<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null,
            CancellationToken cancellationToken = default);
    }
}
