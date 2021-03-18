using System;

namespace Notify.Tests.UnitTests
{
    public static class Constants
    {
        public static String fakeApiKey { get {
                return @"FAKEKEY-fd29e561-24b6-4f32-be5c-e642a1d68641-57bdfd56-ac07-409b-8307-71419d85bb9c";
            }
        }

        public static String fakePhoneNumber { get { return "07766565767"; } }
        public static String fakeEmail { get { return "test@mail.com"; } }

        public static String fakeNotificationId { get { return "902e6534-bc4a-4c87-8c3e-9f4144ca36fd"; } }
        public static String fakeNotificationReference { get { return "some-client-ref"; } }
        public static String fakeTemplateId { get { return "913e9fa6-9cbb-44ad-8f58-38487dccfd82"; } }
        public static String fakeReplyToId { get { return "78ded4ad-e915-4a89-a314-2940ed141d40"; } }
        public static String fakeSMSSenderId { get { return "88ded4ad-e915-4a89-a314-2940ed141d41"; } }
        public static String fakeNotificationJson { get {
                return @"{
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""created_by_name"": ""A. Sender""
                        }";
            }
        }

        public static String fakeNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""created_by_name"": null
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-23T11:21:13.133522Z"",
                            ""email_address"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-23T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""email"",
                            ""created_by_name"": null
                        }
                    ]
                }";
            }
        }
        public static String fakeSmsNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""created_by_name"": null
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-24T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-24T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""created_by_name"": null
                        }
                    ]
                }";
            }
        }

        public static String fakeEmailNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""email_address"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""email"",
                            ""thing"": null
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-24T11:21:13.133522Z"",
                            ""email_address"": ""someone2@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-24T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""email"",
                            ""created_by_name"": null
                        }
                    ]
                }";
            }
        }
        public static String fakeLetterNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": ""Mr Someone"",
                            ""line_2"": ""1 Test Street"",
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": ""SW1 1AA"",
                            ""postage"": ""second"",
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""letter"",
                            ""created_by_name"": null
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-24T11:21:13.133522Z"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": ""Mrs Somebody"",
                            ""line_2"": ""2 Test Street"",
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": ""SW1 1AA"",
                            ""postage"": ""first"",
                            ""reference"": null,
                            ""sent_at"": ""2016-11-24T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""letter"",
                            ""created_by_name"": null
                        }
                    ]
                }";
            }
        }
        public static String fakeTemplateResponseJson { get {
                return @"{
                            ""updated_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""body"": ""test body"",
                            ""subject"": null,
                            ""type"": ""sms"",
                            ""version"": 2
                        }";
            }
        }

        public static String fakeTemplateListResponseJson { get {
                return @"{ ""templates"": [
						{
                            ""updated_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""body"": ""test body"",
                            ""subject"": null,
                            ""type"": ""sms"",
                            ""version"": 2
                        },
                        {
                            ""updated_at"": ""2016-12-23T11:21:13.133522Z"",
                            ""created_at"": ""2016-12-22T11:21:13.133522Z"",
                            ""created_by"": ""someoneelse@example.com"",
                            ""id"": ""902e4312-bc4a-4c86-8c3e-9f4144ca36fd"",
                            ""body"": ""test body 2"",
                            ""subject"": ""test subject 1"",
                            ""type"": ""email"",
                            ""version"": 3
                        }
					]
				}";
            }
        }

        public static String fakeReceivedTextListResponseJson { get {
                return @"{ ""received_text_messages"": [
                        {
                            ""user_number"": ""447700900111"",
                            ""created_at"": ""2017-11-02T15:07:57.197546Z"",
                            ""service_id"": ""a5149c32-f03b-4711-af49-ad6993797d45"",
                            ""id"": ""342786aa-23ce-4695-9aad-7f79e68ee29a"",
                            ""notify_number"": ""testing"",
                            ""content"": ""Hello""
                        },
                        {
                            ""user_number"": ""447700900111"",
                            ""created_at"": ""2017-11-02T15:07:57.197546Z"",
                            ""service_id"": ""a5149c32-f03b-4711-af49-ad6993797d45"",
                            ""id"": ""342786aa-23ce-4695-9aad-7f79e68ee29a"",
                            ""notify_number"": ""testing"",
                            ""content"": ""Hello again""
                        }
					]
				}";
            }
        }

        public static String fakeTemplateEmptyListResponseJson
        {
        	get
        	{
        		return @"{ ""templates"": [] }";
        	}
        }

        public static String fakeTemplateSmsListResponseJson { get {
                return @"{ ""templates"": [{
                            ""updated_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""body"": ""test body"",
                            ""subject"": null,
                            ""type"": ""sms"",
                            ""version"": 2
                        }]}";
            }
        }

        public static String fakeTemplateEmailListResponseJson { get {
                return @"{ ""templates"": [{
                            ""updated_at"": ""2016-12-23T11:21:13.133522Z"",
                            ""created_at"": ""2016-12-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@email.com"",
                            ""id"": ""902e4312-bc4a-4c86-8c3e-9f4144ca36fd"",
                            ""body"": ""test body 2"",
                            ""subject"": ""test subject 2"",
                            ""type"": ""email"",
                            ""version"": 3
                        }]}";
            }
        }

        public static String fakeTemplatePreviewResponseJson { get {
                return @"{
                            ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                            ""type"": ""sms"",
                            ""version"": 2,
                            ""body"": ""test body"",
                            ""subject"": null
                         }";
            }
        }

        public static String fakeSmsNotificationResponseJson { get {
                return @"{
                            ""content"": {
                                ""body"": ""test"",
                                ""from_number"": null },
                            ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                            ""reference"": null,
                            ""template"": {
                                ""id"": ""be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""uri"": ""http://someurl/v2/templates/be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""version"": 1 },
                            ""uri"": ""http://some_url//v2/notifications/d683f7f9-df04-4b9c-8019-15092c4674fd""
                         }";
            }
        }

        public static String fakeSmsNotificationWithSMSSenderIdResponseJson { get {
                return @"{
                            ""content"": {
                                ""body"": ""test"",
                                ""from_number"": ""GOV.UK"" },
                            ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                            ""reference"":  null,
                            ""template"": {
                                ""id"": ""be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""uri"": ""http://someurl/v2/templates/be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""version"": 1 },
                            ""uri"": ""http://some_url//v2/notifications/d683f7f9-df04-4b9c-8019-15092c4674fd""
                         }";
            }
        }

        public static String fakeEmailNotificationResponseJson { get {
                return @"{
                            ""content"": {
                                ""body"": ""Hello someone\n\nFake"",
                                ""from_email"": ""someone@mail.com"",
                                ""subject"": ""Test""
                            },
                            ""id"": ""731b9c83-563f-4b59-afc5-87e9ca717833"",
                            ""reference"":  ""some-client-ref"",
                            ""template"": {
                                ""id"": ""f0bb62f7-5ddb-4bf8-aac7-7ey6aefd1524"",
                                ""uri"": ""https://someurl/v2/templates/c0bs62f7-4ddb-6bf8-cac7-c1e6aefd1524"",
                                ""version"": 5
                            },
                            ""uri"": ""https://someurl//v2/notifications/321b9c43-563f-4c59-sac5-87e9ca325833""
                        }";
            }
        }

		public static String fakeLetterNotificationResponseJson
		{
			get
			{
				return @"{
                            ""content"": {
                                ""body"": ""Hello someone\n\nFake"",
                                ""subject"": ""Test""
                            },
                            ""id"": ""731b9c83-563f-4b59-afc5-87e9ca717833"",
                            ""reference"":  ""some-client-ref"",
                            ""template"": {
                                ""id"": ""f0bb62f7-5ddb-4bf8-aac7-7ey6aefd1524"",
                                ""uri"": ""https://someurl/v2/templates/c0bs62f7-4ddb-6bf8-cac7-c1e6aefd1524"",
                                ""version"": 5
                            },
                            ""uri"": ""https://someurl//v2/notifications/321b9c43-563f-4c59-sac5-87e9ca325833""
                        }";
			}
		}

        public static String fakePrecompiledLetterNotificationResponseJson { get {
                return @"{
                            ""id"": ""731b9c83-563f-4b59-afc5-87e9ca717833"",
                            ""reference"":  ""some-client-ref"",
                            ""postage"": ""second""
                        }";
            }
        }

    }
}
