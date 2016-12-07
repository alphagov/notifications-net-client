using System;
using System.Reflection;

namespace NotifyTests
{
    public static class Constants
    {
        public static String fakeApiKey { get { return "FAKEKEY-fd29e561-24b6-4f32-be5c-e642a1d68641-57bdfd56-ac07-409b-8307-71419d85bb9c"; } }
        public static String userAgent {
            get { return 
                    "NOTIFY-API-NET-CLIENT/" + Assembly.LoadFrom("Notify.dll").GetName().Version;
            } }

        public static String fakePhoneNumber { get { return "07766565767"; } }
        public static String fakeEmail { get { return "test@mail.com"; } }

        public static String fakeNotificationId { get { return "902e6534-bc4a-4c87-8c3e-9f4144ca36fd"; } }
        public static String fakeNotificationReference { get { return "some-client-ref"; } }
        public static String fakeTemplateId { get { return "913e9fa6-9cbb-44ad-8f58-38487dccfd82"; } }
        public static String fakeNotificationJson { get { return "{\n  \"completed_at\": null,\n  \"created_at\": \"2016-11-22T11:21:13.133522Z\",\n  \"email_address\": null,\n  \"id\": \"902e4312-bc4a-4c87-8c3e-9f4144ca36fd\",\n  \"line_1\": null,\n  \"line_2\": null,\n  \"line_3\": null,\n  \"line_4\": null,\n  \"line_5\": null,\n  \"line_6\": null,\n  \"phone_number\": \"+447588767647\",\n  \"postcode\": null,\n  \"reference\": null,\n  \"sent_at\": \"2016-11-22T16:16:09.885808Z\",\n  \"status\": \"sending\",\n  \"template\": {\n    \"id\": \"913e9fa6-9cbb-44ad-8f58-38487dccfd82\",\n    \"uri\": \"/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43\",\n    \"version\": 2\n  },\n  \"type\": \"sms\"\n}"; } }

        public static String fakeSmsNotificationResponseJson { get { return "{\n  \"content\": {\n    \"body\": \"test\",\n    \"from_number\": null\n  },\n  \"id\": \"d683f7f9-df04-4b9c-8019-15092c4674fd\",\n  \"reference\": null,\n  \"template\": {\n    \"id\": \"be35a391-e912-42e9-82e6-3f4953f6cbb0\",\n    \"uri\": \"http://aa50ebb9.ngrok.io//v2/templates/be35a391-e912-42e9-82e6-3f4953f6cbb0\",\n    \"version\": 1\n  },\n  \"uri\": \"http://some_url//v2/notifications/d683f7f9-df04-4b9c-8019-15092c4674fd\"\n}"; ; } }
        public static String fakeEmailNotificationResponseJson { get { return "{\n  \"content\": {\n    \"body\": \"Hello ((name))\\n\\nFake\",\n    \"from_email\": \"someone@mail.com\",\n    \"subject\": \"Test\"\n  },\n  \"id\": \"731b9c83-563f-4b59-afc5-87e9ca717833\",\n  \"reference\":  \"some-client-ref\",\n  \"template\": {\n    \"id\": \"f0bb62f7-5ddb-4bf8-aac7-7ey6aefd1524\",\n    \"uri\": \"https://someurl//v2/templates/c0bs62f7-4ddb-6bf8-cac7-c1e6aefd1524\",\n    \"version\": 5\n  },\n  \"uri\": \"https://someurl//v2/notifications/321b9c43-563f-4c59-sac5-87e9ca325833\"\n}"; ; } }

    }
}
