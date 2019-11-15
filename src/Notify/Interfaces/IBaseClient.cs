using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Notify.Interfaces
{
    public interface IBaseClient
    {
        Task<string> GET(string url);

        Task<string> POST(string url, string json);

        Task<string> MakeRequest(string url, HttpMethod method, HttpContent content = null);

        Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey);

        Uri ValidateBaseUri(string baseUrl);

		string GetUserAgent();
    }
}
