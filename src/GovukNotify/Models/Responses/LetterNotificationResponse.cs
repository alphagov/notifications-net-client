namespace Notify.Models.Responses
{
    public class LetterNotificationResponse : NotificationResponse
    {
        public string postage;
        public LetterResponseContent content;

        public override bool Equals(object response)
        {
            if (!(response is LetterNotificationResponse resp))
            {
                return false;
            }

            return (content == resp.content || content.Equals(resp.content)) && base.Equals(resp) && (postage == resp.postage);
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

        public override bool Equals(object other)
        {
            if (!(other is LetterResponseContent o))
            {
                return false;
            }

            return body == o.body && subject == o.subject;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
