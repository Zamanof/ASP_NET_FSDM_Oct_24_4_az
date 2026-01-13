using ASP_NET_03._Razor_Pages_Product_Site.Models;
using Bogus;

namespace ASP_NET_03._Razor_Pages_Product_Site.Data;

public class InMemoryRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    public InMemoryRepository()
    {
        var faker = new Faker<Product>()
            .RuleFor(p=> p.Id, f=> f.Random.Int(1))
            .RuleFor(p=> p.Name, f=> f.Commerce.Product())
            .RuleFor(p=> p.Description, f=> f.Commerce.ProductDescription())
            .RuleFor(p=> p.Price, f=> f.Random.Decimal(1, 50))
            .RuleFor(p=> p.Count, f=> f.Random.UInt(1))
            .RuleFor(p=> p.IsAvailable, true);

        _products.AddRange(faker.GenerateBetween(20, 20));
    }
    public Product AddProduct(Product product)
    {
        _products.Add(product);
        return product;
    }
    public Task<Product> GetProductByIdAsync(int id)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id))!;

    public Task<IEnumerable<Product>> GetProductsAsync()
        => Task.FromResult(_products.AsEnumerable());
}
