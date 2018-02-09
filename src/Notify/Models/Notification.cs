using Newtonsoft.Json;

namespace Notify.Models
{
    public class Notification
    {
        public string id;
        [JsonProperty("completed_at")]
        public string completedAt;
        [JsonProperty("created_at")]
        public string createdAt;
        [JsonProperty("email_address")]
        public string emailAddress;
        public string body;
        public string subject;
        public string line1;
        public string line2;
        public string line3;
        public string line4;
        public string line5;
        public string line6;
        [JsonProperty("phone_number")]
        public string phoneNumber;
        public string postcode;
        public string reference;
        public string sentAt;
        public string status;
        public Template template;
        public string type;

        public override bool Equals(object notification)
        {
            if (!(notification is Notification note))
            {
                return false;
            }

            return 
                id == note.id &&
                completedAt == note.completedAt &&
                createdAt == note.createdAt &&
                emailAddress == note.emailAddress &&
                line1 == note.line1 &&
                line2 == note.line2 &&
                line3 == note.line3 &&
                line4 == note.line4 &&
                line5 == note.line5 &&
                line6 == note.line6 &&
                phoneNumber == note.phoneNumber &&
                postcode == note.postcode &&
                reference == note.reference &&
                sentAt == note.sentAt &&
                status == note.status &&
                template.id == note.template.id &&
                template.uri == note.template.uri &&
                template.version == note.template.version &&
                type == note.type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
