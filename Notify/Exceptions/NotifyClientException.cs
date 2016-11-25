using Notify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Exceptions
{
    public class NotifyClientException : Exception
    {
        public NotifyClientException() : base() { }

        public NotifyClientException(String message) : base(message) { }

        public NotifyClientException(String format, params object[] args) : base(String.Format(format, args)) { }
    }
}
