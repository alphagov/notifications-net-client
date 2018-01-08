using Newtonsoft.Json;

namespace Notify.Models
{
    public class NotifyHTTPError
    {
        [JsonProperty("error")]
        private string error;

        [JsonProperty("message")]
        private string message;
    }
}
