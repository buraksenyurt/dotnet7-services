using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddHttpClient(name: "AppRadarService", options =>
        {
            options.BaseAddress = new System.Uri("http://localhost:6023/api/report");
        });
    })
    .Build();

host.Run();
