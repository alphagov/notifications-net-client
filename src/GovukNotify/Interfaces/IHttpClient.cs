using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Interfaces
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

        Uri BaseAddress { get; set; }

        void SetClientBaseAddress();

        void AddContentHeader(string header);

        void AddUserAgent(string userAgent);
	}
}
