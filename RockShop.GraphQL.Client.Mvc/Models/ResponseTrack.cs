using RockShop.Shared;

namespace RockShop.GraphQL.Client.Mvc.Models;

public class ResponseTrack
{
    public class DataTracks
    {
        public TrackDto[]? Tracks { get; set; }
    }

    public DataTracks? Data { get; set; }
}