using ASP_NET_03._Razor_Pages_Product_Site.Data;
using ASP_NET_03._Razor_Pages_Product_Site.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder
    .Services
    .AddSingleton<IProductRepository, InMemoryRepository>();

builder
    .Services
    .AddSingleton<ProductService>();

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
