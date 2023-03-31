namespace RockShop.Common.Models.SignalR;

public class Person
{
    public string Name { get; set; }=null!;
    public string ConnectionId{ get; set; }=null!;
    public string? Topics { get; set; }
}
