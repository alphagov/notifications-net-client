using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

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
