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
        public String SEND_LETTER_NOTIFICATION_URL = "v2/notifications/letter";
        public String GET_TEMPLATE_URL = "v2/template/";
        public String GET_ALL_TEMPLATES_URL = "v2/templates";
        public String TYPE_PARAM = "?type=";
        public String VERSION_PARAM = "/version/";

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

        public NotificationList GetNotifications(String templateType = "", String status = "", String reference = "", String olderThanId = "")
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

            if (!String.IsNullOrWhiteSpace(reference))
            {
                query.Add("reference", reference);
            }

            if (!String.IsNullOrWhiteSpace(olderThanId))
            {
                query.Add("older_than", olderThanId);
            }

            String finalUrl = "v2/notifications?" + query.ToString();

            String response = this.GET(finalUrl);

            NotificationList notifications = JsonConvert.DeserializeObject<NotificationList>(response);
            return notifications;
        }

        public TemplateList GetAllTemplates(String templateType = "")
        {
            String finalUrl = string.Format(
                "{0}{1}",
                GET_ALL_TEMPLATES_URL,
                templateType == "" ? "" : TYPE_PARAM + templateType
            );

            String response = this.GET(finalUrl);

            TemplateList templateList = JsonConvert.DeserializeObject<TemplateList>(response);

            return templateList;
        }

        public SmsNotificationResponse SendSms(String mobileNumber, String templateId, Dictionary<String, dynamic> personalisation = null, String clientReference = null)
        {
            JObject o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("phone_number", mobileNumber));

            String response = this.POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None));

            SmsNotificationResponse receipt = JsonConvert.DeserializeObject<SmsNotificationResponse>(response);
            return receipt;
        }

        public EmailNotificationResponse SendEmail(String emailAddress, String templateId, Dictionary<String, dynamic> personalisation = null, String clientReference = null)
        {
            JObject o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("email_address", emailAddress));

            String response = this.POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None));

            EmailNotificationResponse receipt = JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
            return receipt;
        }

        public LetterNotificationResponse SendLetter(String templateId, Dictionary<String, dynamic> personalisation, String clientReference = null)
        {
            JObject o = CreateRequestParams(templateId, personalisation, clientReference);

            String response = this.POST(SEND_LETTER_NOTIFICATION_URL, o.ToString(Formatting.None));

            LetterNotificationResponse receipt = JsonConvert.DeserializeObject<LetterNotificationResponse>(response);
            return receipt;
        }

        public TemplateResponse GetTemplateById(String templateId)
        {
            String url = GET_TEMPLATE_URL + templateId;

            return GetTemplateFromURL(url);
        }

        public TemplateResponse GetTemplateByIdAndVersion(String templateId, int version = 0)
        {
            String pattern = "{0}{1}" + (version > 0 ? VERSION_PARAM + "{2}" : "");
            String url = string.Format(pattern, GET_TEMPLATE_URL, templateId, version);

            return GetTemplateFromURL(url);
        }

        public TemplatePreviewResponse GenerateTemplatePreview(String templateId, Dictionary<String, dynamic> personalisation = null)
        {
            String url = string.Format("{0}{1}/preview", GET_TEMPLATE_URL, templateId);

            JObject o = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            String response = this.POST(url, o.ToString(Formatting.None));

            try
            {
                TemplatePreviewResponse template = JsonConvert.DeserializeObject<TemplatePreviewResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        TemplateResponse GetTemplateFromURL(String url)
        {
            String response = this.GET(url);

            try
            {
                TemplateResponse template = JsonConvert.DeserializeObject<TemplateResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        JObject CreateRequestParams(String templateId, Dictionary<String, dynamic> personalisation = null, String clientReference = null)
        {
            JObject personalisationJson = new JObject();

            if (personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            JObject o = new JObject
            {
                { "template_id", templateId },
                { "personalisation", personalisationJson }
            };

            if (clientReference != null)
            {
                o.Add("reference", clientReference);
            }

            return o;
        }
    }
}
