namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IActivityNotificationService
    {
        Task CreateNotificationAsync(int staffId, string description);
    }

}
