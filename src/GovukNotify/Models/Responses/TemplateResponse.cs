using System;

namespace Notify.Models.Responses
{
    public class TemplateResponse
    {
        public String id;
        public String name;
        public String type;
        public DateTime created_at;
        public DateTime? updated_at;
        public String created_by;
        public int version;
        public String body;
        public String subject;

        public bool EqualTo(TemplateResponse response)
        {
            return (
                id == response.id &&
                name == response.name &&
                type == response.type &&
                created_at == response.created_at &&
                updated_at == response.updated_at &&
                created_by == response.created_by &&
                version == response.version &&
                body == response.body &&
                subject == response.subject
            );
        }
    }
}
