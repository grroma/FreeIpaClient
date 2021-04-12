using System;
using System.Collections.Generic;

namespace FreeIpaClient.Models
{
    public class FreeIpaRequest
    {
        public int Id { get; set; }
        public string Method { get; }
        public object[] Params { get; }

        public FreeIpaRequest(string method, FreeIpaRequestOptions options, IEnumerable<object> args)
        {
            Method = method;
            Params = new object[]
            {
                args,
                options
            };
        }

        public FreeIpaRequest(string method, FreeIpaRequestOptions options)
            : this(method, options, Array.Empty<object>()) { }
    }
}