using Notify.Models;
using Notify.Models.Responses;
using System.Collections.Generic;

namespace Notify.Interfaces
{
    public interface INotificationClient : IBaseClient
    {
        Notification GetNotificationById(string notificationId);

        NotificationList GetNotifications(string templateType = "", string status = "", string reference = "", string olderThanId = "");

        SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null);

        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null);
    }
}
