using RockShop.Shared;

namespace RockShop.GraphQL.Client.Mvc.Models;

public class ResponseAlbum{
    public class DataAlbums{
        public Album[]? Albums { get; set; }        
    }

    public DataAlbums? Data { get; set; }
}