using System;
using System.Collections.Generic;
using System.Linq;

namespace Notify.Models.Responses
{
    public class TemplateResponse
    {
        public string id;
        public string name;
        public string type;
        public DateTime created_at;
        public DateTime? updated_at;
        public string created_by;
        public int version;
        public string body;
        public string subject;
        public string letter_contact_block;
        public Dictionary<string, Dictionary<string, bool>> personalisation;

        public override bool Equals(object response)
        {
            if (!(response is TemplateResponse resp))
            {
                return false;
            }

            return
                id == resp.id &&
                name == resp.name &&
                type == resp.type &&
                created_at == resp.created_at &&
                updated_at == resp.updated_at &&
                created_by == resp.created_by &&
                version == resp.version &&
                body == resp.body &&
                subject == resp.subject &&
                letter_contact_block == resp.letter_contact_block &&
                PersonalisationEquals(resp.personalisation);
        }

        private bool PersonalisationEquals(Dictionary<string, Dictionary<string, bool>> other)
        {
            if (personalisation == null && other == null)
            {
                return true;
            }
            if (personalisation == null || other == null || personalisation.Count != other.Count)
            {
                return false;
            }

            return personalisation.All(item =>
                other.ContainsKey(item.Key) &&
                item.Value != null &&
                other[item.Key] != null &&
                item.Value.Count == other[item.Key].Count &&
                item.Value.All(inner =>
                    other[item.Key].ContainsKey(inner.Key) &&
                    other[item.Key][inner.Key] == inner.Value));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
