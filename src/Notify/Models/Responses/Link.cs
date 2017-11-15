using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class Link
    {
        [JsonProperty("current")]
        public String current;
        [JsonProperty("next")]
        public String next;
    }
}