using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Notify.Interfaces
{
    public interface IBaseClient
    {
        String GET(String url);

        String POST(String url, String json);

        String MakeRequest(String url, HttpMethod method, HttpContent content = null);

        Tuple<String, String> ExtractServiceIdAndApiKey(String fromApiKey);

        Uri ValidateBaseUri(String baseUrl);
    }
}
