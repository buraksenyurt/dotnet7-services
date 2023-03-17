namespace RockShop.GraphQL.Client.Mvc.Models;

public class Error
{
    public string Message { get; set; }
    public ErrorLocation[] ErrorLocations { get; set; }
    public string[] Path { get; set; }
}