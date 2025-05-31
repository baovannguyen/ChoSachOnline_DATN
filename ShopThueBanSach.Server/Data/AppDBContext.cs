using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Customer> Customers { get; set; }
    }
}
