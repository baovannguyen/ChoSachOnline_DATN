namespace ShopThueBanSach.Server.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }       // Mã thể loại
        public string Name { get; set; }          // Tên thể loại
        public string? Description { get; set; }  // Mô tả

        public ICollection<SellBook> SellBooks { get; set; }
    }
}
