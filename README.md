# GOV.UK Notify .NET client

This documentation is for developers interested in using this .NET client to integrate their government service with GOV.UK Notify.

## Table of Contents

* [Installation](#installation)
* [Getting started](#getting-started)
* [Send messages](#send-messages)
* [Get the status of one message](#get-the-status-of-one-message)
* [Get the status of all messages](#get-the-status-of-all-messages)
* [Get a template by ID](#get-a-template-by-id)
* [Get a template by ID and version](#get-a-template-by-id-and-version)
* [Get all templates](#get-all-templates)
* [Generate a preview template](#generate-a-preview-template)
* [Get all received text messages](#get-all-received-text-messages)

## Dependency

Since Notify version 2.0.0 the .Net frameworks supported will be 4.6.2 and .Net Core 2.0, non windows OS only support .Net Core.
It is recommended that you upgrade your .Net Framework in order to fix security vulnerabilities.

## Installation

### Nuget Package Manager

The notifications-net-client is deployed to [Bintray](https://bintray.com/gov-uk-notify/nuget/Notify).
<details>
<summary>
Click here to expand for more information.
</summary>

Navigate to your project directory and install Notify with the following command:
```
nuget install Notify -Source https://api.bintray.com/nuget/gov-uk-notify/notifications-net-client
```

Alternatively if you are using the Nuget Package Manager in Visual Studio, add the source below to install:
```
https://api.bintray.com/nuget/gov-uk-notify/nuget
```
To add a new source to the Nuget Package Manager in Visual Studio - https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui#package-sources

If you are referencing this package from a CI tool you may need to add the bintray package source to your nuget configuration. The easiest way to achieve this is by adding a `nuget.config` file in the same folder as your `.sln` with the following content:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="$\..\packages" />
  </config>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="bintray" value="https://api.bintray.com/nuget/gov-uk-notify/nuget" />
  </packageSources>
</configuration>
```

</details>

### [Visual Studio](https://www.visualstudio.com/) (Windows)

To execute the NUnit tests you will need to install the [NUnit3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter) extension to Visual Studio or via the [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter/) nuget package

<details>
<summary>Click here to expand for more information.</summary>

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
</details>

### [Visual Studio](https://www.visualstudio.com/vs/visual-studio-mac/) (Mac OS)

In order to get the .Net client running in Visual Studio the target framework needs to be set to `.Net standard 2.0` and the application needs to be run from the terminal.

<details>
<summary>Click here to expand for more information.</summary>

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
</details>

## Getting started

```csharp
using Notify.Client;
using Notify.Models;
using Notify.Models.Responses;

NotificationClient client = new NotificationClient(apiKey);
```

Generate an API key by signing in to [GOV.UK Notify](https://www.notifications.service.gov.uk) and going to the **API integration** page.

## Send messages

### Text message

#### Method

If the request is successful, `response` will be a `SmsNotificationResponse `.

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
SmsNotificationResponse response = client.SendSms(mobileNumber, templateId, personalisation, reference, smsSenderId);
```
</details>

#### Response

<details>
<summary>
Click here to expand for more information.
</summary>

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:

|`error.status_code`|`error.message`|
|:---|:---|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM of 10 requests per 10 seconds"`<br>`}]`|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (50) for today"`<br>`}]`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can"t send to this recipient using a team-only API key"`<br>`]}`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can"t send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>


##### `mobileNumber`

The phone number of the recipient, only required for sms notifications.

##### `templateId`

Find by clicking **API info** for the template you want to send.

##### `reference`

An optional identifier you generate. The `reference` can be used as a unique reference for the notification. Because Notify does not require this reference to be unique you could also use this reference to identify a batch or group of notifications.

You can omit this argument if you do not require a reference for the notification.

##### `personalisation`

If a template has placeholders, you need to provide their values, for example:

```net
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```

### `sms_sender_id`

Optional. Specifies the identifier of the sms sender to set for the notification. The identifiers are found in your service Settings, when you 'Manage' your 'Text message sender'.

If you omit this argument your default sms sender will be set for the notification.

</details>


### Email

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
EmailNotificationResponse response = client.SendEmail(emailAddress, templateId, personalisation, reference, emailReplyToId);
```

</details>


#### Response

If the request is successful, `response` will be an `EmailNotificationResponse `.

<details>
<summary>
Click here to expand for more information.
</summary>

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`.

|`error.status_code`|`error.message`|
|:---|:---|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM of 10 requests per 10 seconds"`<br>`}]`|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (50) for today"`<br>`}]`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can"t send to this recipient using a team-only API key"`<br>`]}`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Can"t send to this recipient when service is in trial mode - see https://www.notifications.service.gov.uk/trial-mode"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `emailAddress`

The email address of the recipient, only required for email notifications.

##### `templateId`

Find by clicking **API info** for the template you want to send.

##### `personalisation`

If a template has placeholders you need to provide their values. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```
Otherwise the parameter can be omitted or `null` can be passed in its place.

##### `reference`

An optional identifier you generate. The reference can be used as a unique reference for the notification. Because Notify does not require this reference to be unique you could also use this reference to identify a batch or group of notifications.

You can omit this argument if you do not require a reference for the notification.

##### `emailReplyToId`

Optional. Specifies the identifier of the email reply-to address to set for the notification. The identifiers are found in your service Settings, when you 'Manage' your 'Email reply to addresses'.

If you omit this argument your default email reply-to address will be set for the notification.

</details>


### Letter

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

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

</details>


#### Response

If the request is successful, `response` will be an `LetterNotificationResponse`.
<details>
<summary>
Click here to expand for more information.
</summary>

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
Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`.

|`error.status_code`|`error.message`|
|:---|:---|
|`429`|`[{`<br>`"error": "RateLimitError",`<br>`"message": "Exceeded rate limit for key type TEAM of 10 requests per 20 seconds"`<br>`}]`|
|`429`|`[{`<br>`"error": "TooManyRequestsError",`<br>`"message": "Exceeded send limits (50) for today"`<br>`}]`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters with a team api key"`<br>`}]`|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Cannot send letters when service is in trial mode"`<br>`}]`|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "personalisation address_line_1 is a required property"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `templateId`

Find by clicking **API info** for the template you want to send.

##### `personalisation`

If a template has placeholders you need to provide their values. For example:

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

##### `reference`

An optional identifier you generate. The reference can be used as a unique reference for the notification. Because Notify does not require this reference to be unique you could also use this reference to identify a batch or group of notifications.

You can omit this argument if you do not require a reference for the notification.

</details>


## Get the status of one message

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
Notification notification = client.GetNotificationById(notificationId);
```

</details>


#### Response

If the request is successful, `response` will be a `Notification`.
<details>
<summary>
Click here to expand for more information.
</summary>

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`.

|`error.status_code`|`error.message`|
|:---|:---|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "id is not a valid UUID"`<br>`}]`|

</details>

#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `notificationId`

The ID of the notification.

</details>

## Get the status of all messages

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
NotificationList notifications = client.GetNotifications(templateType, status, reference, olderThanId);
```
</details>


#### Response

If the request is successful, `response` will be a `NotificationList`.
<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
public List<Notification> notifications;
public Link links;

public class Link {
	public String current;
	public String next;
}

```

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:

|`status_code`|`message`|
|:---|:---|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "bad status is not one of [created, sending, delivered, pending, failed, technical-failure, temporary-failure, permanent-failure]"`<br>`}]`|
|`400`|`[{`<br>`"error": "ValidationError",`<br>`"message": "Apple is not one of [sms, email, letter]"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>


##### `templateType`

If omitted all messages are returned. Otherwise you can filter by:

* `email`
* `sms`
* `letter`

##### `status`

If omitted all messages are returned. Otherwise you can filter by:

__email__

You can filter by:

* `sending` - the message is queued to be sent by the provider.
* `delivered` - the message was successfully delivered.
* `failed` - this will return all failure statuses `permanent-failure`, `temporary-failure` and `technical-failure`.
* `permanent-failure` - the provider was unable to deliver message, email does not exist; remove this recipient from your list.
* `temporary-failure` - the provider was unable to deliver message, email box was full; you can try to send the message again.
* `technical-failure` - Notify had a technical failure; you can try to send the message again.

You can omit this argument to ignore this filter.

__text message__

You can filter by:

* `sending` - the message is queued to be sent by the provider.
* `delivered` - the message was successfully delivered.
* `failed` - this will return all failure statuses `permanent-failure`, `temporary-failure` and `technical-failure`.
* `permanent-failure` - the provider was unable to deliver message, phone number does not exist; remove this recipient from your list.
* `temporary-failure` - the provider was unable to deliver message, the phone was turned off; you can try to send the message again.
* `technical-failure` - Notify had a technical failure; you can try to send the message again.

You can omit this argument to ignore this filter.

__letter__

You can filter by:

* `accepted` - the letter has been generated.
* `technical-failure` - Notify had an unexpected error while sending to our printing provider

You can omit this argument to ignore this filter.

##### `reference`

An optional identifier you generate. The reference can be used as a unique reference for the notification. Because Notify does not require this reference to be unique you could also use this reference to identify a batch or group of notifications.

You can omit this argument if you do not require a reference for the notification.

##### `olderThanId`

If omitted all messages are returned. Otherwise you can filter to retrieve all notifications older than the given notification `id`.

</details>

## Get a template by ID

#### Method

This will return the latest version of the template. Use [get_template_version](#get-a-template-by-id-and-version) to retrieve a specific template version.

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
TemplateResponse response = client.GetTemplateById(
    "templateId"
)
```
</details>


#### Response

If the request is successful, `response` will be a `TemplateResponse`.

<details>
<summary>
Click here to expand for more information.
</summary>

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`.

|`status_code`|`message`|
|:---|:---|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `templateId`

Find by clicking **API info** for the template you want to send.

</details>


## Get a template by ID and version

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
TemplateResponse response = client.GetTemplateByIdAndVersion(
    'templateId',
    1   // integer required for version number
)
```

</details>


#### Response

If the request is successful, `response` will be a `TemplateResponse`.
<details>
<summary>
Click here to expand for more information.
</summary>

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:

|`error["status_code"]`|`error["message"]`|
|:---|:---|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `templateId`

Find by clicking **API info** for the template you want to send.

##### `version`

The version number of the template

</details>

## Get all templates

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
TemplateList response = client.GetAllTemplates(
    "sms" | "email" | "letter" // optional
)
```
This will return the latest version for each template.

[See available template types](#templatetype)

</details>


#### Response

If the request is successful, `response` will be a `TemplateList`.
<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
List<TemplateResponse> templates;
```

If no templates exist for a template type or there no templates for a service, the `response` will be a `TemplateList` with an empty `templates` list element:

```csharp
List<TemplateResponse> templates; // empty list of templates
```

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `templateType`

If omitted all messages are returned. Otherwise you can filter by:

* `email`
* `sms`
* `letter`

</details>


## Generate a preview template

#### Method

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
TemplatePreviewResponse response = client.GenerateTemplatePreview(
    'templateId',
    personalisation
)
```

</details>


#### Response

If the request is successful, `response` will be a `TemplatePreviewResponse`.

<details>
<summary>
Click here to expand for more information.
</summary>

```csharp
public String id;
public String type;
public int version;
public String body;
public String subject; // null if a sms message
```

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:

|`error["status_code"]`|`error["message"]`|
|:---|:---|
|`400`|`[{`<br>`"error": "BadRequestError",`<br>`"message": "Missing personalisation: [name]"`<br>`}]`|
|`404`|`[{`<br>`"error": "NoResultFound",`<br>`"message": "No result found"`<br>`}]`|

</details>


#### Arguments

<details>
<summary>
Click here to expand for more information.
</summary>

##### `templateId`

Find by clicking **API info** for the template you want to send.

##### `personalisation`

If a template has placeholders you need to provide their values. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};
```

</details>

## Get all received text messages

```csharp
ReceivedTextListResponse response = client.GetReceivedTexts(olderThanId);
```

#### Response

<details>
<summary>
Click here to expand for more information.
</summary>

If the request is successful, `response` will be a `ReceivedTextListResponse`:

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
</details>

#### Arguments
<details>
<summary>
Click here to expand for more information.
</summary>

##### `olderThanId`

If omitted all messages are returned. Otherwise you can filter to retrieve all received text messages older than the given id.

</details>
