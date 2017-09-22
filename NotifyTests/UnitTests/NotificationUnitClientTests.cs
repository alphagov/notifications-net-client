using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Notify.UnitTests
{
    [TestFixture()]
    public class NotificationUnitClientTests
    {
        Mock<HttpMessageHandler> handler;
        NotificationClient client;

        [SetUp]
        public void SetUp()
        {
            handler = new Mock<HttpMessageHandler>();

            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));
            client = new NotificationClient(w, Constants.fakeApiKey);
        }

        [TearDown]
        public void TearDown()
        {
            handler = null;
            client = null;
        }


        [Test, Category("Unit/NotificationClient")]
        public void CreateNotificationClientWithInvalidApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient("someinvalidkey"));
        }


        [Test, Category("Unit/NotificationClient")]
        public void CreateNotificationClientWithEmptyApiKeyFails()
        {
            Assert.Throws<NotifyAuthException>(() => new NotificationClient(""));
        }


        [Test, Category("Unit/NotificationClient")]
        public void GetNotificationWithIdCreatesExpectedRequest()
        {
            mockRequest(Constants.fakeNotificationJson,
                client.GET_NOTIFICATION_URL + Constants.fakeNotificationId,
                AssertValidRequest);

            client.GetNotificationById(Constants.fakeNotificationId);
        }


        [Test, Category("Unit/NotificationClient")]
        public void GetTemplateWithIdCreatesExpectedRequest()
        {
            mockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId,
                AssertValidRequest);

            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId);
        }


        [Test, Category("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionCreatesExpectedRequest()
        {
            mockRequest(Constants.fakeTemplateResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + client.VERSION_PARAM + "2",
                AssertValidRequest);

            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetNotificationWithIdReceivesExpectedResponse()
        {
            Notification expectedResponse = JsonConvert.DeserializeObject<Notification>(Constants.fakeNotificationJson);

            mockRequest(Constants.fakeNotificationJson);

            Notification responseNotification = client.GetNotificationById(Constants.fakeNotificationId);
            Assert.IsTrue(expectedResponse.EqualTo(responseNotification));
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetTemplateWithIdReceivesExpectedResponse()
        {
            TemplateResponse expectedResponse = JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            mockRequest(Constants.fakeTemplateResponseJson);

            TemplateResponse responseTemplate = client.GetTemplateById(Constants.fakeTemplateId);
            Assert.IsTrue(expectedResponse.EqualTo(responseTemplate));
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionReceivesExpectedResponse()
        {
            TemplateResponse expectedResponse =
                JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            mockRequest(Constants.fakeTemplateResponseJson);

            TemplateResponse responseTemplate = client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
            Assert.IsTrue(expectedResponse.EqualTo(responseTemplate));
        }

        [Test, Category("Unit/NotificationClient")]
        public void GenerateTemplatePreviewGeneratesExpectedRequest()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic> {
                    { "name", "someone" }
            };

            JObject o = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            mockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", AssertValidRequest, HttpMethod.Post);

            TemplatePreviewResponse response = client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GenerateTemplatePreviewReceivesExpectedResponse()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic> {
                    { "name", "someone" }
            };

            JObject expected = new JObject
            {
                { "personalisation", JObject.FromObject(personalisation) }
            };

            mockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview",
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesCreatesExpectedRequest()
        {
            mockRequest(Constants.fakeTemplateListResponseJson,
                 client.GET_ALL_TEMPLATES_URL, AssertValidRequest);

            client.GetAllTemplates();
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesBySmsTypeCreatesExpectedRequest()
        {
            const String type = "sms";
            mockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL+ client.TYPE_PARAM + type, AssertValidRequest);

            client.GetAllTemplates(type);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesByEmailTypeCreatesExpectedRequest()
        {
            const String type = "email";

            mockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL+ client.TYPE_PARAM + type, AssertValidRequest);

            client.GetAllTemplates(type);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesForEmptyListReceivesExpectedResponse()
        {
            TemplateList expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmptyListResponseJson);

               mockRequest(Constants.fakeTemplateEmptyListResponseJson);

            TemplateList templateList = client.GetAllTemplates();

            List<TemplateResponse> templates = templateList.templates;

            Assert.IsTrue(templates.Count == 0);
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesReceivesExpectedResponse()
        {
            TemplateList expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateListResponseJson);

            mockRequest(Constants.fakeTemplateListResponseJson);

            TemplateList templateList = client.GetAllTemplates();

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesBySmsTypeReceivesExpectedResponse()
        {
            const String type = "sms";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateSmsListResponseJson);

            mockRequest(Constants.fakeTemplateSmsListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = client.GetAllTemplates(type);

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
        }

        [Test, Category("Unit/NotificationClient")]
        public void GetAllTemplatesByEmailTypeReceivesExpectedResponse()
        {
            const String type = "email";

            TemplateList expectedResponse =
                JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmailListResponseJson);

            mockRequest(Constants.fakeTemplateEmailListResponseJson,
                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

            TemplateList templateList = client.GetAllTemplates(type);

            List<TemplateResponse> templates = templateList.templates;

            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
        }

        [Test, Category("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedRequest()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };

            mockRequest(Constants.fakeSmsNotificationResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            SmsNotificationResponse response = client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);
        }

        [Test, Category("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedResponse()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            SmsNotificationResponse expectedResponse = JsonConvert.DeserializeObject<SmsNotificationResponse>(Constants.fakeSmsNotificationResponseJson);

            mockRequest(Constants.fakeSmsNotificationResponseJson);

            SmsNotificationResponse actualResponse = client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));
        }

        [Test, Category("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedRequest()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
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

            mockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            EmailNotificationResponse response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedResponse()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            EmailNotificationResponse expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            mockRequest(Constants.fakeEmailNotificationResponseJson);

            EmailNotificationResponse actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));

        }

        [Test, Category("Unit/NotificationClient")]
        public void SendLetterNotificationGeneratesExpectedRequest()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "address_line_1", "Foo" },
                    { "address_line_2", "Bar" },
                    { "postcode", "Baz" }
                };
            JObject expected = new JObject
            {
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
            };

            mockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_LETTER_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            LetterNotificationResponse response = client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit/NotificationClient")]
        public void SendLetterNotificationGeneratesExpectedResponse()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "address_line_1", "Foo" },
                    { "address_line_2", "Bar" },
                    { "postcode", "Baz" }
                };
            LetterNotificationResponse expectedResponse = JsonConvert.DeserializeObject<LetterNotificationResponse>(Constants.fakeLetterNotificationResponseJson);

            mockRequest(Constants.fakeLetterNotificationResponseJson);

            LetterNotificationResponse actualResponse = client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));

        }

        void AssertGetExpectedContent(String expected, String content)
        {
            Assert.IsNotNull(content);
            Assert.AreEqual(expected, content);
        }

        void AssertValidRequest(String uri, HttpRequestMessage r, HttpMethod method = null)
        {
            if (method == null)
                method = HttpMethod.Get;
            Assert.AreEqual(r.Method, method);
            Assert.AreEqual(r.RequestUri.ToString(), client.baseUrl + uri);
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.IsNotNull(r.Headers.UserAgent);
            Assert.AreEqual(r.Headers.UserAgent.ToString(), client.GetUserAgent());
            Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
        }

        void mockRequest(String content, String uri,
                          Action<String, HttpRequestMessage, HttpMethod> _assertValidRequest = null,
                          HttpMethod method = null,
                          Action<String, String> _assertGetExpectedContent = null, String expected = null)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(content)
                    };
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    _assertValidRequest(uri, r, method);

                    if (r.Content != null && _assertGetExpectedContent != null)
                    {
                        String response = r.Content.ReadAsStringAsync().Result;
                        _assertGetExpectedContent(expected, response);
                    }
                });
        }

        void mockRequest(String content)
        {

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(content)
                    };
                }));
        }
    }
}
