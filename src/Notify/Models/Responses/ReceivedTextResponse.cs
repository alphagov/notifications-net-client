using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class ReceivedTextResponse
    {
        public string id;
        [JsonProperty("created_at")]
        public string createdAt;
        [JsonProperty("notify_number")]
        public string notifyNumber;
        [JsonProperty("user_number")]
        public string userNumber;
        [JsonProperty("service_id")]
        public string serviceId;
        public string content;

        public override bool Equals(object receivedText)
        {
            if (!(receivedText is ReceivedTextResponse text))
            {
                return false;
            }

            return
                id == text.id &&
                createdAt == text.createdAt &&
                notifyNumber == text.notifyNumber &&
                userNumber == text.userNumber &&
                serviceId == text.serviceId &&
                content == text.content;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
