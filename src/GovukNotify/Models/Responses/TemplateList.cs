using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class TemplateList
    {
        [JsonProperty("templates")]
        public List<TemplateResponse> templates;
    }
}
