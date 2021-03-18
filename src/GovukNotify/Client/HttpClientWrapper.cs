using Notify.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Notify.Client
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;
        private Uri _baseAddress;

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
            _client.Dispose();
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request).ConfigureAwait(false);
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
