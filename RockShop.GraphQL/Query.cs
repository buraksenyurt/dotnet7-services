namespace RockShop.GraphQL;

public class Query
{
    public string Ping() => "Pong";
    public int LuckyNum()
    {
        Random rnd = new Random();
        return rnd.Next(1, 100);
    }
}