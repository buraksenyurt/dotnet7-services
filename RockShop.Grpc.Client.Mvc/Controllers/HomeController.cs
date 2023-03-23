using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RockShop.Grpc.Client.Mvc.Models;
using Grpc.Net.ClientFactory;
using Grpc.Core;

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

    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        IndexViewModel model = new();
        try
        {
            // Added deadline
            ArtistReply reply = await _jukeBoxClient.GetArtistsAsync(new Mvc.ArtistRequest { PageNumber = pageNumber }, deadline: DateTime.UtcNow.AddSeconds(1.5));
            model.CurrentPage = pageNumber;
            model.Artists = reply.Data;

            // // To use Metadata on RPC
            // AsyncUnaryCall<ArtistReply> rpcCall = _jukeBoxClient.GetArtistsAsync(new ArtistRequest { PageNumber = pageNumber });
            // Metadata metadata = await rpcCall.ResponseHeadersAsync;
            // foreach (Metadata.Entry entry in metadata)
            // {
            //     _logger.LogCritical($"Key: {entry.Key}, Value: {entry.Value}");
            // }
            // ArtistReply reply = await rpcCall.ResponseAsync;

            model.CurrentPage = pageNumber;
            model.Artists = reply.Data;

        }
        catch (RpcException rpcEx) when (rpcEx.StatusCode == global::Grpc.Core.StatusCode.DeadlineExceeded)
        {
            _logger.LogWarning("Grpc Service deadline exceeded");
            ViewData["exception"] = rpcEx.Message;
        }
        catch (Exception e)
        {
            _logger.LogError("RockShop service is not responding.");
            ViewData["exception"] = e.Message;
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
