        using Microsoft.AspNetCore.Identity;
        using Microsoft.EntityFrameworkCore;
        using ShopThueBanSach.Server.Area.Admin.Entities;
        using ShopThueBanSach.Server.Area.Admin.Service;
        using ShopThueBanSach.Server.Area.Admin.Service.Interface;
        using ShopThueBanSach.Server.Entities;
        using ShopThueBanSach.Server.Models.AuthModel;
        using ShopThueBanSach.Server.Models.UserModel;
        using ShopThueBanSach.Server.Services.Interfaces;

        namespace ShopThueBanSach.Server.Services
        {
            public class UserManagerService : IUserManagerService
            {
                private readonly UserManager<User> _userManager;
                private readonly IStaffService _staffService;

                public UserManagerService(UserManager<User> userManager, IStaffService staffService)
                {
                    _userManager = userManager;
                    _staffService = staffService;
                }

                public async Task<IEnumerable<UserDto>> GetAllAsync()
                {
                    var users = await _userManager.Users
                        .Where(u => u.Role == null || u.Role == "Customer")
                        .ToListAsync();

                    return users.Select(u => MapToDto(u));
                }

                public async Task<UserDto?> GetByIdAsync(string id)
                {
                    var user = await _userManager.FindByIdAsync(id);

                    if (user == null || !(user.Role == null || user.Role == "Customer"))
                        return null;

                    return MapToDto(user);
                }

        public async Task<bool> UpdateAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(user.Role == null || user.Role == "Customer" || user.Role == "Staff"))
                return false;

            var oldRole = user.Role;

            if (string.IsNullOrEmpty(user.Email))
            {
                user.Email = $"{Guid.NewGuid()}@placeholder.local";
            }

            if (dto.Address != null) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.Points.HasValue) user.Points = dto.Points.Value;
            if (dto.Role != null) user.Role = dto.Role;

            // ✅ Xử lý ảnh mới nếu có
            if (dto.ImageFile != null)
            {
                user.ImageUser = await SaveImageAsync(dto.ImageFile);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            // ✅ Xử lý Staff logic...
            // (giữ nguyên như bạn đã có)

            return true;
        }


        public async Task<bool> DeleteAsync(string id)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null || !(user.Role == null || user.Role == "Customer"))
                        return false;

                    var result = await _userManager.DeleteAsync(user);
                    return result.Succeeded;
                }

                private static UserDto MapToDto(User u) => new()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Role = u.Role,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth,    
                    Points = u.Points,
                    ImageUser = u.ImageUser
                };
        public async Task<bool> UpdateCustomerAsync(string id, UpdateCustomerDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(user.Role == null || user.Role == "Customer"))
                return false;

            if (dto.Address != null) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.Points.HasValue) user.Points = dto.Points.Value;

            if (dto.ImageFile != null)
            {
                user.ImageUser = await SaveImageAsync(dto.ImageFile);
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Users");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/Images/Users/{uniqueFileName}";
        }



    }
}
