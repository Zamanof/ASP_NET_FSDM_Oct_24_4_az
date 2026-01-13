using ASP_NET_03._Razor_pages___services.Data;
using ASP_NET_03._Razor_pages___services.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Razor_pages___services.Pages;

public class IndexModel : PageModel
{

    private readonly ProductService _service;

    public IndexModel(ProductService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        var products = _service.GetProducts();
        ViewData["Products"] = products;
    }
}
