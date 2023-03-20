using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RockShop.Grpc.Client.Mvc.Models;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;

namespace RockShop.Grpc.Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly JukeBox.JukeBoxClient _jukeBoxClient;

    public HomeController(ILogger<HomeController> logger, GrpcClientFactory factory)
    {
        _logger = logger;
        _jukeBoxClient = factory.CreateClient<JukeBox.JukeBoxClient>("JukeBox");
    }

    public async Task<IActionResult> Index(int pn = 1)
    {
        try
        {
            ArtistReply reply = await _jukeBoxClient.GetArtistsAsync(new Mvc.ArtistRequest { PageNumber = pn });
            ViewData["artistList"] = reply.Data;
        }
        catch (Exception e)
        {
            _logger.LogError("RockShop service is not responding.");
            ViewData["exception"] = e.Message;
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
