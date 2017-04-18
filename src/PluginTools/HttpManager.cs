using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace JeremyTCD.ContDeployer.PluginTools
{
    public class HttpManager : IHttpManager
    {
        private HttpClient _httpClient { get; }
        private ILogger<HttpManager> _logger { get; }
        private SharedOptions _sharedOptions { get; }

        public HttpManager(HttpClient httpClient, ILogger<HttpManager> logger, 
            IOptions<SharedOptions> sharedOptionsAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sharedOptions = sharedOptionsAccessor.Value;
        }

        /// <summary>
        /// Sends http request synchronously
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns>
        /// <see cref="HttpResponseMessage"/>
        /// </returns>
        public HttpResponseMessage Invoke(HttpRequestMessage requestMessage, bool executeOnDryRun = false)
        {
            if(_sharedOptions.DryRun && !executeOnDryRun)
            {
                // Log reqest message details
                _logger.LogInformation("");
            }

            return _httpClient.SendAsync(requestMessage).Result;
        }
    }
}
