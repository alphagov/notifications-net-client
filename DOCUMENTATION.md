# .Net client documentation

This documentation is for .Net developers interested in using GOV.UK Notify to send emails, text messages or letters.

# Set up the client

Refer to the [client change log](https://github.com/alphagov/notifications-net-client/blob/master/CHANGELOG.md) for the client version number and the latest updates.

## Dependency

Since Notify version 2.0.0 the .Net frameworks supported will be 4.6.2 and .Net Core 2.0, non windows OS only support .Net Core.
It is recommended that you upgrade your .Net Framework in order to fix security vulnerabilities.

_Notify supports .Net 4.6.2 and .Net Core 2.0. Non windows only support .Net Core - what does this mean?_

## Install the client

### Nuget Package Manager

The notifications-net-client is deployed to [Bintray](https://bintray.com/gov-uk-notify/nuget/Notify).

Navigate to your project directory and install Notify with the following command:

```
nuget install Notify -Source https://api.bintray.com/nuget/gov-uk-notify/notifications-net-client
```

Alternatively if you are using the Nuget Package Manager in Visual Studio, add the source below to install:

_QP: How?_
_QP: Is there a reason to use one method over another?_

```
https://api.bintray.com/nuget/gov-uk-notify/nuget
```
To add a new source to the Nuget Package Manager in Visual Studio - https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui#package-sources

### [Visual Studio](https://www.visualstudio.com/) (Windows)

To execute the NUnit tests you will need to install the [NUnit3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter) extension to Visual Studio or via the [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter/) nuget package

Setting Windows Environment variables (these must be executed before opening Visual Studio)
```
SETX NOTIFY_API_URL "https://example.notify-api.url"
SETX API_KEY "example_API_test_key"
SETX FUNCTIONAL_TEST_NUMBER "valid mobile number"
SETX FUNCTIONAL_TEST_EMAIL "valid email address"
SETX EMAIL_TEMPLATE_ID "valid email_template_id"
SETX SMS_TEMPLATE_ID "valid sms_template_id"
SETX LETTER_TEMPLATE_ID "valid letter_template_id"
SETX SMS_SENDER_ID "valid sms_sender_id - to test sending to a receiving number, so needs to be a real number"
SETX API_SENDING_KEY "API_whitelist_key for sending an SMS to a receiving number"
SETX INBOUND_SMS_QUERY_KEY "API_test_key to get received text messages"
```

### [Visual Studio](https://www.visualstudio.com/vs/visual-studio-mac/) (Mac OS)

In order to get the .Net client running in Visual Studio the target framework needs to be set to `.Net standard 2.0` and the application needs to be run from the terminal.

_QP: Does this code help you do that or not? Is this so simple and obvious that no words are required?_

```
open -n /Applications/"Visual Studio.app"
```

Setting Mac OS Environment variables (these must be sourced before opening Visual Studio using the command above)

```
export NOTIFY_API_URL=https://example.notify-api.url
export API_KEY=example_API_test_key
export FUNCTIONAL_TEST_NUMBER=valid mobile number
export FUNCTIONAL_TEST_EMAIL=valid email address
export EMAIL_TEMPLATE_ID=valid email_template_id
export SMS_TEMPLATE_ID=valid sms_template_id
export LETTER_TEMPLATE_ID=valid letter_template_id
```


## Create a new instance of the client

Add this code to your application:

```csharp
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;

NotificationClient client = new NotificationClient(apiKey);
```

To get an API key, [log in to GOV.UK Notify](https://www.notifications.service.gov.uk/) and go to the __API integration__ page. You can find more information can be found in the [API keys](/#api-keys) section.

# Send a message

GOV.UK Notify enables you to send text messages, emails and letters.

## Send a text message

### Method

```csharp
SmsNotificationResponse response = client.SendSms(mobileNumber, templateId, personalisation, reference, smsSenderId);
```

### Arguments

#### mobileNumber (required)

The phone number of the recipient of the text message. This number can be UK or international.

#### templateId (required)

The ID of the template. You can find this by logging into [GOV.UK Notify](https://www.notifications.service.gov.uk/) and going to the __Templates__ page.

#### personalisation (optional)

If a template has placeholder fields for personalised information such as name or reference number, you need to __provide their values in a dictionary with key value pairs__. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```

#### reference (optional)

A unique identifier. This reference can identify a single unique notification or a batch of multiple notifications.

_Example?_

#### smsSenderId (optional)

A unique identifier of the sender of the text message notification. To set this up:

1. Log into your GOV.UK Notify account.
1. Go to __Settings__.
1. Check that you are in the correct service. If you are not, select __Switch service__ in the top right corner of the screen and select the correct one.
1. Go to the __Text Messages__ section and select __Manage__ on the "Text Message sender" row.
1. You can do one of the following:
  - copy the ID of the sender you want to use and paste it into the method
  - select __Change__ to change the default sender that the service will use, and select __Save__

_example?_

If you omit this argument from your method, the client will set the default `smsSenderId` for the notification.

### Response

If the request to the client is successful, you will receive the following `SmsNotificationResponse` response:

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

If you are using the [test API key](/#test), all your messages will come back as delivered.

All successfully delivered messages will appear on your dashboard.

### Error codes

If the request is not successful, the client will raise an `HTTPError`:

_UPDATE ERROR CODES_

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`]}`|Use the correct type of API key. Refer to [API keys](/#api-keys) for more information|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Refer to [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode) for more information|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](/#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](/#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](/#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|

## Send an email

### Method

```csharp
EmailNotificationResponse response = client.SendEmail(emailAddress, templateId, personalisation, reference, emailReplyToId);
```

### Arguments

#### emailAddress (required)

The email address of the recipient, only required for email notifications.

#### templateId (required)

The ID of the template. You can find this by logging into GOV.UK Notify and going to the __Templates__ page.

#### personalisation (optional)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values in a dictionary with key value pairs. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```

#### reference (optional)

_QP: Is it yourReferenceString or reference?_

A unique identifier. This reference can identify a single unique notification or a batch of multiple notifications.

_QP: example?_

#### emailReplyToId (optional)

This is an email reply-to address specified by you to receive replies from your users. Your service cannot go live until at least one email address has been set up for this. To set up:

1. Log into your GOV.UK Notify account.
1. Go to __Settings__.
1. Check that you are in the correct service. If you are not, select __Switch service__ in the top right corner of the screen and select the correct one.
1. Go to the Email section and select __Manage__ on the "Email reply to addresses" row.
1. Select __Change__ to specify the email address to receive replies, and select __Save__.

_example?_

If you omit this argument, the client will set your default email reply-to address for the notification.

### Response

If the request to the client is successful, you will receive the following `EmailNotificationResponse` response:

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

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient using a team-only API key"`<br>`]}`|Use the correct type of API key. Refer to [API keys](/#api-keys) for more information|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can't send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Refer to [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode) for more information|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](/#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](/#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](/#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|

## Send a letter

When your service first signs up to GOV.UK Notify, you’ll start in trial mode. You can only send letters in live mode.

### Method

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "address_line_1", "23 Foo Road" },  # required
    { "address_line_2", "Bar Town" }, # required
    { "address_line_3", "London" },
    { "postcode", "BAX S1P" } # required
      ... # any other optional address lines, or personalisation fields found in your template
};

