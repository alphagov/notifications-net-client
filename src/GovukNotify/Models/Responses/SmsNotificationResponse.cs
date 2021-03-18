using Newtonsoft.Json;

namespace Notify.Models.Responses
{
    public class SmsNotificationResponse : NotificationResponse
    {
        public SmsResponseContent content;

        public override bool Equals(object response)
        {
            if (!(response is SmsNotificationResponse resp))
            {
                return false;
            }

            return
                content.body == resp.content.body &&
                content.fromNumber == resp.content.fromNumber &&
                base.Equals(response);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SmsResponseContent
    {
        public string body;
        [JsonProperty("from_number")]
        public string fromNumber;
    }
}
