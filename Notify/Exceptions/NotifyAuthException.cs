using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Exceptions
{
    public class NotifyAuthException : Exception
    {
        public NotifyAuthException() : base() { }

        public NotifyAuthException(String message) : base(message) { }

        public NotifyAuthException(String format, params object[] args) : base(String.Format(format, args)) { }
    }
}
