namespace Notify.Models
{
    public class Template
    {
        public string id;
        public string uri;
        public int version;

        public override bool Equals(object other)
        {
            if (!(other is Template o))
            {
                return false;
            }

            return id == o.id && uri == o.uri && version == o.version;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
