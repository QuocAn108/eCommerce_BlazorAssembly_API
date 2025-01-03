using Microsoft.EntityFrameworkCore;
using PhoneShopShareLibrary.Models;

namespace PhoneShopServer.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<UserAccount> UserAccount { get; set; } = default!;
        public DbSet<UserRole> UserRole { get; set; } = default!;
        public DbSet<SystemRole> SystemRole { get; set; } = default!;
        public DbSet<TokenInfo> TokenInfo { get; set; } = default!;

    }
}
