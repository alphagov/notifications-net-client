using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notify.Client
{
    public class NotificationClient : BaseClient, INotificationClient, IAsyncNotificationClient
    {
        public string GET_RECEIVED_TEXTS_URL = "v2/received-text-messages";
        public string GET_NOTIFICATION_URL = "v2/notifications/";
        public string GET_PDF_FOR_LETTER_URL = "v2/notifications/{0}/pdf";
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

        public async Task<Notification> GetNotificationByIdAsync(string notificationId)
        {
            var url = GET_NOTIFICATION_URL + notificationId;

            var response = await this.GET(url).ConfigureAwait(false);

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

        public async Task<NotificationList> GetNotificationsAsync(string templateType = "", string status = "", string reference = "",
            string olderThanId = "", bool includeSpreadsheetUploads = false)
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

            if (includeSpreadsheetUploads)
            {
                query.Add("include_jobs", "True");
            }

            var finalUrl = GET_ALL_NOTIFICATIONS_URL + ToQueryString(query);
            var response = await GET(finalUrl).ConfigureAwait(false);

            var notifications = JsonConvert.DeserializeObject<NotificationList>(response);
            return notifications;
        }

        public async Task<TemplateList> GetAllTemplatesAsync(string templateType = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_ALL_TEMPLATES_URL,
                templateType == string.Empty ? string.Empty : TYPE_PARAM + templateType
            );

            var response = await GET(finalUrl).ConfigureAwait(false);

            var templateList = JsonConvert.DeserializeObject<TemplateList>(response);

            return templateList;
        }

        public async Task<ReceivedTextListResponse> GetReceivedTextsAsync(string olderThanId = "")
        {
            var finalUrl = string.Format(
                "{0}{1}",
                GET_RECEIVED_TEXTS_URL,
                string.IsNullOrWhiteSpace(olderThanId) ? "" : "?older_than=" + olderThanId
            );

            var response = await this.GET(finalUrl).ConfigureAwait(false);

            var receivedTexts = JsonConvert.DeserializeObject<ReceivedTextListResponse>(response);

            return receivedTexts;
        }

        public async Task<SmsNotificationResponse> SendSmsAsync(string mobileNumber, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string smsSenderId = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("phone_number", mobileNumber));

            if (smsSenderId != null)
            {
                o.Add(new JProperty("sms_sender_id", smsSenderId));
            }

            var response = await POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<SmsNotificationResponse>(response);
        }

        public async Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string emailReplyToId = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("email_address", emailAddress));

            if (emailReplyToId != null)
            {
                o.Add(new JProperty("email_reply_to_id", emailReplyToId));
            }

            var response = await POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
        }

        public async Task<LetterNotificationResponse> SendLetterAsync(string templateId, Dictionary<string, dynamic> personalisation,
            string clientReference = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);

            var response = await this.POST(SEND_LETTER_NOTIFICATION_URL, o.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<LetterNotificationResponse>(response);
        }

        public async Task<LetterNotificationResponse> SendPrecompiledLetterAsync(string clientReference, byte[] pdfContents, string postage = null)
        {
            var requestParams = new JObject
            {
                {"reference", clientReference},
                {"content", System.Convert.ToBase64String(pdfContents)}
            };

            if (postage != null)
            {
                requestParams.Add(new JProperty("postage", postage));
            }

            var response = await this.POST(SEND_LETTER_NOTIFICATION_URL, requestParams.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<LetterNotificationResponse>(response);
        }

        public async Task<TemplateResponse> GetTemplateByIdAsync(string templateId)
        {
            var url = GET_TEMPLATE_URL + templateId;

            return await GetTemplateFromURLAsync(url).ConfigureAwait(false);
        }

        public async Task<TemplateResponse> GetTemplateByIdAndVersionAsync(string templateId, int version = 0)
        {
            var pattern = "{0}{1}" + (version > 0 ? VERSION_PARAM + "{2}" : "");
            var url = string.Format(pattern, GET_TEMPLATE_URL, templateId, version);

            return await GetTemplateFromURLAsync(url).ConfigureAwait(false);
        }

        public async Task<TemplatePreviewResponse> GenerateTemplatePreviewAsync(string templateId,
            Dictionary<string, dynamic> personalisation = null)
        {
            var url = string.Format("{0}{1}/preview", GET_TEMPLATE_URL, templateId);

            var o = new JObject
            {
                {"personalisation", JObject.FromObject(personalisation)}
            };

            var response = await this.POST(url, o.ToString(Formatting.None)).ConfigureAwait(false);

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


        public async Task<byte[]> GetPdfForLetterAsync(string notificationId)
        {
            var finalUrl = string.Format(GET_PDF_FOR_LETTER_URL, notificationId);
            var response = await GETBytes(finalUrl).ConfigureAwait(false);
            return response;
        }

        public static JObject PrepareUpload(byte[] documentContents, bool isCsv = false)
        {
            if (documentContents.Length > 2 * 1024 * 1024) {
                throw new System.ArgumentException("File is larger than 2MB");
            }
            return new JObject
            {
                {"file", System.Convert.ToBase64String(documentContents)},
                {"is_csv", isCsv}
            };
        }

        private async Task<TemplateResponse> GetTemplateFromURLAsync(string url)
        {
            var response = await this.GET(url).ConfigureAwait(false);

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

        private static Exception HandleAggregateException(AggregateException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (ex.InnerExceptions != null && ex.InnerExceptions.Count == 1)
            {
                return ex.InnerException;
            }
            else
            {
                return ex;
            }
        }

        public TemplatePreviewResponse GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation = null)
        {
            try
            {
                return GenerateTemplatePreviewAsync(templateId, personalisation).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateList GetAllTemplates(string templateType = "")
        {
            try
            {
                return GetAllTemplatesAsync(templateType).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public Notification GetNotificationById(string notificationId)
        {
            try
            {
                return GetNotificationByIdAsync(notificationId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public NotificationList GetNotifications(string templateType = "", string status = "", string reference = "", string olderThanId = "", bool includeSpreadsheetUploads = false)
        {
            try
            {
                return GetNotificationsAsync(templateType, status, reference, olderThanId, includeSpreadsheetUploads).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public ReceivedTextListResponse GetReceivedTexts(string olderThanId = "")
        {
            try
            {
                return GetReceivedTextsAsync(olderThanId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateResponse GetTemplateById(string templateId)
        {
            try
            {
                return GetTemplateByIdAsync(templateId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public TemplateResponse GetTemplateByIdAndVersion(string templateId, int version = 0)
        {
            try
            {
                return GetTemplateByIdAndVersionAsync(templateId, version).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public SmsNotificationResponse SendSms(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null)
        {
            try
            {
                return SendSmsAsync(mobileNumber, templateId, personalisation, clientReference, smsSenderId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null)
        {
            try
            {
                return SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public LetterNotificationResponse SendLetter(string templateId, Dictionary<string, dynamic> personalisation, string clientReference = null)
        {
            try
            {
                return SendLetterAsync(templateId, personalisation, clientReference).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public LetterNotificationResponse SendPrecompiledLetter(string clientReference, byte[] pdfContents, string postage = null)
        {
            try
            {
                return SendPrecompiledLetterAsync(clientReference, pdfContents, postage).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public byte[] GetPdfForLetter(string notificationId)
        {
            try
            {
                return GetPdfForLetterAsync(notificationId).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }
    }
}
