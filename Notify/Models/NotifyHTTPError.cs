using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Models
{
    public class NotifyHTTPError
    {
        [JsonProperty("error")]
        private String error;

        [JsonProperty("message")]
        private String message;
    }
}
