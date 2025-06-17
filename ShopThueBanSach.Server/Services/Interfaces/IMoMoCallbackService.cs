using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.PaymentMethod;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IMoMoCallbackService
    {
        public interface IMoMoCallbackService
        {
            Task<IActionResult> HandleCallbackAsync(MoMoResult result);
        }
    }
}
