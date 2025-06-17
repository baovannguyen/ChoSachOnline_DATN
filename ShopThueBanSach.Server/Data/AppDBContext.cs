using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Entities.Relationships;
using ShopThueBanSach.Server.Entities.ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SaleBook> SaleBooks { get; set; }
        public DbSet<RentBook> RentBooks { get; set; }
        public DbSet<RentBookItem> RentBookItems { get; set; }
        public DbSet<ActivityNotification> ActivityNotifications { get; set; }
        public DbSet<Slide> Slides { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Many-to-Many configs
            builder.Entity<AuthorSaleBook>().HasKey(x => new { x.AuthorId, x.SaleBookId });
            builder.Entity<AuthorSaleBook>()
                .HasOne(x => x.Author).WithMany(a => a.AuthorSaleBooks).HasForeignKey(x => x.AuthorId);
            builder.Entity<AuthorSaleBook>()
                .HasOne(x => x.SaleBook).WithMany(s => s.AuthorSaleBooks).HasForeignKey(x => x.SaleBookId);

            builder.Entity<AuthorRentBook>().HasKey(x => new { x.AuthorId, x.RentBookId });
            builder.Entity<AuthorRentBook>()
                .HasOne(x => x.Author).WithMany(a => a.AuthorRentBooks).HasForeignKey(x => x.AuthorId);
            builder.Entity<AuthorRentBook>()
                .HasOne(x => x.RentBook).WithMany(r => r.AuthorRentBooks).HasForeignKey(x => x.RentBookId);

            builder.Entity<CategorySaleBook>().HasKey(x => new { x.CategoryId, x.SaleBookId });
            builder.Entity<CategorySaleBook>()
                .HasOne(x => x.Category).WithMany(c => c.CategorySaleBooks).HasForeignKey(x => x.CategoryId);
            builder.Entity<CategorySaleBook>()
                .HasOne(x => x.SaleBook).WithMany(s => s.CategorySaleBooks).HasForeignKey(x => x.SaleBookId);

            builder.Entity<CategoryRentBook>().HasKey(x => new { x.CategoryId, x.RentBookId });
            builder.Entity<CategoryRentBook>()
                .HasOne(x => x.Category).WithMany(c => c.CategoryRentBooks).HasForeignKey(x => x.CategoryId);
            builder.Entity<CategoryRentBook>()
                .HasOne(x => x.RentBook).WithMany(r => r.CategoryRentBooks).HasForeignKey(x => x.RentBookId);

            // RentBookItem Relations
            builder.Entity<RentBookItem>()
                .HasOne(r => r.RentBook)
                .WithMany(b => b.RentBookItems)
                .HasForeignKey(r => r.RentBookId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ActivityNotification>()
    .HasOne(n => n.Staff)
    .WithMany() // Nếu Staff có ICollection<ActivityNotification> thì thêm .WithMany(s => s.ActivityNotifications)
    .HasForeignKey(n => n.StaffId)
    .OnDelete(DeleteBehavior.Cascade); // hoặc .Restrict nếu bạn muốn giữ thông báo khi xóa staff

        }

    }
}
