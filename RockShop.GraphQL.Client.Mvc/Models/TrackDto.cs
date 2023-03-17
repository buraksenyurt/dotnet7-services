namespace RockShop.GraphQL.Client.Mvc.Models;
public class TrackDto
{
    public string? Name { get; set; }
    public string? Album { get; set; }
    public string? Composer { get; set; }
    public double Duration { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Genre { get; set; }
    public string? MediaType { get; set; }
}