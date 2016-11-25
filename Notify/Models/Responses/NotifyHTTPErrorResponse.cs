using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Models.Responses
{
    public class NotifyHTTPErrorResponse
    {
        [JsonProperty("status_code")]
        private String statusCode;

        [JsonProperty("errors")]
        private List<NotifyHTTPError> errors;

        public String getStatusCode()
        {
            return statusCode;
        }

        public String getErrorsAsJson()
        {
            return JsonConvert.SerializeObject(this.errors, Formatting.Indented); ;
        }
    }
}
