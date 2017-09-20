using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using Notify.Exceptions;
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Notify.IntegrationTests
{
	[TestFixture]
	public class NotificationIntegrationClientTests
	{
		private NotificationClient client;

		private String NOTIFY_API_URL = Environment.GetEnvironmentVariable("NOTIFY_API_URL");
		private String API_KEY = Environment.GetEnvironmentVariable("API_KEY");
		private String FUNCTIONAL_TEST_NUMBER = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_NUMBER");
		private String FUNCTIONAL_TEST_EMAIL = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

		private String EMAIL_TEMPLATE_ID = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_ID");
		private String SMS_TEMPLATE_ID = Environment.GetEnvironmentVariable("SMS_TEMPLATE_ID");

		private String smsNotificationId;
		private String emailNotificationId;

		const String TEST_SUBJECT = "Functional Tests are good";
		const String TEST_EMAIL_BODY = "Hello someone\n\nFunctional test help make our world a better place";
		const String TEST_SMS_BODY = "Hello someone\n\nFunctional Tests make our world a better place";
		const String TEST_TEMPLATE_SMS_BODY = "Hello ((name))\n\nFunctional Tests make our world a better place";
		const String TEST_TEMPLATE_EMAIL_BODY = "Hello ((name))\n\nFunctional test help make our world a better place";

		[SetUp]
		[Test, Category("Integration")]
		public void SetUp()
		{
			this.client = new NotificationClient(NOTIFY_API_URL, API_KEY);
		}

		[Test, Category("Integration")]
		public void SendSmsTestWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			SmsNotificationResponse response =
				this.client.SendSms(FUNCTIONAL_TEST_NUMBER, SMS_TEMPLATE_ID, personalisation, "sample-test-ref");
			this.smsNotificationId = response.id;
			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_SMS_BODY);

			Assert.IsNotNull(response.reference);
			Assert.AreEqual(response.reference, "sample-test-ref");
		}

		[Test, Category("Integration")]
		public void GetSMSNotificationWithIdReturnsNotification()
		{
			SendSmsTestWithPersonalisation();
			Notification notification = this.client.GetNotificationById(this.smsNotificationId);

			Assert.IsNotNull(notification);
			Assert.IsNotNull(notification.id);
			Assert.AreEqual(notification.id, this.smsNotificationId);

			Assert.IsNotNull(notification.body);
			Assert.AreEqual(notification.body, TEST_SMS_BODY);

			Assert.IsNotNull(notification.reference);
			Assert.AreEqual(notification.reference, "sample-test-ref");

			AssertNotification(notification);
		}

		[Test, Category("Integration")]
		public void SendEmailTestWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			EmailNotificationResponse response =
				this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, EMAIL_TEMPLATE_ID, personalisation);
			this.emailNotificationId = response.id;

			Assert.IsNotNull(response);
			Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.content.subject, TEST_SUBJECT);
		}

		[Test, Category("Integration")]
		public void GetEmailNotificationWithIdReturnsNotification()
		{
			SendEmailTestWithPersonalisation();
			Notification notification = this.client.GetNotificationById(this.emailNotificationId);

			Assert.IsNotNull(notification);
			Assert.IsNotNull(notification.id);
			Assert.AreEqual(notification.id, this.emailNotificationId);

			Assert.IsNotNull(notification.body);
			Assert.AreEqual(notification.body, TEST_EMAIL_BODY);
			Assert.IsNotNull(notification.subject);
			Assert.AreEqual(notification.subject, "Functional Tests are good");

			AssertNotification(notification);
		}

		[Test, Category("Integration")]
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

		[Test, Category("Integration")]
		public void GetNotificationWithInvalidIdRaisesClientException()
		{
			var ex = Assert.Throws<NotifyClientException>(() =>
			                                     this.client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f"));
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

		[Test, Category("Integration")]
		public void GetTemplateWithInvalidIdRaisesClientException()
		{
			var ex = Assert.Throws<NotifyClientException>(() =>
		                                         this.client.GetTemplateById("invalid_id"));
            Assert.That(ex.Message, Does.Contain("id is not a valid UUID"));
        }

        [Test, Category("Integration")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
        	var ex = Assert.Throws<NotifyClientException>(() =>
        										 this.client.GetTemplateById("fa5f0a6e-5293-49f1-b99f-3fade784382f"));
        	Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration")]
		public void GetAllTemplates()
		{
			TemplateList templateList = this.client.GetAllTemplates();
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				AssertTemplateResponse(template);
			}
		}

		[Test, Category("Integration")]
		public void GetAllSMSTemplates()
		{
			const String type = "sms";
			TemplateList templateList = this.client.GetAllTemplates(type);
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				AssertTemplateResponse(template, type);
			}
		}

		[Test, Category("Integration")]
		public void GetAllEmailTemplates()
		{
			const String type = "email";
			TemplateList templateList = this.client.GetAllTemplates(type);
			Assert.IsNotNull(templateList);
			Assert.IsTrue(templateList.templates.Count > 0);

			foreach (TemplateResponse template in templateList.templates)
			{
				AssertTemplateResponse(template, type);
			}
		}

		[Test, Category("Integration")]
		public void GetAllInvalidTemplatesRaisesClientException()
		{
			const String type = "invalid";

            var ex = Assert.Throws<NotifyClientException>(() => this.client.GetAllTemplates(type));
            Assert.That(ex.Message, Does.Contain("type invalid is not one of [sms, email, letter]"));
		}

		[Test, Category("Integration")]
		public void GetSMSTemplateWithId()
		{
			TemplateResponse template = this.client.GetTemplateById(SMS_TEMPLATE_ID);
			Assert.AreEqual(template.id, SMS_TEMPLATE_ID);
			Assert.AreEqual(template.body, TEST_TEMPLATE_SMS_BODY);
		}

		[Test, Category("Integration")]
		public void GetEmailTemplateWithId()
		{
			TemplateResponse template = this.client.GetTemplateById(EMAIL_TEMPLATE_ID);
			Assert.AreEqual(template.id, EMAIL_TEMPLATE_ID);
			Assert.AreEqual(template.body, TEST_TEMPLATE_EMAIL_BODY);
		}

		[Test, Category("Integration")]
		public void GenerateSMSPreviewWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			TemplatePreviewResponse response =
				this.client.GenerateTemplatePreview(SMS_TEMPLATE_ID, personalisation);

			Assert.IsNotNull(response);
			Assert.AreEqual(response.body, TEST_SMS_BODY);
			Assert.AreEqual(response.subject, null);
		}

		[Test, Category("Integration")]
		public void GenerateEmailPreviewWithPersonalisation()
		{
			Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
			{
				{ "name", "someone" }
			};

			TemplatePreviewResponse response =
				this.client.GenerateTemplatePreview(EMAIL_TEMPLATE_ID, personalisation);

			Assert.IsNotNull(response);
			Assert.AreEqual(response.body, TEST_EMAIL_BODY);
			Assert.AreEqual(response.subject, TEST_SUBJECT);
		}

        [Test, Category("Integration")]
        public void GenerateEmailPreviewWithMissingPersonalisationRaisesClientException()
        {
        	Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
        			{
        				{ "invalid", "personalisation" }
        			};

            var ex = Assert.Throws<NotifyClientException>(() =>
                                                          this.client.GenerateTemplatePreview(EMAIL_TEMPLATE_ID, personalisation));
            Assert.That(ex.Message, Does.Contain("Missing personalisation: name"));
        }

        public void AssertNotification(Notification notification)
		{
			Assert.IsNotNull(notification.type);
			String notificationType = notification.type;
			String[] allowedNotificationTypes = { "email", "sms", "letter" };
			CollectionAssert.Contains(allowedNotificationTypes, notificationType);
            if (notificationType.Equals("sms"))
            {
                Assert.IsNotNull(notification.phoneNumber);
            }
            else if (notificationType.Equals("email"))
            {
                Assert.IsNotNull(notification.emailAddress);
                Assert.IsNotNull(notification.subject);
            }
            else if (notificationType.Equals("letter"))
            {
                Assert.IsNotNull(notification.subject);
            }

			Assert.IsNotNull(notification.body);
			Assert.IsNotNull(notification.createdAt);

			Assert.IsNotNull(notification.status);
			String notificationStatus = notification.status;
			String[] allowedStatusTypes = { "created", "sending", "delivered", "permanent-failure", "temporary-failure", "technical-failure", "accepted" };
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

		public void AssertTemplateResponse(TemplateResponse template, String type = null)
		{
			Assert.IsNotNull(template);
			Assert.IsNotNull(template.id);
			Assert.IsNotNull(template.name);
			Assert.IsNotNull(template.version);
			Assert.IsNotNull(template.type);
			if (template.type.Equals("email") || (!string.IsNullOrEmpty(type) && type.Equals("email")))
				Assert.IsNotNull(template.subject);
			Assert.IsNotNull(template.created_at);
			Assert.IsNotNull(template.created_by);
			Assert.IsNotNull(template.body);
		}
	}
}
