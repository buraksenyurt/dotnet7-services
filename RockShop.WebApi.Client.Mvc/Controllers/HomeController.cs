using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RockShop.WebApi.Client.Mvc.Models;
using RockShop.Shared;

namespace RockShop.WebApi.Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
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

    [Route("home/albums/{artistName?}")]
    public async Task<IActionResult> Albums(string? artistName)
    {
        var client = _clientFactory.CreateClient(name: "RockShop.WebApi.Service");
        var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: $"api/albums/{artistName}");
        var response = await client.SendAsync(request);
        IEnumerable<Album>? albums = await response.Content.ReadFromJsonAsync<IEnumerable<Album>>();
        ViewData["apiAddress"] = client.BaseAddress;
        return View(albums);
    }
}
