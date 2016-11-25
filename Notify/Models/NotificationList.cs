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
        List<Notification> notifications;
        [JsonProperty("links")]
        Link links;
    }

    public class Link
    {
        [JsonProperty("current")]
        String current;
        [JsonProperty("next")]
        String next;
    }
}
