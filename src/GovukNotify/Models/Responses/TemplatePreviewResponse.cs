using System;

namespace Notify.Models.Responses
{
    public class TemplatePreviewResponse
    {
        public String id;
        public String name;
        public String type;
        public int version;
        public String body;
        public String subject;

        public bool EqualTo(TemplatePreviewResponse response)
        {
            return (
                id == response.id &&
                name == response.name &&
                type == response.type &&
                version == response.version &&
                body == response.body &&
                subject == response.subject
            );
        }
    }
}
