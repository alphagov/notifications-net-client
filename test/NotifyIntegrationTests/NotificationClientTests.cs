using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using Notify.Exceptions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NotifyIntegrationTests
{
    [TestClass]
    public class NotificationClientTests
    {
        private NotificationClient client;

        private String BASE_URL = Environment.GetEnvironmentVariable("NOTIFY_API_URL");
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
            this.client = new NotificationClient(BASE_URL, API_KEY);
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
                this.client.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_PERSONALISATION_TEMPLATE_ID, personalisation, "sample-test-ref");
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, "Hello someone\n\nFunctional Tests make our world a better place");

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetSMSNotificationWithIdReturnsNotification()
        {
            SendSmsTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.smsNotificationId);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.smsNotificationId);

            Assert.IsNotNull(notification.reference);
            Assert.AreEqual(notification.reference, "sample-test-ref");

            AssertNotification(notification);
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
            Assert.AreEqual(response.content.body, "Hello someone\n\nFunctional test help make our world a better place");
            Assert.AreEqual(response.content.subject, "Functional Tests are good");
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetEmailNotificationWithIdReturnsNotification()
        {
            SendEmailTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.emailNotificationId);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.emailNotificationId);

            AssertNotification(notification);
        }

        [TestMethod()]
        [TestCategory("Integration")]
        public void GetAllNotifications()
        {
            NotificationList notificationsResponse = this.client.GetNotifications();
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);

            List<Notification> notifications = notificationsResponse.notifications;

            foreach (Notification notification in notifications)
            {
                AssertNotification(notification);
            }

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
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Status code 404. The following errors occured [\r\n  {\r\n    \"error\": \"NoResultFound\",\r\n    \"message\": \"No result found\"\r\n  }\r\n]");
                throw;
            }

        }

        public void AssertNotification(Notification notification)
        {
            Assert.IsNotNull(notification.type);
            String notificationType = notification.type;
            String[] allowedNotificationTypes = { "email", "sms" };
            CollectionAssert.Contains(allowedNotificationTypes, notificationType);
            if (notificationType.Equals("sms"))
            {
                Assert.IsNotNull(notification.phoneNumber);
            }
            else if (notificationType.Equals("email"))
            {
                Assert.IsNotNull(notification.emailAddress);
            }

            Assert.IsNotNull(notification.createdAt);

            Assert.IsNotNull(notification.status);
            String notificationStatus = notification.status;
            String[] allowedStatusTypes = { "created", "sending", "delivered", "permanent-failure", "temporary-failure", "technical-failure" };
            CollectionAssert.Contains(allowedStatusTypes, notificationStatus);

            if (notificationStatus.Equals("delivered"))
            {
                Assert.IsNotNull(notification.completedAt);
            }

            AssertTemplate(notification.template);
        }

        public void AssertTemplate(Template template)
        {
            Assert.IsNotNull(template);
            Assert.IsNotNull(template.id);
            Assert.IsNotNull(template.uri);
            Assert.IsNotNull(template.version);
        }

    }
}
