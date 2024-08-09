using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models;
using Notify.Models.Responses;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Tests.UnitTests
{
    [TestFixture]
    public class NotificationClientUnitTests
    {
        private Mock<HttpMessageHandler> handler;
        private NotificationClient client;

        [SetUp]
        public void SetUp()
        {
            handler = new Mock<HttpMessageHandler>();

            var w = new HttpClientWrapper(new HttpClient(handler.Object));
            client = new NotificationClient(w, Constants.fakeApiKey);
        }

        [TearDown]
        public void TearDown()
        {
            handler = null;
            client = null;
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void CreateNotificationClientWithInvalidApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient("someinvalidkey"));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void CreateNotificationClientWithEmptyApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient(""));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetNonJsonResponseHandlesException()
        {
            MockRequest("non json response",
                client.GET_ALL_NOTIFICATIONS_URL,
                AssertValidRequest, status: HttpStatusCode.NotFound);

            var ex = Assert.Throws<NotifyClientException>(() => client.GetNotifications());
            Assert.That(ex.Message, Does.Contain("Status code 404. Error: non json response"));
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetNotificationByIdCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeNotificationJson,
                client.GET_NOTIFICATION_URL + Constants.fakeNotificationId,
                AssertValidRequest);

            client.GetNotificationById(Constants.fakeNotificationId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetPdfForLetterCreatesExpectedRequest()
        {
            var responseAsString = "%PDF-1.5 testpdf";
            MockRequest(
                responseAsString,
                string.Format(client.GET_PDF_FOR_LETTER_URL, Constants.fakeNotificationId),
                AssertValidRequest);

            client.GetPdfForLetter(Constants.fakeNotificationId);
        }


        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL,
                AssertValidRequest);

            client.GetNotifications();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllNotificationsWithStatusCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?status=sending",
                AssertValidRequest);

            client.GetNotifications(status: "sending");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllNotificationsWithReferenceCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?reference=foo",
                AssertValidRequest);

            client.GetNotifications(reference: "foo");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllNotificationsWithIncludeSpreadsheetUploadsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?include_jobs=True",
                AssertValidRequest);

            client.GetNotifications(includeSpreadsheetUploads: true);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllSmsNotificationsWithStatusAndReferenceWithCreatesExpectedResult()
        {
            MockRequest(Constants.fakeNotificationsJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms&status=sending&reference=foo",
                AssertValidRequest);

            client.GetNotifications(templateType: "sms", status: "sending", reference: "foo");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllSmsNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeSmsNotificationResponseJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms",
                AssertValidRequest);

            client.GetNotifications(templateType: "sms");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllEmailNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeEmailNotificationResponseJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=email",
                AssertValidRequest);

            client.GetNotifications(templateType: "email");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllLetterNotificationsCreatesExpectedResult()
        {
            MockRequest(Constants.fakeEmailNotificationResponseJson,
                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=letter",
                AssertValidRequest);

            client.GetNotifications(templateType: "letter");
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetTemplateWithIdCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId,
                AssertValidRequest);

            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + client.VERSION_PARAM + "2",
                AssertValidRequest);

            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetNotificationByIdReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<Notification>(Constants.fakeNotificationJson);

            MockRequest(Constants.fakeNotificationJson);

            var responseNotification = client.GetNotificationById(Constants.fakeNotificationId);
            ClassicAssert.AreEqual(expectedResponse, responseNotification);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetPdfForLetterReceivesExpectedResponse()
        {
            var responseAsString = "%PDF-1.5 testpdf";
            var expectedResponse = Encoding.UTF8.GetBytes(responseAsString);

            MockRequest(responseAsString);

            var actualResponse = client.GetPdfForLetter(Constants.fakeNotificationId);
            ClassicAssert.AreEqual(expectedResponse, actualResponse);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetTemplateWithIdReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            MockRequest(Constants.fakeTemplateResponseJson);

            var responseTemplate = client.GetTemplateById(Constants.fakeTemplateId);
            ClassicAssert.AreEqual(expectedResponse, responseTemplate);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionReceivesExpectedResponse()
        {
            var expectedResponse =
                JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            MockRequest(Constants.fakeTemplateResponseJson);

            var responseTemplate = client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
            ClassicAssert.AreEqual(expectedResponse, responseTemplate);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GenerateTemplatePreviewGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
                    { "name", "someone" }
            };

            var o = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", AssertValidRequest, HttpMethod.Post);

            client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GenerateTemplatePreviewReceivesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
                    { "name", "someone" }
            };

            var expected = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview",
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeTemplateListResponseJson,
                 client.GET_ALL_TEMPLATES_URL, AssertValidRequest);

            client.GetAllTemplates();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesBySmsTypeCreatesExpectedRequest()
        {
            const string type = "sms";
            MockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            client.GetAllTemplates(type);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesByEmailTypeCreatesExpectedRequest()
        {
            const string type = "email";

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            client.GetAllTemplates(type);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesForEmptyListReceivesExpectedResponse()
        {
            var expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmptyListResponseJson);

            MockRequest(Constants.fakeTemplateEmptyListResponseJson);

            TemplateList templateList = client.GetAllTemplates();

            List<TemplateResponse> templates = templateList.templates;

            ClassicAssert.AreEqual(templates.Count, 0);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesReceivesExpectedResponse()
        {
            TemplateList expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateListResponseJson);

            MockRequest(Constants.fakeTemplateListResponseJson);

            TemplateList templateList = client.GetAllTemplates();

            List<TemplateResponse> templates = templateList.templates;

            ClassicAssert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                ClassicAssert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesBySmsTypeReceivesExpectedResponse()
        {
            const string type = "sms";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateSmsListResponseJson);

            MockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = client.GetAllTemplates(type);

            List<TemplateResponse> templates = templateList.templates;

            ClassicAssert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                ClassicAssert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllTemplatesByEmailTypeReceivesExpectedResponse()
        {
            const string type = "email";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmailListResponseJson);

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = client.GetAllTemplates(type);

            List<TemplateResponse> templates = templateList.templates;

            ClassicAssert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                ClassicAssert.AreEqual(expectedResponse.templates[i], templates[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllReceivedTextsCreatesExpectedRequest()
        {
            MockRequest(Constants.fakeReceivedTextListResponseJson,
                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            client.GetReceivedTexts();
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void GetAllReceivedTextsReceivesExpectedResponse()
        {
            MockRequest(Constants.fakeReceivedTextListResponseJson,
                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            ReceivedTextListResponse expectedResponse =
                JsonConvert.DeserializeObject<ReceivedTextListResponse>(Constants.fakeReceivedTextListResponseJson);

            MockRequest(Constants.fakeReceivedTextListResponseJson,
                         client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

            ReceivedTextListResponse receivedTextList = client.GetReceivedTexts();

            List<ReceivedTextResponse> receivedTexts = receivedTextList.receivedTexts;

            ClassicAssert.AreEqual(receivedTexts.Count, expectedResponse.receivedTexts.Count);
            for (int i = 0; i < receivedTexts.Count; i++)
            {
                ClassicAssert.AreEqual(expectedResponse.receivedTexts[i], receivedTexts[i]);
            }
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };

            MockRequest(Constants.fakeSmsNotificationResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendSmsNotificationWithSmsSenderIdGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            var expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "sms_sender_id", Constants.fakeSMSSenderId }
            };

            MockRequest(Constants.fakeSmsNotificationWithSMSSenderIdResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendSms(
                Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation: personalisation, smsSenderId: Constants.fakeSMSSenderId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            SmsNotificationResponse expectedResponse = JsonConvert.DeserializeObject<SmsNotificationResponse>(Constants.fakeSmsNotificationResponseJson);

            MockRequest(Constants.fakeSmsNotificationResponseJson);

            SmsNotificationResponse actualResponse = client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

            ClassicAssert.AreEqual(expectedResponse, actualResponse);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            EmailNotificationResponse expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            EmailNotificationResponse actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            ClassicAssert.AreEqual(expectedResponse, actualResponse);

        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationWithReplyToIdGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference },
                { "email_reply_to_id", Constants.fakeReplyToId}
            };

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None));

            client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationWithReplyToIdGeneratesExpectedResponse()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            var actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);

            ClassicAssert.AreEqual(expectedResponse, actualResponse);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClientAsync")]
        public void SendEmailNotificationWithoneClickUnsubscribeURLGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            var expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "one_click_unsubscribe_url", Constants.fakeoneClickUnsubscribeURL },
            };

            MockRequest(
                Constants.fakeEmailNotificationResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None)
            );

            client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, oneClickUnsubscribeURL: Constants.fakeoneClickUnsubscribeURL);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationWithDocumentGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf")) }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", new JObject
                  {
                    {"document", new JObject
                      {
                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
                        {"filename", null},
                        {"confirm_email_before_download", null},
                        {"retention_period", null}
                      }
                    }
                  }
                },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendEmailNotificationWithFilenameDocumentGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"), "report.csv") }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", new JObject
                  {
                    {"document", new JObject
                      {
                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
                        {"filename", "report.csv"},
                        {"confirm_email_before_download", null},
                        {"retention_period", null}
                      }
                    }
                  }
                },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void PrepareUploadWithLargeDocumentGeneratesAnError()
        {
            Assert.That(
                    () => { NotificationClient.PrepareUpload(new byte[3 * 1024 * 1024]); },
                    Throws.ArgumentException
                    );
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void PrepareUploadCanSetConfirmEmailBeforeDownloadAndRetentionPeriod()
        {
            var fileData = new byte[1 * 1024 * 1024];
            var actual = NotificationClient.PrepareUpload(fileData, "report.csv", false, "1 weeks");
            var expected = new JObject
            {
                {"file", System.Convert.ToBase64String(fileData)},
                {"filename", "report.csv"},
                {"confirm_email_before_download", false},
                {"retention_period", "1 weeks"}
            };
            ClassicAssert.AreEqual(actual, expected);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendLetterNotificationGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "address_line_1", "Foo" },
                    { "address_line_2", "Bar" },
                    { "postcode", "SW1 1AA" }
                };
            JObject expected = new JObject
            {
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_LETTER_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendLetterNotificationGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "address_line_1", "Foo" },
                    { "address_line_2", "Bar" },
                    { "postcode", "SW1 1AA" }
                };
            LetterNotificationResponse expectedResponse = JsonConvert.DeserializeObject<LetterNotificationResponse>(Constants.fakeLetterNotificationResponseJson);

            MockRequest(Constants.fakeLetterNotificationResponseJson);

            LetterNotificationResponse actualResponse = client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            ClassicAssert.AreEqual(expectedResponse, actualResponse);

        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendPrecompiledLetterNotificationGeneratesExpectedRequest()
        {
            JObject expected = new JObject
            {
                { "reference", Constants.fakeNotificationReference },
                { "content", "JVBERi0xLjUgdGVzdHBkZg==" }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_LETTER_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendPrecompiledLetter(
                    Constants.fakeNotificationReference,
                    Encoding.UTF8.GetBytes("%PDF-1.5 testpdf")
            );
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendPrecompiledLetterNotificationGeneratesExpectedRequestWithPostage()
        {
            JObject expected = new JObject
            {
                { "reference", Constants.fakeNotificationReference },
                { "content", "JVBERi0xLjUgdGVzdHBkZg==" },
                { "postage", "first" }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_LETTER_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.SendPrecompiledLetter(
                    Constants.fakeNotificationReference,
                    Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"),
                    "first"
            );
        }

        [Test, Category("Unit"), Category("Unit/NotificationClient")]
        public void SendPrecompiledLetterNotificationGeneratesExpectedResponse()
        {
            LetterNotificationResponse expectedResponse = JsonConvert.DeserializeObject<LetterNotificationResponse>(Constants.fakePrecompiledLetterNotificationResponseJson);

            MockRequest(Constants.fakePrecompiledLetterNotificationResponseJson);

            LetterNotificationResponse actualResponse = client.SendPrecompiledLetter(Constants.fakeNotificationReference, Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"));

            ClassicAssert.IsNotNull(expectedResponse.id);
            ClassicAssert.IsNotNull(expectedResponse.reference);
            ClassicAssert.IsNotNull(expectedResponse.postage);
            ClassicAssert.IsNull(expectedResponse.content);
            ClassicAssert.AreEqual(expectedResponse, actualResponse);

        }

        private static void AssertGetExpectedContent(string expected, string content)
        {
            ClassicAssert.IsNotNull(content);
            ClassicAssert.AreEqual(expected, content);
        }

        private void AssertValidRequest(string uri, HttpRequestMessage r, HttpMethod method = null)
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }

            ClassicAssert.AreEqual(r.Method, method);
            ClassicAssert.AreEqual(r.RequestUri.ToString(), client.BaseUrl + uri);
            ClassicAssert.IsNotNull(r.Headers.Authorization);
            ClassicAssert.IsNotNull(r.Headers.UserAgent);
            ClassicAssert.AreEqual(r.Headers.UserAgent.ToString(), client.GetUserAgent());
            ClassicAssert.AreEqual(r.Headers.Accept.ToString(), "application/json");
        }

        private void MockRequest(string content, string uri,
                          Action<string, HttpRequestMessage, HttpMethod> _assertValidRequest = null,
                          HttpMethod method = null,
                          Action<string, string> _assertGetExpectedContent = null,
                          string expected = null,
                          HttpStatusCode status = HttpStatusCode.OK)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(content)
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    _assertValidRequest(uri, r, method);

                    if (r.Content == null || _assertGetExpectedContent == null) return;

                    var response = r.Content.ReadAsStringAsync().Result;
                    _assertGetExpectedContent(expected, response);
                });
        }

        private void MockRequest(string content)
        {

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                }));
        }
    }
}
