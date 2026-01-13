namespace ASP_NET_03._Razor_Pages_Product_Site.Models;

public class Product
{
    public int Id{ get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public uint Count { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}
