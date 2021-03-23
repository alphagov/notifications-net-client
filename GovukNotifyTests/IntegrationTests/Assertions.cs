using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using Notify.Exceptions;
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Notify.IntegrationTests.Assertions
{
	public class NotifyAssertions
	{
		static public void AssertNotification(Notification notification)
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
			String[] allowedStatusTypes = {
				"created",
				"sending",
				"delivered",
				"permanent-failure",
				"temporary-failure",
				"technical-failure",
				"accepted"
			};
			CollectionAssert.Contains(allowedStatusTypes, notificationStatus);

			if (notificationStatus.Equals("delivered"))
			{
				Assert.IsNotNull(notification.completedAt);
			}

			AssertTemplate(notification.template);
		}

		static public void AssertReceivedTextResponse(ReceivedTextResponse receivedTextResponse)
		{
			Assert.IsNotNull(receivedTextResponse);
			Assert.IsNotNull(receivedTextResponse.id);
			Assert.IsNotNull(receivedTextResponse.content);
		}

		static public void AssertTemplate(Template template)
		{
			Assert.IsNotNull(template);
			Assert.IsNotNull(template.id);
			Assert.IsNotNull(template.uri);
			Assert.IsNotNull(template.version);
		}

		static public void AssertTemplateResponse(TemplateResponse template, String type = null)
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