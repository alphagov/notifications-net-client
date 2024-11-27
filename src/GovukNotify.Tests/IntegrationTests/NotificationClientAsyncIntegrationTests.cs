using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using NUnit.Framework;

namespace Notify.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationClientAsyncIntegrationTests
    {
        private NotificationClient client;

        private readonly String FUNCTIONAL_TESTS_API_HOST = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_API_HOST");
        private readonly String API_KEY = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_SERVICE_API_TEST_KEY");
        private readonly String FUNCTIONAL_TESTS_SERVICE_API_KEY = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_SERVICE_API_KEY");

        private readonly String TEST_NUMBER = Environment.GetEnvironmentVariable("TEST_NUMBER");
        private readonly String FUNCTIONAL_TEST_EMAIL = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL");

        private readonly String FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID");
        private readonly String FUNCTIONAL_TEST_SMS_TEMPLATE_ID = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_SMS_TEMPLATE_ID");
        private readonly String FUNCTIONAL_TEST_LETTER_TEMPLATE_ID = Environment.GetEnvironmentVariable("FUNCTIONAL_TEST_LETTER_TEMPLATE_ID");
        private readonly String FUNCTIONAL_TESTS_SERVICE_EMAIL_REPLY_TO_ID = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_SERVICE_EMAIL_REPLY_TO_ID");
        private readonly String FUNCTIONAL_TESTS_SERVICE_SMS_SENDER_ID = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_SERVICE_SMS_SENDER_ID");
        private readonly String FUNCTIONAL_TESTS_SERVICE_API_TEST_KEY = Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_SERVICE_API_TEST_KEY");

        private String smsNotificationId;
        private String emailNotificationId;
        private String letterNotificationId;

        const String TEST_TEMPLATE_SMS_BODY = "Hello ((name))\r\n\r\nFunctional Tests make our world a better place";
        const String TEST_SMS_BODY = "Hello someone\n\nFunctional Tests make our world a better place";

        const String TEST_TEMPLATE_EMAIL_BODY = "Hello ((name))\r\n\r\nFunctional test help make our world a better place";
        const String TEST_EMAIL_BODY = "Hello someone\r\n\r\nFunctional test help make our world a better place";
        const String TEST_EMAIL_SUBJECT = "Functional Tests are good";

        const String TEST_LETTER_BODY = "Hello Foo";
        const String TEST_LETTER_SUBJECT = "Main heading";

        [SetUp]
        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void SetUp()
        {
            this.client = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, API_KEY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendSmsTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            SmsNotificationResponse response =
                await this.client.SendSmsAsync(TEST_NUMBER, FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation, "sample-test-ref");
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_SMS_BODY);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetSMSNotificationWithIdReturnsNotification()
        {
            await SendSmsTestWithPersonalisation();
            Notification notification = await this.client.GetNotificationByIdAsync(this.smsNotificationId);

            for (int i = 0; i < 15; i++)
            {
                if (notification.isCostDataReady)
                {
                    break;
                }
                else
                {
                    await Task.Delay(3000);
                    notification = await this.client.GetNotificationByIdAsync(this.smsNotificationId);
                }
            }

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.smsNotificationId);

            Assert.IsNotNull(notification.body);
            Assert.AreEqual(notification.body, TEST_SMS_BODY);

            Assert.IsNotNull(notification.reference);
            Assert.AreEqual(notification.reference, "sample-test-ref");

            Assert.IsNotNull(notification.costDetails.smsRate);
            Assert.IsNotNull(notification.costDetails.billableSmsFragments);
            Assert.IsNotNull(notification.costDetails.internationalRateMultiplier);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendEmailTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response =
                await this.client.SendEmailAsync(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);
            this.emailNotificationId = response.id;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetEmailNotificationWithIdReturnsNotification()
        {
            await SendEmailTestWithPersonalisation();
            Notification notification = await this.client.GetNotificationByIdAsync(this.emailNotificationId);
            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.emailNotificationId);

            Assert.IsNotNull(notification.body);
            Assert.AreEqual(notification.body, TEST_EMAIL_BODY);
            Assert.IsNotNull(notification.subject);
            Assert.AreEqual(notification.subject, TEST_EMAIL_SUBJECT);

            Assert.IsNull(notification.costDetails.smsRate);
            Assert.IsNull(notification.costDetails.billableSmsFragments);
            Assert.IsNull(notification.costDetails.internationalRateMultiplier);
            Assert.IsNull(notification.costDetails.postage);
            Assert.IsNull(notification.costDetails.billableSheetsOfPaper);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendLetterTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "address_line_1", "Foo" },
                { "address_line_2", "Bar" },
                { "postcode", "SW1 1AA" }
            };

            LetterNotificationResponse response =
                await this.client.SendLetterAsync(FUNCTIONAL_TEST_LETTER_TEMPLATE_ID, personalisation);

            this.letterNotificationId = response.id;

            Assert.IsNotNull(response);

            Assert.AreEqual(response.content.body, TEST_LETTER_BODY);
            Assert.AreEqual(response.content.subject, TEST_LETTER_SUBJECT);

        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetLetterNotificationWithIdReturnsNotification()
        {
            await SendLetterTestWithPersonalisation();
            Notification notification = await this.client.GetNotificationByIdAsync(this.letterNotificationId);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.letterNotificationId);

            Assert.IsNotNull(notification.body);
            Assert.AreEqual(notification.body, TEST_LETTER_BODY);

            Assert.IsNotNull(notification.subject);
            Assert.AreEqual(notification.subject, TEST_LETTER_SUBJECT);

            Assert.IsNotNull(notification.costDetails.postage);
            Assert.IsNotNull(notification.costDetails.billableSheetsOfPaper);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendPrecompiledLetterTest()
        {

            string reference = System.Guid.NewGuid().ToString();
            string postage = "first";
            byte[] pdfContents;
            try
            {
                pdfContents = File.ReadAllBytes("../../../IntegrationTests/test_files/one_page_pdf.pdf");
            }
            catch (DirectoryNotFoundException)
            {
                pdfContents = File.ReadAllBytes("IntegrationTests/test_files/one_page_pdf.pdf");
            }

            LetterNotificationResponse response = await this.client.SendPrecompiledLetterAsync(reference, pdfContents, postage);

            Assert.IsNotNull(response.id);
            Assert.AreEqual(response.reference, reference);
            Assert.AreEqual(response.postage, postage);

            Notification notification = await this.client.GetNotificationByIdAsync(response.id);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, response.id);

            Assert.AreEqual(notification.reference, response.reference);
            Assert.AreEqual(notification.postage, response.postage);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendEmailWithDocumentPersonalisationTest()
        {
            byte[] pdfContents;

            try
            {
                pdfContents = File.ReadAllBytes("../../../IntegrationTests/test_files/one_page_pdf.pdf");
            }
            catch (DirectoryNotFoundException)
            {
                pdfContents = File.ReadAllBytes("IntegrationTests/test_files/one_page_pdf.pdf");
            }

            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", NotificationClient.PrepareUpload(pdfContents) }
            };

            EmailNotificationResponse response =
                await this.client.SendEmailAsync(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
            Assert.IsTrue(response.content.body.Contains("https://documents."));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendEmailWithCSVDocumentPersonalisationTestUsingEmailConfirmationAndRetentionPeriod()
        {
            byte[] pdfContents;

            try
            {
                pdfContents = File.ReadAllBytes("../../../IntegrationTests/test_files/one_page_pdf.pdf");
            }
            catch (DirectoryNotFoundException)
            {
                pdfContents = File.ReadAllBytes("IntegrationTests/test_files/one_page_pdf.pdf");
            }

            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", NotificationClient.PrepareUpload(pdfContents, "report.csv", true, "4 weeks") }
            };

            EmailNotificationResponse response =
                await this.client.SendEmailAsync(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
            Assert.IsTrue(response.content.body.Contains("https://documents."));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetAllNotifications()
        {
            NotificationList notificationsResponse = await this.client.GetNotificationsAsync();
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);

            List<Notification> notifications = notificationsResponse.notifications;

            foreach (Notification notification in notifications)
            {
                NotifyAssertions.AssertNotification(notification);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetReceivedTexts()
        {
            NotificationClient client_inbound = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, FUNCTIONAL_TESTS_SERVICE_API_TEST_KEY);
            ReceivedTextListResponse receivedTextListResponse = await client_inbound.GetReceivedTextsAsync();
            Assert.IsNotNull(receivedTextListResponse);
            Assert.IsNotNull(receivedTextListResponse.receivedTexts);
            Assert.AreNotEqual(receivedTextListResponse.receivedTexts.Count, 0);

            List<ReceivedTextResponse> receivedTexts = receivedTextListResponse.receivedTexts;

            foreach (ReceivedTextResponse receivedText in receivedTexts)
            {
                NotifyAssertions.AssertReceivedTextResponse(receivedText);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            var ex = Assert.ThrowsAsync<NotifyClientException>(async () =>
                await this.client.GetNotificationByIdAsync("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            var ex = Assert.ThrowsAsync<NotifyClientException>(async () =>
                await this.client.GetTemplateByIdAsync("invalid_id")
            );
            Assert.That(ex.Message, Does.Contain("id is not a valid UUID"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            var ex = Assert.ThrowsAsync<NotifyClientException>(async () =>
                await this.client.GetTemplateByIdAsync("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetAllTemplates()
        {
            TemplateList templateList = await this.client.GetAllTemplatesAsync();
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetAllSMSTemplates()
        {
            const String type = "sms";
            TemplateList templateList = await this.client.GetAllTemplatesAsync(type);
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template, type);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetAllEmailTemplates()
        {
            const String type = "email";
            TemplateList templateList = await this.client.GetAllTemplatesAsync(type);
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template, type);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void GetAllInvalidTemplatesRaisesClientException()
        {
            const String type = "invalid";

            var ex = Assert.ThrowsAsync<NotifyClientException>(async () => await this.client.GetAllTemplatesAsync(type));
            Assert.That(ex.Message, Does.Contain("type invalid is not one of [sms, email, letter, broadcast]"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetSMSTemplateWithId()
        {
            TemplateResponse template = await this.client.GetTemplateByIdAsync(FUNCTIONAL_TEST_SMS_TEMPLATE_ID);
            Assert.AreEqual(template.id, FUNCTIONAL_TEST_SMS_TEMPLATE_ID);
            Assert.AreEqual(template.body, TEST_TEMPLATE_SMS_BODY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GetEmailTemplateWithId()
        {
            TemplateResponse template = await this.client.GetTemplateByIdAsync(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID);
            Assert.AreEqual(template.id, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID);
            Assert.AreEqual(template.body, TEST_TEMPLATE_EMAIL_BODY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GenerateSMSPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            TemplatePreviewResponse response =
                await this.client.GenerateTemplatePreviewAsync(FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, TEST_SMS_BODY);
            Assert.AreEqual(response.subject, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task GenerateEmailPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            TemplatePreviewResponse response =
                await this.client.GenerateTemplatePreviewAsync(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.subject, TEST_EMAIL_SUBJECT);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void GenerateEmailPreviewWithMissingPersonalisationRaisesClientException()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "invalid", "personalisation" }
            };

            var ex = Assert.ThrowsAsync<NotifyClientException>(async () =>
                await this.client.GenerateTemplatePreviewAsync(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation)
            );
            Assert.That(ex.Message, Does.Contain("Missing personalisation: name"));
        }


        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public void SendEmailTestEmailReplyToNotPresent()
        {
            String fakeReplayToId = System.Guid.NewGuid().ToString();
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            var ex = Assert.ThrowsAsync<NotifyClientException>(async () => await this.client.SendEmailAsync(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation, emailReplyToId: fakeReplayToId));
            Assert.That(ex.Message, Does.Contain("email_reply_to_id " + fakeReplayToId));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendEmailTestAllArguments()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            EmailNotificationResponse response = await this.client.SendEmailAsync(
                FUNCTIONAL_TEST_EMAIL,
                FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID,
                personalisation,
                clientReference: "TestReference",
                emailReplyToId: FUNCTIONAL_TESTS_SERVICE_EMAIL_REPLY_TO_ID,
                oneClickUnsubscribeURL: "https://www.example.com/unsubscribe"
            );
            this.emailNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_EMAIL_BODY);
            Assert.AreEqual(response.content.subject, TEST_EMAIL_SUBJECT);
            Assert.AreEqual(response.reference, "TestReference");
            Assert.AreEqual(response.content.oneClickUnsubscribeURL, "https://www.example.com/unsubscribe");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClientAsync")]
        public async Task SendSmsTestWithPersonalisationAndSmsSenderId()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "name", "someone" }
            };

            NotificationClient client_sending = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, FUNCTIONAL_TESTS_SERVICE_API_KEY);

            SmsNotificationResponse response =
                await client_sending.SendSmsAsync(TEST_NUMBER, FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation, "sample-test-ref", FUNCTIONAL_TESTS_SERVICE_SMS_SENDER_ID);
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.content.body, TEST_SMS_BODY);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }
    }
}
