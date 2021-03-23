using Notify.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Notify.Client
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;
        private Uri _baseAddress;

        public Uri BaseAddress
        {
            get { return _baseAddress; }
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

        public HttpResponseMessage SendAsync(HttpRequestMessage request)
        {
            return _client.SendAsync(request).Result;
        }

        public void SetClientBaseAddress()
        {
            _client.BaseAddress = _baseAddress;
        }

        public void AddContentHeader(String header)
        {
            _client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(header));
        }

        public void AddUserAgent(String userAgent)
        {
            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }
    }
}
