using Newtonsoft.Json;

namespace Notify.Models
{
    public class NotifyHTTPError
    {
        #pragma warning disable 169
        [JsonProperty("error")]
        private string error;

        [JsonProperty("message")]
        private string message;
    }
}
