using Newtonsoft.Json;
using Notify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsClient.Models
{
    public class NotificationList
    {
        [JsonProperty("notifications")]
        public List<Notification> notifications;
        [JsonProperty("links")]
        public Link links;
    }

    public class Link
    {
        [JsonProperty("current")]
        public String current;
        [JsonProperty("next")]
        public String next;
    }
}
