using ASP_NET_02._Dependency_injection.Models;

namespace ASP_NET_02._Dependency_injection.Data;

public interface IProductRepository
{
    public Product AddProduct(Product product);

    public IEnumerable<Product> GetProducts();
}
