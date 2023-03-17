using RockShop.Shared;
using System.Net;

namespace RockShop.GraphQL.Client.Mvc.Models;

public class IndexViewModel
{
    public TrackDto[]? Tracks { get; set; }
    public Error[]? Errors { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? RawResponse { get; set; }
}