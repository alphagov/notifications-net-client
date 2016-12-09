using Newtonsoft.Json;
using System;

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
