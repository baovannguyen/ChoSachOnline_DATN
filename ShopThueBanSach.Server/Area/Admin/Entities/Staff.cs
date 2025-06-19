﻿using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Area.Admin.Entities
{
    public class Staff
    {
        [Key]
        public string StaffId { get; set; } = Guid.NewGuid().ToString(); // hoặc tạo mã theo quy tắc riêng

        public string FullName { get; set; }

        public string Role { get; set; } = "Staff";

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public DateTime? DateOfBirth { get; set; }


    }

}
