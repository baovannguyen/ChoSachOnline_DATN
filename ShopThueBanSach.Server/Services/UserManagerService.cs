﻿using Microsoft.AspNetCore.Identity;
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
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserManagerService(UserManager<User> userManager, IStaffService staffService, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_staffService = staffService;
			_roleManager = roleManager;
		}

		public async Task<IEnumerable<UserDto>> GetAllAsync()
		{
			var users = await _userManager.Users.ToListAsync();
			var result = new List<UserDto>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				// Hiển thị tất cả trừ Admin và Staff
				if (!roles.Contains("Admin") && !roles.Contains("Staff"))
				{
					result.Add(new UserDto
					{
						Id = user.Id,
						Email = user.Email ?? string.Empty,
						UserName = user.UserName,
						Address = user.Address,
						PhoneNumber = user.PhoneNumber,
						DateOfBirth = user.DateOfBirth,
						Points = user.Points,
						ImageUser = user.ImageUser,
						Role = roles.FirstOrDefault()
					});
				}
			}

			return result;
		}

		public async Task<UserDto?> GetByIdAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return null;
			return await MapToDto(user, _userManager);
		}

		public async Task<bool> UpdateAsync(string id, UpdateUserDto dto)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return false;

			user.Address = dto.Address ?? user.Address;
			user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
			user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
			user.Points = dto.Points ?? user.Points;

			if (dto.ImageFile != null)
			{
				user.ImageUser = await SaveImageAsync(dto.ImageFile);
			}

			if (!string.IsNullOrWhiteSpace(dto.Role))
			{
				var currentRoles = await _userManager.GetRolesAsync(user);
				if (currentRoles.Any())
				{
					var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
					if (!removeResult.Succeeded) return false;
				}

				var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
				if (!addResult.Succeeded) return false;
			}

			var updateResult = await _userManager.UpdateAsync(user);
			return updateResult.Succeeded;
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return false;

			var roles = await _userManager.GetRolesAsync(user);
			if (!roles.Contains("Customer")) return false;

			var result = await _userManager.DeleteAsync(user);
			return result.Succeeded;
		}

		public async Task<bool> UpdateCustomerAsync(string id, UpdateCustomerDto dto)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return false;

			var roles = await _userManager.GetRolesAsync(user);
			if (!roles.Contains("Customer")) return false;

			user.Address = dto.Address ?? user.Address;
			user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
			user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
			user.Points = dto.Points ?? user.Points;

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

		private static async Task<UserDto> MapToDto(User user, UserManager<User> userManager)
		{
			var roles = await userManager.GetRolesAsync(user);
			return new UserDto
			{
				Id = user.Id,
				Email = user.Email ?? string.Empty,
				UserName = user.UserName,
				Address = user.Address,
				PhoneNumber = user.PhoneNumber,
				DateOfBirth = user.DateOfBirth,
				Points = user.Points,
				ImageUser = user.ImageUser,
				Role = roles.FirstOrDefault()
			};
		}
	}
}
//