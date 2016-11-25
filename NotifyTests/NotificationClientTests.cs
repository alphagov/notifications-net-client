using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Notify.Exceptions;
using Notify.Models;
using Notify;
using Notify.Client;
using Notify.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notify.Models.Responses;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using NotifyTests;
using System.Reflection;

namespace NotifyUnitTests
{
    [TestClass()]
    public class NotificationClientTests
    {
        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        [ExpectedException(typeof(NotifyAuthException), "A client was instantiated with an invalid key")]
        public void CreateNotificationClientWithInvalidApiKeyFails()
        {
            try
            {
                NotificationClient n = new NotificationClient("someinvalidkey");
            }
            catch(Exception e)
            {
                Assert.AreEqual(e.Message, "The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
                throw;
            }
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        [ExpectedException(typeof(NotifyAuthException), "A client was instantiated with an invalid key")]
        public void CreateNotificationClientWithEmptyApiKeyFails()
        {
            try
            {
                NotificationClient n = new NotificationClient("");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
                throw;
            }
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetNotificationWithIdCreatesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeNotificationJson)
                    };
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) => 
                {
                    Assert.AreEqual(r.Method, HttpMethod.Get);
                    Assert.AreEqual(r.RequestUri.ToString(), n.baseUrl + n.GET_NOTIFICATION_URL + Constants.fakeNotificationId);
                    Assert.IsNotNull(r.Headers.Authorization);
                    Assert.IsNotNull(r.Headers.UserAgent);
                    Assert.AreEqual(r.Headers.UserAgent.ToString(), Constants.userAgent);
                    Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
                });

            n.GetNotificationById(Constants.fakeNotificationId);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetNotificationWithIdReceivesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            Notification expectedResponse = JsonConvert.DeserializeObject<Notification>(Constants.fakeNotificationJson);

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeNotificationJson)
                    };
                }));

            Notification responseNotification = n.GetNotificationById(Constants.fakeNotificationId);
            Assert.IsTrue(expectedResponse.EqualTo(responseNotification));
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            JObject o = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };

            String content = "";
            HttpRequestMessage request = null;
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeSmsNotificationResponseJson)
                    };
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    content = r.Content.ReadAsStringAsync().Result;
                    request = r;
                });

            SmsNotificationResponse response = n.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

            Assert.AreEqual(request.Method, HttpMethod.Post);
            Assert.AreEqual(request.RequestUri.ToString(), n.baseUrl + n.SEND_SMS_NOTIFICATION_URL);
            Assert.IsNotNull(request.Headers.Authorization);
            Assert.IsNotNull(request.Headers.UserAgent);
            Assert.AreEqual(request.Headers.UserAgent.ToString(), Constants.userAgent);
            Assert.AreEqual(request.Headers.Accept.ToString(), "application/json");
            Assert.IsNotNull(content);
            Assert.AreEqual(o.ToString(Formatting.None), content);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void SendSmsNotificationGeneratesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            JObject o = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };
            SmsNotificationResponse expectedResponse = JsonConvert.DeserializeObject<SmsNotificationResponse>(Constants.fakeSmsNotificationResponseJson);

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeSmsNotificationResponseJson)
                    };
                }));

            SmsNotificationResponse actualResponse = n.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));

        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            JObject o = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };

            String content = "";
            HttpRequestMessage request = null;
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeEmailNotificationResponseJson)
                    };
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    content = r.Content.ReadAsStringAsync().Result;
                    request = r;
                });

            EmailNotificationResponse response = n.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation);

            Assert.AreEqual(request.Method, HttpMethod.Post);
            Assert.AreEqual(request.RequestUri.ToString(), n.baseUrl + n.SEND_EMAIL_NOTIFICATION_URL);

            Assert.IsNotNull(request.Headers.Authorization);
            Assert.IsNotNull(request.Headers.UserAgent);
            Assert.AreEqual(request.Headers.UserAgent.ToString(), Constants.userAgent);
            Assert.AreEqual(request.Headers.Accept.ToString(), "application/json");
            Assert.IsNotNull(content);
            String before = o.ToString(Formatting.None);
            Assert.AreEqual(before, content);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void SendEmailNotificationGeneratesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                {
                    { "name", "someone" }
                };
            JObject o = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) }
            };
            EmailNotificationResponse expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(Constants.fakeEmailNotificationResponseJson)
                    };
                }));

            EmailNotificationResponse actualResponse = n.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));

        }

    }
}