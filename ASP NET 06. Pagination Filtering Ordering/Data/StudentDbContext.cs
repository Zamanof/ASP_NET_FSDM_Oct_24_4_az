using ASP_NET_06._Pagination_Filtering_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_06._Pagination_Filtering_Ordering.Data;

public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions options) 
        : base(options)
    {}
    public DbSet<Student> Students => Set<Student>();
}
