using Microsoft.Marketplace.SaaS;
using Microsoft.Marketplace.SaaS.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using MySql.Data.MySqlClient.Memcached;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyCard.Domain;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using Microsoft.Rest.Serialization;
using System.Net.Http.Headers;
using System.Text;

namespace MyCard.Web.Controllers
{
    public class SaasMarketApiController : Controller
    {

        public async Task GetBarrerTokeAsync()
        {
            using (var client = new HttpClient())
            {
              

                var make_url = "https://login.microsoftonline.com/62d1faee-64c4-4033-b1fd-837a518ff898/oauth2/token";

                var _httpRequest = new HttpRequestMessage();
                HttpResponseMessage _httpResponse = null;
                _httpRequest.Method = new HttpMethod("POST");
                _httpRequest.RequestUri = new System.Uri(make_url);
                _httpRequest.Headers.Add("application/x-www-form-urlencoded", "application/x-www-form-urlencoded");
                _httpRequest.Headers.Add("grant_type", "client_credentials");
                _httpRequest.Headers.Add("client_id", "f45dea0f-d778-4ed9-9423-2a47223d6fc5");
                _httpRequest.Headers.Add("client_secret", "DGA0NUGOvsvGYoz2_zsMdkEZH1r_E8_H_~");
                _httpRequest.Headers.Add("resource", "20e940b3-4c77-4b0b-9a53-9e16a1b010a7");

            }
        }
        public JsonSerializerSettings DeserializationSettings { get; private set; }
        public JsonSerializerSettings SerializationSettings { get; private set; }
        public ServiceClientCredentials Credentials { get; private set; }
        public bool? GenerateClientRequestId { get; set; }
        public MarketplaceSaaSClient Client { get; private set; }

