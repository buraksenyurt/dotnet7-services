using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RockShop.OData.Client.Mvc.Models;
using RockShop.OData.Client.Mvc.Models.OData;

namespace RockShop.OData.Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index(string nameWith = "Abc")
    {
        try
        {
            var client = _clientFactory.CreateClient(name: "RockShop.OData");
            var request = new HttpRequestMessage(
                method: HttpMethod.Get
                , requestUri: $"jukebox/v1/albums/?$filter=startswith(Title,'{nameWith}')&select=AlbumId,Title");
            var response = await client.SendAsync(request);

            ViewData["albumNameStartsWith"] = nameWith;
            ViewData["albums"] = (await response.Content.ReadFromJsonAsync<Albums>())?.Value;

        }
        catch (Exception e)
        {
            _logger.LogError($"OData service client error. Exception detail {e.Message}");
        }
        return View();
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
