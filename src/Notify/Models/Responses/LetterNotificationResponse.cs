namespace Notify.Models.Responses
{
    public class LetterNotificationResponse : NotificationResponse
    {
        public LetterResponseContent content;

        public override bool Equals(object response)
        {
            if (!(response is LetterNotificationResponse resp))
            {
                return false;
            }

            return 
                content.body == resp.content.body &&
                content.subject == resp.content.subject &&
                base.Equals(resp);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class LetterResponseContent
    {
        public string body;
        public string subject;
    }
}
