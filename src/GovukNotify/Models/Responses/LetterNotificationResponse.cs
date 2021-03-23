using System;
using Newtonsoft.Json;

namespace Notify.Models.Responses
{
    public class LetterNotificationResponse : NotificationResponse
    {
        public LetterResponseContent content;

        public bool IsEqualTo(LetterNotificationResponse response)
        {
            return (
                content.body == response.content.body &&
                content.subject == response.content.subject &&
                base.EqualTo(response)
            );
        }
    }

    public class LetterResponseContent
    {
        public String body;
        public String subject;
    }
}
