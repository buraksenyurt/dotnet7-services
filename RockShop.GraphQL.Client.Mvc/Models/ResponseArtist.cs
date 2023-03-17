using RockShop.Shared;

namespace RockShop.GraphQL.Client.Mvc.Models;

public class ResponseArtist{
    public class DataArtists{
        public Artist[]? Artists { get; set; }        
    }

    public DataArtists? Data { get; set; }
}