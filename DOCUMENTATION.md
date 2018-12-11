# .NET client documentation

This documentation is for developers interested in using the GOV.UK Notify .NET client to send emails (including documents), text messages or letters. GOV.UK Notify supports .NET framework 4.6.2 and .NET Core 2.0.

# Set up the client

## Prerequisites

This documentation assumes you are using [Microsoft Visual Studio](https://visualstudio.microsoft.com/) [external link] with the [NuGet Package Manager](https://www.nuget.org/) [external link].

Refer to the [client changelog](https://github.com/alphagov/notifications-net-client/blob/master/CHANGELOG.md) for the version number and the latest updates.

## Install the client

The GOV.UK Notify client deploys to [Bintray](https://bintray.com/) [external link].

You can install the GOV.UK Notify client package using either the command line or Microsoft Visual Studio.

### Use the command line

Go to your project directory and run the following in the command line to install the client package:

```
nuget sources Add -Name NotifyBintray -Source https://api.bintray.com/nuget/gov-uk-notify/nuget
nuget install Notify
```

### Use Microsoft Visual Studio

Use the [NuGet Package Manager](https://docs.microsoft.com/en-us/nuget/what-is-nuget) [external link] to install the `notifications-net-client` client package in Visual Studio.

You can use either the [console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console) [external link] or [the UI](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui) [external link].

#### Console

Run the following in the NuGet package manager console to install the client package:

```
nuget install Notify -Source https://api.bintray.com/nuget/gov-uk-notify/nuget
```

#### UI

1. Add the `https://api.bintray.com/nuget/gov-uk-notify/nuget` package source to your project.

1. Use the Package Manager UI to [search for and install the client package](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui#finding-and-installing-a-package) [external link].

## Create a new instance of the client

Add this code to your application:

```csharp
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;

var client = new NotificationClient(apiKey);
```

To get an API key, [sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/) and go to the __API integration__ page. Refer to the [API keys](#api-keys) section of this documentation for more information.

If you use a proxy, you must set the proxy configuration in the `web.config` file. Refer to the [Microsoft documentation on proxy configuration](https://docs.microsoft.com/en-us/dotnet/framework/network-programming/proxy-configuration) for more information.

# Send a message

You can use GOV.UK Notify to send text messages, emails and letters.

## Send a text message

### Method

```csharp
SmsNotificationResponse response = client.SendSms(
  mobileNumber: "+447900900123",
  templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a",
  );
```

### Arguments

#### mobileNumber (required)

The phone number of the recipient of the text message. This can be a UK or international number.

```csharp
string mobileNumber: "+447900900123";
```

#### templateId (required)

Sign in to [GOV.UK Notify](https://www.notifications.service.gov.uk/) and go to the __Templates__ page to find the template ID.

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### personalisation (optional)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a `Dictionary`. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    {"first_name", "Amala"},
    {"application_date", "2018-01-01"}
};
```

You can leave out this argument if a template does not have any placeholder fields for personalised information.

#### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

#### smsSenderId (optional)

A unique identifier of the sender of the text message notification. You can find this information in the __Text Message sender__ settings.

1. Sign in to your GOV.UK Notify account.
1. Go to __Settings__.
1. If you need to change to another service, select __Switch service__ in the top right corner of the screen and select the correct one.
1. Go to the __Text Messages__ section and select __Manage__ on the __Text Message sender__ row.

You can then either:

  - copy the sender ID that you want to use and paste it into the method
  - select __Change__ to change the default sender that the service uses, and select __Save__

For example:

```csharp
string smsSenderId: "8e222534-7f05-4972-86e3-17c5d9f894e2";
```

You can leave out this argument if your service only has one text message sender, or if you want to use the default sender.

### Response

If the request to the client is successful, the client returns an `SmsNotificationResponse`:

```csharp
public String fromNumber;
public String body;
public String id;
public String reference;
public String uri;
public Template template;

public class Template {
    public String id;
    public String uri;
    public Int32 version;
}

```

If you use the [test API key](#test), all your messages come back with a `delivered` status.

All messages sent using the [team and whitelist](#team-and-whitelist) or [live](#live) keys appear on your dashboard.

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code.

|error.code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`]}`|Use the correct type of [API key](/ruby.html#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification|

## Send an email

### Method

```csharp
client.SendEmail(
    emailAddress: "sender@something.com",
    templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a"
);
```

### Arguments

#### emailAddress (required)

The email address of the recipient. For example:

```csharp
string emailAddress: "sender@something.com";
```

#### templateId (required)

Sign in to [GOV.UK Notify](https://www.notifications.service.gov.uk/) and go to the __Templates__ page to find the template ID. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### personalisation (optional)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a `Dictionary`. For example:


```csharp
Dictionary<String, dynamic> personalisation: new Dictionary<String, dynamic>
{
    { "first_name", "Amala"
      "application_date", "2018-01-01"
    }
};
```

You can leave out this argument if a template does not have any placeholder fields for personalised information.

#### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

#### emailReplyToId (optional)

This is an email reply-to address you can set to receive replies from your users. Your service cannot go live until you set up at least one of these email addresses.

1. Sign in to your GOV.UK Notify account.
1. Go to __Settings__.
1. If you need to change to another service, select __Switch service__ in the top right corner of the screen and select the correct one.
1. Go to the __Email__ section and select __Manage__ on the __Email reply to addresses__ row.
1. Select __Change__ to specify the email address to receive replies, and select __Save__.

For example:

```csharp
string emailReplyToId: "8e222534-7f05-4972-86e3-17c5d9f894e2";
```

You can leave out this argument if your service only has one email reply-to address, or you want to use the default email address.

### Response

If the request to the client is successful, the client returns an `EmailNotificationResponse`:

```csharp
public String fromEmail;
public String body;
public String subject;
public String id;
public String reference;
public String uri;
public Template template;

public class Template
{
    public String id;
    public String uri;
    public Int32 version;
}
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code.

|error.status_code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`]}`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Unsupported document type '{}'. Supported types are: {}"`<br>`}]`|The attached document must be a PDF file|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Document didn't pass the virus scan"`<br>`}]`|The attached document must be virus free|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|
|N/A|`System.ArgumentException("Document is larger than 2MB")`|Document size was too large, upload a smaller file|

## Send a document by email

Send files without the need for email attachments.

This is an invitation-only feature. [Contact the GOV.UK Notify team](https://www.notifications.service.gov.uk/support) to enable this function for your service.

To send a document by email, add a placeholder field to the template and then upload a file. The placeholder field will contain a secure link to download the document.

#### Add a placeholder field to the template

1. Sign in to [GOV.UK Notify](https://www.notifications.service.gov.uk/).
1. Go to the __Templates__ page and select the relevant email template.
1. Add a placeholder field to the email template using double brackets. For example:

"Download your document at: ((link_to_document))"

#### Upload the document

The document you upload must be a PDF file smaller than 2MB.

1. Convert the PDF to a `byte[]`.
1. Pass the `byte[]` to the personalisation argument.
1. Call the [sendEmail method](#send-an-email).

For example:

```csharp

byte[] documentContents = File.ReadAllBytes("<document file path>");

Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" },
    { "link_to_document", NotificationClient.PrepareUpload(documentContents)}
};
```

### Error codes

If the request is not successful, the client returns an `HTTPError` containing the relevant error code.

|error.status_code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`]}`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Unsupported document type '{}'. Supported types are: {}"`<br>`}]`|The document you upload must be a PDF file|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Document didn't pass the virus scan"`<br>`}]`|The document you upload must be virus free|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Service is not allowed to send documents"`<br>`}]`|Contact the GOV.UK Notify team|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|
|`N\A`|`Document is larger than 2MB`|Document size was too large, upload a smaller file|

