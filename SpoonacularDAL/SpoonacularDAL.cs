using HomeUtilities.Common.Attributes;
using HomeUtilities.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HomeUtilities.Common.Interfaces;
using SpoonacularDAL.Requests;
using SpoonacularDAL.Responses;
using System.Net;
using System.Reflection;
using System.Text;

namespace SpoonacularDAL
{
    public class SpoonacularDAL : BaseDataLayer
    {
        private int _maxRetries = 3;
        private int _retryDelayMilliseconds = 50;
        private string _apiBaseUrl, _apiKey;

        private readonly IConfiguration _configuration;

        public SpoonacularDAL(ILogger<SpoonacularDAL> logger, IConfiguration configuration, IMemoryCache memoryCache) : base(logger, memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            _apiBaseUrl = _configuration.GetSection("Settings").GetSection("SpoonacularBaseUrl").Value ?? "";
            _apiKey = _configuration.GetSection("Settings").GetSection("SpoonacularApiKey").Value ?? "";
        }

        #region Recipes

        public async Task<GetRandomRecipesResponse> GetRandomRecipes(GetRandomRecipesRequest request)
        {
            try
            {
                if (request == null)
                    return new GetRandomRecipesResponse { ErrorCode = "-302", Message = "Get Random Recipes Request cannot be null." };

                var requestUrl = $"{_apiBaseUrl}/recipes/random?apiKey={_apiKey}&number={request.Quantity}";

                if (request.IncludedTags?.Any() ?? false)
                    requestUrl += $"&include-tags={string.Join(',', request.IncludedTags)}";

                if (request.ExcludedTags?.Any() ?? false)
                    requestUrl += $"&exclude-tags={string.Join(',', request.ExcludedTags)}";

                var response = await SendRequestAsync<GetRandomRecipesResponse>(HttpMethod.Get, requestUrl, null);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in {nameof(GetRandomRecipes)}. See exception for details.");
                return new GetRandomRecipesResponse { ErrorCode = "-1", Message = "An exception occurred. See exception for more details.", DALException = ex };
            }
        }

        [Cacheable(1440, 1440)]
        public async Task<GetRecipeInformationResponse> GetRecipeInformation(int? spoonacularId)
        {
            try
            {
                if (spoonacularId == null)
                    return new GetRecipeInformationResponse { ErrorCode = "-302", Message = "Get Random Recipes Request cannot be null." };

                var cacheKey = $"GetRecipeInformation_{spoonacularId}";
                return await GetCachedResponseAsync(cacheKey, async () =>
                {
                    var requestUrl = $"{_apiBaseUrl}/recipes/{spoonacularId}/information?apiKey={_apiKey}";
                    return await SendRequestAsync<GetRecipeInformationResponse>(HttpMethod.Get, requestUrl, null);
                }, GetMethodCachingOptions(MethodBase.GetCurrentMethod()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in {nameof(GetRecipeInformation)}. See exception for details.");
                return new GetRecipeInformationResponse { ErrorCode = "-1", Message = "An exception occurred. See exception for more details.", DALException = ex };
            }
        }

        #endregion        

        private async Task<T> SendRequestAsync<T>(HttpMethod httpMethod, string baseUrl, string json = null, int extendTimeoutInSeconds = 0, bool clearCookies = false) where T : BaseResponse, new()
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                HttpResponseMessage httpResponse = null;

                var retry = true;
                var numTries = 0;
                var currentDelay = 0d;
                var previousDelay = 0d;
                var responseJson = string.Empty;

                if (extendTimeoutInSeconds != 0)
                    httpClient.Timeout = TimeSpan.FromSeconds(extendTimeoutInSeconds);

                do
                {
                    try
                    {
                        var request = RequestUri(httpMethod, baseUrl, json);

                        if (currentDelay > 0)
                            System.Threading.Thread.Sleep((int)currentDelay);

                        httpResponse = await httpClient.SendAsync(request);
                        responseJson = (httpResponse?.Content != null ? await httpResponse.Content.ReadAsStringAsync() : null) ?? string.Empty;

                        retry = !(httpResponse?.IsSuccessStatusCode ?? false);

                        currentDelay = numTries == 0 ? _retryDelayMilliseconds : Math.Ceiling(previousDelay * 1.4);
                        previousDelay = currentDelay;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred in {nameof(SendRequestAsync)} while sending the request. See exception for details.");
                    }

                    numTries++;
                } while (retry && numTries < _maxRetries);

                try
                {
                    var response = !string.IsNullOrWhiteSpace(responseJson) ? JsonConvert.DeserializeObject<T>(responseJson) : new T();

                    response.ErrorCode = (httpResponse?.IsSuccessStatusCode ?? false) ? null : "-1";

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred in {nameof(SendRequestAsync)}. See exception for details.");

                    return new T
                    {
                        ErrorCode = "-1",
                        Message = "An error occurred while deserializing the response from AudienceView. See exception for more details.",
                        DALException = ex
                    };
                }
            }
        }

        private static HttpRequestMessage RequestUri(HttpMethod httpMethod, string requestUrl, string json)
        {
            var requestUri = new Uri(requestUrl);

            var request = new HttpRequestMessage { RequestUri = requestUri };

            if (!string.IsNullOrWhiteSpace(json))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("Accept", "application/json");
            }

            request.Method = httpMethod;

            return request;
        }
    }
}
