## [4.0.1] - 2021-04-23

* Fixed a build warning that version 7.0 of the JWT package does not exist
* Bump System.Net.Http packagefrom 4.3.3 to 4.3.4

## [4.0.0] - 2021-02-08

* Fixed an issue when importing the JWT dependency by pinning the version to 7.x. It is not possible to support both version 6.x and 7.x of JWT because version 7.0 introduced a breaking change.

## [3.0.0] - 2021-01-27

* Upgrade the JWT dependency. Version 6.x or 7.x of the JWT dependency is now required.

## [2.9.0] - 2020-07-13

* Add support for a `letter_contact_block` attribute in the `TemplateResponse` schema. This will allow our users to check contact blocks for their letter templates.

## [2.8.0] - 2020-06-01

* Add support for an optional `isCsv` parameter in the `PrepareUpload()` function. This fixes a bug when sending a CSV file by email. This ensures that the file is downloaded as a CSV rather than a TXT file.

## [2.7.1] - 2020-01-27

* change error message to refer to file rather than document

## [2.7.0] - 2019-12-31

* Added `NotificationClient.getPdfForLetter` function
  - accepts `string notificationId`
  - returns a `byte[]` containing the final printable PDF for a precompiled or templated letter

## [2.6.0] - 2019-12-20

** Add an optional `includeSpreadsheetUploads` argument to `GetNotifications`.

## [2.5.2] - 2019-11-18

* Applied fix for issue with synchronous version of NotificationClient API

## [2.5.1] - 2019-04-26

* Update JWT version to the latest

## [2.5.0] - 2019-02-14

* Implement asynchronous versions of the NotificationClient methods

## [2.4.0] - 2019-02-11

* Add an optional `postage` argument to `SendPrecompiledLetter`.
* Add `postage` attribute to `LetterNotificationResponse` model.
* Add `postage` attribute to `Notification` model.

## [2.3.0] - 2018-10-02

* Implement the `INotificationClient` interface to make mocking easier (see https://github.com/alphagov/notifications-net-client/pull/57)

## [2.2.0] - 2018-09-13

* Add `NotificationClient.SendPrecompiledLetter` method.
* Add support for document uploads using `NotificationClient.PrepareUpload`
* Fixed `NotificationResponse.Equals` and `LetterNotificationResponse.Equals` for instances with `.template` and `.content` attributes set to `null` in order to support pre-compiled letter responses.

## [2.1.0] - 2018-08-14

* The Notification class has a new `createdByName` property.
    * If the notification was sent manually this will be the name of the sender. If the notification was sent through the API this will be `null`.

## [2.0.1] - 2018-03-29

* Add `pending-virus-check` and `virus-scan-failed` to statuses

## [2.0.0] - 2018-02-09

* Migrate to .Net core 2.0.0 and .Net framework 4.6.2

## [1.6.0] - 2017-11-15
## Changed

* Update to `NotificationsClient.SendSms`
    * added `smsSenderId`: an optional smsSenderId specified when adding a text message sender under service settings, if this is not provided it will default to the service name.
* Added `GetReceivedTexts` - retrieves all received text messages, links provided with page size of 250
* Added `Makefile` in order to run build, tests and nuget package from the terminal.

## [1.5.3] - 2017-11-15
## Changed

* Pin dependencies for JWT and Newtonsoft.json
    * Pinned to no higher than JWT 2.4.2 and Newtonsoft.json 10.

## [1.5.2] - 2017-10-11
## Changed

* Update to `NotificationsClient.send_email_notification`
    * added `emailReplyToId`: an optional emailReplyToId specified when adding Email reply to addresses under service settings, if this is not provided the reply to email will be the service default reply to email. `email_reply_to_id` can be omitted.

## [1.5.1] - 2017-09-22
## Changed

* Make personalisation non-null in SendLetter

## [1.5.0] - 2017-09-22
## Changed

* Add new method for sending letters:
    * `SendLetter` - send a letter

## [1.4.0] - 2017-08-30
## Changed

* Add template name to `TemplateResponse` model

## [1.3.0] - 2017-08-09
## Changed

* Update integration tests to support letter templates:

## [1.2.0] - 2017-05-11
## Changed

* Added new methods for managing templates:
    * `GetTemplateById` - retrieve a single template
    * `GetTemplateByIdAndVersion` - retrieve a specific version for a desired template
    * `GetAllTemplates` - retrieve all templates (can filter by type)
    * `GenerateTemplatePreview` - preview a template with personalisation applied

* Refactored MSTest tests to NUnit tests.
    * This allows for wider compatibility with a variety of IDEs.

## [1.1.0] - 2016-12-16
### Changed

* Update to `Client.GetNotifications()`
    * Notifications can now be filtered by `reference` and `olderThanId`, see the README for details.
    * Updated method signature:

 ```csharp
client.GetNotifications(String templateType = "", String status = "", String reference = "", String olderThanId = "")
```
     * Each one of these parameters can be `null`

# Prior versions

Changelog not recorded - please see pull requests on github.
