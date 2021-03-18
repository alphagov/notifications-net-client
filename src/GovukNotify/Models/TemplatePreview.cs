using System;

namespace Notify.Models
{
    public class TemplatePreview
    {
        public string personalisation;

        public override bool Equals(object template)
        {
            if (!(template is TemplatePreview temp))
            {
                return false;
            }

            return personalisation == temp.personalisation;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
