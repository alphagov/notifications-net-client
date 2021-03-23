using Newtonsoft.Json;
using System;

namespace Notify.Models.Responses
{
    public class ReceivedTextResponse
    {
        public String id;
        [JsonProperty("created_at")]
        public String createdAt;
        [JsonProperty("notify_number")]
        public String notifyNumber;
        [JsonProperty("user_number")]
        public String userNumber;
        [JsonProperty("service_id")]
        public String serviceId;
        public String content;

        public bool EqualTo(ReceivedTextResponse receivedText)
        {
            return (
                id == receivedText.id &&
                createdAt == receivedText.createdAt &&
                notifyNumber == receivedText.notifyNumber &&
                userNumber == receivedText.userNumber &&
                serviceId == receivedText.serviceId &&
                content == receivedText.content
            );
        }
    }
}
