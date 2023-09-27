using Notify.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Client
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;

        private Uri _baseAddress;

        private bool _disposed;

        public Uri BaseAddress
        {
            get => _baseAddress;
            set
            {
                _baseAddress = value;
                SetClientBaseAddress();
            }
        }

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
        }

        public void SetClientBaseAddress()
        {
            _client.BaseAddress = _baseAddress;
        }

        public void AddContentHeader(string header)
        {
            _client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(header));
        }

        public void AddUserAgent(string userAgent)
        {
            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }
    }
}
