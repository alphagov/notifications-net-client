using Newtonsoft.Json;

namespace Notify.Models.Responses
{
    public class EmailNotificationResponse : NotificationResponse
    {
        public EmailResponseContent content;

        public override bool Equals(object response)
        {
            if (!(response is EmailNotificationResponse resp))
            {
                return false;
            }

            return
                content.fromEmail == resp.content.fromEmail &&
                content.body == resp.content.body &&
                content.subject == resp.content.subject &&
                base.Equals(resp);

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public class EmailResponseContent
    {
        [JsonProperty("from_email")]
        public string fromEmail;
        public string body;
        public string subject;
    }

}
