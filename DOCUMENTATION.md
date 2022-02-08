# .NET client documentation

This documentation is for developers interested in using the GOV.UK Notify .NET client to send emails (including documents), text messages or letters. GOV.UK Notify supports .NET framework 4.6.2 and .NET Core 2.0.

## Set up the client

### Prerequisites

This documentation assumes you are using [Microsoft Visual Studio](https://visualstudio.microsoft.com/) [external link] with the [NuGet Package Manager](https://www.nuget.org/) [external link].

Refer to the [client changelog](https://github.com/alphagov/notifications-net-client/blob/master/CHANGELOG.md) for the version number and the latest updates.

### Install the client

The GOV.UK Notify client can be installed from [Nuget.org](https://www.nuget.org/packages/GovukNotify/) [external link].

You can install the GOV.UK Notify client package using either the command line or Microsoft Visual Studio.

<div class="govuk-warning-text">
  <span class="govuk-warning-text__icon" aria-hidden="true">!</span>
  <strong class="govuk-warning-text__text">
    <span class="govuk-warning-text__assistive">Warning</span>
      If you need a version of our API client older than 4.0.0, you should download it from
      <a href="https://github.com/alphagov/notifications-net-client/releases">Github</a> [external link]
  </strong>
</div>

#### Use the dotnet command line interface

Go to your project directory and run the following in the command line to install the client package:

```
dotnet add package GovukNotify
```

#### Use Microsoft Visual Studio

Use the [NuGet Package Manager](https://docs.microsoft.com/en-us/nuget/what-is-nuget) [external link] to install the `GovukNotify` client package in Visual Studio.

You can use either the [console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console) [external link] or [the UI](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui) [external link].

##### Console

Run the following in the NuGet package manager console to install the client package:

```
nuget install GovukNotify
```

##### UI

Use the Package Manager UI to [search for and install the client package](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui#finding-and-installing-a-package) [external link].

### Create a new instance of the client

Add this code to your application:

```csharp
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;

var client = new NotificationClient(apiKey);
```

To get an API key, [sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __API integration__ page. You can find more information in the [API keys](#api-keys) section of this documentation.

If you use a proxy, you must set the proxy configuration in the `web.config` file. Refer to the [Microsoft documentation on proxy configuration](https://docs.microsoft.com/en-us/dotnet/framework/network-programming/proxy-configuration) for more information.

```csharp
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;
using System.Net.Http;

var httpClientWithProxy = new HttpClientWrapper(new HttpClient(...));
var client = new NotificationClient(httpClientWithProxy, apiKey);
```

## Send a message

You can use GOV.UK Notify to send text messages, emails and letters.

### Send a text message

#### Method

```csharp
SmsNotificationResponse response = client.SendSms(
  mobileNumber: "+447900900123",
  templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a",
  );
```

#### Arguments

##### mobileNumber (required)

The phone number of the recipient of the text message. This can be a UK or international number.

```csharp
string mobileNumber: "+447900900123";
```

##### templateId (required)

To find the template ID:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Templates__ page and select the relevant template.
1. Select __Copy template ID to clipboard__.

For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

##### personalisation (optional)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a `Dictionary`. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    {"first_name", "Amala"},
    {"application_date", "2018-01-01"}
};
```

You can leave out this argument if a template does not have any placeholder fields for personalised information.

##### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

##### smsSenderId (optional)

A unique identifier of the sender of the text message notification.

To find the text message sender:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Settings__ page.
1. In the __Text Messages__ section, select __Manage__ on the __Text Message sender__ row.

You can then either:

  - copy the sender ID that you want to use and paste it into the method
  - select __Change__ to change the default sender that the service uses, and select __Save__

For example:

```csharp
string smsSenderId: "8e222534-7f05-4972-86e3-17c5d9f894e2";
```

You can leave out this argument if your service only has one text message sender, or if you want to use the default sender.

#### Response

If the request to the client is successful, the client returns an `SmsNotificationResponse`:

```csharp
public String id;
public String reference;
public String uri;
public Template template;
public SmsResponseContent;

public class Template {
    public String id;
    public String uri;
    public Int32 version;
}
public class SmsResponseContent{
    public string body;
    public string fromNumber;
}

```

If you use the [test API key](#test), all your messages come back with a `delivered` status.

All messages sent using the [team and guest list](#team-and-guest-list) or [live](#live) keys appear on your dashboard.

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#daily-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification|

### Send an email

#### Method

```csharp
EmailNotificationResponse response = client.SendEmail(emailAddress, templateId, personalisation, reference, emailReplyToId);
```

```csharp
client.SendEmail(
    emailAddress: "sender@something.com",
    templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a"
);
```

#### Arguments

##### emailAddress (required)

The email address of the recipient. For example:

```csharp
string emailAddress: "sender@something.com";
```

##### templateId (required)

To find the template ID:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Templates__ page and select the relevant template.
1. Select __Copy template ID to clipboard__.

For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

##### personalisation (optional)

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

##### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

##### emailReplyToId (optional)

This is an email address specified by you to receive replies from your users. You must add at least one reply-to email address before your service can go live.

To add a reply-to email address:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Settings__ page.
1. In the __Email__ section, select __Manage__ on the __Reply-to email addresses__ row.
1. Select __Add reply-to address__.
1. Enter the email address you want to use, and select __Add__.

For example:

```csharp
string emailReplyToId: "8e222534-7f05-4972-86e3-17c5d9f894e2";
```

You can leave out this argument if your service only has one email reply-to address, or you want to use the default email address.

#### Response

If the request to the client is successful, the client returns an `EmailNotificationResponse`:

```csharp
public String id;
public String reference;
public String uri;
public Template template;
public EmailResponseContent content;

public class Template{
    public String id;
    public String uri;
    public Int32 version;
}
public class EmailResponseContent{
  public String fromEmail;
  public String body;
  public String subject;
}
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "File did not pass the virus scan"`<br>`}]`|The file contains a virus|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information.|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [daily limits](#daily-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|

### Send a file by email

To send a file by email, add a placeholder to the template then upload a file. The placeholder will contain a secure link to download the file.

The file will be available for the recipient to download for 18 months.

The links are unique and unguessable. GOV.UK Notify cannot access or decrypt your file.

#### Add contact details to the file download page

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Settings__ page.
1. In the __Email__ section, select __Manage__ on the __Send files by email__ row.
1. Enter the contact details you want to use, and select __Save__.

#### Add a placeholder to the template

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Templates__ page and select the relevant email template.
1. Select __Edit__.
1. Add a placeholder to the email template using double brackets. For example:

"Download your file at: ((link_to_file))"

#### Upload your file

You can upload PDF, CSV, .odt, .txt, .rtf, .xlsx and MS Word Document files. Your file must be smaller than 2MB. [Contact the GOV.UK Notify team](https://www.notifications.service.gov.uk/support/ask-question-give-feedback) if you need to send other file types.

1. Convert the PDF to a `byte[]`.
1. Pass the `byte[]` to the personalisation argument.
1. Call the [sendEmail method](#send-an-email).

For example:

```csharp

byte[] documentContents = File.ReadAllBytes("<file path>");

Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" },
    { "link_to_file", NotificationClient.PrepareUpload(documentContents)}
};
```

##### CSV files

Uploads for CSV files should use the `isCsv` parameter on the `PrepareUpload()` function. For example:

```csharp

byte[] documentContents = File.ReadAllBytes("<file path>");

Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" },
    { "link_to_file", NotificationClient.PrepareUpload(documentContents, isCsv = true)}
};
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Unsupported file type '(FILE TYPE)'. Supported types are: '(ALLOWED TYPES)'"`<br>`}]`|Wrong file type. You can only upload .pdf, .csv, .txt, .doc, .docx, .xlsx, .rtf or .odt files|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "File did not pass the virus scan"`<br>`}]`|The file contains a virus|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Send files by email has not been set up - add contact details for your service at https://www.notifications.service.gov.uk/services/(SERVICE ID)/service-settings/send-files-by-email"`<br>`}]`|See how to [add contact details to the file download page](#add-contact-details-to-the-file-download-page)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information.|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#daily-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|
|`N/A`|`File is larger than 2MB`|The file is too big. Files must be smaller than 2MB|

### Send a letter

When you add a new service it will start in [trial mode](https://www.notifications.service.gov.uk/features/trial-mode). You can only send letters when your service is live.

To send Notify a request to go live:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Settings__ page.
1. In the __Your service is in trial mode__ section, select __request to go live__.

#### Method

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "address_line_1", "The Occupier" }, // required
    { "address_line_2", "123 High Street" }, // required
    { "address_line_3", "SW14 6BF" }, // required
      ... // any other optional address lines, or personalisation fields found in your template
};

LetterNotificationResponse response = client.SendLetter(templateId, personalisation, reference);
```

#### Arguments

##### templateId (required)

To find the template ID:

1. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in).
1. Go to the __Templates__ page and select the relevant template.
1. Select __Copy template ID to clipboard__.

