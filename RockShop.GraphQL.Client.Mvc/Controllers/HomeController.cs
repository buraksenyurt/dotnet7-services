using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RockShop.GraphQL.Client.Mvc.Models;
using System.Text;

namespace RockShop.GraphQL.Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index(string pageNumber = "1")
    {
        IndexViewModel model = new();
        try
        {
            var client = _clientFactory.CreateClient(name: "RockShop.GraphQL");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: "/");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                model.StatusCode = response.StatusCode;
                model.Errors = new[] { new Error { Message = "Service is not availably right know. Try again later." } };
                return View(model);
            }

            request = new(method: HttpMethod.Post, requestUri: "graphql");
            // Used C#11 raw interpolated string literal syntax
            request.Content = new StringContent(content: $$$"""
            {
                "query":"{tracks(page: {{{pageNumber}}}){album name composer}}"
            }
            """, encoding: Encoding.UTF8,
            mediaType: "application/json");
            response = await client.SendAsync(request);

            model.StatusCode = response.StatusCode;
            model.RawResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model.Tracks = (await response.Content.ReadFromJsonAsync<ResponseTrack>())?.Data?.Tracks;
            }
            else
            {
                model.Errors = (await response.Content.ReadFromJsonAsync<ErrorResponse>())?.Errors;
            }
        }
        catch (Exception exp)
        {
            _logger.LogError($"RockShop.GraphQL service exception: {exp.Message}");
            model.Errors = new[] { new Error { Message = exp.Message } };
        }
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
