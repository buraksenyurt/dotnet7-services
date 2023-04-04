using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace RockShop.Azure.Service.Isolated
{
    public class AppRadarFunction
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public AppRadarFunction(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<AppRadarFunction>();
            _httpClientFactory = httpClientFactory;
        }

        [Function(nameof(AppRadarFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var client = _httpClientFactory.CreateClient("AppRadarService");
            _logger.LogInformation($"Using address -> {client.BaseAddress}");
            var response = await client.GetAsync("report");
            var content = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(content);
            var data = reader.ReadToEnd();
            _logger.LogInformation(data);
            return new OkObjectResult(data);
        }
    }
}
