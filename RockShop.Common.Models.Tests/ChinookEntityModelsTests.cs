using RockShop.Shared;

namespace RockShop.Common.Models.Tests;

public class ChinookEntityModelsTest
{
    [Fact]
    public void Can_Connection_Is_True()
    {
        using (ChinookDbContext db = new())
        {
            bool status = db.Database.CanConnect();
            Assert.True(status);
        }
    }

    [Fact]
    public void Is_Provider_Postgresql()
    {
        using (ChinookDbContext db = new())
        {
            string? provider = db.Database.ProviderName;

            Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", provider);
        }
    }

    [Fact]
    public void Is_There_Any_Employee()
    {
        using (ChinookDbContext db = new())
        {
            var employees_count = db.Employees.Where(e => e.Country == "Canada").Count();

            Assert.True(employees_count > 1);
        }
    }

    [Fact]
    public void Employee_Refreshed_In_Ten_Seconds()
    {
        using (ChinookDbContext db = new())
        {
            var empl = db.Employees.Single(e => e.EmployeeId == 1);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            Assert.InRange(actual: empl.LastRefreshed, low: now.Subtract(TimeSpan.FromSeconds(5)),
            high: now.AddSeconds(5));
        }
    }
}