using Newtonsoft.Json;
using System;

namespace Notify.Models.Responses
{
    public class ReceivedTextResponse
    {
        public String id;
        [JsonProperty("created_at")]
        public String createdAt;
        [JsonProperty("provider_date")]
        public String providerDate;
        [JsonProperty("notify_number")]
        public String notifyNumber;
        [JsonProperty("user_number")]
        public String userNumber;
        [JsonProperty("provider_reference")]
        public String providerReference;
        [JsonProperty("service_id")]
        public String serviceId;
        public String content;

        public bool EqualTo(ReceivedTextResponse receivedText)
        {
            return (
                id == receivedText.id &&
                createdAt == receivedText.createdAt &&
                providerDate == receivedText.providerDate &&
                notifyNumber == receivedText.notifyNumber &&
                userNumber == receivedText.userNumber &&
                providerReference == receivedText.providerReference &&
                serviceId == receivedText.serviceId &&
                content == receivedText.content
            );
        }
    }
}
