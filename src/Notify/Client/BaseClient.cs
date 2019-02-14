using Newtonsoft.Json;
using Notify.Authentication;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models.Responses;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Client
{
    public class BaseClient : IBaseClient
    {
        private const int SERVICE_ID_START_POSITION = 73;
        private const int SERVICE_API_KEY_START_POSITION = 36;
        private const int GUID_LENGTH = 36;
        private const string NOTIFY_CLIENT_NAME = "NOTIFY-API-NET-CLIENT/";

        public string BaseUrl;

        private readonly IHttpClient client;
        private readonly string serviceId;
        private readonly string apiKey;

        public BaseClient(IHttpClient client, string apiKey, string baseUrl = "https://api.notifications.service.gov.uk/")
        {
            var serviceCredentials = ExtractServiceIdAndApiKey(apiKey);
            serviceId = serviceCredentials.Item1;
            this.apiKey = serviceCredentials.Item2;
            BaseUrl = baseUrl;
            this.client = client;
            this.client.BaseAddress = ValidateBaseUri(BaseUrl);
            this.client.AddContentHeader("application/json");

            var productVersion = typeof(BaseClient).GetTypeInfo().Assembly.GetName().Version.ToString();
            this.client.AddUserAgent(NOTIFY_CLIENT_NAME + productVersion);
        }

        public async Task<string> GET(string url)
        {
            return await MakeRequest(url, HttpMethod.Get);
        }

        public async Task<string> POST(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await MakeRequest(url, HttpMethod.Post, content);
        }

        public async Task<string> MakeRequest(string url, HttpMethod method, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
            {
                request.Content = content;
            }

            var notifyToken = Authenticator.CreateToken(this.apiKey, this.serviceId);
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + notifyToken);

            HttpResponseMessage response;

            try
            {
                response = await this.client.SendAsync(request);
            }
            catch (AggregateException ae)
            {
                ae.Handle(x =>
                {
                    if (x is HttpRequestException)
                    {
                        throw new NotifyClientException(x.InnerException.Message);
                    }
                    throw x;
                });
                throw ae.Flatten();
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return responseContent;
            }

            try
            {
                var errorResponse = JsonConvert.DeserializeObject<NotifyHTTPErrorResponse>(responseContent);
                throw new NotifyClientException("Status code {0}. The following errors occured {1}", errorResponse.getStatusCode(), errorResponse.getErrorsAsJson());
            }
            catch (Exception ex)
            {
                throw new NotifyClientException("Status code {0}. Error: {1}, Exception: {2}", response.StatusCode.GetHashCode(), responseContent, ex.Message);
            }
        }

        public Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey)
        {
            if (fromApiKey.Length < 74 || string.IsNullOrWhiteSpace(fromApiKey) || fromApiKey.Contains(" "))
            {
                throw new NotifyAuthException("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
            }

            var serviceId = fromApiKey.Substring(fromApiKey.Length - SERVICE_ID_START_POSITION, GUID_LENGTH);
            var apiKey = fromApiKey.Substring(fromApiKey.Length - SERVICE_API_KEY_START_POSITION, GUID_LENGTH);

            return Tuple.Create(serviceId, apiKey);
        }

        public Uri ValidateBaseUri(string baseUrl)
        {
            var result = Uri.TryCreate(baseUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https");

            if (!result)
            {
                throw new NotifyAuthException("Invalid URL provided");
            }

            return uriResult;
            
        }

        public string GetUserAgent()
        {
            return NOTIFY_CLIENT_NAME + typeof(BaseClient).GetTypeInfo().Assembly.GetName().Version;
        }
    }
}
