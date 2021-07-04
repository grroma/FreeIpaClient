using System.Collections.Generic;
using FreeIpaClient.Exceptions;

namespace FreeIpaClient.Models
{
    public class FreeIpaResponse<TResult, TValue>
    {
        public FreeIpaResult<TResult, TValue> Result { get; set; }
        public FreeIpaError Error { get; set; }
        public string Id { get; set; }
        public string Principal { get; set; }
        public string Version { get; set; }   
    }

    public class FreeIpaResult<TResult, TValue>
    {   
        public TResult Result { get; set; }
        public TValue Value { get; set; }
        public string Summary { get; set; }
        public List<FreeIpaResultMessage> Messages { get; set; }
    }
    
    public class FreeIpaResultMessage
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
    }
}