using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RockShop.Shared
{
    public static class ChinookDbContextExtensions
    {
        public static IServiceCollection AddChinookDbContext(
            this IServiceCollection services
            , string connStr = "Host=localhost;Database=chinook;Username=scoth;Password=tiger")
        {
            services.AddDbContext<ChinookDbContext>(options =>
            {
                options.UseNpgsql(connStr);
                options.LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting });
            });

            return services;
        }
    }
}