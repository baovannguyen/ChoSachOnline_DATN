namespace ShopThueBanSach.Server.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }        // Mã tác giả
        public string Name { get; set; }         // Tên tác giả
        public string? Description { get; set; } // Mô tả

     /*   public ICollection<SellBook> SellBooks { get; set; }*/
    }
}
