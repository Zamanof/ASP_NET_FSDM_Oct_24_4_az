using ASP_NET_03._Razor_pages___services.Models;

namespace ASP_NET_03._Razor_pages___services.Data;

public interface IProductRepository
{
    public Product AddProduct(Product product);

    public IEnumerable<Product> GetProducts();
}
