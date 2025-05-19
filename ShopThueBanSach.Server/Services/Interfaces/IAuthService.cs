using ShopThueBanSach.Server.Models;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto model);
        Task<AuthResult> LoginAsync(LoginDto model);
        Task<AuthResult> RefreshTokenAsync(TokenRequestDto model);
        Task<AuthResult> SendEmailConfirmationCodeAsync(string email);
        Task<AuthResult> ConfirmEmailByCodeAsync(string email, string code);
        Task<AuthResult> SendResetPasswordCodeAsync(string email);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto model);
    }
}
