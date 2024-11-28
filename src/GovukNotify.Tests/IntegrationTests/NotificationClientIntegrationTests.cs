using System;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using NUnit.Framework;

namespace Notify.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationClientIntegrationTests
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

        [SetUp]
        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SetUp()
        {
            this.client = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, API_KEY);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            SmsNotificationResponse response =
                this.client.SendSms(TEST_NUMBER, FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation, "sample-test-ref");
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.content.body);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetSMSNotificationWithIdReturnsNotification()
        {
            SendSmsTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.smsNotificationId);

            for (int i = 0; i < 24; i++)
            {
                if (notification.isCostDataReady)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(5000);
                    notification = this.client.GetNotificationById(this.smsNotificationId);
                }
            }

            Assert.IsTrue(notification.isCostDataReady);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.smsNotificationId);

            Assert.IsNotNull(notification.body);
            StringAssert.Contains("someone", notification.body);

            Assert.IsNotNull(notification.reference);
            Assert.AreEqual(notification.reference, "sample-test-ref");

            Assert.IsNotNull(notification.costDetails.smsRate);
            Assert.IsNotNull(notification.costDetails.billableSmsFragments);
            Assert.IsNotNull(notification.costDetails.internationalRateMultiplier);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);
            this.emailNotificationId = response.id;

            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.content.body);
            Assert.IsNotNull(response.content.subject);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetEmailNotificationWithIdReturnsNotification()
        {
            SendEmailTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.emailNotificationId);
            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.emailNotificationId);

            Assert.IsNotNull(notification.body);
            StringAssert.Contains("someone", notification.body);
            Assert.IsNotNull(notification.subject);

            Assert.IsNull(notification.costDetails.smsRate);
            Assert.IsNull(notification.costDetails.billableSmsFragments);
            Assert.IsNull(notification.costDetails.internationalRateMultiplier);
            Assert.IsNull(notification.costDetails.postage);
            Assert.IsNull(notification.costDetails.billableSheetsOfPaper);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendLetterTestWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "address_line_1", "Foo" },
                { "address_line_2", "Bar" },
                { "postcode", "SW1 1AA" },
                { "build_id", "some build id" }
            };

            LetterNotificationResponse response =
                this.client.SendLetter(FUNCTIONAL_TEST_LETTER_TEMPLATE_ID, personalisation);

            this.letterNotificationId = response.id;

            Assert.IsNotNull(response);

            StringAssert.Contains("some build id", response.content.body);
            Assert.IsNotNull(response.content.subject);

        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetLetterNotificationWithIdReturnsNotification()
        {
            SendLetterTestWithPersonalisation();
            Notification notification = this.client.GetNotificationById(this.letterNotificationId);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, this.letterNotificationId);

            Assert.IsNotNull(notification.body);
            StringAssert.Contains("some build id", notification.body);

            Assert.IsNotNull(notification.subject);

            Assert.IsNotNull(notification.costDetails.postage);
            Assert.IsNotNull(notification.costDetails.billableSheetsOfPaper);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendPrecompiledLetterTest()
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

            LetterNotificationResponse response = this.client.SendPrecompiledLetter(reference, pdfContents, postage);

            Assert.IsNotNull(response.id);
            Assert.AreEqual(response.reference, reference);
            Assert.AreEqual(response.postage, postage);

            Notification notification = this.client.GetNotificationById(response.id);

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.id);
            Assert.AreEqual(notification.id, response.id);

            Assert.AreEqual(notification.reference, response.reference);
            Assert.AreEqual(notification.postage, response.postage);

            NotifyAssertions.AssertNotification(notification);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailWithDocumentPersonalisationTest()
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
                { "build_id", NotificationClient.PrepareUpload(pdfContents) }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.IsNotNull(response.content.subject);
            Assert.IsTrue(response.content.body.Contains("https://documents."));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailWithCSVDocumentPersonalisationTestUsingEmailConfirmationAndRetentionPeriod()
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
                { "build_id", NotificationClient.PrepareUpload(pdfContents, "report.csv", true, "4 weeks") }
            };

            EmailNotificationResponse response =
                this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response.id);
            Assert.IsNotNull(response.template.id);
            Assert.IsNotNull(response.template.uri);
            Assert.IsNotNull(response.template.version);
            Assert.IsNotNull(response.content.subject);
            Assert.IsTrue(response.content.body.Contains("https://documents."));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllNotifications()
        {
            NotificationList notificationsResponse = this.client.GetNotifications();
            Assert.IsNotNull(notificationsResponse);
            Assert.IsNotNull(notificationsResponse.notifications);

            List<Notification> notifications = notificationsResponse.notifications;

            foreach (Notification notification in notifications)
            {
                NotifyAssertions.AssertNotification(notification);
            }

        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetReceivedTexts()
        {
            INotificationClient client_inbound = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, FUNCTIONAL_TESTS_SERVICE_API_TEST_KEY);
            ReceivedTextListResponse receivedTextListResponse = client_inbound.GetReceivedTexts();
            Assert.IsNotNull(receivedTextListResponse);
            Assert.IsNotNull(receivedTextListResponse.receivedTexts);
            Assert.AreNotEqual(receivedTextListResponse.receivedTexts.Count, 0);

            List<ReceivedTextResponse> receivedTexts = receivedTextListResponse.receivedTexts;

            foreach (ReceivedTextResponse receivedText in receivedTexts)
            {
                NotifyAssertions.AssertReceivedTextResponse(receivedText);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetNotificationWithInvalidIdRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetNotificationById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithInvalidIdRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetTemplateById("invalid_id")
            );
            Assert.That(ex.Message, Does.Contain("id is not a valid UUID"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetTemplateWithIdWithoutResultRaisesClientException()
        {
            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GetTemplateById("fa5f0a6e-5293-49f1-b99f-3fade784382f")
            );
            Assert.That(ex.Message, Does.Contain("No result found"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllTemplates()
        {
            TemplateList templateList = this.client.GetAllTemplates();
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllSMSTemplates()
        {
            const String type = "sms";
            TemplateList templateList = this.client.GetAllTemplates(type);
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template, type);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllEmailTemplates()
        {
            const String type = "email";
            TemplateList templateList = this.client.GetAllTemplates(type);
            Assert.IsNotNull(templateList);
            Assert.AreNotEqual(templateList.templates.Count, 0);

            foreach (TemplateResponse template in templateList.templates)
            {
                NotifyAssertions.AssertTemplateResponse(template, type);
            }
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetAllInvalidTemplatesRaisesClientException()
        {
            const String type = "invalid";

            var ex = Assert.Throws<NotifyClientException>(() => this.client.GetAllTemplates(type));
            Assert.That(ex.Message, Does.Contain("type invalid is not one of [sms, email, letter, broadcast]"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetSMSTemplateWithId()
        {
            TemplateResponse template = this.client.GetTemplateById(FUNCTIONAL_TEST_SMS_TEMPLATE_ID);
            Assert.AreEqual(template.id, FUNCTIONAL_TEST_SMS_TEMPLATE_ID);
            StringAssert.Contains("((build_id))", template.body);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetEmailTemplateWithId()
        {
            TemplateResponse template = this.client.GetTemplateById(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID);
            Assert.AreEqual(template.id, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID);
            StringAssert.Contains("((build_id))", template.body);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetLetterTemplateWithIdCheckContactBlock()
        {
            TemplateResponse template = this.client.GetTemplateById(FUNCTIONAL_TEST_LETTER_TEMPLATE_ID);
            Assert.AreEqual(template.id, FUNCTIONAL_TEST_LETTER_TEMPLATE_ID);
            Assert.IsNotNull(template.letter_contact_block);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateSMSPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            TemplatePreviewResponse response =
                this.client.GenerateTemplatePreview(FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.body);
            Assert.AreEqual(response.subject, null);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateEmailPreviewWithPersonalisation()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            TemplatePreviewResponse response =
                this.client.GenerateTemplatePreview(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation);

            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.body);
            Assert.IsNotNull(response.subject);
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GenerateEmailPreviewWithMissingPersonalisationRaisesClientException()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "invalid", "personalisation" }
            };

            var ex = Assert.Throws<NotifyClientException>(() =>
                this.client.GenerateTemplatePreview(FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation)
            );
            Assert.That(ex.Message, Does.Contain("Missing personalisation: build_id"));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestEmailReplyToNotPresent()
        {
            String fakeReplayToId = System.Guid.NewGuid().ToString();
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            var ex = Assert.Throws<NotifyClientException>(() => this.client.SendEmail(FUNCTIONAL_TEST_EMAIL, FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID, personalisation, emailReplyToId: fakeReplayToId));
            Assert.That(ex.Message, Does.Contain("email_reply_to_id " + fakeReplayToId));
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendEmailTestAllArguments()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            EmailNotificationResponse response = this.client.SendEmail(
                FUNCTIONAL_TEST_EMAIL,
                FUNCTIONAL_TEST_EMAIL_TEMPLATE_ID,
                personalisation,
                clientReference: "TestReference",
                emailReplyToId: FUNCTIONAL_TESTS_SERVICE_EMAIL_REPLY_TO_ID,
                oneClickUnsubscribeURL: "https://www.example.com/unsubscribe"
            );
            this.emailNotificationId = response.id;
            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.content.body);
            Assert.IsNotNull(response.content.subject);
            Assert.AreEqual(response.reference, "TestReference");
            Assert.AreEqual(response.content.oneClickUnsubscribeURL, "https://www.example.com/unsubscribe");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void SendSmsTestWithPersonalisationAndSmsSenderId()
        {
            Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
            {
                { "build_id", "someone" }
            };

            NotificationClient client_sending = new NotificationClient(FUNCTIONAL_TESTS_API_HOST, FUNCTIONAL_TESTS_SERVICE_API_KEY);

            SmsNotificationResponse response =
                client_sending.SendSms(TEST_NUMBER, FUNCTIONAL_TEST_SMS_TEMPLATE_ID, personalisation, "sample-test-ref", FUNCTIONAL_TESTS_SERVICE_SMS_SENDER_ID);
            this.smsNotificationId = response.id;
            Assert.IsNotNull(response);
            StringAssert.Contains("someone", response.content.body);

            Assert.IsNotNull(response.reference);
            Assert.AreEqual(response.reference, "sample-test-ref");
        }

        [Test, Category("Integration"), Category("Integration/NotificationClient")]
        public void GetPdfForLetter()
        {
            byte[] pdfData = null;
            for (int i = 0; i < 24; i++)
            {
                try
                {
                    pdfData = this.client.GetPdfForLetter(this.letterNotificationId);
                    break;
                }
                catch (NotifyClientException e)
                {
                    if (!e.Message.Contains("PDFNotReadyError"))
                    {
                        throw e;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            Assert.IsNotNull(pdfData);
            var expectedResponse = Encoding.UTF8.GetBytes("%PDF-");
            Assert.AreEqual(pdfData.Take(expectedResponse.Length).ToArray(), expectedResponse);
        }
    }
}
