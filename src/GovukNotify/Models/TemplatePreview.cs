using System;

namespace Notify.Models
{
    public class TemplatePreview
    {
        public String personalisation;
        
        public bool EqualTo(TemplatePreview template)
        {
            return (
                personalisation == template.personalisation
            );
        }
    }
}