using ASP_NET_03._Razor_Pages_Product_Site.Models;
using ASP_NET_03._Razor_Pages_Product_Site.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Razor_Pages_Product_Site.Pages;

public class ProductsModel : PageModel
{
    private readonly ProductService _service;

    public ProductsModel(ProductService service)
    {
        _service = service;
    }

    public IEnumerable<Product> Products { get; set; } 
        = Enumerable.Empty<Product>();
    public async Task OnGetAsync()
    {
        Products = await _service.GetProductsAsync();
    }
}
