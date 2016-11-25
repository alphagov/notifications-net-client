using Notify;
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using Notify.Exceptions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NotificationsClient.Models;

namespace NotifyIntegrationTests
{
    [TestClass]
    public class NotificationClientTests
    {
        private NotificationClient client;

        private String API_KEY = Environment.GetEnvironmentVariable("API_KEY");
        private String FUNCTIONAL_TEST_NUMBER = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_NUMBER");
        private String FUNCTIONAL_TEST_EMAIL_ADDRESS = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

        private String EMAIL_PERSONALISATION_TEMPLATE_ID = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_ID");
        private String SMS_PERSONALISATION_TEMPLATE_ID = Environment.GetEnvironmentVariable("SMS_TEMPLATE_ID");

        private String smsNotificationId;
        private String emailNotificationId;

        [TestInitialize]
        [TestCategory("Integration")]
        public void TestInitialise()
        {
            this.client = new NotificationClient(API_KEY);
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void SendSmsTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            SmsNotificationResponse response = 
                this.client.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_PERSONALISATION_TEMPLATE_ID, personalisation);
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "Hello ((name))\n\nFunctional Tests make our world a better place");
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetSMSNotificationWithIdReturnsNotification()
        {
            SendSmsTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.smsNotificationId);

            Assert.IsNotNull(notification);
            Assert.AreEqual(notification.id, this.smsNotificationId);
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void SendEmailTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response = 
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL_ADDRESS, EMAIL_PERSONALISATION_TEMPLATE_ID, personalisation);
            this.emailNotificationId = response.id;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "Hello ((name))\n\nFunctional test help make our world a better place");
            Assert.AreEqual(response.content.subject, "Functional Tests are good");
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetEmailNotificationWithIdReturnsNotification()
        {
            SendEmailTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.emailNotificationId);

            Assert.IsNotNull(notification);
            Assert.AreEqual(notification.id, this.emailNotificationId);
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetAllNotifications()
        {
            NotificationList notifications = this.client.GetNotifications();

            Assert.IsNotNull(notifications);
        }

        [TestMethod()]
        [TestCategory("Integration")]
        [ExpectedException(typeof(NotifyClientException), "A client was instantiated with an invalid key")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            try
            {
                this.client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f");
            }
            catch(Exception e)
            {
                Assert.AreEqual(e.Message, "Status code 404. The following errors occured [\r\n  {\r\n    \"error\": \"NoResultFound\",\r\n    \"message\": \"No result found\"\r\n  }\r\n]");
                throw;
            }
        }

    }
}
