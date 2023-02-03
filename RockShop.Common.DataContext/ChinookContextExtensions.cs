using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RockShop.Shared
{
    public static class ChinookContextExtensions
    {
        public static IServiceCollection AddChinookContext(
            this IServiceCollection services
            , string connStr = "Host=localhost;Database=postgres;Username=scoth;Password=tiger")
        {
            services.AddDbContext<ChinookContext>(options =>
            {
                options.UseNpgsql(connStr);
                options.LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting });
            });

            return services;
        }
    }
}