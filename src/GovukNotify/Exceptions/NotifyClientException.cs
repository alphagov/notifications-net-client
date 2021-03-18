using System;

namespace Notify.Exceptions
{
    public class NotifyClientException : Exception
    {
        public NotifyClientException() { }

        public NotifyClientException(string message) : base(message) { }

        public NotifyClientException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}
