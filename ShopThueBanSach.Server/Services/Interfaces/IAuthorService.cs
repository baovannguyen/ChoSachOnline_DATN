using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAsync();
        Task<Author?> GetByIdAsync(int id);
        Task<Author> CreateAsync(Author author);
        Task<bool> UpdateAsync(Author author);
        Task<bool> DeleteAsync(int id);
    }
}
