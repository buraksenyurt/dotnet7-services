namespace Chinook.Common;

public class SuggestionModel
{
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string? Content { get; set; }
}