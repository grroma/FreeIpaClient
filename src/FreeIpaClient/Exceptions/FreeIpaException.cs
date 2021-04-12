using System;
using System.Net;

namespace FreeIpaClient.Exceptions
{
    public class FreeIpaException : Exception
    {
        public const int InvalidCredential = 2100;        
        public const int NotFound = 4001;
        public const int AlreadyExist = 4002;
        public const int AlreadyActive = 4009;
        public const int AlreadyInactive = 4010;        
        public const int NoMatchingEntryFound = 4031;
        public const int NoModifications = 4202;

        public HttpStatusCode StatusCode { get; }
        public FreeIpaError Error { get; }
        public FreeIpaException(string message, HttpStatusCode statusCode, FreeIpaError error = null)
            : base(message)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public bool IsInvalidCredential
        {
            get { return Error?.Code == InvalidCredential; }
        }

        public bool IsNoMatchingEntryFound
        {
            get { return Error?.Code == NoMatchingEntryFound; }
        }

        public bool IsNotFound
        {
            get { return Error?.Code == NotFound; }
        }

        public bool IsAlreadyInactive
        {
            get { return Error?.Code == AlreadyInactive; }
        }

        public bool IsAlreadyActive
        {
            get { return Error?.Code == AlreadyActive; }
        }
    }
}