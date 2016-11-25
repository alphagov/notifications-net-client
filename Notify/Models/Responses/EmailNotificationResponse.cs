using Newtonsoft.Json;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Models.Responses
{
    public class EmailNotificationResponse : NotificationResponse
    {
        public EmailResponseContent content;

        public bool IsEqualTo(EmailNotificationResponse response)
        {
            return (
                content.fromEmail == response.content.fromEmail &&
                content.body == response.content.body &&
                content.subject == response.content.subject &&
                base.EqualTo(response)
                );
        }

    }

    public class EmailResponseContent
    {
        [JsonProperty("from_email")]
        public String fromEmail;
        public String body;
        public String subject;
    }

}
