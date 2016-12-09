using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace Notify.Client
{
    public class NotificationClient : BaseClient
    {
        public String GET_NOTIFICATION_URL = "v2/notifications/";
        public String SEND_SMS_NOTIFICATION_URL = "v2/notifications/sms";
        public String SEND_EMAIL_NOTIFICATION_URL = "v2/notifications/email";

        public NotificationClient(String apiKey) : base(new HttpClientWrapper(new HttpClient()), apiKey)
        {

        }

        public NotificationClient(String baseUrl, String apiKey) : base(new HttpClientWrapper(new HttpClient()), apiKey, baseUrl)
        {

        }

        public NotificationClient(IHttpClient client, String apiKey) : base(client, apiKey)
        {

        }

        public Notification GetNotificationById(String notificationId)
        {
            String url = GET_NOTIFICATION_URL + notificationId;

            String response = this.GET(url);

            try
            {
                Notification notification = JsonConvert.DeserializeObject<Notification>(response);
                return notification;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Notification object from response: {0}", response);
            }

        }

        public NotificationList GetNotifications(String templateType = "", String status = "")
        {
            var query = HttpUtility.ParseQueryString("");

            if (!String.IsNullOrWhiteSpace(templateType))
            {
                query.Add("template_type", templateType);
            }

            if (!String.IsNullOrWhiteSpace(status))
            {
                query.Add("status", status);
            }

            String finalUrl = "v2/notifications?" + query.ToString();

            String response = this.GET(finalUrl);

            NotificationList notifications = JsonConvert.DeserializeObject<NotificationList>(response);
            return notifications;
        }

        public SmsNotificationResponse SendSms(String mobileNumber, String templateId, Dictionary<String, dynamic> personalisation = null, String clientReference = null)
        {

            JObject personalisationJson = new JObject();

            if(personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            JObject o = new JObject
            {
                { "phone_number", mobileNumber },
                { "template_id", templateId },
                { "personalisation", personalisationJson }
            };

            if (clientReference != null)
            {
                o.Add("reference", clientReference);
            }

            String response = this.POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None));

            SmsNotificationResponse receipt = JsonConvert.DeserializeObject<SmsNotificationResponse>(response);
            return receipt;
        }

        public EmailNotificationResponse SendEmail(String emailAddress, String templateId, Dictionary<String, dynamic> personalisation = null, String clientReference = null)
        {
            JObject personalisationJson = new JObject();

            if(personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            JObject o = new JObject
            {
                { "email_address", emailAddress },
                { "template_id", templateId },
                { "personalisation", personalisationJson }
            };

            if(clientReference != null)
            {
                o.Add("reference", clientReference);
            }

            String response = this.POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None));

            EmailNotificationResponse receipt = JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
            return receipt;
        }

    }
}
