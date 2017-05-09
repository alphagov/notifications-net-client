using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models;
using Notify.Models.Responses;
using NotifyTests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


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
        public void GetTemplateWithIdCreatesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            mockResponse(handler, n, Constants.fakeTemplateResponseJson, 
            	n.GET_TEMPLATE_URL + Constants.fakeTemplateId);

            n.GetTemplateByIdAndVersion(Constants.fakeTemplateId);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionCreatesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            mockResponse(handler, n, Constants.fakeTemplateResponseJson, 
            	n.GET_TEMPLATE_URL + Constants.fakeTemplateId + n.VERSION_PARAM + "2");

            n.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
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
        public void GetTemplateWithIdReceivesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            TemplateResponse expectedResponse = JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            mockResponse(handler, n, Constants.fakeTemplateResponseJson);            
            
            TemplateResponse responseTemplate = n.GetTemplateById(Constants.fakeTemplateId);
            Assert.IsTrue(expectedResponse.EqualTo(responseTemplate));
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateWithIdAndVersionReceivesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            TemplateResponse expectedResponse = 
            	JsonConvert.DeserializeObject<TemplateResponse>(Constants.fakeTemplateResponseJson);

            mockResponse(handler, n, Constants.fakeTemplateResponseJson);            

            TemplateResponse responseTemplate = n.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
            Assert.IsTrue(expectedResponse.EqualTo(responseTemplate));
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GenerateTemplatePreviewGeneratesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic> {
                    { "name", "someone" }            	
            };
            
            JObject o = new JObject
            {
            	{ "personalisation", JObject.FromObject(personalisation) }
            };
            
            mockResponse(handler, n, Constants.fakeTemplatePreviewResponseJson, 
            	n.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", HttpMethod.Post);

            TemplatePreviewResponse response = n.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GenerateTemplatePreviewReceivesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);
            
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic> {
                    { "name", "someone" }            	
            };
            
            JObject expected = new JObject
            {
            	{ "personalisation", JObject.FromObject(personalisation) }
            };

            mockResponse(handler, n, Constants.fakeTemplatePreviewResponseJson, 
            	n.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", 
            	HttpMethod.Post, expected.ToString(Formatting.None), checkPreviewContent);
            
            n.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
        }

        void checkPreviewContent(String expected, String content){
            Assert.IsNotNull(content);
            Assert.AreEqual(expected, content);
        }        
        
        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListCreatesExpectedRequest()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            mockResponse(handler, n, Constants.fakeTemplateListResponseJson, 
            	 n.GET_TEMPLATE_LIST_URL);

            n.GetTemplateList();
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListBySmsTypeCreatesExpectedRequest()
        {
        	const String type = "sms";
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            mockResponse(handler, n, Constants.fakeTemplateSmsListResponseJson, 
                         n.GET_TEMPLATE_LIST_URL + n.TYPE_PARAM + type);

            n.GetTemplateList(type);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListByEmailTypeCreatesExpectedRequest()
        {
        	const String type = "email";
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            mockResponse(handler, n, Constants.fakeTemplateEmailListResponseJson, 
                         n.GET_TEMPLATE_LIST_URL + n.TYPE_PARAM + type);

            n.GetTemplateList(type);
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListReceivesExpectedResponse()
        {
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            TemplateList expectedResponse = JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateListResponseJson);

            mockResponse(handler, n, Constants.fakeTemplateListResponseJson);

            TemplateList templateList = n.GetTemplateList();
            
            List<TemplateResponse> templates = templateList.templates;
            
            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for(int i=0; i < templates.Count; i++) {
            	Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListBySmsTypeReceivesExpectedResponse()
        {
        	const String type = "sms";
        	
            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            TemplateList expectedResponse = 
            	JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateSmsListResponseJson);
            
            mockResponse(handler, n, Constants.fakeTemplateSmsListResponseJson, 
                         n.GET_TEMPLATE_LIST_URL + n.TYPE_PARAM + type);

            TemplateList templateList = n.GetTemplateList(type);
            
            List<TemplateResponse> templates = templateList.templates;
            
            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for(int i=0; i < templates.Count; i++) {
            	Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
        }

        [TestMethod()]
        [TestCategory("Unit/NotificationClient")]
        public void GetTemplateListByEmailTypeReceivesExpectedResponse()
        {
        	const String type = "email";

            var handler = new Mock<HttpMessageHandler>();
            HttpClientWrapper w = new HttpClientWrapper(new HttpClient(handler.Object));

            NotificationClient n = new NotificationClient(w, Constants.fakeApiKey);

            TemplateList expectedResponse = 
            	JsonConvert.DeserializeObject<TemplateList>(Constants.fakeTemplateEmailListResponseJson);
            
            mockResponse(handler, n, Constants.fakeTemplateEmailListResponseJson, 
                         n.GET_TEMPLATE_LIST_URL + n.TYPE_PARAM + type);

            TemplateList templateList = n.GetTemplateList(type);
            
            List<TemplateResponse> templates = templateList.templates;
            
            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
            for(int i=0; i < templates.Count; i++) {
            	Assert.IsTrue(expectedResponse.templates[i].EqualTo(templates[i]));
            }
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
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
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

            EmailNotificationResponse response = n.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

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

            EmailNotificationResponse actualResponse = n.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            Assert.IsTrue(expectedResponse.IsEqualTo(actualResponse));

        }
        
        void mockResponse(Moq.Mock<System.Net.Http.HttpMessageHandler> handler, NotificationClient n, 
                          String content, String uri, HttpMethod method = null, 
                          String expected = null, Action<String, String> _checkContent = null) {
        	if (method == null)
        		method = HttpMethod.Get;
        	String response = "";
        	
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
                    Assert.AreEqual(r.Method, method);
                    Assert.AreEqual(r.RequestUri.ToString(), n.baseUrl + uri);
                    Assert.IsNotNull(r.Headers.Authorization);
                    Assert.IsNotNull(r.Headers.UserAgent);
                    Assert.AreEqual(r.Headers.UserAgent.ToString(), Constants.userAgent);
                    Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
                    
                    if (r.Content != null && _checkContent != null) {
	                    response = r.Content.ReadAsStringAsync().Result;
	                    _checkContent(expected, response);
                    }
                });
        }
        
        void mockResponse(Moq.Mock<System.Net.Http.HttpMessageHandler> handler, NotificationClient n, String content) {
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