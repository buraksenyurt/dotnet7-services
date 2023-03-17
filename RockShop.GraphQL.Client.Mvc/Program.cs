using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registered the HttpClient for calling GraphQL service
builder.Services.AddHttpClient(name: "RockShop.GraphQL", configureClient: options =>
{
    options.BaseAddress = new Uri("http://localhost:5034/");
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