## Send a letter

When your service first signs up to GOV.UK Notify, you’ll start in trial mode. You can only send letters in live mode. You must ask GOV.UK Notify to make your service live.

1. Sign in to [GOV.UK Notify](https://www.notifications.service.gov.uk/).
1. Select __Using Notify__.
1. Select __Requesting to go live__.

### Method

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "address_line_1", "The Occupier" },
    { "address_line_2", "123 High Street" },
    { "address_line_3", "London" },
    { "postcode", "SW14 6BF" }
};

LetterNotificationResponse response = client.SendLetter(
    templateId,
    personalisation,
    reference
);
```

### Arguments

#### templateId (required)

Sign in to GOV.UK Notify and go to the __Templates__ page to find the template ID. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### personalisation (required)

The personalisation argument always contains the following required parameters for the letter recipient's address:

- `address_line_1`
- `address_line_2`
- `postcode`

Any other placeholder fields included in the letter template also count as required parameters. You need to provide their values in a `Dictionary`. For example:

```python
personalisation: {
  "address_line_1": "The Occupier",
  "address_line_2": "123 High Street",
  "postcode": "SW14 6BF",
  "name": "John Smith",
  "application_id": "4134325"
}
```

#### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

#### personalisation (optional)

The following parameters in the letter recipient's address are optional:

```csharp
Dictionary<String, dynamic> personalisation: new Dictionary<String, dynamic>
{
{ "address_line_3", "The Ridings" }
{ "address_line_4", "23 Foo Road" },
{ "address_line_5", "Bar Town" },
{ "address_line_6", "London" },
};
```

### Response

If the request to the client is successful, the client returns a `LetterNotificationResponse`:

```csharp
public String id;
public String body;
public String subject;
public String reference;
public String uri;
public Template template;

