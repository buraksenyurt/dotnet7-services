namespace RockShop.GraphQL.Client.Mvc.Models;

public class Error
{
    public string Message { get; set; } = string.Empty;
    public ErrorLocation[] ErrorLocations { get; set; } = Array.Empty<ErrorLocation>();
    public string[] Path { get; set; } = Array.Empty<string>();
}