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

## Send a message

Text message:

```csharp
SmsNotificationResponse response = client.SendSms(mobileNumber, templateId);
```

Email:

```csharp
EmailNotificationResponse response = client.SendEmail(emailAddress, templateId);
```

Find `templateId` by clicking **API info** for the template you want to send.

If a template has placeholders, you need to provide their values in `personalisation`, for example:

```csharp
Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
{
    { "name", "someone" }
};

EmailNotificationResponse response = client.SendEmail(emailAddress, emailTemplateId, personalisation);
```

## Get the status of one message

```csharp
Notification notification = client.GetNotificationById(notificationId);
```
 
## Get the status of all messages

```csharp
NotificationList notifications = client.GetNotifications(templateType, status);
```

Optional `status` can be one of:

* `null` (returns all messages)
* `created`
* `sending`
* `delivered`
* `failed`

Optional `notificationType` can be one of:

* `null` (returns all messages)
* `email`
* `sms`

