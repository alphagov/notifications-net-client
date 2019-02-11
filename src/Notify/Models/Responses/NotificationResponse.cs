using System;

namespace Notify.Models.Responses
{
    public class NotificationResponse
    {
        public string id;
        public string reference;
        public string uri;
        public Template template;

        public override bool Equals(object response)
        {
            if (!(response is NotificationResponse resp))
            {
                return false;
            }

            return
                id == resp.id &&
                reference == resp.reference &&
                uri == resp.uri &&
                (template == resp.template || template.Equals(resp.template));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
