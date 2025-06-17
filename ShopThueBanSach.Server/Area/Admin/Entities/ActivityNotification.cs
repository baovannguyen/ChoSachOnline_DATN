﻿using ShopThueBanSach.Server.Area.Admin.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ActivityNotification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string NotificationId { get; set; }

    public int StaffId { get; set; }  // Đổi từ UserId thành StaffId

    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public Staff? Staff { get; set; } // Navigation property
}
