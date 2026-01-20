namespace ASP_NET_06._Pagination_Filtering_Ordering.Models;

public class PaginationViewModel<TModel>
{
    public IEnumerable<TModel> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PaginationViewModel(IEnumerable<TModel> items, int page, int pageSize, int count)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = Convert.ToInt32(Math.Ceiling((float)count/pageSize));
        // 8 / 3 = 2.666666666666667 => 3
    }
}
