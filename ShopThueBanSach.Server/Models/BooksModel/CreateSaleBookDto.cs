﻿using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Models.BooksModel
{
    public class CreateSaleBookDto
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Publisher { get; set; }

        public string? Translator { get; set; }

        public string? Size { get; set; }

        public int Pages { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        // ✅ Ẩn / hiện sách
        public bool IsHidden { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> AuthorIds { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> CategoryIds { get; set; }

        // ✅ Thêm dòng này để cho phép thêm khuyến mãi (hoặc bỏ qua nếu không có)
        public string? PromotionId { get; set; }
    }


}
