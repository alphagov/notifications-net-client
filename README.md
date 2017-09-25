# GOV.UK Notify .NET client

## Installation

### Nuget Package Manager

The notifications-net-client is deployed to [Bintray](https://bintray.com/gov-uk-notify/nuget/Notify). Navigate to your project directory and install Notify with the following command
```
nuget install Notify -Source https://api.bintray.com/nuget/gov-uk-notify/notifications-net-client
```

Alternatively if you are using the Nuget Package Manager in Visual Studio, add the source below to install.
```
https://api.bintray.com/nuget/gov-uk-notify/nuget
```

## Development
### Visual Studio (Windows)

To execute the NUnit tests you will need to install the [NUnit3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter) extension to Visual Studio.

<details>
<summary>Setting Windows Environment variables</summary>

```
SETX NOTIFY_API_URL "https://example.notify-api.url"
SETX API_KEY "example_API_test_key"
SETX FUNCTIONAL_TEST_NUMBER "valid mobile number"
SETX FUNCTIONAL_TEST_EMAIL "valid email address"
SETX EMAIL_TEMPLATE_ID "valid email_template_id"
SETX SMS_TEMPLATE_ID "valid sms_template_id"
SETX LETTER_TEMPLATE_ID "valid letter_template_id"
```
</details>

### [Visual Studio](https://www.visualstudio.com/vs/visual-studio-mac/) (Mac OS)

In order to get the .Net client running in Xamarin the target `.Net Framework` needs to be set to `4.5.2` and the application needs to be run from the terminal

```
open -n /Applications/"Visual Studio.app"
```

<details>
<summary>Setting Mac OS Environment variables (these must be sourced before opening the Visual Application using the command above)</summary>

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

Generate an API key by signing in to
[GOV.UK Notify](https://www.notifications.service.gov.uk) and going to
the **API integration** page.

## Send messages

### Text message

```csharp
SmsNotificationResponse response = client.SendSms(mobileNumber, templateId, personalisation, reference);
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `SmsNotificationResponse `:

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
<table>
<thead>
<tr>
<th>

`error["status_code"]`</th>
<th>

`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "RateLimitError",
    "message": "Exceeded rate limit for key type TEAM of 10 requests per 10 seconds"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "TooManyRequestsError",
    "message": "Exceeded send limits (50) for today"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Can"t send to this recipient using a team-only API key"
]}
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Can"t send to this recipient when service is in trial mode
                - see https://www.notifications.service.gov.uk/trial-mode"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>

### Email

```csharp
EmailNotificationResponse response = client.SendEmail(emailAddress, templateId, personalisation, reference);
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be an `EmailNotificationResponse `:

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:
<table>
<thead>
<tr>
<th>

`error["status_code"]`</th>
<th>

`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "RateLimitError",
    "message": "Exceeded rate limit for key type TEAM of 10 requests per 10 seconds"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "TooManyRequestsError",
    "message": "Exceeded send limits (50) for today"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Can"t send to this recipient using a team-only API key"
]}
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Can"t send to this recipient when service is in trial mode
                - see https://www.notifications.service.gov.uk/trial-mode"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>


### Arguments


#### `templateId`

Find by clicking **API info** for the template you want to send.

#### `personalisation`

If a template has placeholders you need to provide their values. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```

Otherwise the parameter can be omitted or `null` can be passed in its place.

#### `reference`

An optional identifier you generate if you don't want to use Notify's `id`. It can be used to identify a single  notification or a batch of notifications. Otherwise the parameter can be omitted or `null` can be passed in its place.

### Letter

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

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be an `LetterNotificationResponse `:

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:
<table>
<thead>
<tr>
<th>

`error["status_code"]`</th>
<th>

`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "RateLimitError",
    "message": "Exceeded rate limit for key type live of 10 requests per 20 seconds"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>429</pre>
</td>
<td>
<pre>
[{
    "error": "TooManyRequestsError",
    "message": "Exceeded send limits (50) for today"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Cannot send letters with a team api key"
]}
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Cannot send letters when service is in trial mode"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "ValidationError",
    "message": "personalisation address_line_1 is a required property"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>


### Arguments


#### `templateId`

Find by clicking **API info** for the template you want to send.

#### `personalisation`
	
If a template has placeholders you need to provide their values. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "Foo" }
};
```

Otherwise the parameter can be omitted or `null` can be passed in its place.

#### `reference`

An optional identifier you generate if you don't want to use Notify's `id`. It can be used to identify a single  notification or a batch of notifications. Otherwise the parameter can be omitted or `null` can be passed in its place.

## Get the status of one message
```csharp
Notification notification = client.GetNotificationById(notificationId);
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `Notification`:

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

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:
<table>
<thead>
<tr>
<th>`error["status_code"]`</th>
<th>`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>404</pre>
</td>
<td>
<pre>
[{
    "error": "NoResultFound",
    "message": "No result found"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "ValidationError",
    "message": "id is not a valid UUID"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>

## Get the status of all messages
```csharp
NotificationList notifications = client.GetNotifications(templateType, status, reference, olderThanId);
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `NotificationList`:

