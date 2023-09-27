using Notify.Models;
using Notify.Models.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Interfaces
{
    public interface IAsyncNotificationClient : IBaseClient
    {
        Task<TemplatePreviewResponse> GenerateTemplatePreviewAsync(string templateId, Dictionary<string, dynamic> personalisation = null, CancellationToken cancellationToken = default);

        Task<TemplateList> GetAllTemplatesAsync(string templateType = "", CancellationToken cancellationToken = default);

        Task<Notification> GetNotificationByIdAsync(string notificationId, CancellationToken cancellationToken = default);

        Task<NotificationList> GetNotificationsAsync(string templateType = "", string status = "", string reference = "", string olderThanId = "", bool includeSpreadsheetUploads = false, CancellationToken cancellationToken = default);

        Task<ReceivedTextListResponse> GetReceivedTextsAsync(string olderThanId = "", CancellationToken cancellationToken = default);

        Task<TemplateResponse> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken = default);

        Task<TemplateResponse> GetTemplateByIdAndVersionAsync(string templateId, int version = 0, CancellationToken cancellationToken = default);

        Task<SmsNotificationResponse> SendSmsAsync(string mobileNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null, CancellationToken cancellationToken = default);

        Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null, CancellationToken cancellationToken = default);

        Task<LetterNotificationResponse> SendLetterAsync(string templateId, Dictionary<string, dynamic> personalisation, string clientReference = null, CancellationToken cancellationToken = default);

        Task<LetterNotificationResponse> SendPrecompiledLetterAsync(string clientReference, byte[] pdfContents, string postage, CancellationToken cancellationToken = default);
    }
}
