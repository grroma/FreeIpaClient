using System.Collections.Generic;
using System.Threading.Tasks;
using FreeIpaClient.Models;

namespace FreeIpaClient.Interfaces
{
    public interface IFreeIpaApiClient
    {
        Task Ping();
        
        Task<TResult> Post<TResult, TValue>(string method,
            FreeIpaRequestOptions options,
            bool sendNulls = false,
            bool? all = null,
            bool? raw = null,
            IEnumerable<object> args = null);
    }
}