public class Template
{
    public String id;
    public String uri;
    public Int32 version;
}
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code.

|error.code|error.message|How to fix|
|:--- |:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`]}`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|Ensure that your template has a field for the first line of the address, refer to [personalisation](#personalisation-required) for more information|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification|

## Send a precompiled letter

This is an invitation-only feature. Contact the GOV.UK Notify team on the [support page](https://www.notifications.service.gov.uk/support) or through the [Slack channel](https://ukgovernmentdigital.slack.com/messages/govuk-notify) for more information.

### Method

```csharp
LetterNotificationsResponse response = client.SendPrecompiledLetter(
    clientReference,
    pdfContents
    );
```

### Arguments

#### clientReference (required)

A unique identifier you create. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address.

#### pdfContents (required for the SendPrecompiledLetter method)

The precompiled letter must be a PDF file. The method sends the contents of the file to GOV.UK Notify.

```csharp
byte[] pdfContents = File.ReadAllBytes("<PDF file path>");
```
### Response

If the request to the client is successful, the client returns a `LetterNotificationResponse` with the `id` and `reference` set:

```csharp
public String id;
public String reference;
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code:

|httpResult|Message|How to fix|
|:--- |:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`]}`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|Send a valid PDF file|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Service is not allowed to send precompiled letters"`<br>`}]`|Contact the GOV.UK Notify team|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#service-limits) for the limit number|
|N/A|`"message":"precompiledPDF must be a valid PDF file"`|Send a valid PDF file|
|N/A|`"message":"reference cannot be null or empty"`|Populate the reference parameter|
|N/A|`"message":"precompiledPDF cannot be null or empty"`|Send a PDF file with data in it|

# Get message status

Message status depends on the type of message you have sent.

You can only get the status of messages that are 7 days old or newer.

## Status - text and email

|Status|Information|
|:---|:---|
|Created|The message is queued to be sent to the provider. The notification usually remains in this state for a few seconds.|
|Sending|The message is queued to be sent by the provider to the recipient, and GOV.UK Notify is waiting for delivery information.|
|Delivered|The message was successfully delivered.|
|Failed|This covers all failure statuses:<br>- `permanent-failure` - "The provider was unable to deliver message, email or phone number does not exist; remove this recipient from your list"<br>- `temporary-failure` - "The provider was unable to deliver message, email inbox was full or phone was turned off; you can try to send the message again"<br>- `technical-failure` - "Notify had a technical failure; you can try to send the message again"|

## Status - text only

|Status|Information|
|:---|:---|
|Pending|GOV.UK Notify received a callback from the provider but the device has not yet responded. Another callback from the provider determines the final status of the notification.|
|Sent|The text message was delivered internationally. This only applies to text messages sent to non-UK phone numbers. GOV.UK Notify may not receive additional status updates depending on the recipient's country and telecoms provider.|

## Status - letter

|Status|information|
|:---|:---|
|Failed|The only failure status that applies to letters is `technical-failure`. GOV.UK Notify had an unexpected error while sending to our printing provider.|
|Accepted|GOV.UK Notify is printing and posting the letter.|
|Received|The provider has received the letter to deliver.|

## Status - precompiled letter

