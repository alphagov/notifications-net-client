using Newtonsoft.Json;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class NotifyHTTPErrorResponse
    {
        #pragma warning disable 169, 649
        [JsonProperty("status_code")]
        private string statusCode;

        [JsonProperty("errors")]
        private List<NotifyHTTPError> errors;

        [JsonProperty("exception")]
        private string exception;
        
        public string getStatusCode()
            => statusCode;

        public string getException()
            => exception;

        public string getErrorsAsJson(Formatting format = Formatting.Indented)
            => JsonConvert.SerializeObject(errors, format);
    }
}
