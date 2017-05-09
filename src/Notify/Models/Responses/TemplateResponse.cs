using System;
using System.Text;
using System.Reflection;

namespace Notify.Models.Responses
{
    public class TemplateResponse
    {
        public String id;
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
                type == response.type &&
                created_at == response.created_at &&
                updated_at == response.updated_at &&
                created_by == response.created_by &&
                version == response.version &&
                body == response.body &&
                subject == response.subject
            );
        }
        
        public override String ToString(){
        	return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", id, type, created_at, updated_at, created_by, version, body, subject);
        }
    }
}