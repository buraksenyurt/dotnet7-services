namespace RockShop.Common.Models.SignalR;

public class Suggestion
{
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string? Content { get; set; }
}