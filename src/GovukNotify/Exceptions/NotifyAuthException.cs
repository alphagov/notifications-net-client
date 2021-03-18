using System;

namespace Notify.Exceptions
{
    public class NotifyAuthException : Exception
    {
        public NotifyAuthException() { }

        public NotifyAuthException(string message) : base(message) { }

        public NotifyAuthException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
