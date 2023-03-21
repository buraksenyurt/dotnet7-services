using Google.Protobuf.Collections;

namespace RockShop.Grpc.Client.Mvc.Models;

public class IndexViewModel
{
    public RepeatedField<ArtistMessage>? Artists { get; set; }
    public int CurrentPage { get; set; }
}