        public async Task<AzureOperationResponse<ResolvedSubscription>> IndexAsync(string Value)
        {
            string contenttype = "application/json";
            string xMsMarketplaceToken = "";
            string requestId = "";
            string correlationId = "";
            string AcceptLanguage = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjVPZjlQNUY5Z0NDd0NtRjJCT0hIeEREUS1EayIsImtpZCI6IjVPZjlQNUY5Z0NDd0NtRjJCT0hIeEREUS1EayJ9.eyJhdWQiOiIyMGU5NDBiMy00Yzc3LTRiMGItOWE1My05ZTE2YTFiMDEwYTciLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC82MmQxZmFlZS02NGM0LTQwMzMtYjFmZC04MzdhNTE4ZmY4OTgvIiwiaWF0IjoxNjA4OTgzMTc5LCJuYmYiOjE2MDg5ODMxNzksImV4cCI6MTYwODk4NzA3OSwiYWlvIjoiRTJKZ1lORFdZTklOME1xOTdGRFRwM3N5K3o4SEFBPT0iLCJhcHBpZCI6ImY0NWRlYTBmLWQ3NzgtNGVkOS05NDIzLTJhNDcyMjNkNmZjNSIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzYyZDFmYWVlLTY0YzQtNDAzMy1iMWZkLTgzN2E1MThmZjg5OC8iLCJvaWQiOiJjNjU5Zjc5NS03ZWNiLTRmMmEtOGY5Ny0zOThiODdhODhiNTQiLCJyaCI6IjAuQVNBQTd2clJZc1JrTTBDeF9ZTjZVWV80bUFfcVhmUjQxOWxPbENNcVJ5STliOFVnQUFBLiIsInN1YiI6ImM2NTlmNzk1LTdlY2ItNGYyYS04Zjk3LTM5OGI4N2E4OGI1NCIsInRpZCI6IjYyZDFmYWVlLTY0YzQtNDAzMy1iMWZkLTgzN2E1MThmZjg5OCIsInV0aSI6Ii1sVG9wdG1TRzB5a3Z6NUNYaHpFQUEiLCJ2ZXIiOiIxLjAifQ.1ifS_-WO3mHD4LGYjvMSvEKDODg2aSthcCc2kSAY17JMzDv_paNwoQaAue65zbX9K6G3oJ1JQScKyDZY2OlIVO1H-FoNSYhpSg8y6esmqXxwrFAF_gtW7tYAFxpPwQoEDjoxHnxHAFxJDriVirry951vBM8CVAZdYqae1BCw0iAejQj8T32ENrmTQoqYDb9mFJ1YZ_DWqzhcQ84YZ-54d0q9UXL2AV47vxCCdnvyYKgDLgtEgKfYbQU1gFwXiH8UxSyNLBVJDkGFb0gzk8B8uq-CaXcTATO_xralSfME65vF3xJ7PsHqkvIl71wX5gqTDgodSdmAfpxwl-0co4t8NQ";
            Dictionary<string, List<string>> customHeaders = null;
            CancellationToken cancellationToken = default(CancellationToken);
            bool? GenerateClientRequestId = true;
            if (xMsMarketplaceToken == null)
            {
                throw new Microsoft.Rest.ValidationException(ValidationRules.CannotBeNull, "xMsMarketplaceToken");
            }
            string apiVersion = "2018-08-31";
            // Tracing
            bool _shouldTrace = true;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("apiVersion", apiVersion);
                tracingParameters.Add("requestId", "");
                tracingParameters.Add("correlationId", "");
                tracingParameters.Add("xMsMarketplaceToken", xMsMarketplaceToken); 
                tracingParameters.Add("cancellationToken", "");
                ServiceClientTracing.Enter(_invocationId, this, "Resolve", tracingParameters);
            }
            // Construct URL
            var _baseUrl = "https://marketplaceapi.microsoft.com/api";
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "saas/subscriptions/resolve").ToString();
            List<string> _queryParameters = new List<string>();
            if (apiVersion != null)
            {
                _queryParameters.Add(string.Format("api-version={0}", System.Uri.EscapeDataString(apiVersion)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += (_url.Contains("?") ? "&" : "?") + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            // Set Headers
            //if (GenerateClientRequestId != null && GenerateClientRequestId.Value)
            //{
            //    _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", System.Guid.NewGuid().ToString());
            //}
            if (requestId != null)
            {
                if (_httpRequest.Headers.Contains("x-ms-requestid"))
                {
                    _httpRequest.Headers.Remove("x-ms-requestid");
                }
                _httpRequest.Headers.TryAddWithoutValidation("x-ms-requestid", Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(requestId, SerializationSettings).Trim('"'));

            }


            _httpRequest.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            if (correlationId != null)
            {
                if (_httpRequest.Headers.Contains("x-ms-correlationid"))
                {
                    _httpRequest.Headers.Remove("x-ms-correlationid");
                }
                _httpRequest.Headers.TryAddWithoutValidation("x-ms-correlationid", Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(correlationId, SerializationSettings).Trim('"'));
            }
            if (xMsMarketplaceToken != null)
            {
                if (_httpRequest.Headers.Contains("x-ms-marketplace-token"))
                {
                    _httpRequest.Headers.Remove("x-ms-marketplace-token");
                }
                _httpRequest.Headers.TryAddWithoutValidation("x-ms-marketplace-token", xMsMarketplaceToken);
            }
            if (AcceptLanguage != null)
            {
                if (_httpRequest.Headers.Contains("authorization"))
                {
                    _httpRequest.Headers.Remove("authorization");
                }
                _httpRequest.Headers.TryAddWithoutValidation("authorization", AcceptLanguage);
            }


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Set Credentials
            //if (Client.Credentials != null)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();
            //    await Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            //}
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 400 && (int)_statusCode != 403 && (int)_statusCode != 404 && (int)_statusCode != 500)
            {
                var ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                try
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    CloudError _errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, DeserializationSettings);
                    if (_errorBody != null)
                    {
                        ex = new CloudException(_errorBody.Message);
                        ex.Body = _errorBody;
                    }
                }
                catch (JsonException)
                {
                    // Ignore the exception
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_httpResponse.Headers.Contains("x-ms-request-id"))
                {
                    ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault();
                }
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new AzureOperationResponse<ResolvedSubscription>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            if (_httpResponse.Headers.Contains("x-ms-request-id"))
            {
                _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault();
            }
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<ResolvedSubscription>(_responseContent, DeserializationSettings);
                    var response = _result.Body;
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new Microsoft.Rest.SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        
        }
    }
}