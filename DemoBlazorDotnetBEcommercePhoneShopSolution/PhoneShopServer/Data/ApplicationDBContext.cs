﻿using Microsoft.EntityFrameworkCore;
using PhoneShopShareLibrary.Models;

namespace PhoneShopServer.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;

    }
}
