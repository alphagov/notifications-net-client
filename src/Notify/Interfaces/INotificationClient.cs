using Notify.Models;
using Notify.Models.Responses;
using System.Collections.Generic;

namespace Notify.Interfaces
{
    public interface INotificationClient : IBaseClient
    {
        Notification GetNotificationById(string notificationId);

        List<Notification> GetNotifications(string templateType = "", string status = "");

        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null);

        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null);
    }
}
