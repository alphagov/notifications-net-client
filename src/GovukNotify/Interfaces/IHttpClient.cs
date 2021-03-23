using System;
using System.Net.Http;

namespace Notify.Interfaces
{
    public interface IHttpClient : IDisposable
    {
        HttpResponseMessage SendAsync(HttpRequestMessage request);

        Uri BaseAddress { get; set; }

        void SetClientBaseAddress();

        void AddContentHeader(String header);

        void AddUserAgent(String userAgent);
	}
}
