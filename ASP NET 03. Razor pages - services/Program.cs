using ASP_NET_03._Razor_pages___services.Data;
using ASP_NET_03._Razor_pages___services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Dependency Injection
// Ninject, AutoFac, Simple Inject, IoC, ...
// DI

builder
    .Services
    .AddSingleton<IProductRepository, InMemoryRepository>();

// builder.Services.Add(new ServiceDescriptor(typeof(IProductRepository), typeof(InMemoryRepository), ServiceLifetime.Singleton));
//builder
//    .Services
//    .AddSingleton<IProductRepository>(new InMemoryRepository("Best of the best"));

builder.Services.AddSingleton<ProductService>();

// ASP Services Lyfecycle - Singleton, Transient, Scoped

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
