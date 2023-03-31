partial class Program
{
    static async IAsyncEnumerable<string> LoadRequestedServerNames()
    {
        string[] names = { "ISTSERV01","LONSERVO2","ISTTSERV02","MANCHSERV05" };
        foreach (var name in names)
        {
            yield return name;
            await Task.Delay(2000);
        }

    }
}