```csharp
public List<Notification> notifications;
public Link links;

public class Link {
	public String current;
	public String next;
}

```

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:
<table>
<thead>
<tr>
<th>`error["status_code"]`</th>
<th>`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    'error': 'ValidationError',
    'message': 'bad status is not one of [created, sending, delivered, pending, failed, technical-failure, temporary-failure, permanent-failure]'
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "ValidationError",
    "message": "Apple is not one of [sms, email, letter]"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>

### Arguments

#### `templateType`

If omitted all messages are returned. Otherwise you can filter by:

* `email`
* `sms`
* `letter`


#### `status`

If omitted all messages are returned. Otherwise you can filter by:

##### email

You can filter by:

* `sending` - the message is queued to be sent by the provider.
* `delivered` - the message was successfully delivered.
* `failed` - this will return all failure statuses `permanent-failure`, `temporary-failure` and `technical-failure`.
* `permanent-failure` - the provider was unable to deliver message, email does not exist; remove this recipient from your list.
* `temporary-failure` - the provider was unable to deliver message, email box was full; you can try to send the message again.
* `technical-failure` - Notify had a technical failure; you can try to send the message again.

You can omit this argument to ignore this filter.

##### text message

You can filter by:

* `sending` - the message is queued to be sent by the provider.
* `delivered` - the message was successfully delivered.
* `failed` - this will return all failure statuses `permanent-failure`, `temporary-failure` and `technical-failure`.
* `permanent-failure` - the provider was unable to deliver message, phone number does not exist; remove this recipient from your list.
* `temporary-failure` - the provider was unable to deliver message, the phone was turned off; you can try to send the message again.
* `technical-failure` - Notify had a technical failure; you can try to send the message again.

You can omit this argument to ignore this filter.

##### letter

You can filter by:

* `accepted` - the letter has been generated.
* `technical-failure` - Notify had an unexpected error while sending to our printing provider

You can omit this argument to ignore this filter.

#### `reference`


This is the `reference` you gave at the time of sending the notification. This can be omitted to ignore the filter.

#### `olderThanId`

If omitted all messages are returned. Otherwise you can filter to retrieve all notifications older than the given notification `id`.

## Get a template by ID

_This will return the latest version of the template. Use [get_template_version](#get-a-template-by-id-and-version) to retrieve a specific template version_

```csharp
TemplateResponse response = client.GetTemplateById(
    "templateId"
)
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `TemplateResponse`:

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
<table>
<thead>
<tr>
<th>

`error["status_code"]`</th>
<th>

`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>404</pre>
</td>
<td>
<pre>
[{
    "error": "NoResultFound",
    "message": "No result found"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>

### Arguments

#### `templateId`

Find by clicking **API info** for the template you want to send.

## Get a template by ID and version

```csharp
TemplateResponse response = client.GetTemplateByIdAndVersion(
    'templateId',
    1   // integer required for version number
)
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `TemplateResponse`:

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
<table>
<thead>
<tr>
<th>`error["status_code"]`</th>
<th>`error["message"]`</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>404</pre>
</td>
<td>
<pre>
[{
    "error": "NoResultFound",
    "message": "No result found"
}]
</pre>
</td>
</tr>
</tbody>
</table>
</details>

### Arguments

#### `templateId`

Find by clicking **API info** for the template you want to send.

#### `version`

The version number of the template


## Get all templates

```csharp
TemplateList response = client.GetAllTemplates(
    "sms" | "email" | "letter" // optional
)
```
_This will return the latest version for each template_

[See available template types](#templatetype)

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `TemplateList`:

```csharp
List<TemplateResponse> templates;
```

If no templates exist for a template type or there no templates for a service, the `response` will be a `TemplateList` with an empty `templates` list element:

```csharp
List<TemplateResponse> templates; // empty list of templates
```

</details>

### Arguments


#### `templateType`

If omitted all messages are returned. Otherwise you can filter by:

* `email`
* `sms`
* `letter`

## Generate a preview template

```csharp
TemplatePreviewResponse response = client.GenerateTemplatePreview(
    'templateId', 
    personalisation
)
```

<details>
<summary>
Response
</summary>

If the request is successful, `response` will be a `TemplatePreviewResponse`:

```csharp
public String id;
public String type;
public int version;
public String body;
public String subject; // null if a sms message
```

Otherwise the client will raise a `Notify.Exceptions.NotifyClientException`:
<table>
<thead>
<tr>
<th>

`error["status_code"]`
</th>
<th>

`error["message"]`
</th>
</tr>
</thead>
<tbody>
<tr>
<td>
<pre>400</pre>
</td>
<td>
<pre>
[{
    "error": "BadRequestError",
    "message": "Missing personalisation: [name]"
}]
</pre>
</td>
</tr>
<tr>
<td>
<pre>404</pre>
</td>
<td>
<pre>
[{
    "error": "NoResultFound",
    "message": "No result found"
]}
</pre>
</td>
</tr>
</tbody>
</table>
</details>

### Arguments


#### `templateId`

Find by clicking **API info** for the template you want to send.

#### `personalisation`

If a template has placeholders you need to provide their values. For example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};
```
