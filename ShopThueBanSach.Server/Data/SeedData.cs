using Microsoft.AspNetCore.Identity;
using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminUsername = "admin";
            string adminEmail = "cloneduong001@gmail.com";
            string adminPassword = "Admin@123";

            // Tạo role Admin nếu chưa có
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Kiểm tra nếu user đã tồn tại
            var existingAdmin = await userManager.FindByNameAsync(adminUsername);
            if (existingAdmin == null)
            {
                var admin = new User
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Role = "Admin",
                    Address = "Trụ sở",
                    DateOfBirth = DateTime.Parse("1990-01-01"),
                    Points = 99999999
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