|Status|information|
|:---|:---|
|Pending virus check|GOV.UK Notify virus scan of the precompiled letter file is not yet complete.|
|Virus scan failed|GOV.UK Notify virus scan has identified a potential virus in the precompiled letter file.|

## Get the status of one message

You can only get the status of messages that are 7 days old or newer.

### Method

```csharp
Notification notification = client.GetNotificationById(notificationId);
```

### Arguments

#### notificationId (required)

The ID of the notification. You can find the notification ID in the response to the [original notification method call](#response).

You can also find it in your [GOV.UK Notify Dashboard](https://www.notifications.service.gov.uk).

1. Sign in to GOV.UK Notify and select __Dashboard__.
1. Select either __emails sent__, __text messages sent__ or __letters sent__.
1. Select the relevant notification.
1. Copy the notification ID from the end of the page URL, for example `https://www.notifications.service.gov.uk/services/af90d4cb-ae88-4a7c-a197-5c30c7db423b/notification/ID`.

### Response

If the request to the client is successful, the client returns a `Notification`.

```csharp
public String id;
public String completedAt;
public String createdAt;
public String emailAddress;
public String body;
public String subject;
public String line1;
public String line2;
public String line3;
public String line4;
public String line5;
public String line6;
public String phoneNumber;
public String postcode;
public String reference;
public String sentAt;
public String status;
public Template template;
public String type;
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code:

|error.status_code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the notification ID|


## Get the status of multiple messages

This API call returns the status of multiple messages. You can get either the most recent messages, or get older messages by specifying a particular notification ID in the `olderThanId` argument.

You can only get messages that are 7 days old or newer.

### Method

```csharp
NotificationList notifications = client.GetNotifications(
  templateType,
  status,
  reference,
  olderThanId
  );
```

### Arguments

You can leave out these arguments to ignore these filters.

#### templateType (optional)

You can filter by:

* `email`
* `sms`
* `letter`

#### status (optional)

| status | description | text | email | letter |
|:--- |:--- |:--- |:--- |:--- |
|`created` |The message is queued to be sent to the provider|Yes|Yes||
|`sending` |The message is queued to be sent by the provider to the recipient|Yes|Yes||
|`delivered`|The message was successfully delivered|Yes|Yes||
|`pending`|GOV.UK Notify received a callback from the provider but the device has not yet responded|Yes|||
|`sent`|The text message was delivered internationally|Yes|Yes||
|`failed`|This returns all failure statuses:<br>- `permanent-failure`<br>- `temporary-failure`<br>- `technical-failure`|Yes|Yes||
|`permanent-failure`|The provider was unable to deliver message, email or phone number does not exist; remove this recipient from your list|Yes|Yes||
|`temporary-failure`|The provider was unable to deliver message, email inbox was full or phone was turned off. You can try to send the message again|Yes|Yes||
|`technical-failure`|Email / Text: Notify had a technical failure. You can try to send the message again. <br><br>Letter: Notify had an unexpected error while sending to our printing provider. <br><br>You can leave out this argument to ignore this filter.|Yes|Yes||
|`accepted`|Notify is printing and posting the letter|||Yes|
|`received`|The provider has received the letter to deliver|||Yes|

#### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference = "STRING";
```

#### olderThanId (optional)

Input the ID of a notification into this argument to return the next 250 received notifications older than the given ID. For example:

```csharp
string olderThanId: "e194efd1-c34d-49c9-9915-e4267e01e92e";
```

If you leave out this argument, the client returns the most recent 250 notifications.

The client only returns notifications that are 7 days old or newer. If the notification specified in this argument is older than 7 days, the client returns an empty response.

### Response

If the request to the client is successful, the client returns a `Notify.Exceptions.NotifyClientException`.

```csharp
public List<Notification> notifications;
public Link links;

public class Link {
	public String current;
	public String next;
}
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code:

|error.status_code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "bad status is not one of [created, sending, delivered, pending, failed, technical-failure, temporary-failure, permanent-failure]"`<br>`}]`|Contact the GOV.UK Notify team|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Apple is not one of [sms, email, letter]"`<br>`}]`|Contact the GOV.UK Notify team|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|

# Get a template

## Get a template by ID

### Method

This returns the latest version of the template.

```csharp
TemplateResponse response = client.GetTemplateById(
    "templateId"
);
```

### Arguments

#### templateId (required)

The ID of the template. Sign in to GOV.UK Notify and go to the __Templates__ page to find this. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

### Response

If the request to the client is successful, the client returns a `TemplateResponse`.

```csharp
public String id;
public String name;
public String type;
public DateTime created_at;
public DateTime? updated_at;
public String created_by;
public int version;
public String body;
public String subject; // null if an sms message
```


### Error codes

If the request is not successful, the client returns an `HTTPError` and an error code:

|error.code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](#get-a-template-by-id-arguments-id-required)|


## Get a template by ID and version

### Method

```csharp
TemplateResponse response = client.GetTemplateByIdAndVersion(
    "templateId",
    1   // integer required for version number
);
```

### Arguments

#### templateId (required)

The ID of the template. Sign in to GOV.UK Notify and go to the __Templates__ page to find this. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### version (required)

The version number of the template.

### Response

If the request to the client is successful, the client returns a `TemplateResponse`.

```csharp
public String id;
public String name;
public String type;
public DateTime created_at;
public DateTime? updated_at;
public String created_by;
public int version;
public String body;
public String subject; // null if an sms message
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code:

|error.code|error.message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](/ruby.html#get-a-template-by-id-and-version-arguments-id-required) and [version](#version-required)|


## Get all templates

### Method

This returns the latest version of all templates.

```csharp
TemplateList response = client.GetAllTemplates(
    "sms"
);
```

### Arguments

#### templateType (optional)

If left out, the method returns all templates. Otherwise you can filter by:

- `email`
- `sms`
- `letter`

### Response

If the request to the client is successful, the client returns a `TemplateList`.

```csharp
List<TemplateResponse> templates;
```

If no templates exist for a template type or there no templates for a service, the client returns a `TemplateList` with an empty `templates` list element:

```csharp
List<TemplateResponse> templates; // empty list of templates
```

## Generate a preview template

### Method

This generates a preview version of a template.

```csharp
TemplatePreviewResponse response = client.GenerateTemplatePreview(
    templateId,
    personalisation
);
```

The parameters in the personalisation argument must match the placeholder fields in the actual template. The API notification client ignores any extra fields in the method.

### Arguments

#### templateId (required)

The ID of the template. Sign in to GOV.UK Notify and go to the __Templates__ page. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### personalisation (required)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a `Dictionary`. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};
```

You can leave out this argument if a template does not have any placeholder fields for personalised information.

### Response

If the request to the client is successful, you receive a `TemplatePreviewResponse` response.

```csharp
public String id;
public String type;
public int version;
public String body;
public String subject; // null if a sms message
```

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code:

|error.status_code|error.message|Notes|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Missing personalisation: [PERSONALISATION FIELD]"`<br>`}]`|Check that the personalisation arguments in the method match the placeholder fields in the template|
|`400`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the [template ID](#generate-a-preview-template-arguments-template-id-required)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|


# Get received text messages

This API call returns one page of up to 250 received text messages. You can get either the most recent messages, or get older messages by specifying a particular notification ID in the `olderThanId` argument.

You can only get the status of messages that are 7 days old or newer.

### Method

```csharp
ReceivedTextListResponse response = client.GetReceivedTexts(olderThanId);
```

### Arguments

#### olderThanId (optional)

Input the ID of a notification into this argument to return the next 250 received notifications older than the given ID. For example:

```csharp
olderThanId: "740e5834-3a29-46b4-9a6f-16142fde533a"
```

If you leave out the `olderThanId` argument, the client returns the most recent 250 notifications.

The client only returns notifications that are 7 days old or newer. If the notification specified in this argument is older than 7 days, the client returns an empty `ReceivedTextListResponse` response.

### Response

If the request to the client is successful, the client returns a `ReceivedTextListResponse` that returns all received text messages.

```csharp
public List<ReceivedText> receivedTextList;
public Link links;

public class Link {
	       public String current;
	       public String next;
}
```

```csharp
public String id;
public String userNumber;
public String createdAt;
public String serviceId;
public String notifyNumber;
public String content;
```
If the notification specified in the `olderThanId` argument is older than 7 days, the client returns an empty `ReceivedTextListResponse` response.

### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` and an error code.

|error.code|error.message|How to fix|
|:---|:---|:---|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
