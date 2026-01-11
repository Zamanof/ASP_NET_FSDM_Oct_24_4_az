using ASP_NET_02._Dependency_injection.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Dependency Injection
// Ninject, AutoFac, Simple Inject, IoC, ...
// DI
builder.Services.AddSingleton<IProductRepository, InMemoryRepository>();

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
