using ASP_NET_02._Dependency_injection.Models;

namespace ASP_NET_02._Dependency_injection.Data;

public class InMemoryRepository : IProductRepository
{
    private readonly IDictionary<Guid, Product> _products =
        new Dictionary<Guid, Product>();

    public InMemoryRepository()
    {
        AddProduct(new Product { Name = "Lenovo", Description = "Lenovo is the best" });
        AddProduct(new Product { Name = "HP", Description = "HP is the best of the best" });
        AddProduct(new Product { Name = "Pendir", Description = "Seher seher shirinchaynan" });
        AddProduct(new Product { Name = "Sheker tozu", Description = "Shirinchay uchun" });
        AddProduct(new Product { Name = "ASP", Description = "Chorek aqaci" });
        AddProduct(new Product { Name = "React", Description = "Best Web JS library" });
    }

    public Product AddProduct(Product product)
    {
        product.Id = Guid.NewGuid();
        _products.Add(product.Id, product);
        return product;
    }

    public IEnumerable<Product> GetProducts() => _products.Values;
}
