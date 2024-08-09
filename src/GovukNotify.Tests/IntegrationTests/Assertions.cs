using Notify.Models;
using Notify.Models.Responses;
using NUnit.Framework.Legacy;
using System;

namespace Notify.Tests.IntegrationTests
{
	public class NotifyAssertions
	{
		public static void AssertNotification(Notification notification)
		{
            ClassicAssert.IsNotNull(notification.type);
			String notificationType = notification.type;
			String[] allowedNotificationTypes = { "email", "sms", "letter" };
			CollectionAssert.Contains(allowedNotificationTypes, notificationType);
			if (notificationType.Equals("sms"))
			{
                ClassicAssert.IsNotNull(notification.phoneNumber);
			}
			else if (notificationType.Equals("email"))
			{
                ClassicAssert.IsNotNull(notification.emailAddress);
                ClassicAssert.IsNotNull(notification.subject);
			}
			else if (notificationType.Equals("letter"))
			{
                ClassicAssert.IsNotNull(notification.subject);
			}

            ClassicAssert.IsNotNull(notification.body);
            ClassicAssert.IsNotNull(notification.createdAt);

            ClassicAssert.IsNotNull(notification.status);
			String notificationStatus = notification.status;
			String[] allowedStatusTypes = {
				"created",
				"sending",
				"delivered",
				"permanent-failure",
				"temporary-failure",
				"technical-failure",
				"accepted",
				"received",
				"pending-virus-check",
				"virus-scan-failed",
				"validation-failed"
			};
			CollectionAssert.Contains(allowedStatusTypes, notificationStatus);

			if (notificationStatus.Equals("delivered"))
			{
                ClassicAssert.IsNotNull(notification.completedAt);
			}

			AssertTemplate(notification.template);
		}

		public static void AssertReceivedTextResponse(ReceivedTextResponse receivedTextResponse)
		{
            ClassicAssert.IsNotNull(receivedTextResponse);
            ClassicAssert.IsNotNull(receivedTextResponse.id);
            ClassicAssert.IsNotNull(receivedTextResponse.content);
		}

		public static void AssertTemplate(Template template)
		{
            ClassicAssert.IsNotNull(template);
            ClassicAssert.IsNotNull(template.id);
            ClassicAssert.IsNotNull(template.uri);
            ClassicAssert.IsNotNull(template.version);
		}

		public static void AssertTemplateResponse(TemplateResponse template, String type = null)
		{
            ClassicAssert.IsNotNull(template);
            ClassicAssert.IsNotNull(template.id);
            ClassicAssert.IsNotNull(template.name);
            ClassicAssert.IsNotNull(template.version);
            ClassicAssert.IsNotNull(template.type);
			if (template.type.Equals("email") || (!string.IsNullOrEmpty(type) && type.Equals("email")))
                ClassicAssert.IsNotNull(template.subject);
            ClassicAssert.IsNotNull(template.created_at);
            ClassicAssert.IsNotNull(template.created_by);
            ClassicAssert.IsNotNull(template.body);
		}
    }
}
