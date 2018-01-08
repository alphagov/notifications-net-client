using Newtonsoft.Json;
using System.Collections.Generic;

namespace Notify.Models.Responses
{
    public class NotifyHTTPErrorResponse
    {
        [JsonProperty("status_code")]
        private string statusCode;

        [JsonProperty("errors")]
        private List<NotifyHTTPError> errors;

        public string getStatusCode()
        {
            return statusCode;
        }

        public string getErrorsAsJson()
        {
            return JsonConvert.SerializeObject(errors, Formatting.Indented);
        }
    }
}
