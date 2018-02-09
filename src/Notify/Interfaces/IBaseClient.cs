using System;
using System.Net.Http;

namespace Notify.Interfaces
{
    public interface IBaseClient
    {
        string GET(string url);

        string POST(string url, string json);

        string MakeRequest(string url, HttpMethod method, HttpContent content = null);

        Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey);

        Uri ValidateBaseUri(string baseUrl);

		string GetUserAgent();
    }
}
