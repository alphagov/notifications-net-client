using Notify.Models;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;

namespace Notify.Interfaces
{
    public interface INotificationClient : IBaseClient
    {
        Notification GetNotificationById(String notificationId);

        List<Notification> GetNotifications(String templateType = "", String status = "");

        SmsNotificationResponse SendSms(String mobileNumber, String templateId, Dictionary<String, dynamic> personalisation = null);

        EmailNotificationResponse SendEmail(String emailAddress, String templateId, Dictionary<String, dynamic> personalisation = null);
    }
}
