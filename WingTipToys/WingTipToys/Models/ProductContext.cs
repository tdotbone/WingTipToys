using System.Data.Entity;

namespace WingTipToys.Models
{
    public class ProductContext : DbContext  // ProductContext inherits from DbContext
    {
        public ProductContext() : base("WingTipToys")
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
    }
}