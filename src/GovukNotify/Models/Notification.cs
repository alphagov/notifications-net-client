using Newtonsoft.Json;
using System;

namespace Notify.Models
{
    public class Notification
    {
        public String id;
        [JsonProperty("completed_at")]
        public String completedAt;
        [JsonProperty("created_at")]
        public String createdAt;
        [JsonProperty("email_address")]
        public String emailAddress;
        public String body;
        public String subject;
        public String line1;
        public String line2;
        public String line3;
        public String line4;
        public String line5;
        public String line6;
        [JsonProperty("phone_number")]
        public String phoneNumber;
        public String postcode;
        public String reference;
        public String sentAt;
        public String status;
        public Template template;
        public String type;

        public bool EqualTo(Notification notification)
        {
            return (
                id == notification.id &&
                completedAt == notification.completedAt &&
                createdAt == notification.createdAt &&
                emailAddress == notification.emailAddress &&
                line1 == notification.line1 &&
                line2 == notification.line2 &&
                line3 == notification.line3 &&
                line4 == notification.line4 &&
                line5 == notification.line5 &&
                line6 == notification.line6 &&
                phoneNumber == notification.phoneNumber &&
                postcode == notification.postcode &&
                reference == notification.reference &&
                sentAt == notification.sentAt &&
                status == notification.status &&
                template.id == notification.template.id &&
                template.uri == notification.template.uri &&
                template.version == notification.template.version &&
                type == notification.type
            );
        }

    }
}
