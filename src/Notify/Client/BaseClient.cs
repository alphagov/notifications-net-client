using Newtonsoft.Json;
using Notify.Authentication;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models.Responses;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Notify.Client
{
    public class BaseClient : IBaseClient
    {
        private const Int32 SERVICE_ID_START_POSITION = 73;
        private const Int32 SERVICE_API_KEY_START_POSITION = 36;
        private const Int32 GUID_LENGTH = 36;
        private const String NOTIFY_CLIENT_NAME = "NOTIFY-API-NET-CLIENT/";

        public String baseUrl;
        private IHttpClient client;
        private String serviceId;
        private String apiKey;

        public BaseClient(IHttpClient client, String apiKey, String baseUrl= "https://api.notifications.service.gov.uk/")
        {
            Tuple<String, String> serviceCredentials = ExtractServiceIdAndApiKey(apiKey);
            this.serviceId = serviceCredentials.Item1;
            this.apiKey = serviceCredentials.Item2;
            this.baseUrl = baseUrl;
            this.client = client;
            this.client.BaseAddress = ValidateBaseUri(this.baseUrl);
            this.client.AddContentHeader("application/json");

            String productVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.client.AddUserAgent(NOTIFY_CLIENT_NAME + productVersion);

        }

        public String GET(String url)
        {
            return this.MakeRequest(url, HttpMethod.Get);
        }

        public String POST(String url, String json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return this.MakeRequest(url, HttpMethod.Post, content);
        }

        public String MakeRequest(String url, HttpMethod method, HttpContent content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            if(content != null)
            {
                request.Content = content;
            }

            String notifyToken = Authenticator.CreateToken(this.apiKey, this.serviceId);
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + notifyToken);

            HttpResponseMessage response;

            try
            {
                response = this.client.SendAsync(request);
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is HttpRequestException) 
                    {
                        throw new NotifyClientException(x.InnerException.Message);
                    }
                    throw x; 
                });
                throw ae.Flatten();
            }
            catch (Exception e)
            {
                throw e;
            }

            String responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return responseContent;
            }
            else
            {
                try
                {
                    NotifyHTTPErrorResponse errorResponse = JsonConvert.DeserializeObject<NotifyHTTPErrorResponse>(responseContent);
                    throw new NotifyClientException("Status code {0}. The following errors occured {1}", errorResponse.getStatusCode(), errorResponse.getErrorsAsJson());
                }
                catch(Newtonsoft.Json.JsonReaderException)
                {
                    throw new NotifyClientException("Status code {0}. Error: ", responseContent);
                }
            }
            
        }

        public Tuple<String, String> ExtractServiceIdAndApiKey(String fromApiKey)
        {
            if (fromApiKey.Length < 74 || String.IsNullOrWhiteSpace(fromApiKey) || fromApiKey.Contains(" "))
            {
                throw new NotifyAuthException("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
            }

            String serviceId = fromApiKey.Substring(fromApiKey.Length - SERVICE_ID_START_POSITION, GUID_LENGTH);
            String apiKey = fromApiKey.Substring(fromApiKey.Length - SERVICE_API_KEY_START_POSITION, GUID_LENGTH);

            return Tuple.Create(serviceId, apiKey);
        }

        public Uri ValidateBaseUri(String baseUrl)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(baseUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if(!result)
            {
                throw new NotifyAuthException("Invalid URL provided");
            }
            else
            {
                return uriResult;
            }
        }

        public String GetUserAgent()
        {
            return NOTIFY_CLIENT_NAME + Assembly.GetExecutingAssembly().GetName().Version.ToString();		
        }
    }
}
