using Microsoft.AspNetCore.SignalR.Client;
using RockShop.Common.Models.SignalR;

WriteLine("Wellcome community trace board.\n");
Write("What is your name?");
string? personName = ReadLine();

if (!string.IsNullOrEmpty(personName))
{
    WriteLine("Please enter your interested topics for suggestion board");
    string? topics = ReadLine();

    HubConnection conn = new HubConnectionBuilder()
        .WithUrl("https://localhost:7044/board",
                    (opts) =>
                    {
                        opts.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler clientHandler)
                                // to bypass SSL certificate(use for development only)
                                clientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    })
        .Build();

    conn.On<Suggestion>("ReceiveSuggestion", message =>
    {
        WriteLine($"From {message.From} To {message.To} : {message.Content}");
    });

    await conn.StartAsync();
    WriteLine("Board is online");

    Person registration = new()
    {
        Name = personName,
        Topics = topics
    };

    await conn.InvokeAsync("Register", registration);

    WriteLine("Registration completed.\nBoard active.");
    WriteLine("Press any key to exit");
    ReadLine();

}
else
{
    WriteLine("No Name. Bye!");
    return;
}