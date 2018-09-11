using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Notify.Client
{
    public class NotificationClient : BaseClient
    {
        public string GET_RECEIVED_TEXTS_URL = "v2/received-text-messages";
        public string GET_NOTIFICATION_URL = "v2/notifications/";
        public string SEND_SMS_NOTIFICATION_URL = "v2/notifications/sms";
        public string SEND_EMAIL_NOTIFICATION_URL = "v2/notifications/email";
        public string SEND_LETTER_NOTIFICATION_URL = "v2/notifications/letter";
        public string GET_TEMPLATE_URL = "v2/template/";
        public string GET_ALL_NOTIFICATIONS_URL = "v2/notifications";
        public string GET_ALL_TEMPLATES_URL = "v2/templates";
        public string TYPE_PARAM = "?type=";
        public string VERSION_PARAM = "/version/";

        public NotificationClient(string apiKey) : base(new HttpClientWrapper(new HttpClient()), apiKey)
        {
        }

        public NotificationClient(string baseUrl, string apiKey) : base(new HttpClientWrapper(new HttpClient()), apiKey,
            baseUrl)
        {
        }

        public NotificationClient(IHttpClient client, string apiKey) : base(client, apiKey)
        {
        }

        public Notification GetNotificationById(string notificationId)
        {
            var url = GET_NOTIFICATION_URL + notificationId;

            var response = this.GET(url);

            try
            {
                var notification = JsonConvert.DeserializeObject<Notification>(response);
                return notification;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Notification object from response: {0}", response);
            }
        }

        public static string ToQueryString(NameValueCollection nvc)
        {
            if (nvc.Count == 0) return "";

            IEnumerable<string> segments = from key in nvc.AllKeys
                                           from value in nvc.GetValues(key)
                                           select string.Format("{0}={1}",
                                           WebUtility.UrlEncode(key),
                                           WebUtility.UrlEncode(value));
            return "?" + string.Join("&", segments);
        }

        public NotificationList GetNotifications(string templateType = "", string status = "", string reference = "",
            string olderThanId = "")
        {
            var query = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(templateType))
            {
                query.Add("template_type", templateType);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query.Add("status", status);
            }

            if (!string.IsNullOrWhiteSpace(reference))
            {
                query.Add("reference", reference);
            }

            if (!string.IsNullOrWhiteSpace(olderThanId))
            {
                query.Add("older_than", olderThanId);
            }

            var finalUrl = GET_ALL_NOTIFICATIONS_URL + ToQueryString(query);
            var response = GET(finalUrl);

            var notifications = JsonConvert.DeserializeObject<NotificationList>(response);
            return notifications;
        }

        public TemplateList GetAllTemplates(string templateType = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_ALL_TEMPLATES_URL,
                templateType == string.Empty ? string.Empty : TYPE_PARAM + templateType
            );

            var response = GET(finalUrl);

            var templateList = JsonConvert.DeserializeObject<TemplateList>(response);

            return templateList;
        }

        public ReceivedTextListResponse GetReceivedTexts(string olderThanId = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_RECEIVED_TEXTS_URL,
                string.IsNullOrWhiteSpace(olderThanId) ? "" : "?older_than=" + olderThanId
            );

            var response = this.GET(finalUrl);

            var receivedTexts = JsonConvert.DeserializeObject<ReceivedTextListResponse>(response);

            return receivedTexts;
        }

        public SmsNotificationResponse SendSms(string mobileNumber, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string smsSenderId = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("phone_number", mobileNumber));

            if (smsSenderId != null)
            {
                o.Add(new JProperty("sms_sender_id", smsSenderId));
            }

            var response = POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None));

            return JsonConvert.DeserializeObject<SmsNotificationResponse>(response);            
        }

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string emailReplyToId = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("email_address", emailAddress));

            if (emailReplyToId != null)
            {
                o.Add(new JProperty("email_reply_to_id", emailReplyToId));
            }

            var response = POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None));

            return JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
        }

        public LetterNotificationResponse SendLetter(string templateId, Dictionary<string, dynamic> personalisation,
            string clientReference = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);

            var response = this.POST(SEND_LETTER_NOTIFICATION_URL, o.ToString(Formatting.None));

            return JsonConvert.DeserializeObject<LetterNotificationResponse>(response);            
        }

        public LetterNotificationResponse SendPrecompiledLetter(string clientReference, byte[] pdfContents)
        {
            var requestParams = new JObject
            {
                {"reference", clientReference},
                {"content", System.Convert.ToBase64String(pdfContents)}
            };

            var response = this.POST(SEND_LETTER_NOTIFICATION_URL, requestParams.ToString(Formatting.None));

            return JsonConvert.DeserializeObject<LetterNotificationResponse>(response);
        }

        public TemplateResponse GetTemplateById(string templateId)
        {
            var url = GET_TEMPLATE_URL + templateId;

            return GetTemplateFromURL(url);
        }

        public TemplateResponse GetTemplateByIdAndVersion(string templateId, int version = 0)
        {
            var pattern = "{0}{1}" + (version > 0 ? VERSION_PARAM + "{2}" : "");
            var url = string.Format(pattern, GET_TEMPLATE_URL, templateId, version);

            return GetTemplateFromURL(url);
        }

        public TemplatePreviewResponse GenerateTemplatePreview(string templateId,
            Dictionary<string, dynamic> personalisation = null)
        {
            var url = string.Format("{0}{1}/preview", GET_TEMPLATE_URL, templateId);

            var o = new JObject
            {
                {"personalisation", JObject.FromObject(personalisation)}
            };

            var response = this.POST(url, o.ToString(Formatting.None));

            try
            {
                var template = JsonConvert.DeserializeObject<TemplatePreviewResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        private TemplateResponse GetTemplateFromURL(string url)
        {
            var response = this.GET(url);

            try
            {
                var template = JsonConvert.DeserializeObject<TemplateResponse>(response);
                return template;
            }
            catch (JsonReaderException)
            {
                throw new NotifyClientException("Could not create Template object from response: {0}", response);
            }
        }

        private static JObject CreateRequestParams(string templateId, Dictionary<string, dynamic> personalisation = null,
            string clientReference = null)
        {
            var personalisationJson = new JObject();

            if (personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            var o = new JObject
            {
                {"template_id", templateId},
                {"personalisation", personalisationJson}
            };

            if (clientReference != null)
            {
                o.Add("reference", clientReference);
            }

            return o;
        }
    }
}
