using System;
using System.Net;

namespace FreeIpaClient.Exceptions
{
    public class FreeIpaException : Exception
    {
        private const int InvalidCredential = 2100;        
        private const int NotFound = 4001;
        private const int AlreadyExist = 4002;
        private const int AlreadyActive = 4009;
        private const int AlreadyInactive = 4010;        
        private const int NoMatchingEntryFound = 4031;
        private const int NoModifications = 4202;

        private FreeIpaError Error { get; }
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