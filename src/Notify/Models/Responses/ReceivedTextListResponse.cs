using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class ReceivedTextListResponse
    {
        [JsonProperty("received_text_messages")]
        public List<ReceivedTextResponse> receivedTexts;
        [JsonProperty("links")]
        public Link links;
    }
}