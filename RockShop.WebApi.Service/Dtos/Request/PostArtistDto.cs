namespace RockShop.WebApi.Service.Dtos.Response
{
    public class PostArtistDto{
        public string? Name { get; set; }
        public List<PostAlbumDto>? Albums { get; set; }
    }

    public class PostAlbumDto{
        public string? Title { get; set; }
    }
}