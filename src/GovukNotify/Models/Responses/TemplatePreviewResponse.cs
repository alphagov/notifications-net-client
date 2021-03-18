namespace Notify.Models.Responses
{
    public class TemplatePreviewResponse
    {
        public string id;
        public string name;
        public string type;
        public int version;
        public string body;
        public string subject;

        public override bool Equals(object response)
        {
            if (!(response is TemplatePreviewResponse resp))
            {
                return false;
            }

            return
                id == resp.id &&
                name == resp.name &&
                type == resp.type &&
                version == resp.version &&
                body == resp.body &&
                subject == resp.subject;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
