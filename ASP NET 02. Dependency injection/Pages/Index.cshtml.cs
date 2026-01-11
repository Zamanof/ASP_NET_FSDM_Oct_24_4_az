using ASP_NET_02._Dependency_injection.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_02._Dependency_injection.Pages;

public class IndexModel : PageModel
{

    private readonly IProductRepository repository;

    public IndexModel(IProductRepository repository)
    {
        this.repository = repository;
    }

    public void OnGet()
    {
        var products = repository.GetProducts();
        ViewData["Products"] = products;
    }
}
