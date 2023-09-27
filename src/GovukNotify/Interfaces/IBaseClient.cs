using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Notify.Interfaces
{
    public interface IBaseClient
    {
        Task<string> GET(string url, CancellationToken cancellationToken = default);

        Task<string> POST(string url, string json, CancellationToken cancellationToken = default);

        Task<string> MakeRequest(string url, HttpMethod method, HttpContent content = null, CancellationToken cancellationToken = default);

        Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey);

        Uri ValidateBaseUri(string baseUrl);

		string GetUserAgent();
    }
}
