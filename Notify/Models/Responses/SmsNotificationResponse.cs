using Newtonsoft.Json;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Models.Responses
{
    public class SmsNotificationResponse : NotificationResponse
    {
        public SmsResponseContent content;

        public bool IsEqualTo(SmsNotificationResponse response)
        {
            return (
                content.body == response.content.body &&
                content.fromNumber == response.content.fromNumber &&
                base.EqualTo(response)
                );
        }

    }

    public class SmsResponseContent
    {
        public String body;
        [JsonProperty("from_number")]
        public String fromNumber;
    }
}
