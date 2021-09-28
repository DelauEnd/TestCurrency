using Microsoft.EntityFrameworkCore;

namespace TestCurrency.Data
{
    public class ProductsDBContext : DbContext
    {
        public ProductsDBContext(DbContextOptions<ProductsDBContext> options) 
            : base(options) 
        {
            Database.EnsureCreated();       
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
    }
} 
