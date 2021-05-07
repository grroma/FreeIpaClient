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

        public FreeIpaError Error { get; }
        public FreeIpaException(string message, HttpStatusCode statusCode, FreeIpaError error = null)
            : base(message)
        {
            Error = error;
        }

        public bool IsInvalidCredential => Error?.Code == InvalidCredential;

        public bool IsNoMatchingEntryFound => Error?.Code == NoMatchingEntryFound;

        public bool IsNotFound => Error?.Code == NotFound;

        public bool IsAlreadyInactive => Error?.Code == AlreadyInactive;

        public bool IsAlreadyActive => Error?.Code == AlreadyActive;
    }
}