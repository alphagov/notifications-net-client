using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Notify.Models
{
    public class NotificationList
    {
        [JsonProperty("notifications")]
        public List<Notification> notifications;
        [JsonProperty("links")]
        public Link links;
    }
}