using System;

namespace Notify.Models.Responses
{
    public class NotificationResponse
    {
        public String id;
        public String reference;
        public String uri;
        public Template template;

        public bool EqualTo(NotificationResponse response)
        {
            return (
                id == response.id &&
                reference == response.reference &&
                uri == response.uri &&
                template.id == response.template.id &&
                template.uri == response.template.uri &&
                template.version == response.template.version
                );
        }

    }
}