For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

##### personalisation (required)

The personalisation argument always contains the following parameters for the letter recipient’s address:

- `address_line_1`
- `address_line_2`
- `address_line_3`
- `address_line_4`
- `address_line_5`
- `address_line_6`
- `address_line_7`

The address must have at least 3 lines.

The last line needs to be a real UK postcode or the name of a country outside the UK.

Notify checks for international addresses and will automatically charge you the correct postage.

The `postcode` personalisation argument has been replaced. If your template still uses `postcode`, Notify will treat it as the last line of the address.

Any other placeholder fields included in the letter template also count as required parameters. You need to provide their values in a `Dictionary`. For example:

```csharp
personalisation: {
  "address_line_1": "The Occupier",
  "address_line_2": "123 High Street",
  "address_line_3": "Richmond upon Thames",
  "address_line_4": "Middlesex",
  "address_line_5": "SW14 6BF",
  "name": "John Smith",
  "application_id": "4134325"
}
```

##### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference: "STRING";
```
You can leave out this argument if you do not have a reference.

#### Response

If the request to the client is successful, the client returns a `LetterNotificationResponse`:

```csharp
public String id;
public String reference;
public String uri;
public Template template;
public string postage;
public LetterResponseContent content;

public class Template
{
    public String id;
    public String uri;
    public Int32 version;
}
public class LetterResponseContent{
    public string body;
    public string subject;
}
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:--- |:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`}]`|Use the correct type of [API key](#api-keys).|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode).|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|Ensure that your template has a field for the first line of the address, refer to [personalisation](#personalisation-required) for more information.|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Must be a real UK postcode"`<br>`}]`|Ensure that the value for the last line of the address is a real UK postcode.|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Last line of address must be a real UK postcode or another country"`<br>`}]`|Ensure that the value for the last line of the address is a real UK postcode or the name of a country outside the UK.|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock.|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information.|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#rate-limits) for more information.|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#daily-limits) for the limit number.|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|

### Send a precompiled letter

#### Method

```csharp
LetterNotificationsResponse response = client.SendPrecompiledLetter(
    clientReference,
    pdfContents,
    postage
    );
```

#### Arguments

##### clientReference (required)

A unique identifier you create. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address.

##### pdfContents (required for the SendPrecompiledLetter method)

The precompiled letter must be a PDF file which meets [the GOV.UK Notify letter specification](https://www.notifications.service.gov.uk/using-notify/guidance/letter-specification). The method sends the contents of the file to GOV.UK Notify.

```csharp
byte[] pdfContents = File.ReadAllBytes("<PDF file path>");
```

##### postage (optional)

You can choose first or second class postage for your precompiled letter. Set the value to `first` for first class, or `second` for second class. If you do not pass in this argument, the postage will default to second class.


#### Response

If the request to the client is successful, the client returns a `LetterNotificationResponse` with the `id`, `reference` and `postage`:

```csharp
public String id;
public String reference;
public String postage;
public String uri;  // null for this response
public Template template;  // null for this response
public LetterResponseContent content;  // null for this response

```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:--- |:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`}]`|Use the correct type of [API key](#api-keys)|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Your service cannot send this notification in [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode)|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|Send a valid PDF file|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "reference is a required property"`<br>`}]`|Add a `reference` argument to the method call|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "postage invalid. It must be either first or second."`<br>`}]`|Change the value of `postage` argument in the method call to either 'first' or 'second'|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](#rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](#daily-limits) for the limit number|
|N/A|`"message":"precompiledPDF must be a valid PDF file"`|Send a valid PDF file|
|N/A|`"message":"reference cannot be null or empty"`|Populate the reference parameter|
|N/A|`"message":"precompiledPDF cannot be null or empty"`|Send a PDF file with data in it|

## Get message status
### Get the status of one message

You can only get the status of messages sent within the retention period. The default retention period is 7 days.

#### Method

```csharp
Notification notification = client.GetNotificationById(notificationId);
```

#### Arguments

##### notificationId (required)

The ID of the notification. To find the notification ID, you can either:

* check the response to the [original notification method call](#response)
* [sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __API integration__ page

#### Response

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
public String line7;
public String phoneNumber;
public String postage;
public String reference;
public String sentAt;
public String status;
public Template template;
public String type;
public string createdByName;

public class Template
{
    public String id;
    public String uri;
    public Int32 version;
}


```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the notification ID|


### Get the status of multiple messages

This API call returns the status of multiple messages. You can get either the most recent messages, or get older messages by specifying a particular notification ID in the `olderThanId` argument.

You can only get messages that are 7 days old or newer.

#### Method

```csharp
NotificationList notifications = client.GetNotifications(templateType, status, reference, olderThanId, includeSpreadsheetUploads);
```

#### Arguments

You can leave out these arguments to ignore these filters.

##### templateType (optional)

You can filter by:

* `email`
* `sms`
* `letter`

##### status (optional)

You can filter by each:

* [email status](#email-status-descriptions)
* [text message status](#text-message-status-descriptions)
* [letter status](#letter-status-descriptions)
* [precompiled letter status](#precompiled-letter-status-descriptions)

You can leave out this argument to ignore this filter.

##### reference (optional)

A unique identifier you can create if you need to. This reference identifies a single unique notification or a batch of notifications. It must not contain any personal information such as name or postal address. For example:

```csharp
string reference = "STRING";
```

##### olderThanId (optional)

Input the ID of a notification into this argument to return the next 250 received notifications older than the given ID. For example:

```csharp
string olderThanId: "e194efd1-c34d-49c9-9915-e4267e01e92e";
```

If you leave out this argument, the client returns the most recent 250 notifications.

The client only returns notifications that are 7 days old or newer. If the notification specified in this argument is older than 7 days, the client returns an empty response.

##### includeSpreadsheetUploads (optional)

Specifies whether the response should include notifications which were sent by uploading a spreadsheet of recipients to Notify.

```csharp
bool includeSpreadsheetUploads = true;
```

If you do not pass in this argument it defaults to `false`.

#### Response

If the request to the client is successful, the client returns a `NotificationList`.

```csharp
public List<Notification> notifications;
public Link links;

public class Link {
	public String current;
	public String next;
}
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "bad status is not one of [created, sending, delivered, pending, failed, technical-failure, temporary-failure, permanent-failure]"`<br>`}]`|Contact the GOV.UK Notify team|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Apple is not one of [sms, email, letter]"`<br>`}]`|Contact the GOV.UK Notify team|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|

### Email status descriptions

|Status|Description|
|:---|:---|
|#`created`|GOV.UK Notify has placed the message in a queue, ready to be sent to the provider. It should only remain in this state for a few seconds.|
|#`sending`|GOV.UK Notify has sent the message to the provider. The provider will try to deliver the message to the recipient for up to 72 hours. GOV.UK Notify is waiting for delivery information.|
|#`delivered`|The message was successfully delivered.|
|#`permanent-failure`|The provider could not deliver the message because the email address was wrong. You should remove these email addresses from your database.|
|#`temporary-failure`|The provider could not deliver the message. This can happen when the recipient’s inbox is full or their anti-spam filter rejects your email. [Check your content does not look like spam](https://www.gov.uk/service-manual/design/sending-emails-and-text-messages#protect-your-users-from-spam-and-phishing) before you try to send the message again.|
|#`technical-failure`|Your message was not sent because there was a problem between Notify and the provider.<br>You’ll have to try sending your messages again.|

### Text message status descriptions

|Status|Description|
|:---|:---|
|#`created`|GOV.UK Notify has placed the message in a queue, ready to be sent to the provider. It should only remain in this state for a few seconds.|
|#`sending`|GOV.UK Notify has sent the message to the provider. The provider will try to deliver the message to the recipient for up to 72 hours. GOV.UK Notify is waiting for delivery information.|
|#`pending`|GOV.UK Notify is waiting for more delivery information.<br>GOV.UK Notify received a callback from the provider but the recipient’s device has not yet responded. Another callback from the provider determines the final status of the text message.|
|#`sent`|The message was sent to an international number. The mobile networks in some countries do not provide any more delivery information. The GOV.UK Notify website displays this status as 'Sent to an international number'.|
|#`delivered`|The message was successfully delivered.|
|#`permanent-failure`|The provider could not deliver the message. This can happen if the phone number was wrong or if the network operator rejects the message. If you’re sure that these phone numbers are correct, you should [contact GOV.UK Notify support](https://www.notifications.service.gov.uk/support). If not, you should remove them from your database. You’ll still be charged for text messages that cannot be delivered.
|#`temporary-failure`|The provider could not deliver the message. This can happen when the recipient’s phone is off, has no signal, or their text message inbox is full. You can try to send the message again. You’ll still be charged for text messages to phones that are not accepting messages.|
|#`technical-failure`|Your message was not sent because there was a problem between Notify and the provider.<br>You’ll have to try sending your messages again. You will not be charged for text messages that are affected by a technical failure.|

### Letter status descriptions

|Status|Description|
|:---|:---|
|#`accepted`|GOV.UK Notify has sent the letter to the provider to be printed.|
|#`received`|The provider has printed and dispatched the letter.|
|#`cancelled`|Sending cancelled. The letter will not be printed or dispatched.|
|#`technical-failure`|GOV.UK Notify had an unexpected error while sending the letter to our printing provider.|
|#`permanent-failure`|The provider cannot print the letter. Your letter will not be dispatched.|

### Precompiled letter status descriptions

|Status|Description|
|:---|:---|
|#`accepted`|GOV.UK Notify has sent the letter to the provider to be printed.|
|#`received`|The provider has printed and dispatched the letter.|
|#`cancelled`|Sending cancelled. The letter will not be printed or dispatched.|
|#`pending-virus-check`|GOV.UK Notify has not completed a virus scan of the precompiled letter file.|
|#`virus-scan-failed`|GOV.UK Notify found a potential virus in the precompiled letter file.|
|#`validation-failed`|Content in the precompiled letter file is outside the printable area. See the [GOV.UK Notify letter specification](https://www.notifications.service.gov.uk/using-notify/guidance/letter-specification) for more information.|
|#`technical-failure`|GOV.UK Notify had an unexpected error while sending the letter to our printing provider.|
|#`permanent-failure`|The provider cannot print the letter. Your letter will not be dispatched.|

### Get a PDF for a letter notification

#### Method

This returns the pdf contents of a letter notification.

```csharp
byte[] pdfFile = client.GetPdfForLetter(notificationId);
```

#### Arguments

##### notificationId (required)

The ID of the notification. To find the notification ID, you can either:

* check the response to the [original notification method call](#get-the-status-of-one-message-response)
* [sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __API integration__ page

#### Response

If the request to the client is successful, the client will return a `byte[]` object containing the raw PDF data.

#### Error codes

If the request is not successful, the client returns a `NotificationClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Notification is not a letter"`<br>`}]`|Check that you are looking up the correct notification|
|`400`|`[{`<br>`"error": "PDFNotReadyError",`<br>`"message": "PDF not available yet, try again later"`<br>`}]`|Wait for the notification to finish processing. This usually takes a few seconds|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "File did not pass the virus scan"`<br>`}]`|You cannot retrieve the contents of a letter notification that contains a virus|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "PDF not available for letters in technical-failure"`<br>`}]`|You cannot retrieve the contents of a letter notification in technical-failure|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the notification ID|


## Get a template

### Get a template by ID

#### Method

This returns the latest version of the template.

```csharp
TemplateResponse response = client.GetTemplateById(
    "templateId"
);
```

#### Arguments

##### templateId (required)

The ID of the template. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __Templates__ page to find it. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

#### Response

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
public String letter_contact_block; // null if not a letter template or contact block not set for letter
```


#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](#get-a-template-by-id-arguments-templateid-required)|


### Get a template by ID and version

#### Method

```csharp
TemplateResponse response = client.GetTemplateByIdAndVersion(
    "templateId",
    1   // integer required for version number
);
```

#### Arguments

##### templateId (required)

The ID of the template. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __Templates__ page to find it. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

##### version (required)

The version number of the template.

#### Response

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
public String letter_contact_block; // null if not a letter template or contact block not set for letter
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](#get-a-template-by-id-and-version-arguments-templateid-required) and [version](#version-required)|


### Get all templates

#### Method

This returns the latest version of all templates.

```csharp
TemplateList response = client.GetAllTemplates(
    "sms" | "email" | "letter" // optional
);
```

#### Arguments

##### templateType (optional)

If left out, the method returns all templates. Otherwise you can filter by:

- `email`
- `sms`
- `letter`

#### Response

If the request to the client is successful, the client returns a `TemplateList`.

```csharp
List<TemplateResponse> templates;
```

If no templates exist for a template type or there no templates for a service, the client returns a `TemplateList` with an empty `templates` list element:

```csharp
List<TemplateResponse> templates; // empty list of templates
```

### Generate a preview template

#### Method

This generates a preview version of a template.

```csharp
TemplatePreviewResponse response = client.GenerateTemplatePreview(
    templateId,
    personalisation
);
```

The parameters in the personalisation argument must match the placeholder fields in the actual template. The API notification client ignores any extra fields in the method.

#### Arguments

##### templateId (required)

The ID of the template. [Sign in to GOV.UK Notify](https://www.notifications.service.gov.uk/sign-in) and go to the __Templates__ page to find it. For example:

```csharp
string templateId: "f33517ff-2a88-4f6e-b855-c550268ce08a";
```

##### personalisation (required)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a `Dictionary`. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};
```

You can leave out this argument if a template does not have any placeholder fields for personalised information.

#### Response

If the request to the client is successful, you receive a `TemplatePreviewResponse` response.

```csharp
public String id;
public String type;
public int version;
public String body;
public String subject; // null if a sms message
```

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Missing personalisation: [PERSONALISATION FIELD]"`<br>`}]`|Check that the personalisation arguments in the method match the placeholder fields in the template|
|`400`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the [template ID](#generate-a-preview-template-arguments-templateid-required)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|


## Get received text messages

This API call returns one page of up to 250 received text messages. You can get either the most recent messages, or get older messages by specifying a particular notification ID in the `olderThanId` argument.

You can only get the status of messages that are 7 days old or newer.

You can also set up [callbacks](#callbacks) for received text messages.

### Enable received text messages

Contact the GOV.UK Notify team using the [support page](https://www.notifications.service.gov.uk/support) or [chat to us on Slack](https://ukgovernmentdigital.slack.com/messages/C0E1ADVPC) to request a unique number for text message replies.

### Get a page of received text messages

#### Method

```csharp
ReceivedTextListResponse response = client.GetReceivedTexts(olderThanId);
```

#### Arguments

##### olderThanId (optional)

Input the ID of a notification into this argument to return the next 250 received notifications older than the given ID. For example:

```csharp
olderThanId: "740e5834-3a29-46b4-9a6f-16142fde533a"
```

If you leave out the `olderThanId` argument, the client returns the most recent 250 notifications.

The client only returns notifications that are 7 days old or newer. If the notification specified in this argument is older than 7 days, the client returns an empty `ReceivedTextListResponse` response.

#### Response

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

#### Error codes

If the request is not successful, the client returns a `Notify.Exceptions.NotifyClientException` containing the relevant error code.

|Status code|Message|How to fix|
|:---|:---|:---|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: API key not found"`<br>`}]`|Use the correct API key. Refer to [API keys](#api-keys) for more information|
