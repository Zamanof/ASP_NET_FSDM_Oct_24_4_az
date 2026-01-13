using ASP_NET_03._Razor_Pages_Product_Site.Data;
using ASP_NET_03._Razor_Pages_Product_Site.Models;
using Bogus;

namespace ASP_NET_03._Razor_Pages_Product_Site.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Product AddProduct(Product product)
    {
        var faker = new Faker<Product>().RuleFor(p => p.Id, f => f.Random.Int(1));
        product.Id = faker.Generate().Id;
        if (product.Count > 0) product.IsAvailable = true;
        _repository.AddProduct(product);
        return product;
    }
    public async Task<Product> GetProductByIdAsync(int id)
        => await _repository.GetProductByIdAsync(id);

    public async Task<IEnumerable<Product>> GetProductsAsync()
        => await _repository.GetProductsAsync();
}
