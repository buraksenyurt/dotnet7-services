using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace RockShop.Azure.Service.Isolated
{
    public class TopSalesFunction
    {
        private readonly ILogger _logger;

        public TopSalesFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TopSalesFunction>();
        }

        [Function("TopSalesFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation($"Request method is '{req.Method}'");
            Region? region = null;

            if (req.Method == "POST")
            {
                try
                {
                    var body = await new StreamReader(req.Body).ReadToEndAsync();
                    _logger.LogInformation($"Incoming data {body}");
                    region = JsonSerializer.Deserialize<Region>(body);
                }
                catch (Exception excp)
                {
                    _logger.LogCritical($"{excp.Message}");
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var products = new List<Product>{
                new Product{Name="AMZN",Total=1200.50M},
                new Product{Name="FCBK",Total=1090.93M},
                new Product{Name="GOOG",Total=985.93M},
                new Product{Name="TWTR",Total=250.00M},
                new Product{Name="TORB",Total=4500.55M}
            };

            var report = new Report
            {
                Region = region ?? new Region { Name = "COMMON" },
                Products = products
            };

            var data = JsonSerializer.Serialize(report);
            await response.WriteStringAsync(data);

            return await Task.FromResult(response);
        }
    }

    public class Report
    {
        public Region Region { get; set; } = new Region();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class Region
    {
        public string Name { get; set; } = string.Empty;
    }
}