LetterNotificationResponse response = client.SendLetter(templateId, personalisation, reference);
```

### Arguments

#### templateId (required)

The ID of the template. You can find this by logging into GOV.UK Notify and going to the __Templates__ page.

#### personalisation (required)

The personalisation argument always contains the following required parameters for the letter recipient's address:

- `address_line_1`
- `address_line_2`
- `postcode`

Any other placeholder fields included in the letter template also count as required parameters. You need to provide their values in a dictionary with key value pairs. For example:

_QP: How do you provide the information?_

_example?_

#### "yourReferenceString" (optional)

A unique identifier. This reference can identify a single unique notification or a batch of multiple notifications.

```python
reference=’STRING’ # optional string - identifies notification(s)
```

#### personalisation (optional)

The following parameters in the letter recipient's address are optional:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
{ "address_line_1", "23 Foo Road" }, # required
{ "address_line_2", "Bar Town" }, # required
{ "address_line_3", "London" },
{ "postcode", "BAX S1P" } # required
... # any other optional address lines, or personalisation fields found in your template
};
```
Otherwise the parameter can be omitted or `null` can be passed in its place.

### Response

If the request to the client is successful, you will receive the following `LetterNotificationResponse` response:

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

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`]}`|Use the correct type of API key. Refer to [API keys](/#api-keys) for more information|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|Refer to [trial mode](https://www.notifications.service.gov.uk/features/using-notify#trial-mode) for more information|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|Ensure that your template has a field for the first line of the address, check [personlisation](/#send-a-letter-required-arguments-personalisation) for more information.|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the API key. Refer to [API keys](/#api-keys) for more information|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM/TEST/LIVE of 3000 requests per 60 seconds"`<br>`}]`|Refer to [API rate limits](/#api-rate-limits) for more information|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (LIMIT NUMBER) for today"`<br>`}]`|Refer to [service limits](/#service-limits) for the limit number|
|`500`|`[{`<br>`"error": "Exception",`<br>`"message": "Internal server error"`<br>`}]`|Notify was unable to process the request, resend your notification.|


# Get message status

The possible status of a message depends on the message type.

## Status - text and email

_QP: Should I add created and sent for text and email and text respectively?_

### Sending

The message is queued to be sent by the provider.

### Delivered

The message was successfully delivered.

### Failed

This covers all failure statuses:

- permanent-failure - "The provider was unable to deliver message, email or phone number does not exist; remove this recipient from your list"
- temporary-failure - "The provider was unable to deliver message, email inbox was full or phone was turned off; you can try to send the message again"
- technical-failure - "Notify had a technical failure; you can try to send the message again"

## Status - letter

### Failed

The only failure status that applies to letters is `technical-failure` - Notify had an unexpected error while sending to our printing provider.

### Accepted

Notify is printing and posting the letter.

## Get the status of one message

### Method

```csharp
Notification notification = client.GetNotificationById(notificationId);
```

### Arguments

#### notificationId (required)

The ID of the notification.

### Response

If the request to the client is successful, you will receive the following `Notification` response:

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

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|Check the notification ID|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Check your API key, refer to [API keys](/#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the notification ID|


## Get the status of all messages

This API call returns the status of all messages. You can either get the status of all messages in one call, or one page of up to 250 messages.

### Method

#### All messages

This will return all your messages with statuses. They will be displayed in pages of up to 250 messages each.

_?_

You can filter the returned messages by including the following optional arguments in the method:

- [`notificationType`](/#template-type)
- [`status`](/#status)
- [`reference`](/#get-the-status-of-all-messages-optional-arguments-reference)
- [`olderThanId`](/#older-than)


#### One page of up to 250 messages

_QP: Does this apply?_

This will return one page of up to 250 messages and statuses. You can get either the most recent messages, or get older messages by specifying a particular notification ID in the [`older_than`](/#older-than) argument.

##### Most recent messages

_?_

You must set the [`status`](/#status) argument to `sending`.

##### Older messages

To get older messages:

1. Get the ID of an older notification.
1. Add the following code to your application, with the older notification ID in the [`older_than`](/#older-than) argument.

```python
response = get_all_notifications_iterator(status="sending",older_than="NOTIFICATION ID")
```

You must set the [`status`](/#status) argument to `sending`.

This method will return the next oldest messages from the specified notification ID.

### Arguments

You can omit any of these arguments to ignore these filters.

#### notificationType (optional)

You can filter by:

* `email`
* `sms`
* `letter`

#### status (optional)

| status | description | text | email | letter |
|:--- |:--- |:--- |:--- |:--- |
|`sending` |The message is queued to be sent by the provider|Yes|Yes||
|`delivered`|The message was successfully delivered|Yes|Yes||
|`failed`|This will return all failure statuses:<br>- `permanent-failure`<br>- `temporary-failure`<br>- `technical-failure`|Yes|Yes||
|`permanent-failure`|The provider was unable to deliver message, email or phone number does not exist; remove this recipient from your list|Yes|Yes||
|`temporary-failure`|The provider was unable to deliver message, email inbox was full or phone was turned off; you can try to send the message again|Yes|Yes||
|`technical-failure`|Email / Text: Notify had a technical failure; you can try to send the message again. <br><br>Letter: Notify had an unexpected error while sending to our printing provider. <br><br>You can omit this argument to ignore this filter.|Yes|Yes||
|`accepted`|Notify is printing and posting the letter|||Yes|

#### reference (optional)

A unique identifier. This reference can identify a single unique notification or a batch of multiple notifications.

_example?_

#### olderThanId (optional)

Input the ID of a notification into this argument. If you use this argument, the next 250 received notifications older than the given ID are returned.

_example?_

If this argument is omitted, the most recent 250 notifications are returned.

### Response

If the request to the client is successful, you will receive a `dict` response.

#### All messages

_?_

#### One page of up to 250 messages

_QP: Does this apply?_

```python
<generator object NotificationsAPIClient.get_all_notifications_iterator at 0x1026c7410>
```

### Error codes

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "bad status is not one of [created, sending, delivered, pending, failed, technical-failure, temporary-failure, permanent-failure]"`<br>`}]`|Contact the Notify team|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Apple is not one of [sms, email, letter]"`<br>`}]`|Contact the Notify team|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Check your API key, refer to [API keys](/#api-keys) for more information|


# Get a template

## Get a template by ID

### Method

This will return the latest version of the template.

```csharp
TemplateResponse response = client.GetTemplateById(
  "templateId"
  );
```

### Arguments

#### templateId (required)

The ID of the template. You can find this by logging into GOV.UK Notify and going to the __Templates__ page.

### Response

If the request to the client is successful, you will receive a `dict` response.

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

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](/#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](/#arguments-template-id)|


## Get a template by ID and version

### Method

This will return the latest version of the template.

```csharp
TemplateResponse response = client.GetTemplateByIdAndVersion(
    'templateId',
    1   // integer required for version number
);
```

### Arguments

#### templateId (required)

The ID of the template. You can find this by logging into GOV.UK Notify and going to the __Templates__ page.

#### version (required)

The version number of the template.

### Response

If the request to the client is successful, you will receive a `dict` response.

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

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](/#api-keys) for more information|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No Result Found"`<br>`}]`|Check your [template ID](/#get-a-template-by-id-and-version-required-arguments-template-id) and [version](/#version)|


## Get all templates

### Method

This will return the latest version of all templates.

```csharp
TemplateList response = client.GetAllTemplates(
    "sms" | "email" | "letter" // optional
);
```

### Arguments

#### templateType (optional)

If omitted all templates are returned. Otherwise you can filter by:

- `email`
- `sms`
- `letter`

### Response

If the request to the client is successful, you will receive a `dict` response.

```csharp
List<TemplateResponse> templates;
```

If no templates exist for a template type or there no templates for a service, the templates list will be empty:

```csharp
List<TemplateResponse> templates; // empty list of templates
```

## Generate a preview template

### Method

This will generate a preview version of a template.

```csharp
TemplatePreviewResponse response = client.GenerateTemplatePreview(
    'templateId',
    personalisation
);
```


The parameters in the personalisation argument must match the placeholder fields in the actual template. The API notification client will ignore any extra fields in the method.

### Arguments

#### templateId (required)

The ID of the template. You can find this by logging into GOV.UK Notify and going to the __Templates__ page.

#### personalisation (required)

If a template has placeholder fields for personalised information such as name or reference number, you need to provide their values, for example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};
```

### Response

If the request to the client is successful, you will receive a `dict` response.

```csharp
public String id;
public String type;
public int version;
public String body;
public String subject; // null if a sms message
```


### Error codes

If the request is not successful, the client will raise an `HTTPError`:

|`error.status_code`|`error.message`|How to fix|
|:---|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Missing personalisation: [PERSONALISATION FIELD]"`<br>`}]`|Check that the personalisation arguments in the method match the placeholder fields in the template|
|`400`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|Check the [template ID](/#generate-a-preview-template-required-arguments-template-id)|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Error: Your system clock must be accurate to within 30 seconds"`<br>`}]`|Check your system clock|
|`403`|`[{`<br>`"error": "AuthError",`<br>`"message": "Invalid token: signature, api token not found"`<br>`}]`|Use the correct API key. Refer to [API keys](/#api-keys) for more information|


# Get received text messages

This API call returns received text messages. Depending on which method you use, you can either get all received text messages, or a page of up to 250 text messages.

## Get all received text messages

This method will return a `ReceivedTextMessageList` with all received text messages.

### Method

```csharp
ReceivedTextListResponse response = client.GetReceivedTexts(olderThanId);
```

### Arguments

#### olderThanId (optional)

Input the ID of a received text message into this argument. If you use this argument, the next 250 received text messages older than the given ID are returned.

_EXAMPLE_

If this argument is omitted, the most recent 250 text messages are returned.

### Response

If the request to the client is successful, you will receive a `ReceivedTextMessageList` response that will return all received texts.

```csharp
public List<ReceivedText> receivedTextList;
public Link links;

public class Link {
	public String current;
	public String next;
}

```

A `ReceivedText` will have the following properties -

```csharp
public String id;
public String userNumber;
public String createdAt;
public String serviceId;
public String notifyNumber;
public String content;

```

## Get one page of received text messages

_DOES THIS APPLY?_

This will return one page of up to 250 text messages.  

### Method

```python
response = client.get_received_texts(older_than)
```

You can specify which texts to receive by inputting the ID of a received text message into the [`older_than`](/#get-one-page-of-received-text-messages-optional-arguments-older-than) argument.

### Arguments

#### olderThanId (optional)

Input the ID of a received text message into this argument. If you use this argument, the next 250 received text messages older than the given ID are returned.

```python
older_than=’740e5834-3a29-46b4-9a6f-16142fde533a’ # optional string - notification ID
```

If this argument is omitted, the most recent 250 text messages are returned.

### Response

If the request to the client is successful, you will receive a `dict` response.


```python
{
  "received_text_messages":
  [
    {
      "id": "STRING", # required string - ID of received text message
      "user_number": "STRING", # required string
      "notify_number": "STRING", # required string - receiving number
      "created_at": "STRING", # required string - date and time template created
      "service_id": "STRING", # required string - service ID
      "content": "STRING" # required string - text content
    },
    …
  ],
  "links": {
    "current": "/received-text-messages",
    "next": "/received-text-messages?other_than=last_id_in_list"
  }
}
```
