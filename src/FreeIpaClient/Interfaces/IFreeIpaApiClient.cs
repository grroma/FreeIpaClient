using System.Collections.Generic;
using System.Threading.Tasks;
using FreeIpaClient.Models;
using FreeIpaClient.RequestOptions;

namespace FreeIpaClient.Interfaces
{
    public interface IFreeIpaApiClient
    {
        Task Ping();
        Task<FreeIpaUser> UserAdd(FreeIpaUserRequestOptions options, bool stage = false);
        Task<FreeIpaUser> UserMod(FreeIpaUserAddModRequestOptions options, bool stage = false);
        Task<bool> Passwd(FreeIpaPasswdRequestOptions options);
        Task<FreeIpaUser[]> UserFind(FreeIpaUserFindRequestOptions options);
        Task<FreeIpaUser[]> StageUserFind(FreeIpaStageUserFindRequestOptions options);
        Task<FreeIpaUser[]> UserShow(FreeIpaUserShowRequestOptions options);
        Task<bool> UserDisable(FreeIpaUserDisableRequestOptions options);
        Task<bool> UserEnable(FreeIpaUserEnableRequestOptions options);
        Task<string[]> UserDel(FreeIpaUserDelRequestOptions options, bool stage = false);
        Task<string[]> UserUndel(FreeIpaUserUndelRequestOptions options);
        Task<FreeIpaUser> StageUserActivate(FreeIpaStageUserActivateRequestOptions options);
        Task<TResult> Post<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null);
    }
}